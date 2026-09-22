using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core;

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
    }
}