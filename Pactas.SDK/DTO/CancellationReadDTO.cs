using System;
using System.Collections.Generic;
using System.Linq;

namespace Pactas.SDK.DTO
{
    public class CancellationOutputDTO
    {
        public DateTime? CancellationDate { get; set; }
        public DateTime? NextPossibleCancellationDate { get; set; }
        public DateTime EndDate { get; set; }
        public InvoicePreviewDTO Invoice { get; set; }
        public ContractReadDTO ContractAfter { get; set; }
    }
}
