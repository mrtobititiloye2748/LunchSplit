using System.Collections.Generic;

namespace LunchSplit.Core
{
    public class Bill
    {
        public decimal Subtotal { get; }
        public decimal Tax { get; }
        public decimal Tip { get; }
        public List<Attendee> Attendees { get; }
        public List<Share> Shares { get; }

        public Bill(decimal subtotal, decimal tax, decimal tip, List<Attendee> attendees, List<Share> shares)
        {
            Subtotal = subtotal;
            Tax = tax;
            Tip = tip;
            Attendees = attendees;
            Shares = shares;
        }
    }
}
