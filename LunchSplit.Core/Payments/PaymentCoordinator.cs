/*
 * FILE          : PaymentCoordinator.cs
 * PROJECT       : CSCN72010 Software Quality I - Assignment 3 (LunchSplit)
 * PROGRAMMER    : mrtobititiloye2748
 * FIRST VERSION : 2026-11-12
 * DESCRIPTION   :
 *   The LunchSplit "payment manager". It connects the A1 bill-splitting engine
 *   (Splitter) to a payment gateway. It only depends on the IPaymentProcessor
 *   interface, which is injected through the constructor (Dependency Injection),
 *   so unit tests can pass in a Moq mock instead of the real Square service.
 */

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LunchSplit.Core.Payments
{
    // Charges attendees for their share of a bill through an IPaymentProcessor.
    public class PaymentCoordinator
    {
        // Gateway abstraction (real Square service in production, mock in unit tests)
        private readonly IPaymentProcessor _paymentProcessor;

        // Currency used for every charge made by this coordinator
        private readonly string _currencyCode;

        /*
         * METHOD      : PaymentCoordinator (constructor)
         * DESCRIPTION : Creates a coordinator that charges in Canadian dollars.
         * PARAMETERS  : IPaymentProcessor paymentProcessor - injected gateway
         * RETURNS     : n/a
         */
        public PaymentCoordinator(IPaymentProcessor paymentProcessor)
            : this(paymentProcessor, "CAD")
        {
        }

        /*
         * METHOD      : PaymentCoordinator (constructor)
         * DESCRIPTION : Creates a coordinator that charges in the given currency.
         * PARAMETERS  : IPaymentProcessor paymentProcessor - injected gateway
         *               string currencyCode                - ISO currency code, e.g. "CAD"
         * RETURNS     : n/a
         */
        public PaymentCoordinator(IPaymentProcessor paymentProcessor, string currencyCode)
        {
            if (paymentProcessor == null)
            {
                throw new ArgumentNullException(nameof(paymentProcessor));
            }

            _paymentProcessor = paymentProcessor;
            _currencyCode = currencyCode;
        }

        /*
         * METHOD      : PayShareAsync
         * DESCRIPTION : Charges one attendee for their share.
         * PARAMETERS  : string payerName - attendee name
         *               decimal amount   - share amount in dollars
         *               string cardNonce - card token for this attendee
         * RETURNS     : Task<PaymentResult> - gateway result
         */
        public async Task<PaymentResult> PayShareAsync(string payerName, decimal amount, string cardNonce)
        {
            // Build the domain request; every charge gets its own idempotency key
            PaymentRequest request = new PaymentRequest();
            request.PayerName = payerName;
            request.Amount = amount;
            request.CurrencyCode = _currencyCode;
            request.CardNonce = cardNonce;
            request.IdempotencyKey = Guid.NewGuid().ToString();

            try
            {
                return await _paymentProcessor.ProcessPaymentAsync(request);
            }
            catch (HttpRequestException ex)
            {
                // Network failure / HTTP 503 from the gateway
                throw new PaymentGatewayTimeoutException(
                    "Payment gateway could not be reached while charging " + payerName + ".", ex);
            }
            catch (TaskCanceledException ex)
            {
                // HttpClient timeout elapsed before the gateway answered
                throw new PaymentGatewayTimeoutException(
                    "Payment gateway timed out while charging " + payerName + ".", ex);
            }
        }

        /*
         * METHOD      : PayBillAsync
         * DESCRIPTION : Splits a bill with the A1 Splitter and charges every included attendee.
         * PARAMETERS  : Bill bill        - the bill to collect
         *               string cardNonce - card token used for the charges
         * RETURNS     : Task<BatchPaymentSummary> - validation outcome and per-attendee results
         */
        public async Task<BatchPaymentSummary> PayBillAsync(Bill bill, string cardNonce)
        {
            // 1. Precondition checks - reject bad bills BEFORE any gateway call is made
            string validationError = ValidateBill(bill);
            if (validationError.Length > 0)
            {
                return BatchPaymentSummary.Invalid(validationError);
            }

            // 2. Reuse the A1 splitting engine to work out what each attendee owes
            Splitter splitter = new Splitter();
            Dictionary<Attendee, decimal> splits = splitter.Split(bill);

            // 3. Charge each attendee that owes money, one gateway call per attendee
            BatchPaymentSummary summary = new BatchPaymentSummary();
            foreach (KeyValuePair<Attendee, decimal> entry in splits)
            {
                Attendee attendee = entry.Key;
                decimal amountOwed = entry.Value;

                // Excluded attendees (or $0.00 shares) are skipped - nothing to charge
                if (!attendee.Included || amountOwed <= 0m)
                {
                    continue;
                }

                // A declined card is a normal (failed) result, so the loop keeps going and the
                // remaining attendees are still charged - one bad card does not block the group.
                PaymentResult result = await PayShareAsync(attendee.Name, amountOwed, cardNonce);
                summary.RecordResult(attendee.Name, result);
            }

            return summary;
        }

        /*
         * METHOD      : ValidateBill
         * DESCRIPTION : Checks the bill before charging anyone.
         * PARAMETERS  : Bill bill - the bill to check
         * RETURNS     : string - an error message, or an empty string when the bill is valid
         */
        private static string ValidateBill(Bill bill)
        {
            if (bill == null)
            {
                return "Bill is missing.";
            }

            if (bill.Attendees == null || bill.Attendees.Count == 0)
            {
                return "Bill has no attendees to charge.";
            }

            // Grand total = subtotal + tax + tip + extra shared items
            decimal total = bill.Subtotal + bill.Tax + bill.Tip;
            if (bill.Shares != null)
            {
                foreach (Share share in bill.Shares)
                {
                    total += share.Amount;
                }
            }

            if (total <= 0m)
            {
                return "Bill total must be greater than $0.00.";
            }

            return string.Empty;
        }
    }
}
