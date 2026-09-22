using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core;

namespace LunchSplit.Tests
{
    [TestClass]
    public class BillTests
    {
        [TestMethod]
        public void Bill_StoresValuesCorrectly()
        {
            // Arrange
            decimal subtotal = 100m;
            decimal tax = 13m;
            TipMode mode = TipMode.Percent;
            decimal tipInput = 15m;

            // Act
            var bill = new Bill(subtotal, tax, mode, tipInput);

            // Assert
            Assert.AreEqual(subtotal, bill.Subtotal);
            Assert.AreEqual(tax, bill.Tax);
            Assert.AreEqual(mode, bill.TipMode);
            Assert.AreEqual(tipInput, bill.TipInput);
        }
    }
}