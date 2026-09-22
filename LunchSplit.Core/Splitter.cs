using System;

namespace LunchSplit.Core
{
    public class Splitter
    {
        public decimal ComputeTip(decimal subtotal, TipMode mode, decimal tipInput)
        {
            if (subtotal < 0)
                throw new ArgumentOutOfRangeException(nameof(subtotal));

            if (mode == TipMode.Fixed && tipInput < 0)
                throw new ArgumentOutOfRangeException(nameof(tipInput));

            if (mode == TipMode.None)
                return 0m;

            if (mode == TipMode.Percent)
                return subtotal * (tipInput / 100m);

            if (mode == TipMode.Fixed)
                return tipInput;

            return 0m;
        }
    }
}