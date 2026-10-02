using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core;
using System.Collections.Generic;

namespace LunchSplit.Tests
{
    [TestClass]
    public class BillTests
    {
        [TestMethod]
        public void Bill_StoresSubtotal()
        {
            var bill = new Bill(50m, 5m, 10m, new List<Attendee>(), new List<Share>());
            Assert.AreEqual(50m, bill.Subtotal);
        }

        [TestMethod]
        public void Bill_StoresTax()
        {
            var bill = new Bill(50m, 5m, 10m, new List<Attendee>(), new List<Share>());
            Assert.AreEqual(5m, bill.Tax);
        }

        [TestMethod]
        public void Bill_StoresTip()
        {
            var bill = new Bill(50m, 5m, 10m, new List<Attendee>(), new List<Share>());
            Assert.AreEqual(10m, bill.Tip);
        }

        [TestMethod]
        public void Bill_StoresAttendees()
        {
            var attendees = new List<Attendee>
            {
                new Attendee("Tobi", 2, true),
                new Attendee("Sam", 1, true)
            };

            var bill = new Bill(50m, 5m, 10m, attendees, new List<Share>());

            Assert.AreEqual(2, bill.Attendees.Count);
            Assert.AreEqual("Tobi", bill.Attendees[0].Name);
        }

        [TestMethod]
        public void Bill_StoresShares()
        {
            var shares = new List<Share>
            {
                new Share("Fries", 4.50m),
                new Share("Burger", 8.00m)
            };

            var bill = new Bill(50m, 5m, 10m, new List<Attendee>(), shares);

            Assert.AreEqual(2, bill.Shares.Count);
            Assert.AreEqual("Fries", bill.Shares[0].Name);
        }
    }
}
