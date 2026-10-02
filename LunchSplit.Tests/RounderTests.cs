using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core;

namespace LunchSplit.Tests
{
    [TestClass]
    public class RounderTests
    {
        [TestMethod]
        public void RoundSubtotal_RoundsToTwoDecimals()
        {
            var rounder = new Rounder();
            var result = rounder.RoundSubtotal(12.345m);
            Assert.AreEqual(12.35m, result);
        }

        [TestMethod]
        public void RoundTax_RoundsToTwoDecimals()
        {
            var rounder = new Rounder();
            var result = rounder.RoundTax(5.678m);
            Assert.AreEqual(5.68m, result);
        }

        [TestMethod]
        public void RoundTip_RoundsToTwoDecimals()
        {
            var rounder = new Rounder();
            var result = rounder.RoundTip(8.999m);
            Assert.AreEqual(9.00m, result);
        }

        [TestMethod]
        public void RoundShare_RoundsToTwoDecimals()
        {
            var rounder = new Rounder();
            var result = rounder.RoundShare(4.501m);
            Assert.AreEqual(4.50m, result);
        }

        [TestMethod]
        public void RoundWeight_RoundsToTwoDecimals()
        {
            var rounder = new Rounder();
            var result = rounder.RoundWeight(2.555m);
            Assert.AreEqual(2.56m, result);
        }

        [TestMethod]
        public void RoundSplit_RoundsToTwoDecimals()
        {
            var rounder = new Rounder();
            var result = rounder.RoundSplit(17.234m);
            Assert.AreEqual(17.23m, result);
        }
    }
}
