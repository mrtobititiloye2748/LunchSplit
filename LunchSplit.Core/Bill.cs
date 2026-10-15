using System.Collections.Generic;

namespace LunchSplit.Core
{
    // The Bill class stores all information needed to calculate the final split.
    public class Bill
    {
        // The original subtotal before tax or tip
        public decimal Subtotal { get; }

        // The tax amount applied to the bill
        public decimal Tax { get; }

        // The tip amount applied to the bill
        public decimal Tip { get; }

        // List of attendees who will share the bill
        public List<Attendee> Attendees { get; }

        // List of additional share items (like food items or extras)
        public List<Share> Shares { get; }

        // Constructor to build a Bill object
        public Bill(decimal subtotal, decimal tax, decimal tip, List<Attendee> attendees, List<Share> shares)
        {
            Subtotal = subtotal;
            Tax = tax;
            Tip = tip;

            // Store attendees list
            Attendees = attendees;

            // Store share list
            Shares = shares;
        }
    }
}