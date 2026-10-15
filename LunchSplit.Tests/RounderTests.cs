using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core;

namespace LunchSplit.Tests
{
    // Tests for the Rounder class to make sure rounding works correctly
    [TestClass]
    public class RounderTests
    {
        [TestMethod]
        public void RoundSubtotal_RoundsToTwoDecimals()
        {
            // Arrange:
            // Create a Rounder object to test its rounding method.
            Rounder rounder = new Rounder();

            // Act:
            // Round the value 12.345 → expected result is 12.35
            decimal result = rounder.RoundSubtotal(12.345m);

            // Assert:
            // Check that the rounded value matches the expected result.
            Assert.AreEqual(12.35m, result);
        }

        [TestMethod]
        public void RoundTax_RoundsToTwoDecimals()
        {
            // Arrange
            Rounder rounder = new Rounder();

            // Act:
            // 5.678 rounds to 5.68 using banker’s rounding.
            decimal result = rounder.RoundTax(5.678m);

            // Assert
            Assert.AreEqual(5.68m, result);
        }

        [TestMethod]
        public void RoundTip_RoundsToTwoDecimals()
        {
            // Arrange
            Rounder rounder = new Rounder();

            // Act:
            // 8.999 rounds to 9.00
            decimal result = rounder.RoundTip(8.999m);

            // Assert
            Assert.AreEqual(9.00m, result);
        }

        [TestMethod]
        public void RoundShare_RoundsToTwoDecimals()
        {
            // Arrange
            Rounder rounder = new Rounder();

            // Act:
            // 4.501 rounds down to 4.50
            decimal result = rounder.RoundShare(4.501m);

            // Assert
            Assert.AreEqual(4.50m, result);
        }

        [TestMethod]
        public void RoundWeight_RoundsToTwoDecimals()
        {
            // Arrange
            Rounder rounder = new Rounder();

            // Act:
            // 2.555 rounds to 2.56
            decimal result = rounder.RoundWeight(2.555m);

            // Assert
            Assert.AreEqual(2.56m, result);
        }

        [TestMethod]
        public void RoundSplit_RoundsToTwoDecimals()
        {
            // Arrange
            Rounder rounder = new Rounder();

            // Act:
            // 17.234 rounds to 17.23
            decimal result = rounder.RoundSplit(17.234m);

            // Assert
            Assert.AreEqual(17.23m, result);
        }
    }
}