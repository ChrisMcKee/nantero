using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pactas.SDK.DTO.SelfService2
{

#region Response DTOs

    public enum ErrorCategory
    {
        Validation,
        Payment,
        Itero
    }

    public enum PaymentErrorCode
    {
        UnmappedError = 0,
        LimitExceeded = 1,
        BearerInvalid = 2,
        BearerExpired = 3,
        InvalidCountry = 4,
        InvalidAmount = 5,
        InvalidCurrency = 6,
        LoginError = 7,
        InvalidData = 8,
        InsufficientBalance = 9,
        AlreadyExecuted = 10,
        InvalidPreconditions = 11,
        InternalError = 12, // Our internal error in payment module
        InternalProviderError = 13,
        RateLimit = 14,
        InvalidConfiguration = 15,
        PermissionDenied = 16,
        Canceled = 17,
        Rejected = 18,
    }

    public enum IteroErrorCode
    {
        CustomerNotFound,
        ContractNotFound,
        OrderNotFound,
        LegalEntityNotFound, // TBD: Better use something like ObjectNotFound and additional information? 
                             //The above enums are not for user provided ids. This will rather result in validation error
        InternalError, // TBD: How many information do we want to give to our customers if we have an internal error? Maybe only InternalReference described below?
        OrderCreationFailed,
        ContractCreationFailed,
        CustomerCreationFailed,
        InvalidOrderStatus,
        LockingError,
        AuthenticationError,
        //Others...
    }

    public enum ValidationErrorCode
    {
        Missing,
        Invalid,
        InvalidFormat,
        NotAllowed
    }

    public abstract class CustomerSelfServiceErrorDTO
    {
        public abstract ErrorCategory Category { get; }
        public string InteralReference { get; set; } // Maybe for bugs it could be helpful to have some kinf of reference to log entry for support purposes!?

        /* Currently existing properties
               public string ErrorMessage { get; set; }
                public string UserErrorMessage { get; set; }
                public string PaymentTransactionStatus { get; set; }
                public string Details { get; set; }*/
    }

    // TBD: How do we handle validation errors returned by a payment provider? Actually, this would end up in PaymentErrorDTO
    public class ValidationErrorDTO : CustomerSelfServiceErrorDTO
    {
        public override ErrorCategory Category { get { return ErrorCategory.Validation; } }
        public ValidationErrorCode ValidationErrorCode { get; set; }
        public string FieldName { get; set; } // Could also be an enum
    }

    public class PaymentErrorDTO : CustomerSelfServiceErrorDTO
    {
        public override ErrorCategory Category { get { return ErrorCategory.Payment; } }
        public PaymentErrorCode ErrorCode { get; set; }
        public string ProviderErrorCode { get; set; }
        public string ProviderErrorMessage { get; set; }
    }

    public class IteroErrorDTO : CustomerSelfServiceErrorDTO
    {
        public override ErrorCategory Category { get { return ErrorCategory.Itero; } }
        public IteroErrorCode ErrorCode { get; set; }
        public string ErrorMessage { get; set; } // More detailed, non localized information
    }

    public class CustomerSelfServiceSuccessDTO
    {
        public string OrderId { get; set; }
        public string ContractId { get; set; }
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string Url { get; set; }
    }

    public class CustomerSelfServiceResponseDTO
    {
        public CustomerSelfServiceErrorDTO Error { get; set; }
        public CustomerSelfServiceSuccessDTO Success { get; set;}
    }

    // Possible errors in itero.js that should be handled as far as possible:
    // - Failed request to Paymill bridge and PayOne Client API
    // - Failed requests to Itero that do not return CustomerSelfServiceDTO

#endregion

}
