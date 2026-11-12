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
using Square.Payments;

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
        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // Translate the domain DTO into Square's CreatePayment payload
            CreatePaymentRequest squareRequest = BuildSquareRequest(request);

            try
            {
                // POST /v2/payments on the Square Sandbox
                CreatePaymentResponse response = await _client.Payments.CreateAsync(squareRequest);

                // Translate Square's response back into the domain DTO
                string paymentId = string.Empty;
                if (response.Payment != null && response.Payment.Id != null)
                {
                    paymentId = response.Payment.Id;
                }

                return PaymentResult.Success(paymentId);
            }
            catch (SquareApiException ex)
            {
                // Square answered with a 4xx/5xx status (declined card, bad currency, ...).
                // This is a business failure, not a crash, so return a failed domain result.
                return BuildFailureResult(ex);
            }
        }

        /*
         * METHOD      : BuildFailureResult
         * DESCRIPTION : Converts a Square API error into a failed domain PaymentResult. The first
         *               error in Square's "errors" array holds the main error code.
         * PARAMETERS  : SquareApiException ex - exception thrown by the Square SDK
         * RETURNS     : PaymentResult - IsSuccess = false with ErrorCode / ErrorMessage
         */
        private static PaymentResult BuildFailureResult(SquareApiException ex)
        {
            string errorCode = "UNKNOWN_ERROR";
            string errorMessage = ex.Message;

            if (ex.Errors != null && ex.Errors.Count > 0)
            {
                Error firstError = ex.Errors[0];
                errorCode = firstError.Code.ToString();   // e.g. "CARD_DECLINED"
                if (firstError.Detail != null)
                {
                    errorMessage = firstError.Detail;
                }
            }

            return PaymentResult.Failure(errorCode, errorMessage);
        }

        /*
         * METHOD      : BuildSquareRequest
         * DESCRIPTION : Maps a domain PaymentRequest onto Square's CreatePaymentRequest.
         * PARAMETERS  : PaymentRequest request - domain request
         * RETURNS     : CreatePaymentRequest - Square SDK payload
         */
        private CreatePaymentRequest BuildSquareRequest(PaymentRequest request)
        {
            // Square expects the amount in the smallest currency unit (cents) and an ISO currency code
            Money amountMoney = new Money();
            amountMoney.Amount = ConvertToCents(request.Amount);
            amountMoney.Currency = Currency.FromCustom(request.CurrencyCode.ToUpperInvariant());

            CreatePaymentRequest squareRequest = new CreatePaymentRequest
            {
                SourceId = request.CardNonce,            // card nonce (magic nonce in the sandbox)
                IdempotencyKey = request.IdempotencyKey, // prevents double charges on retries
                AmountMoney = amountMoney,
                LocationId = _settings.LocationId,
                Autocomplete = true,                     // capture immediately so the status becomes COMPLETED
                Note = "LunchSplit share for " + request.PayerName
            };

            return squareRequest;
        }

        /*
         * METHOD      : ConvertToCents
         * DESCRIPTION : Converts a dollar amount to whole cents using banker's rounding,
         *               the same rounding rule used by the A1 Rounder class.
         * PARAMETERS  : decimal amount - amount in dollars (for example 25.50)
         * RETURNS     : long - amount in cents (for example 2550)
         */
        private static long ConvertToCents(decimal amount)
        {
            decimal cents = Math.Round(amount * 100m, 0, MidpointRounding.ToEven);
            return (long)cents;
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
