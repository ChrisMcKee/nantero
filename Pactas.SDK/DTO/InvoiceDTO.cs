using System;
using System.Collections.Generic;

namespace Pactas.SDK.DTO
{
    public class InvoiceListDTO
    {
        public string Id { get; set; }
        public string InvoiceNumber { get; set; }

        public string CustomerId { get; set; }

        public DateTime SentAt { get; set; }
        public DateTime? DueDate { get; set; }

        public string RecipientName { get; set; }
        public string RecipientSubName { get; set; }
        public AddressDTO RecipientAddress { get; set; }

        public string Currency { get; set; }

        public decimal TotalNet { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalGross { get; set; }

        public bool IsInvoice { get; set; }
    }

    public class InvoiceDetailDTO : InvoiceListDTO
    {
        public string SenderName { get; set; }
        public AddressDTO SenderAddress { get; set; }
        
        public string SenderVatId { get; set; }
        public string SenderTaxNumber { get; set; }
        public string RecipientVatId { get; set; }
        public string RecipientTaxNumber { get; set; }

        public List<LineItemDTO> ItemList { get; set; }
        public string Prologue { get; set; }
        public string Epilogue { get; set; }

        public DateTime DeliveryPeriodStart { get; set; }
        public DateTime DeliveryPeriodEnd { get; set; }

        public List<VatDescriptorDTO> VatDescriptors { get; set; }
        public BearerMedium BearerMedium { get; set; }

        public bool ReverseCharge { get; set; }
    }

    // CM, 2013-02-25, proposed structure of document DTOs:
    // ------------------------------------------------------
    // DocumentDTO (abstract)
    // DocumentReadDTO : DocumentDTO
    // ----
    // DocumentInvoiceDTO : DocumentDTO
    // DocumentInvoiceReadDTO : DocumentReadDTO
    // ----
    // DocumentListDTO: DocumentReadDTO

    public enum DocumentType
    {
        Quote = 0,
        //Order = 1,
        Invoice = 2,
        Dunning = 3,
        CreditNote = 4,
        DeliveryNote = 5,
        PlainText = 6,
        InvoiceCancellation = 7,
        Contract = 8
    }

    /*public enum DocumentStatus
    {
        Created = 0,
        Sent = 1,
        Countermanded = 2,
        //
        Read = 10,
        Accepted = 11,
        Disputed = 12,
        Denied = 13
    }*/

    /*public enum InvoiceStatus
    {
        Open = 0,
        Paid = 1,
        Overdue = 2,
        Countermanded = 3,
        Declined = 4,
        Dunned = 6,
    }*/

    /*public enum CreditNoteStatus
    {
        Open = 0,
        Paid = 1,
        Overdue = 2,
        Countermanded = 3,
        //Declined = 4,
        //Completed = 5
        //Dunned = 6,
    }*/

    /*public enum DocumentViewCategory
    {
        None = 0,
        Draft = 1,
        Out = 2,
        In = 3
    }*/
}
