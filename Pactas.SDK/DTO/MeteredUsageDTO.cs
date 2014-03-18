using System;

namespace Pactas.SDK.DTO
{
    public class MeteredUsageDTO
    {
        public string ComponentId { get; set; }
        public string ExternalComponentId { get; set; }

        public decimal Quantity { get; set; }

        [Obsolete("Use Quantity instead")]
        public decimal? Amount { get; set; }

        public string Memo { get; set; }

        /// <summary>
        /// A unique key that is provided by the customer so we can avoid creating
        /// duplicates. The key is compound, unique and sparse, so, in other words,
        /// we'll have to make sure that we move this to a fake combined field instead
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The point in time to which this usage is to be associated
        /// </summary>
        public DateTime DueDate { get; set; }
    }

    public class MeteredUsageReadDTO : MeteredUsageDTO
    {
        public string Id { get; set; }
        public string ContractId { get; set; }
        public DateTime TransferredAt { get; set; }
        public DateTime? BilledOn { get; set; }
        public string BilledInvoiceId { get; set; }
    }
}
