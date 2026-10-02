using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core;
using System.Collections.Generic;

namespace LunchSplit.Tests
{
    [TestClass]
    public class ComputeTipTests
    {
        [TestMethod]
        public void Tip_RoundsCorrectly()
        {
            var rounder = new Rounder();
            var result = rounder.RoundTip(8.999m);
            Assert.AreEqual(9.00m, result);
        }

        [TestMethod]
        public void Tip_IncludedInTotalSplit()
        {
            var attendees = new List<Attendee>
            {
                new Attendee("Tobi", 1, true),
                new Attendee("Sam", 1, true)
            };

            var bill = new Bill(40m, 4m, 6m, attendees, new List<Share>());
            var splitter = new Splitter();

            var result = splitter.Split(bill);

            // Total = 40 + 4 + 6 = 50
            // Each gets half = 25
            Assert.AreEqual(25m, result[attendees[0]]);
            Assert.AreEqual(25m, result[attendees[1]]);
        }

        [TestMethod]
        public void Tip_ZeroTip_LeavesSubtotalAndTaxOnly()
        {
            var attendees = new List<Attendee>
            {
                new Attendee("Tobi", 1, true),
                new Attendee("Sam", 1, true)
            };

            var bill = new Bill(40m, 4m, 0m, attendees, new List<Share>());
            var splitter = new Splitter();

            var result = splitter.Split(bill);

            // Total = 40 + 4 = 44
            // Each gets half = 22
            Assert.AreEqual(22m, result[attendees[0]]);
            Assert.AreEqual(22m, result[attendees[1]]);
        }
    }
}