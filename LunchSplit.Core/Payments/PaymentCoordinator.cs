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
        public Task<BatchPaymentSummary> PayBillAsync(Bill bill, string cardNonce)
        {
            // Not implemented yet - driven by test M3-05 (TDD)
            throw new NotImplementedException();
        }
    }
}
