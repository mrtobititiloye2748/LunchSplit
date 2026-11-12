/*
 * FILE          : BatchPaymentSummary.cs
 * PROJECT       : CSCN72010 Software Quality I - Assignment 3 (LunchSplit)
 * PROGRAMMER    : mrtobititiloye2748
 * FIRST VERSION : 2026-11-12
 * DESCRIPTION   :
 *   Report returned by PaymentCoordinator.PayBillAsync after it tries to charge
 *   every attendee of a bill. It says whether the bill passed validation and
 *   keeps the gateway result for each attendee that was charged.
 */

using System.Collections.Generic;

namespace LunchSplit.Core.Payments
{
    // Summary of a batch (whole-bill) payment run.
    public class BatchPaymentSummary
    {
        // False when the bill failed the precondition checks (no payments were attempted)
        public bool IsValid { get; set; }

        // Reason the bill was rejected (empty when IsValid is true)
        public string ValidationMessage { get; set; }

        // One gateway result per attendee that was charged, in attendee order
        public List<PaymentResult> Results { get; }

        // Default constructor: valid, no message, no results yet
        public BatchPaymentSummary()
        {
            IsValid = true;
            ValidationMessage = string.Empty;
            Results = new List<PaymentResult>();
        }

        /*
         * METHOD      : Invalid
         * DESCRIPTION : Factory helper for a bill that failed validation.
         * PARAMETERS  : string message - why the bill was rejected
         * RETURNS     : BatchPaymentSummary - IsValid = false, no results
         */
        public static BatchPaymentSummary Invalid(string message)
        {
            BatchPaymentSummary summary = new BatchPaymentSummary();
            summary.IsValid = false;
            summary.ValidationMessage = message;
            return summary;
        }
    }
}
