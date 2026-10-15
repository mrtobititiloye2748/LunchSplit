using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core;
using System.Collections.Generic;

namespace LunchSplit.Tests
{
    // Tests for the Bill class to make sure it stores values correctly
    [TestClass]
    public class BillTests
    {
        [TestMethod]
        public void Bill_StoresSubtotal()
        {
            // Arrange: create empty lists for attendees and shares
            List<Attendee> attendees = new List<Attendee>();
            List<Share> shares = new List<Share>();

            // Create a Bill object with known values
            Bill bill = new Bill(50m, 5m, 10m, attendees, shares);

            // Act: read the subtotal stored inside the Bill
            decimal result = bill.Subtotal;

            // Assert: check that the subtotal is exactly what we passed in
            Assert.AreEqual(50m, result);
        }

        [TestMethod]
        public void Bill_StoresTax()
        {
            // Arrange
            List<Attendee> attendees = new List<Attendee>();
            List<Share> shares = new List<Share>();
            Bill bill = new Bill(50m, 5m, 10m, attendees, shares);

            // Act
            decimal result = bill.Tax;

            // Assert
            Assert.AreEqual(5m, result);
        }

        [TestMethod]
        public void Bill_StoresTip()
        {
            // Arrange
            List<Attendee> attendees = new List<Attendee>();
            List<Share> shares = new List<Share>();
            Bill bill = new Bill(50m, 5m, 10m, attendees, shares);

            // Act
            decimal result = bill.Tip;

            // Assert
            Assert.AreEqual(10m, result);
        }

        [TestMethod]
        public void Bill_StoresAttendees()
        {
            // Arrange: create two attendees
            List<Attendee> attendees = new List<Attendee>();
            attendees.Add(new Attendee("Jack", 2, true));
            attendees.Add(new Attendee("Sam", 1, true));

            // No shares needed for this test
            List<Share> shares = new List<Share>();

            // Create the bill
            Bill bill = new Bill(50m, 5m, 10m, attendees, shares);

            // Act: check how many attendees were stored and the first attendee's name
            int count = bill.Attendees.Count;
            string firstName = bill.Attendees[0].Name;

            // Assert: verify both values
            Assert.AreEqual(2, count);
            Assert.AreEqual("Jack", firstName);
        }

        [TestMethod]
        public void Bill_StoresShares()
        {
            // Arrange: create two share items
            List<Share> shares = new List<Share>();
            shares.Add(new Share("Fries", 4.50m));
            shares.Add(new Share("Burger", 8.00m));

            // No attendees needed for this test
            List<Attendee> attendees = new List<Attendee>();

            // Create the bill
            Bill bill = new Bill(50m, 5m, 10m, attendees, shares);

            // Act: check how many shares were stored and the first share's name
            int count = bill.Shares.Count;
            string firstName = bill.Shares[0].Name;

            // Assert
            Assert.AreEqual(2, count);
            Assert.AreEqual("Fries", firstName);
        }
    }
}