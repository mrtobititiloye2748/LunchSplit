/*
 * FILE          : PaymentRequest.cs
 * PROJECT       : CSCN72010 Software Quality I - Assignment 3 (LunchSplit)
 * PROGRAMMER    : mrtobititiloye2748
 * FIRST VERSION : 2026-11-12
 * DESCRIPTION   :
 *   Domain DTO (Data Transfer Object) describing one payment that LunchSplit
 *   wants to charge. It uses plain domain types (decimal dollars, string
 *   currency code) so that Core never has to know about Square's Money type.
 */

namespace LunchSplit.Core.Payments
{
    // Holds everything the gateway needs to charge one attendee.
    public class PaymentRequest
    {
        // Name of the attendee being charged (used for notes and batch reports)
        public string PayerName { get; set; }

        // Amount to charge in dollars (for example 25.50m). The gateway wrapper converts it to cents.
        public decimal Amount { get; set; }

        // ISO 4217 currency code, for example "CAD"
        public string CurrencyCode { get; set; }

        // Unique key so the gateway never double-charges if the same request is sent twice
        public string IdempotencyKey { get; set; }

        // Card token from the payment form (in the sandbox: a magic nonce such as "cnon:card-nonce-ok")
        public string CardNonce { get; set; }

        // Default constructor sets safe empty values so no property is ever null
        public PaymentRequest()
        {
            PayerName = string.Empty;
            Amount = 0m;
            CurrencyCode = "CAD";
            IdempotencyKey = string.Empty;
            CardNonce = string.Empty;
        }
    }
}
