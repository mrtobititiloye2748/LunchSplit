using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core;
using System.Collections.Generic;

namespace LunchSplit.Tests
{
    [TestClass]
    public class FormatterTests
    {
        [TestMethod]
        public void FormatBill_OutputsCorrectStructure()
        {
            var attendees = new List<Attendee>
            {
                new Attendee("Tobi", 2, true),
                new Attendee("Sam", 1, false)
            };

            var shares = new List<Share>
            {
                new Share("Fries", 4.50m),
                new Share("Burger", 8.00m)
            };

            var bill = new Bill(50m, 5m, 10m, attendees, shares);
            var formatter = new Formatter();

            var output = formatter.FormatBill(bill);

            StringAssert.Contains(output, "Subtotal: $50.00");
            StringAssert.Contains(output, "Tax: $5.00");
            StringAssert.Contains(output, "Tip: $10.00");

            StringAssert.Contains(output, "Shares:");
            StringAssert.Contains(output, "Fries: $4.50");
            StringAssert.Contains(output, "Burger: $8.00");

            StringAssert.Contains(output, "Attendees:");
            StringAssert.Contains(output, "Tobi (weight 2, included)");
            StringAssert.Contains(output, "Sam (weight 1, excluded)");
        }

        [TestMethod]
        public void FormatSplits_OutputsCorrectAmounts()
        {
            var attendees = new List<Attendee>
            {
                new Attendee("Tobi", 1, true),
                new Attendee("Sam", 1, true)
            };

            var splits = new Dictionary<Attendee, decimal>
            {
                { attendees[0], 31.25m },
                { attendees[1], 31.25m }
            };

            var formatter = new Formatter();
            var output = formatter.FormatSplits(splits);

            StringAssert.Contains(output, "Final Splits:");
            StringAssert.Contains(output, "Tobi: $31.25");
            StringAssert.Contains(output, "Sam: $31.25");
        }
    }
}
