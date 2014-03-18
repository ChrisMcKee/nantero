using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pactas.SDK.DTO
{
    public enum PeriodUnit
    {
        Day = 1,
        Week = 2,
        Month = 3,
        Year = 4,
        Hour = 5,
        Minute = 6
    }

    public class PeriodDTO
    {
        [EnumDataType(typeof(PeriodUnit))]
        public PeriodUnit Unit { get; set; }

        [Range(0, 9999)]
        public int Quantity { get; set; }
    }

    public enum BillingTriggerMode
    {
        TransactionBasedPush = 1,
        AutomatedRecurring = 2,
    }

    public enum PaymentPeriodMode
    {
        PrePaid = 0,
        PostPaid = 1,
        PrePaidBillingPeriod = 2
    }

    /// <summary>
    /// Transitions:
    /// 
    /// Trial -> TrialExpired
    /// Trial -> Active
    /// Active -> Ended
    /// Active -> TemporarilyInactive
    /// 
    /// 
    /// </summary>
    public enum ContractLifecycleStatus
    {
        Undefined = 0,
        Draft = 1,
        InOrderProcess = 2,
        InTrial = 3,
        TrialExpired = 4,
        Active = 5,
        Ended = 6,
        TemporarilyInactive = 7
    }

    public enum ContractPhaseType
    {
        Inactive = 0,
        Normal = 1,
        Trial = 2
    }

    public class ContractPhaseDTO
    {
        public ContractPhaseType Type { get; set; }
        public DateTime StartDate { get; set; }
        public string PlanVariantId { get; set; }
        public string PlanId { get; set; }
    }

    public class ContractDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? BilledUntil { get; set; }

        public string PlanVariantId { get; set; }
        public string Notes { get; set; }

        public string PaymentMethod { get; set; }
    }

    public class ContractReadDTO : ContractDTO
    {
        public string Id { get; set; }
        public DateTime? LastBillingDate { get; set; }

        public DateTime? NextBillingDate { get; set; }

        public string PlanId { get; set; }//denormalized
        public string CustomerId { get; set; }
        
        public ContractLifecycleStatus LifecycleStatus { get; set; }
        // Status (trial, cancelled, trial not extended,  ...)

        public string CustomerName { get; set; }
        //public PaymentPeriodMode PaymentPeriodMode { get; set; }
        public List<ContractPhaseDTO> Phases { get; set; }

        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public string PlanGroupId { get; set; }

        public PaymentBearerDTO PaymentBearer { get; set; }
        public string PaymentProvider { get; set; }

        public bool EscalationSuspended { get; set; }

        public bool RecurringPaymentsPaused { get; set; }
    }
}