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

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LunchSplit.Core;
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

        /*
         * TEST ID     : M3-04
         * DESCRIPTION : Parameter and invocation verification - checks that the coordinator
         *               passes the exact dollar amount, currency and nonce to the gateway.
         * EXPECTED    : ProcessPaymentAsync called exactly once with Amount == 25.50 (Mock.Verify).
         */
        [TestMethod]
        public async Task M3_04_PayShare_ValidShare_InvokesProcessorOnceWithExactAmount()
        {
            // Arrange: the mock gateway approves any payment
            Mock<IPaymentProcessor> paymentMock = new Mock<IPaymentProcessor>();
            paymentMock.Setup(p => p.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
                       .ReturnsAsync(PaymentResult.Success("TX_999"));

            PaymentCoordinator coordinator = new PaymentCoordinator(paymentMock.Object);
            decimal expectedAmount = 25.50m;

            // Act: charge Alice her share
            await coordinator.PayShareAsync("Alice", expectedAmount, TestNonce);

            // Assert: the gateway received exactly one request with the exact values we expect
            paymentMock.Verify(p => p.ProcessPaymentAsync(It.Is<PaymentRequest>(r =>
                                    r.Amount == expectedAmount
                                    && r.CurrencyCode == "CAD"
                                    && r.CardNonce == TestNonce
                                    && r.PayerName == "Alice")),
                               Times.Once());

            // No other gateway methods (e.g. status queries) should have been called
            paymentMock.VerifyNoOtherCalls();
        }

        /*
         * TEST ID     : M3-05
         * DESCRIPTION : Precondition short-circuit - a $0.00 bill is handed to the payment manager.
         * EXPECTED    : Validation fails and IPaymentProcessor is NEVER invoked (Times.Never).
         */
        [TestMethod]
        public async Task M3_05_PayBill_ZeroDollarBill_FailsValidationAndNeverCallsProcessor()
        {
            // Arrange: two attendees but the bill total is $0.00 (subtotal, tax and tip all zero)
            List<Attendee> attendees = new List<Attendee>();
            attendees.Add(new Attendee("Alice", 1, true));
            attendees.Add(new Attendee("Bob", 1, true));
            Bill zeroBill = new Bill(0m, 0m, 0m, attendees, new List<Share>());

            Mock<IPaymentProcessor> paymentMock = new Mock<IPaymentProcessor>();
            PaymentCoordinator coordinator = new PaymentCoordinator(paymentMock.Object);

            // Act: try to collect the $0.00 bill
            BatchPaymentSummary summary = await coordinator.PayBillAsync(zeroBill, TestNonce);

            // Assert: the bill is rejected before any network call is made
            Assert.IsFalse(summary.IsValid, "A $0.00 bill must fail validation.");
            paymentMock.Verify(p => p.ProcessPaymentAsync(It.IsAny<PaymentRequest>()), Times.Never());
        }

        /*
         * TEST ID     : M3-06
         * DESCRIPTION : Batch payment iteration across a 4-attendee split bill.
         * EXPECTED    : ProcessPaymentAsync is called exactly 4 times (Times.Exactly(4)),
         *               once for each attendee.
         */
        [TestMethod]
        public async Task M3_06_PayBill_FourAttendees_CallsProcessorExactlyFourTimes()
        {
            // Arrange: $80 + $8 tax + $12 tip = $100 split evenly between 4 attendees ($25 each)
            List<Attendee> attendees = new List<Attendee>();
            attendees.Add(new Attendee("Alice", 1, true));
            attendees.Add(new Attendee("Bob", 1, true));
            attendees.Add(new Attendee("Carol", 1, true));
            attendees.Add(new Attendee("Dave", 1, true));
            Bill bill = new Bill(80m, 8m, 12m, attendees, new List<Share>());

            Mock<IPaymentProcessor> paymentMock = new Mock<IPaymentProcessor>();
            paymentMock.Setup(p => p.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
                       .ReturnsAsync(PaymentResult.Success("TX_BATCH"));

            PaymentCoordinator coordinator = new PaymentCoordinator(paymentMock.Object);

            // Act: collect the whole bill
            await coordinator.PayBillAsync(bill, TestNonce);

            // Assert: one gateway call per attendee - no more, no less
            paymentMock.Verify(p => p.ProcessPaymentAsync(It.IsAny<PaymentRequest>()), Times.Exactly(4));
        }
    }
}
