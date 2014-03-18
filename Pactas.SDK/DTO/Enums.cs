
namespace Pactas.SDK.DTO
{
    public enum OrderType
    {
        Signup = 0,
        Upgrade = 1
    }

    public enum PSPAccountWarning
    {
        NoRecurringPayments = 1
    }

    public enum EscalationAction
    {
        SendEmail = 0,
        TriggerWebhook = 1,
        CreateDunning = 2,
        CloseAccount = 10,
    }

    public enum PayPalApiType
    { 
        ReferenceTransactions = 0,
        AdaptivePayments = 1,
    }

    public enum PayPalEnvironment
    {
        Sandbox = 0,
        Live = 1,
    }

    public enum PayPalAPMode
    {
        WhiteLabel = 0,
        BlackLabel = 1,
    }

    public enum PayOneMode
    {
        Test = 0,
        Live = 1,
    }

    public enum PaymentCallbackStatus
    {
        Pending = 0,
        Running = 1,
        Finished = 2,
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
        InternalError = 12,
        InternalProviderError = 13,
        RateLimit = 14,
        InvalidConfiguration = 15,
        PermissionDenied = 16,
        Canceled = 17,
        Rejected = 18,
    }

    public enum PaymentFailedReason
    {
        None = 0,
        TransportLayer = 1,
        PaymentRelated = 2,
        UnrecoverableError = 3,
        MaxAttemptsExceeded = 4
    }

    public enum PaymentRetryStrategy
    {
        None = 0,
        Fast = 1,
        Slow = 2,
        NoRetry = 10
    }

    public enum PaymentTrigger
    {
        Unspecified = 0,
        Signup = 1,
        ContractActivation = 2,
        Recurring = 3,
        PaymentChange = 4,
        Upgrade = 5
    }

    public enum PaymentStatusValue
    {
        Created = 0,
        InProgress = 1,
        Prepared = 2,
        PreliminarySucceeded = 3,
        Succeeded = 4,
        Failed = 5, //Currently, not used anymore. We always let transaction fail finally. Retries are handled differently now --> LedgerInfo
        Cancelled = 6,
        Chargeback = 7,
        Pending = 8,
        FinallyFailed = 9,
        Unmapped = 10, //Used for provider status values that can't be mapped
        Refunded = 11, // The payment was refunded in the payment provider system
        ThreeDSecurePending = 12
    }

    public enum LedgerEntryType
    {
        Claim = 0,
        Payment = 1,
        Refund = 2,
        Adjustment = 3,
        Chargeback = 4
    }

    public enum LedgerAmountType
    {
        TaxableNetAmount = 0,
        TaxAmount = 1,
        ReverseChargeNetAmount = 2,
        GrossAmount = 3
    }

    public enum PaymentProvider
    {
        External = 0, // Used if not charged by a supported provider
        DirectDebit = 1,
        PayPal = 2,
        Paymill = 3,
        DTAUS = 4,
        Skrill = 5,
        FakeProvider = 6,
        InvoicePayment = 7,
        PayOne = 8,
        None = 9
    }

    public enum PaymentProviderRole
    {
        Fake = 0,
        BlackLabel = 1,
        CreditCard = 2,
        Debit = 3,
        None = 4
    }

    /*public enum MessageType
    {
        Plain = 0,
        Invoice = 1,
        Order = 2,
        RFQ = 3,
        Quote = 4
    }*/

    /// <summary>
    /// Enumerates the available media for documents.
    /// </summary>
    public enum BearerMedium
    {
        // QES = 0,
        /// <summary>
        /// The default medium: Deliver via Pactas if the recipient is a well-known user or via Email if not
        /// </summary>
        Email = 1,
        /// <summary>
        /// Sends a hardcopy via snail mail, if possible
        /// </summary>
        SnailMail = 2,
        /// <summary>
        /// Merely archives the document and otherwise treats it as if were sent by pactas
        /// </summary>
        ArchiveOnly = 3,
    }

    /// <summary>
    /// Identifies bases for taxation, i.e. which number is to be used as a multiplier
    /// in taxation. This plays a role for reminders, because we can demand interest
    /// for the VAT if the taxation is based on what was agreed upon, rather than what
    /// has been paid already ("Soll-Versteuerung / Ist-Versteuerung")
    /// </summary>
    public enum TaxationBase
    {
        /// <summary>
        /// Taxation is based on the agreement, i.e. on the invoiceCancellation. Invoice senders have to pay
        /// taxes, even if they haven't received their money yet. ("Soll-Versteuerung")
        /// </summary>
        AgreedRemuneration,
        /// <summary>
        /// Taxation is based on actual cash flow, i.e. invoiceCancellation senders need to pay taxes only
        /// after the debtor has paid. ("Ist-Versteuerung")
        /// </summary>
        ReceivedRemuneration
    }

    public enum AdministrationType
    {
        Management = 0,
        ExecutiveBoard = 1,
        Other = 2
    }

}
