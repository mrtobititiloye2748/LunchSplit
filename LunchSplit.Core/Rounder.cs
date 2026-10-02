using System;

namespace LunchSplit.Core
{
    public class Rounder
    {
        private decimal R(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.ToEven);
        }

        public decimal RoundSubtotal(decimal subtotal) => R(subtotal);
        public decimal RoundTax(decimal tax) => R(tax);
        public decimal RoundTip(decimal tip) => R(tip);
        public decimal RoundShare(decimal amount) => R(amount);
        public decimal RoundWeight(decimal weight) => R(weight);
        public decimal RoundSplit(decimal splitAmount) => R(splitAmount);
    }
}