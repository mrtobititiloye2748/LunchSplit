/*
 * FILE          : SquarePaymentService.cs
 * PROJECT       : CSCN72010 Software Quality I - Assignment 3 (LunchSplit)
 * PROGRAMMER    : mrtobititiloye2748
 * FIRST VERSION : 2026-11-12
 * DESCRIPTION   :
 *   Infrastructure wrapper that implements the IPaymentProcessor contract using
 *   the official Square .NET SDK in Sandbox mode. It is the ONLY class that
 *   knows about Square types; it translates domain DTOs to Square payloads and
 *   Square responses back to domain DTOs.
 */

using System;
using System.Net.Http;
using System.Threading.Tasks;
using LunchSplit.Core.Payments;
using Square;

namespace LunchSplit.Infrastructure
{
    // Square implementation of the payment gateway contract.
    public class SquarePaymentService : IPaymentProcessor
    {
        // Connection settings (token, location, timeout)
        private readonly SquareSettings _settings;

        // Square SDK client used for every API call
        private readonly SquareClient _client;

        /*
         * METHOD      : SquarePaymentService (constructor)
         * DESCRIPTION : Builds a Square SDK client pointed at the Sandbox with an explicit
         *               HttpClient timeout so live calls cannot hang the test run.
         * PARAMETERS  : SquareSettings settings - credentials and timeout loaded from config
         * RETURNS     : n/a
         */
        public SquarePaymentService(SquareSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _settings = settings;

            // Explicit HttpClient timeout (assignment rule: non-hanging network timeouts)
            TimeSpan timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = timeout;

            // Point the SDK at the Sandbox, reuse our HttpClient and turn off automatic retries
            ClientOptions options = new ClientOptions
            {
                BaseUrl = settings.BaseUrl,
                HttpClient = httpClient,
                Timeout = timeout,
                MaxRetries = 0
            };

            _client = new SquareClient(settings.AccessToken, options);
        }

        /*
         * METHOD      : ProcessPaymentAsync
         * DESCRIPTION : Charges a card through Square's CreatePayment endpoint.
         * PARAMETERS  : PaymentRequest request - domain payment request
         * RETURNS     : Task<PaymentResult> - domain result
         */
        public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        {
            // Not implemented yet - driven by test M3-01 (TDD)
            throw new NotImplementedException();
        }

        /*
         * METHOD      : GetTransactionStatusAsync
         * DESCRIPTION : Reads a payment back from Square's GetPayment endpoint.
         * PARAMETERS  : string transactionId - Square payment id
         * RETURNS     : Task<PaymentStatus> - mapped domain status
         */
        public Task<PaymentStatus> GetTransactionStatusAsync(string transactionId)
        {
            // Not implemented yet - driven by test M3-10 (TDD)
            throw new NotImplementedException();
        }
    }
}
