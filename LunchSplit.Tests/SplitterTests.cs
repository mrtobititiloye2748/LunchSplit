using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core;
using System.Collections.Generic;

namespace LunchSplit.Tests
{
    [TestClass]
    public class SplitterTests
    {
        [TestMethod]
        public void Split_NoAttendees_ReturnsEmpty()
        {
            var bill = new Bill(50m, 5m, 10m, new List<Attendee>(), new List<Share>());
            var splitter = new Splitter();

            var result = splitter.Split(bill);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Split_SingleAttendee_GetsFullAmount()
        {
            var attendees = new List<Attendee>
            {
                new Attendee("Tobi", 1, true)
            };

            var bill = new Bill(50m, 5m, 10m, attendees, new List<Share>());
            var splitter = new Splitter();

            var result = splitter.Split(bill);

            // Total = 50 + 5 + 10 = 65
            Assert.AreEqual(65m, result[attendees[0]]);
        }

        [TestMethod]
        public void Split_TwoAttendees_WeightedSplit()
        {
            var attendees = new List<Attendee>
            {
                new Attendee("Tobi", 2, true),
                new Attendee("Sam", 1, true)
            };

            var bill = new Bill(60m, 6m, 9m, attendees, new List<Share>());
            var splitter = new Splitter();

            var result = splitter.Split(bill);

            // Total = 60 + 6 + 9 = 75
            // Weight = 2 + 1 = 3
            // Tobi = 2/3 * 75 = 50
            // Sam = 1/3 * 75 = 25

            Assert.AreEqual(50m, result[attendees[0]]);
            Assert.AreEqual(25m, result[attendees[1]]);
        }

        [TestMethod]
        public void Split_SharesIncluded()
        {
            var attendees = new List<Attendee>
            {
                new Attendee("Tobi", 1, true),
                new Attendee("Sam", 1, true)
            };

            var shares = new List<Share>
            {
                new Share("Fries", 4.50m),
                new Share("Burger", 8.00m)
            };

            var bill = new Bill(40m, 4m, 6m, attendees, shares);
            var splitter = new Splitter();

            var result = splitter.Split(bill);

            // Total = 40 + 4 + 6 + 12.50 = 62.50
            // Each gets half = 31.25

            Assert.AreEqual(31.25m, result[attendees[0]]);
            Assert.AreEqual(31.25m, result[attendees[1]]);
        }

        [TestMethod]
        public void Split_ExcludedAttendee_GetsZero()
        {
            var attendees = new List<Attendee>
            {
                new Attendee("Tobi", 1, true),
                new Attendee("Sam", 1, false)
            };

            var bill = new Bill(40m, 4m, 6m, attendees, new List<Share>());
            var splitter = new Splitter();

            var result = splitter.Split(bill);

            // Total = 50
            // Only Tobi included → Tobi gets 50, Sam gets 0

            Assert.AreEqual(50m, result[attendees[0]]);
            Assert.AreEqual(0m, result[attendees[1]]);
        }
    }
}
