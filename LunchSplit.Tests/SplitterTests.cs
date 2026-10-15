using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core;
using System.Collections.Generic;

namespace LunchSplit.Tests
{
    // Tests for the Splitter class to make sure splitting logic works
    [TestClass]
    public class SplitterTests
    {
        [TestMethod]
        public void Split_NoAttendees_ReturnsEmpty()
        {
            // Arrange: no attendees and no shares
            List<Attendee> attendees = new List<Attendee>();
            List<Share> shares = new List<Share>();
            Bill bill = new Bill(50m, 5m, 10m, attendees, shares);

            Splitter splitter = new Splitter();

            // Act: split the bill
            Dictionary<Attendee, decimal> result = splitter.Split(bill);

            // Assert: no attendees means no results
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Split_SingleAttendee_GetsFullAmount()
        {
            // Arrange: one attendee included
            List<Attendee> attendees = new List<Attendee>();
            attendees.Add(new Attendee("Jack", 1, true));

            List<Share> shares = new List<Share>();

            // Bill total = subtotal + tax + tip = 50 + 5 + 10 = 65
            Bill bill = new Bill(50m, 5m, 10m, attendees, shares);

            Splitter splitter = new Splitter();

            // Act
            Dictionary<Attendee, decimal> result = splitter.Split(bill);

            // Assert: Jack should pay the full amount
            Assert.AreEqual(65m, result[attendees[0]]);
        }

        [TestMethod]
        public void Split_TwoAttendees_WeightedSplit()
        {
            // Arrange: Jack weight 2, Sam weight 1
            List<Attendee> attendees = new List<Attendee>();
            attendees.Add(new Attendee("Jack", 2, true));
            attendees.Add(new Attendee("Sam", 1, true));

            List<Share> shares = new List<Share>();

            // Total bill = 60 + 6 + 9 = 75
            // Total weight = 2 + 1 = 3
            // Jack gets 2/3 of 75 = 50
            // Sam gets 1/3 of 75 = 25
            Bill bill = new Bill(60m, 6m, 9m, attendees, shares);

            Splitter splitter = new Splitter();

            // Act
            Dictionary<Attendee, decimal> result = splitter.Split(bill);

            // Assert
            Assert.AreEqual(50m, result[attendees[0]]);
            Assert.AreEqual(25m, result[attendees[1]]);
        }

        [TestMethod]
        public void Split_SharesIncluded()
        {
            // Arrange: two equal attendees
            List<Attendee> attendees = new List<Attendee>();
            attendees.Add(new Attendee("Jack", 1, true));
            attendees.Add(new Attendee("Sam", 1, true));

            // Shares total = 4.50 + 8.00 = 12.50
            List<Share> shares = new List<Share>();
            shares.Add(new Share("Fries", 4.50m));
            shares.Add(new Share("Burger", 8.00m));

            // Total bill = 40 + 4 + 6 + 12.50 = 62.50
            // Each gets half = 31.25
            Bill bill = new Bill(40m, 4m, 6m, attendees, shares);

            Splitter splitter = new Splitter();

            // Act
            Dictionary<Attendee, decimal> result = splitter.Split(bill);

            // Assert
            Assert.AreEqual(31.25m, result[attendees[0]]);
            Assert.AreEqual(31.25m, result[attendees[1]]);
        }

        [TestMethod]
        public void Split_ExcludedAttendee_GetsZero()
        {
            // Arrange: Jack included, Sam excluded
            List<Attendee> attendees = new List<Attendee>();
            attendees.Add(new Attendee("Jack", 1, true));
            attendees.Add(new Attendee("Sam", 1, false));

            List<Share> shares = new List<Share>();

            // Total bill = 40 + 4 + 6 = 50
            Bill bill = new Bill(40m, 4m, 6m, attendees, shares);

            Splitter splitter = new Splitter();

            // Act
            Dictionary<Attendee, decimal> result = splitter.Split(bill);

            // Assert: Jack pays 50, Sam pays 0
            Assert.AreEqual(50m, result[attendees[0]]);
            Assert.AreEqual(0m, result[attendees[1]]);
        }
    }
}