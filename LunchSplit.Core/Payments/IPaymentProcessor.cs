/*
 * FILE          : IPaymentProcessor.cs
 * PROJECT       : CSCN72010 Software Quality I - Assignment 3 (LunchSplit)
 * PROGRAMMER    : mrtobititiloye2748
 * FIRST VERSION : 2026-11-12
 * DESCRIPTION   :
 *   Core domain contract for any external payment gateway. LunchSplit.Core only
 *   depends on this interface, never on a vendor SDK. In production the
 *   Square implementation (SquarePaymentService) is injected; in unit tests a
 *   Moq Mock<IPaymentProcessor> is injected instead (Inversion of Control).
 */

using System.Threading.Tasks;

namespace LunchSplit.Core.Payments
{
    // Contract that every payment gateway wrapper must implement.
    public interface IPaymentProcessor
    {
        /*
         * METHOD      : ProcessPaymentAsync
         * DESCRIPTION : Sends one payment (charge) to the payment gateway.
         * PARAMETERS  : PaymentRequest request - amount, currency, idempotency key and card nonce
         * RETURNS     : Task<PaymentResult> - success flag, transaction id or error details
         */
        Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);

        /*
         * METHOD      : GetTransactionStatusAsync
         * DESCRIPTION : Looks up the current status of a payment that was already created.
         * PARAMETERS  : string transactionId - the id returned in PaymentResult.TransactionId
         * RETURNS     : Task<PaymentStatus> - the gateway status mapped to the domain enum
         */
        Task<PaymentStatus> GetTransactionStatusAsync(string transactionId);
    }
}
