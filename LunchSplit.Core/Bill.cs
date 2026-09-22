namespace LunchSplit.Core
{
    // Tip mode enum as described in the assignment
    public enum TipMode
    {
        None,
        Percent,
        Fixed
    }

    // Immutable Bill model
    public class Bill
    {
        public decimal Subtotal { get; }
        public decimal Tax { get; }
        public TipMode TipMode { get; }
        public decimal TipInput { get; }

        public Bill(decimal subtotal, decimal tax, TipMode tipMode, decimal tipInput)
        {
            Subtotal = subtotal;
            Tax = tax;
            TipMode = tipMode;
            TipInput = tipInput;
        }
    }
}