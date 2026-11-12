using System;
using System.Collections.Generic;

namespace LunchSplit.Core
{
    // The Splitter class calculates how much each attendee owes.
    public class Splitter
    {
        private readonly Rounder _rounder = new Rounder();

        // Splits the bill among attendees using weighted distribution.
        public Dictionary<Attendee, decimal> Split(Bill bill)
        {
            // If no attendees, return an empty dictionary
            if (bill.Attendees.Count == 0)
            {
                return new Dictionary<Attendee, decimal>();
            }

            // 1. Round subtotal, tax, and tip
            decimal subtotal = _rounder.RoundSubtotal(bill.Subtotal);
            decimal tax = _rounder.RoundTax(bill.Tax);
            decimal tip = _rounder.RoundTip(bill.Tip);

            // 2. Round all share amounts
            decimal shareTotal = 0m;
            foreach (Share share in bill.Shares)
            {
                decimal roundedShare = _rounder.RoundShare(share.Amount);
                shareTotal += roundedShare;
            }

            // 3. Round attendee weights.
            //    The rounded weights are kept in a separate list (same order as bill.Attendees)
            //    so the ORIGINAL Attendee objects can be used as dictionary keys. Callers look
            //    up results with the attendee objects they passed in (result[attendees[0]]).
            List<int> roundedWeights = new List<int>();
            foreach (Attendee attendee in bill.Attendees)
            {
                int roundedWeight = (int)_rounder.RoundWeight(attendee.Weight);
                roundedWeights.Add(roundedWeight);
            }

            // 4. Compute total weight of included attendees
            int totalWeight = 0;
            for (int i = 0; i < bill.Attendees.Count; i++)
            {
                if (bill.Attendees[i].Included)
                {
                    totalWeight += roundedWeights[i];
                }
            }

            // If nobody is included, everyone owes 0
            if (totalWeight == 0)
            {
                Dictionary<Attendee, decimal> zeroResults = new Dictionary<Attendee, decimal>();
                foreach (Attendee attendee in bill.Attendees)
                {
                    zeroResults.Add(attendee, 0m);
                }
                return zeroResults;
            }

            // 5. Compute total bill
            decimal totalBill = subtotal + tax + tip + shareTotal;

            // 6. Compute each attendee's share (keyed by the original Attendee object)
            Dictionary<Attendee, decimal> results = new Dictionary<Attendee, decimal>();

            for (int i = 0; i < bill.Attendees.Count; i++)
            {
                Attendee attendee = bill.Attendees[i];

                // If attendee is excluded, they owe 0
                if (!attendee.Included)
                {
                    results.Add(attendee, 0m);
                    continue;
                }

                // Compute weighted share
                decimal weightRatio = roundedWeights[i] / (decimal)totalWeight;
                decimal rawSplit = totalBill * weightRatio;

                // Round the final split
                decimal roundedSplit = _rounder.RoundSplit(rawSplit);

                results.Add(attendee, roundedSplit);
            }

            return results;
        }
    }
}