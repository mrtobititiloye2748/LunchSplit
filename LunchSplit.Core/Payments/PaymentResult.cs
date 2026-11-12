/*
 * FILE          : PaymentResult.cs
 * PROJECT       : CSCN72010 Software Quality I - Assignment 3 (LunchSplit)
 * PROGRAMMER    : mrtobititiloye2748
 * FIRST VERSION : 2026-11-12
 * DESCRIPTION   :
 *   Domain DTO returned by IPaymentProcessor.ProcessPaymentAsync. It reports
 *   whether the charge worked, the gateway transaction id on success, or the
 *   gateway error code / message on failure.
 */

namespace LunchSplit.Core.Payments
{
    // Outcome of one payment attempt.
    public class PaymentResult
    {
        // True when the gateway accepted the payment
        public bool IsSuccess { get; set; }

        // Gateway payment id (empty when the payment failed)
        public string TransactionId { get; set; }

        // Gateway error code such as "CARD_DECLINED" (empty when the payment worked)
        public string ErrorCode { get; set; }

        // Raw error code exactly as the gateway sent it (e.g. Square's "GENERIC_DECLINE").
        // ErrorCode holds the normalised LunchSplit code; this keeps the original for logging/debugging.
        public string GatewayErrorCode { get; set; }

        // Human readable error detail from the gateway (empty when the payment worked)
        public string ErrorMessage { get; set; }

        // HTTP status code returned by the gateway (200 on success, e.g. 400 for validation errors, 0 if unknown)
        public int HttpStatusCode { get; set; }

        // Default constructor sets safe empty values so no property is ever null
        public PaymentResult()
        {
            IsSuccess = false;
            TransactionId = string.Empty;
            ErrorCode = string.Empty;
            GatewayErrorCode = string.Empty;
            ErrorMessage = string.Empty;
            HttpStatusCode = 0;
        }

        /*
         * METHOD      : Success
         * DESCRIPTION : Factory helper that builds a successful result.
         * PARAMETERS  : string transactionId - id of the payment created by the gateway
         * RETURNS     : PaymentResult - IsSuccess = true with the transaction id filled in
         */
        public static PaymentResult Success(string transactionId)
        {
            PaymentResult result = new PaymentResult();
            result.IsSuccess = true;
            result.TransactionId = transactionId;
            result.HttpStatusCode = 200;
            return result;
        }

        /*
         * METHOD      : Failure
         * DESCRIPTION : Factory helper that builds a failed result.
         * PARAMETERS  : string errorCode    - gateway / domain error code
         *               string errorMessage - readable description of the problem
         * RETURNS     : PaymentResult - IsSuccess = false with the error filled in
         */
        public static PaymentResult Failure(string errorCode, string errorMessage)
        {
            // Status code is unknown here; gateway wrappers can use the overload below
            return Failure(errorCode, errorMessage, 0);
        }

        /*
         * METHOD      : Failure
         * DESCRIPTION : Factory helper that builds a failed result including the HTTP status.
         * PARAMETERS  : string errorCode     - gateway / domain error code
         *               string errorMessage  - readable description of the problem
         *               int httpStatusCode   - HTTP status returned by the gateway
         * RETURNS     : PaymentResult - IsSuccess = false with the error and status filled in
         */
        public static PaymentResult Failure(string errorCode, string errorMessage, int httpStatusCode)
        {
            PaymentResult result = new PaymentResult();
            result.IsSuccess = false;
            result.ErrorCode = errorCode;
            result.ErrorMessage = errorMessage;
            result.HttpStatusCode = httpStatusCode;
            return result;
        }
    }
}
