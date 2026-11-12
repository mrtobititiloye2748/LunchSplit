/*
 * FILE          : SquareSandboxTests.cs
 * PROJECT       : CSCN72010 Software Quality I - Assignment 3 (LunchSplit)
 * PROGRAMMER    : mrtobititiloye2748
 * FIRST VERSION : 2026-11-12
 * DESCRIPTION   :
 *   Tier 1 - live integration tests. These tests call the REAL Square Sandbox
 *   API over HTTPS through SquarePaymentService, using Square's magic test
 *   nonces (for example "cnon:card-nonce-ok") instead of real card numbers.
 *   Test IDs: M3-01, M3-02, M3-08, M3-09, M3-10.
 *
 *   If the sandbox credentials are not configured, the tests are reported as
 *   Inconclusive (skipped) with a clear message instead of crashing.
 */

using System;
using System.Threading.Tasks;
using LunchSplit.Core.Payments;
using LunchSplit.Infrastructure;

namespace LunchSplit.Tests.Integration
{
    // Live Square Sandbox integration tests (Tier 1).
    [TestClass]
    public class SquareSandboxTests
    {
        // Upper limit for one live test (ms) - extra safety on top of the 5 second HttpClient timeout
        private const int LiveTestTimeoutMs = 30000;

        // Settings and currency are loaded once for the whole class
        private static SquareSettings _settings = new SquareSettings();
        private static string _currency = "CAD";

        /*
         * METHOD      : ClassInit
         * DESCRIPTION : Loads the sandbox credentials once before any test in this class runs.
         * PARAMETERS  : TestContext context - supplied by MSTest (not used)
         * RETURNS     : void
         */
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _settings = SquareTestConfiguration.LoadSettings();
            _currency = SquareTestConfiguration.LoadCurrency();
        }

        /*
         * METHOD      : CreateService
         * DESCRIPTION : Creates the real Square wrapper, or marks the test Inconclusive when
         *               credentials are missing.
         * PARAMETERS  : none
         * RETURNS     : IPaymentProcessor - the live SquarePaymentService
         */
        private static IPaymentProcessor CreateService()
        {
            if (!_settings.HasCredentials())
            {
                Assert.Inconclusive("Square Sandbox credentials not configured. Set the "
                    + SquareTestConfiguration.AccessTokenKey + " and "
                    + SquareTestConfiguration.LocationIdKey + " environment variables.");
            }

            return new SquarePaymentService(_settings);
        }

        /*
         * METHOD      : BuildRequest
         * DESCRIPTION : Builds a payment request with a brand-new idempotency key.
         * PARAMETERS  : string cardNonce   - Square magic test nonce
         *               decimal amount     - amount in dollars
         *               string currency    - ISO currency code
         * RETURNS     : PaymentRequest - ready to send to the gateway
         */
        private static PaymentRequest BuildRequest(string cardNonce, decimal amount, string currency)
        {
            PaymentRequest request = new PaymentRequest();
            request.PayerName = "Sandbox Tester";
            request.Amount = amount;
            request.CurrencyCode = currency;
            request.CardNonce = cardNonce;

            // A new GUID for each request so independent tests never collide
            request.IdempotencyKey = Guid.NewGuid().ToString();

            return request;
        }

