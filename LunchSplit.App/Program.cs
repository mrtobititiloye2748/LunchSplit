using System;
using System.Collections.Generic;
using LunchSplit.Core;

namespace LunchSplit.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== LunchSplit ===");
            Console.WriteLine();

            // Collect subtotal
            Console.Write("Enter subtotal: ");
            var subtotalInput = Console.ReadLine() ?? "0";
            decimal subtotal = decimal.TryParse(subtotalInput, out var subtotalVal) ? subtotalVal : 0m;

            // Collect tax
            Console.Write("Enter tax: ");
            var taxInput = Console.ReadLine() ?? "0";
            decimal tax = decimal.TryParse(taxInput, out var taxVal) ? taxVal : 0m;

            // Collect tip
            Console.Write("Enter tip: ");
            var tipInput = Console.ReadLine() ?? "0";
            decimal tip = decimal.TryParse(tipInput, out var tipVal) ? tipVal : 0m;

            // Collect attendees
            var attendees = new List<Attendee>();
            Console.WriteLine();
            Console.WriteLine("Enter attendees (name, weight, included). Type 'done' to finish.");

            while (true)
            {
                Console.Write("Attendee name (or 'done'): ");
                var name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name) || name.ToLower() == "done")
                    break;

                Console.Write("Weight: ");
                var weightInput = Console.ReadLine() ?? "0";
                int weight = int.TryParse(weightInput, out var weightVal) ? weightVal : 0;

                Console.Write("Included (y/n): ");
                var includedInput = Console.ReadLine() ?? "n";
                bool included = includedInput.Trim().ToLower() == "y";

                attendees.Add(new Attendee(name, weight, included));
                Console.WriteLine();
            }

            // Collect shares
            var shares = new List<Share>();
            Console.WriteLine();
            Console.WriteLine("Enter shares (name, amount). Type 'done' to finish.");

            while (true)
            {
                Console.Write("Share name (or 'done'): ");
                var shareName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(shareName) || shareName.ToLower() == "done")
                    break;

                Console.Write("Amount: ");
                var amountInput = Console.ReadLine() ?? "0";
                decimal amount = decimal.TryParse(amountInput, out var amountVal) ? amountVal : 0m;

                shares.Add(new Share(shareName, amount));
                Console.WriteLine();
            }

            // Build bill
            var bill = new Bill(subtotal, tax, tip, attendees, shares);

            // Compute splits
            var splitter = new Splitter();
            var splits = splitter.Split(bill);

            // Format output
            var formatter = new Formatter();

            Console.WriteLine();
            Console.WriteLine("=== Bill Summary ===");
            Console.WriteLine(formatter.FormatBill(bill));

            Console.WriteLine();
            Console.WriteLine("=== Final Splits ===");
            Console.WriteLine(formatter.FormatSplits(splits));

            Console.WriteLine();
            Console.WriteLine("Done.");
        }
    }
}
