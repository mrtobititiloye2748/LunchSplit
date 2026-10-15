using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core;
using System.Collections.Generic;

namespace LunchSplit.Tests
{
    // Tests for the Formatter class to make sure output strings are correct
    [TestClass]
    public class FormatterTests
    {
        [TestMethod]
        public void FormatBill_OutputsCorrectStructure()
        {
            // Arrange: create attendees
            List<Attendee> attendees = new List<Attendee>();
            attendees.Add(new Attendee("Jack", 2, true));
            attendees.Add(new Attendee("Sam", 1, false));

            // Arrange: create shares
            List<Share> shares = new List<Share>();
            shares.Add(new Share("Fries", 4.50m));
            shares.Add(new Share("Burger", 8.00m));

            // Create the bill
            Bill bill = new Bill(50m, 5m, 10m, attendees, shares);

            // Create formatter
            Formatter formatter = new Formatter();

            // Act: get formatted output
            string output = formatter.FormatBill(bill);

            // Assert: check that important lines appear in the output
            StringAssert.Contains(output, "Subtotal: $50.00");
            StringAssert.Contains(output, "Tax: $5.00");
            StringAssert.Contains(output, "Tip: $10.00");

            StringAssert.Contains(output, "Shares:");
            StringAssert.Contains(output, "Fries: $4.50");
            StringAssert.Contains(output, "Burger: $8.00");

            StringAssert.Contains(output, "Attendees:");
            StringAssert.Contains(output, "Jack (weight 2, included)");
            StringAssert.Contains(output, "Sam (weight 1, excluded)");
        }

        [TestMethod]
        public void FormatSplits_OutputsCorrectAmounts()
        {
            // Arrange: create attendees
            List<Attendee> attendees = new List<Attendee>();
            attendees.Add(new Attendee("Jack", 1, true));
            attendees.Add(new Attendee("Sam", 1, true));

            // Arrange: create split results
            Dictionary<Attendee, decimal> splits = new Dictionary<Attendee, decimal>();
            splits.Add(attendees[0], 31.25m);
            splits.Add(attendees[1], 31.25m);

            // Create formatter
            Formatter formatter = new Formatter();

            // Act: get formatted output
            string output = formatter.FormatSplits(splits);

            // Assert: check that the split lines appear
            StringAssert.Contains(output, "Final Splits:");
            StringAssert.Contains(output, "Jack: $31.25");
            StringAssert.Contains(output, "Sam: $31.25");
        }
    }
}