        /*
         * TEST ID     : M3-01
         * DESCRIPTION : Live payment authorisation with the magic nonce "cnon:card-nonce-ok".
         * EXPECTED    : IsSuccess == true and a non-empty TransactionId.
         */
        [TestMethod]
        [Timeout(LiveTestTimeoutMs)]
        public async Task M3_01_ProcessPayment_CardNonceOk_ReturnsSuccessWithTransactionId()
        {
            // Arrange: real Square service and a $12.50 request using the "ok" magic nonce
            IPaymentProcessor service = CreateService();
            PaymentRequest request = BuildRequest("cnon:card-nonce-ok", 12.50m, _currency);

            // Act: send the payment to the live Square Sandbox
            PaymentResult result = await service.ProcessPaymentAsync(request);

            // Assert: the payment was accepted and Square gave us a payment id
            Assert.IsTrue(result.IsSuccess, "Expected success but got: " + result.ErrorCode + " " + result.ErrorMessage);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.TransactionId), "TransactionId should not be empty.");
        }

        /*
         * TEST ID     : M3-02
         * DESCRIPTION : Live card decline with the magic nonce "cnon:card-nonce-declined".
         * EXPECTED    : IsSuccess == false and ErrorCode == "CARD_DECLINED".
         */
        [TestMethod]
        [Timeout(LiveTestTimeoutMs)]
        public async Task M3_02_ProcessPayment_CardNonceDeclined_ReturnsCardDeclinedError()
        {
            // Arrange: real Square service and a request using the "declined" magic nonce
            IPaymentProcessor service = CreateService();
            PaymentRequest request = BuildRequest("cnon:card-nonce-declined", 12.50m, _currency);

            // Act: Square answers with HTTP 400; the wrapper must turn that into a failed result
            PaymentResult result = await service.ProcessPaymentAsync(request);

            // Assert: the payment failed with Square's CARD_DECLINED error code
            Assert.IsFalse(result.IsSuccess, "A declined card must not be reported as a success.");
            Assert.AreEqual("CARD_DECLINED", result.ErrorCode);
        }

        /*
         * TEST ID     : M3-08
         * DESCRIPTION : Live unsupported currency enforcement - charge in EUR against a
         *               sandbox location that only accepts its own currency (CAD).
         * EXPECTED    : IsSuccess == false and Square returns an HTTP 400 validation error.
         */
        [TestMethod]
        [Timeout(LiveTestTimeoutMs)]
        public async Task M3_08_ProcessPayment_UnsupportedCurrency_ReturnsHttp400ValidationError()
        {
            // Arrange: use EUR (or USD if the sandbox location itself happens to be in EUR)
            IPaymentProcessor service = CreateService();
            string wrongCurrency = "EUR";
            if (_currency == "EUR")
            {
                wrongCurrency = "USD";
            }
            PaymentRequest request = BuildRequest("cnon:card-nonce-ok", 12.50m, wrongCurrency);

            // Act: send the mismatched-currency payment to the live sandbox
            PaymentResult result = await service.ProcessPaymentAsync(request);

            // Assert: rejected by Square's request validation with HTTP 400 Bad Request
            Assert.IsFalse(result.IsSuccess, "A payment in the wrong currency must be rejected.");
            Assert.AreEqual(400, result.HttpStatusCode);
        }

        /*
         * TEST ID     : M3-09
         * DESCRIPTION : Live idempotency key verification - the SAME request (same
         *               idempotency_key) is sent twice, as if a client retried after a glitch.
         * EXPECTED    : The second response returns the original TransactionId, proving Square
         *               did not create (charge) a second payment.
         */
        [TestMethod]
        [Timeout(LiveTestTimeoutMs)]
        public async Task M3_09_ProcessPayment_DuplicateIdempotencyKey_ReturnsOriginalTransactionId()
        {
            // Arrange: one request object, so both calls share the exact same idempotency key
            IPaymentProcessor service = CreateService();
            PaymentRequest request = BuildRequest("cnon:card-nonce-ok", 15.75m, _currency);

            // Act: send the identical payment twice
            PaymentResult firstResult = await service.ProcessPaymentAsync(request);
            PaymentResult secondResult = await service.ProcessPaymentAsync(request);

            // Assert: both calls succeed and point at the SAME payment (no double charge)
            Assert.IsTrue(firstResult.IsSuccess, "First payment failed: " + firstResult.ErrorCode);
            Assert.IsTrue(secondResult.IsSuccess, "Replayed payment failed: " + secondResult.ErrorCode);
            Assert.AreEqual(firstResult.TransactionId, secondResult.TransactionId,
                "A duplicate idempotency key must return the original payment, not a new one.");
        }
    }
}
