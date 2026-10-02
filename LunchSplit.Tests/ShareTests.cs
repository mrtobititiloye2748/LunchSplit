using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core;

namespace LunchSplit.Tests
{
    [TestClass]
    public class ShareTests
    {
        [TestMethod]
        public void Share_StoresNameAndAmount()
        {
            var share = new Share("Fries", 4.50m);

            Assert.AreEqual("Fries", share.Name);
            Assert.AreEqual(4.50m, share.Amount);
        }
    }
}