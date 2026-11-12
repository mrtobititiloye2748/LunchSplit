/*
 * FILE          : PaymentProcessorMoqTests.cs
 * PROJECT       : CSCN72010 Software Quality I - Assignment 3 (LunchSplit)
 * PROGRAMMER    : mrtobititiloye2748
 * FIRST VERSION : 2026-11-12
 * DESCRIPTION   :
 *   Tier 2 - in-memory unit tests. The unit under test is PaymentCoordinator.
 *   Its only external dependency, IPaymentProcessor, is replaced by a Moq
 *   Mock<IPaymentProcessor>, so these tests never open a network socket and
 *   run in milliseconds. Moq lets us fake gateway outages (.ThrowsAsync),
 *   canned answers (.ReturnsAsync / .SetupSequence) and check how the gateway
 *   was called (.Verify with Times.Once / Never / Exactly).
 *   Test IDs: M3-03, M3-04, M3-05, M3-06, M3-07.
 */

using System.Net.Http;
using System.Threading.Tasks;
using LunchSplit.Core.Payments;
using Moq;

namespace LunchSplit.Tests.Unit
{
    // Moq-based unit tests for PaymentCoordinator (Tier 2).
    [TestClass]
    public class PaymentProcessorMoqTests
    {
        // Magic nonce value passed through to the mock (no real card is ever used)
        private const string TestNonce = "cnon:card-nonce-ok";

        /*
         * TEST ID     : M3-03
         * DESCRIPTION : Simulates a gateway network outage by making the mock throw
         *               HttpRequestException ("503 Service Unavailable").
         * EXPECTED    : PaymentCoordinator wraps it in a PaymentGatewayTimeoutException.
         */
        [TestMethod]
        public async Task M3_03_PayShare_GatewayThrowsHttpRequestException_ThrowsPaymentGatewayTimeoutException()
        {
            // Arrange: the mock gateway fails every call with a network exception
            Mock<IPaymentProcessor> paymentMock = new Mock<IPaymentProcessor>();
            paymentMock.Setup(p => p.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
                       .ThrowsAsync(new HttpRequestException("503 Service Unavailable"));

            PaymentCoordinator coordinator = new PaymentCoordinator(paymentMock.Object);

            // Act + Assert: the low-level network error must surface as the domain timeout exception
            await Assert.ThrowsExceptionAsync<PaymentGatewayTimeoutException>(
                async () => await coordinator.PayShareAsync("Alice", 25.50m, TestNonce));
        }
    }
}
