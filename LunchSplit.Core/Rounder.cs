using System;

namespace LunchSplit.Core
{
    // The Rounder class handles rounding values to two decimal places.
    public class Rounder
    {
        // Rounds a decimal value using standard banker’s rounding.
        private decimal RoundValue(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.ToEven);
        }

        // Rounds the subtotal amount
        public decimal RoundSubtotal(decimal subtotal)
        {
            return RoundValue(subtotal);
        }

        // Rounds the tax amount
        public decimal RoundTax(decimal tax)
        {
            return RoundValue(tax);
        }

        // Rounds the tip amount
        public decimal RoundTip(decimal tip)
        {
            return RoundValue(tip);
        }

        // Rounds a share amount
        public decimal RoundShare(decimal amount)
        {
            return RoundValue(amount);
        }

        // Rounds an attendee weight
        public decimal RoundWeight(decimal weight)
        {
            return RoundValue(weight);
        }

        // Rounds the final split amount
        public decimal RoundSplit(decimal splitAmount)
        {
            return RoundValue(splitAmount);
        }
    }
}
