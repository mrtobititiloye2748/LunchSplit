/*
 * FILE          : PaymentGatewayTimeoutException.cs
 * PROJECT       : CSCN72010 Software Quality I - Assignment 3 (LunchSplit)
 * PROGRAMMER    : mrtobititiloye2748
 * FIRST VERSION : 2026-11-12
 * DESCRIPTION   :
 *   Domain exception thrown when the payment gateway cannot be reached
 *   (network failure, HTTP 503, socket timeout). Low-level exceptions such as
 *   HttpRequestException are wrapped in this type so callers of LunchSplit.Core
 *   only need to handle one domain-specific error.
 */

using System;

namespace LunchSplit.Core.Payments
{
    // Raised when the payment gateway is unreachable or times out.
    public class PaymentGatewayTimeoutException : Exception
    {
        /*
         * METHOD      : PaymentGatewayTimeoutException (constructor)
         * DESCRIPTION : Creates the exception with a message only.
         * PARAMETERS  : string message - description of the problem
         * RETURNS     : n/a
         */
        public PaymentGatewayTimeoutException(string message)
            : base(message)
        {
        }

        /*
         * METHOD      : PaymentGatewayTimeoutException (constructor)
         * DESCRIPTION : Creates the exception and keeps the original low-level exception.
         * PARAMETERS  : string message           - description of the problem
         *               Exception innerException - the original network exception
         * RETURNS     : n/a
         */
        public PaymentGatewayTimeoutException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
