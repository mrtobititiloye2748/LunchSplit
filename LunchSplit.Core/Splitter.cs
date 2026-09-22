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

            return mode switch
            {
                TipMode.None => 0m,
                TipMode.Percent => subtotal * (tipInput / 100m),
                TipMode.Fixed => tipInput,
                _ => 0m
            };
        }
    }
}