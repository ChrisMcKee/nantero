using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses;
using Nancy.Responses.Negotiation;
using CodeContract = System.Diagnostics.Contracts.Contract;

namespace Nantero.Global
{
    /// <summary>
    /// From Octopus Deploy / Paul Stovell, http://paulstovell.com/blog/consistent-error-handling-with-nancy
    /// </summary>
    public class ErrorResponse : JsonResponse
    {
        readonly Error error;

        private ErrorResponse(Error error)
            : base(error, new DefaultJsonSerializer())
        {
            CodeContract.Requires<ArgumentNullException>(error != null);
            this.error = error;
        }

        public string ErrorMessage { get { return error.ErrorMessage; } }
        public string FullException { get { return error.FullException; } }
        public string[] Errors { get { return error.Errors; } }

        public static ErrorResponse FromMessage(string message)
        {
            return new ErrorResponse(new Error { ErrorMessage = message });
        }

        public static ErrorResponse FromException(Exception ex)
        {
            var exception = ex;
            var summary = exception.Message;
            var statusCode = HttpStatusCode.InternalServerError;

            var error = new Error { ErrorMessage = summary, FullException = exception.ToString() };

            var response = new ErrorResponse(error);
            response.StatusCode = statusCode;
            return response;
        }

        class Error
        {
            public string ErrorMessage { get; set; }
            public string FullException { get; set; }
            public string[] Errors { get; set; }
        }
    }

    public sealed class ErrorStatusCodeHandler : IStatusCodeHandler
    {
        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound
                || statusCode == HttpStatusCode.InternalServerError
                || statusCode == HttpStatusCode.Forbidden
                || statusCode == HttpStatusCode.Unauthorized;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var clientWantsHtml = ShouldRenderFriendlyErrorPage(context);
            if (!clientWantsHtml)
            {
                if (context.Response is NotFoundResponse)
                {
                    // Normally we return 404's ourselves so we have an ErrorResponse. 
                    // But if no route is matched, Nancy will set a NotFound response itself. 
                    // When this happens we still want to return our nice JSON response.
                    context.Response = ErrorResponse.FromMessage("The resource you requested was not found.").WithStatusCode(statusCode);
                }

                // Pass the existing response through
                return;
            }

            var error = context.Response as ErrorResponse;
            switch (statusCode)
            {
                case HttpStatusCode.Unauthorized:
                    context.Response = new RedirectResponse("/login");//WebRoutes.Web.Accounts.Login());
                    break;
                case HttpStatusCode.Forbidden:
                    context.Response = new ErrorHtmlPageResponse(statusCode)
                    {
                        Title = "Permission",
                        Summary = error == null ? "Sorry, you do not have permission to perform that action." : error.ErrorMessage
                    };
                    break;
                case HttpStatusCode.NotFound:
                    context.Response = new ErrorHtmlPageResponse(statusCode)
                    {
                        Title = "404 Not found",
                        Summary = "Sorry, the resource you requested was not found."
                    };
                    break;
                case HttpStatusCode.InternalServerError:
                    context.Response = new ErrorHtmlPageResponse(statusCode)
                    {
                        Title = "Sorry, something went wrong",
                        Summary = error == null ? "An unexpected error occurred." : error.ErrorMessage,
                        Details = error == null ? null : error.FullException
                    };
                    break;
            }
        }

        static bool ShouldRenderFriendlyErrorPage(NancyContext context)
        {
            var enumerable = context.Request.Headers.Accept;

            var ranges = enumerable.OrderByDescending(o => o.Item2).Select(o => MediaRange.FromString(o.Item1)).ToList();
            foreach (var item in ranges)
            {
                if (item.Matches("application/json"))
                    return false;
                if (item.Matches("text/json"))
                    return false;
                if (item.Matches("text/html"))
                    return true;
            }

            return true;
        }
    }

    public class ErrorHtmlPageResponse : HtmlResponse
    {
        static readonly Regex ReplacementTokenRegex = new Regex("\\#\\{(?<variable>.+?)\\}", RegexOptions.Compiled | RegexOptions.Singleline);
        static readonly string ErrorTemplate;

        static ErrorHtmlPageResponse()
        {
            var viewName = "Nantero." + (typeof(ErrorStatusCodeHandler).Namespace + ".error.html").ToLowerInvariant();
            var names = typeof(ErrorStatusCodeHandler).Assembly.GetManifestResourceNames();
            var stream = typeof(ErrorStatusCodeHandler).Assembly.GetManifestResourceStream(viewName);
            
            using (var reader = new StreamReader(stream))
            {
                ErrorTemplate = reader.ReadToEnd();
            }
        }

        public ErrorHtmlPageResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
            ContentType = "text/html; charset=utf-8";
            Contents = Render;
        }

        public string Title { get; set; }
        public string Summary { get; set; }
        public string Details { get; set; }

        void Render(Stream stream)
        {
            var formatArguments = GetErrorPageDetails();

            var page = ReplacementTokenRegex.Replace(ErrorTemplate, match =>
            {
                string value;
                return formatArguments.TryGetValue(match.Groups["variable"].Value, out value) ? value : string.Empty;
            });

            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine(page);
                writer.Flush();
            }
        }

        Dictionary<string, string> GetErrorPageDetails()
        {
            var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            parameters["ErrorTitle"] = Title;
            parameters["ErrorSummary"] = Summary;
            if (!string.IsNullOrWhiteSpace(Details))
            {
                parameters["ErrorDetails"] = "<h3>Details</h3><pre>" + Details + "</pre>";
            }
            parameters["EmailSubject"] = "Error: " + Summary;
            return parameters;
        }
    }
}
