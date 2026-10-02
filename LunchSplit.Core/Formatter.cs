using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LunchSplit.Core
{
    public class Formatter
    {
        public string FormatBill(Bill bill)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Subtotal: ${bill.Subtotal:F2}");
            sb.AppendLine($"Tax: ${bill.Tax:F2}");
            sb.AppendLine($"Tip: ${bill.Tip:F2}");
            sb.AppendLine();

            sb.AppendLine("Shares:");
            if (bill.Shares.Count == 0)
            {
                sb.AppendLine(" (none)");
            }
            else
            {
                foreach (var share in bill.Shares)
                    sb.AppendLine($" - {share.Name}: ${share.Amount:F2}");
            }

            sb.AppendLine();
            sb.AppendLine("Attendees:");
            foreach (var attendee in bill.Attendees)
            {
                var included = attendee.Included ? "included" : "excluded";
                sb.AppendLine($" - {attendee.Name} (weight {attendee.Weight}, {included})");
            }

            return sb.ToString().TrimEnd();
        }

        public string FormatSplits(Dictionary<Attendee, decimal> splits)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Final Splits:");
            foreach (var kvp in splits)
            {
                var attendee = kvp.Key;
                var amount = kvp.Value;

                sb.AppendLine($" - {attendee.Name}: ${amount:F2}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
