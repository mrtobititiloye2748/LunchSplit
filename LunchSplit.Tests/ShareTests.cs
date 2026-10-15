using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core;

namespace LunchSplit.Tests
{
    // Tests for the Share class to make sure it stores values correctly
    [TestClass]
    public class ShareTests
    {
        [TestMethod]
        public void Share_StoresNameAndAmount()
        {
            // Arrange:
            // Create a Share object with known values.
            // Name = "Fries"
            // Amount = 4.50
            Share share = new Share("Fries", 4.50m);

            // Act:
            // Read back the stored values from the Share object.
            string name = share.Name;
            decimal amount = share.Amount;

            // Assert:
            // Verify that the values match exactly what we passed in.
            Assert.AreEqual("Fries", name);
            Assert.AreEqual(4.50m, amount);
        }
    }
}