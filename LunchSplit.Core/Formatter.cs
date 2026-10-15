using System.Collections.Generic;
using System.Text;

namespace LunchSplit.Core
{
    // The Formatter class builds readable text output for the bill and final splits.
    public class Formatter
    {
        // Formats the bill details into a readable string
        public string FormatBill(Bill bill)
        {
            StringBuilder sb = new StringBuilder();

            // Basic bill amounts
            sb.AppendLine("Subtotal: $" + bill.Subtotal.ToString("F2"));
            sb.AppendLine("Tax: $" + bill.Tax.ToString("F2"));
            sb.AppendLine("Tip: $" + bill.Tip.ToString("F2"));
            sb.AppendLine();

            // Shares section
            sb.AppendLine("Shares:");
            if (bill.Shares.Count == 0)
            {
                sb.AppendLine(" (none)");
            }
            else
            {
                foreach (Share share in bill.Shares)
                {
                    sb.AppendLine(" - " + share.Name + ": $" + share.Amount.ToString("F2"));
                }
            }

            sb.AppendLine();

            // Attendees section
            sb.AppendLine("Attendees:");
            foreach (Attendee attendee in bill.Attendees)
            {
                string includedText = attendee.Included ? "included" : "excluded";
                sb.AppendLine(" - " + attendee.Name + " (weight " + attendee.Weight + ", " + includedText + ")");
            }

            return sb.ToString().TrimEnd();
        }

        // Formats the final split amounts into a readable string
        public string FormatSplits(Dictionary<Attendee, decimal> splits)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Final Splits:");

            foreach (KeyValuePair<Attendee, decimal> entry in splits)
            {
                Attendee attendee = entry.Key;
                decimal amount = entry.Value;

                sb.AppendLine(" - " + attendee.Name + ": $" + amount.ToString("F2"));
            }

            return sb.ToString().TrimEnd();
        }
    }
}