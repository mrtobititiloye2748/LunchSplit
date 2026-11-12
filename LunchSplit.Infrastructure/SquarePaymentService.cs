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

        // Domain error code used by LunchSplit for any generic card decline
        private const string DomainCardDeclined = "CARD_DECLINED";

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
         * DESCRIPTION : Converts a Square API error into a failed domain PaymentResult.
         *               Square can return several errors; if any of them is CARD_DECLINED that
         *               one is used, otherwise the first error is used. Square's generic
         *               decline codes are then normalised to the domain code "CARD_DECLINED"
         *               (anti-corruption layer), while the raw Square code is kept in
         *               GatewayErrorCode.
         * PARAMETERS  : SquareApiException ex - exception thrown by the Square SDK
         * RETURNS     : PaymentResult - IsSuccess = false with ErrorCode / ErrorMessage / HttpStatusCode
         */
        private static PaymentResult BuildFailureResult(SquareApiException ex)
        {
            string gatewayCode = "UNKNOWN_ERROR";
            string errorMessage = ex.Message;

            if (ex.Errors != null && ex.Errors.Count > 0)
            {
                // Default to the first error, but prefer an explicit CARD_DECLINED if present
                Error chosenError = ex.Errors[0];
                foreach (Error error in ex.Errors)
                {
                    if (error.Code.ToString() == DomainCardDeclined)
                    {
                        chosenError = error;
                        break;
                    }
                }

                gatewayCode = chosenError.Code.ToString();   // e.g. "GENERIC_DECLINE"
                if (chosenError.Detail != null)
                {
                    errorMessage = chosenError.Detail;
                }
            }

            // Translate Square's code into the LunchSplit domain code
            string domainCode = gatewayCode;
            if (IsGenericDeclineCode(gatewayCode))
            {
                domainCode = DomainCardDeclined;
            }

            // Keep Square's HTTP status (e.g. 400 Bad Request) so callers can tell validation errors apart
            PaymentResult result = PaymentResult.Failure(domainCode, errorMessage, ex.StatusCode);
            result.GatewayErrorCode = gatewayCode;
            return result;
        }

        /*
         * METHOD      : IsGenericDeclineCode
         * DESCRIPTION : Checks whether a Square error code means "the bank declined the card"
         *               without a more specific reason. Specific reasons such as
         *               INSUFFICIENT_FUNDS or CVV_FAILURE are deliberately NOT included so
         *               callers can still tell them apart.
         * PARAMETERS  : string squareCode - error code returned by Square
         * RETURNS     : bool - true when the code is a generic card decline
         */
        private static bool IsGenericDeclineCode(string squareCode)
        {
            switch (squareCode)
            {
                case "CARD_DECLINED":
                case "GENERIC_DECLINE":
                case "CARD_DECLINED_CALL_ISSUER":
                case "CARD_DECLINED_VERIFICATION_REQUIRED":
                    return true;
                default:
                    return false;
            }
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
        public async Task<PaymentStatus> GetTransactionStatusAsync(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                throw new ArgumentException("A transaction id is required.", nameof(transactionId));
            }

            try
            {
                // GET /v2/payments/{payment_id} on the Square Sandbox
                GetPaymentsRequest squareRequest = new GetPaymentsRequest
                {
                    PaymentId = transactionId
                };
                GetPaymentResponse response = await _client.Payments.GetAsync(squareRequest);

                string squareStatus = string.Empty;
                if (response.Payment != null && response.Payment.Status != null)
                {
                    squareStatus = response.Payment.Status;
                }

                return MapStatus(squareStatus);
            }
            catch (SquareApiException)
            {
                // Payment not found or request rejected - the status cannot be determined
                return PaymentStatus.Unknown;
            }
        }

        /*
         * METHOD      : MapStatus
         * DESCRIPTION : Translates Square's payment status text into the domain enum.
         * PARAMETERS  : string squareStatus - e.g. "COMPLETED", "APPROVED", "FAILED"
         * RETURNS     : PaymentStatus - matching domain value (Unknown if not recognised)
         */
        private static PaymentStatus MapStatus(string squareStatus)
        {
            switch (squareStatus.ToUpperInvariant())
            {
                case "APPROVED":
                    return PaymentStatus.Approved;
                case "PENDING":
                    return PaymentStatus.Pending;
                case "COMPLETED":
                    return PaymentStatus.Completed;
                case "CANCELED":
                    return PaymentStatus.Canceled;
                case "FAILED":
                    return PaymentStatus.Failed;
                default:
                    return PaymentStatus.Unknown;
            }
        }
    }
}
