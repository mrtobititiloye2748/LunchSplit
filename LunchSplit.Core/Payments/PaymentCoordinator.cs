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
        public Task<PaymentResult> PayShareAsync(string payerName, decimal amount, string cardNonce)
        {
            // Not implemented yet - driven by test M3-03 (TDD)
            throw new NotImplementedException();
        }
    }
}
