using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactas.SDK.DTO
{
    public abstract class PaymentBearerDTO
    {
        public abstract string Type { get; protected set; }
    }

    public class PaymentBearerCreditCardDTO : PaymentBearerDTO
    {
        /// <summary>
        /// Visa, Mastercard, ...
        /// </summary>
        public string CardType { get; set; }

        /// <summary>
        /// Expiry month of the credit card
        /// </summary>
        public int ExpiryMonth { get; set; }

        /// <summary>
        /// Expiry year of the credit card
        /// </summary>
        public int ExpiryYear { get; set; }

        /// <summary>
        /// Name of the card holder
        /// </summary>
        public string Holder { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// The last four digits of the credit card
        /// </summary>
        public string Last4 { get; set; }

        public override string Type { get { return "CreditCard"; } protected set { } }
    }

    public class PaymentBearerBankAccountDTO : PaymentBearerDTO
    {
        /// <summary>
        /// The used Bank Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Name of the account holder
        /// </summary>
        public string Holder { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// The used account number, for security reasons the number is masked
        /// </summary>
        public string Account { get; set; }

        public string IBAN { get; set; }
        public string BIC { get; set; }

        public override string Type { get { return "BankAccount"; } protected set { } }
    }

    public class PaymentBearerBlackLabelDTO : PaymentBearerDTO
    {
        public PaymentBearerBlackLabelDTO(PaymentProvider provider)
        {
            if (provider == PaymentProvider.InvoicePayment)
                Type = "auf Rechnung"; //HACK: PaymentBearerBlackLabelDTO is solely used in self service so for the moment this is ok, but it is really hacky.
            else
                Type = provider.ToString("G");
        }
        public override string Type { get; protected set; }
    }

}
