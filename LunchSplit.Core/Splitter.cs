using System;
using System.Collections.Generic;
using System.Linq;

namespace LunchSplit.Core
{
    public class Splitter
    {
        private readonly Rounder _rounder = new Rounder();

        public Dictionary<Attendee, decimal> Split(Bill bill)
        {
            if (bill.Attendees.Count == 0)
                return new Dictionary<Attendee, decimal>();

            // 1. Round subtotal, tax, tip
            var subtotal = _rounder.RoundSubtotal(bill.Subtotal);
            var tax = _rounder.RoundTax(bill.Tax);
            var tip = _rounder.RoundTip(bill.Tip);

            // 2. Round shares
            var shareTotal = bill.Shares
                .Select(s => _rounder.RoundShare(s.Amount))
                .Sum();

            // 3. Round attendee weights
            var attendees = bill.Attendees
                .Select(a => new Attendee(a.Name, (int)_rounder.RoundWeight(a.Weight), a.Included))
                .ToList();

            // 4. Compute total weight
            var totalWeight = attendees.Where(a => a.Included).Sum(a => a.Weight);

            if (totalWeight == 0)
                return attendees.ToDictionary(a => a, a => 0m);

            // 5. Compute total bill
            var totalBill = subtotal + tax + tip + shareTotal;

            // 6. Compute each attendee’s share
            var results = new Dictionary<Attendee, decimal>();

            foreach (var attendee in attendees)
            {
                if (!attendee.Included)
                {
                    results[attendee] = 0m;
                    continue;
                }

                var rawSplit = totalBill * (attendee.Weight / (decimal)totalWeight);
                var roundedSplit = _rounder.RoundSplit(rawSplit);

                results[attendee] = roundedSplit;
            }

            return results;
        }
    }
}
