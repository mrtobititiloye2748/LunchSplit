using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit;
using LunchSplit.Core;
using System;

namespace LunchSplit.Tests
{
    [TestClass]
    public class ComputeTipTests
    {
        [TestMethod]
        public void ComputeTip_NoTipMode_ReturnsZero()
        {
            // Arrange
            var splitter = new Splitter();
            decimal subtotal = 100m;
            TipMode mode = TipMode.None;
            decimal input = 0m;

            // Act
            decimal result = splitter.ComputeTip(subtotal, mode, input);

            // Assert
            Assert.AreEqual(0m, result);
        }

        [TestMethod]
        public void ComputeTip_FixedTipMode_ReturnsFixedAmount()
        {
            // Arrange
            var splitter = new Splitter();
            decimal subtotal = 120m;
            TipMode mode = TipMode.Fixed;
            decimal input = 15m;

            // Act
            decimal result = splitter.ComputeTip(subtotal, mode, input);

            // Assert
            Assert.AreEqual(15m, result);
        }

        [TestMethod]
        public void ComputeTip_NegativeSubtotal_ThrowsException()
        {
            // Arrange
            var splitter = new Splitter();
            decimal subtotal = -10m;
            TipMode mode = TipMode.Percent;
            decimal input = 10m;

            // Act + Assert
            try
            {
                splitter.ComputeTip(subtotal, mode, input);
                Assert.Fail("Expected ArgumentOutOfRangeException.");
            }
            catch (ArgumentOutOfRangeException)
            {
                // success
            }

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                splitter.ComputeTip(subtotal, mode, input);
            });
        }
    }
}

