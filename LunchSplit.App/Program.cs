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
            string subtotalInput = Console.ReadLine();
            decimal subtotal = 0m;
            decimal.TryParse(subtotalInput, out subtotal);

            // Collect tax
            Console.Write("Enter tax: ");
            string taxInput = Console.ReadLine();
            decimal tax = 0m;
            decimal.TryParse(taxInput, out tax);

            // Collect tip
            Console.Write("Enter tip: ");
            string tipInput = Console.ReadLine();
            decimal tip = 0m;
            decimal.TryParse(tipInput, out tip);

            // Collect attendees
            List<Attendee> attendees = new List<Attendee>();
            Console.WriteLine();
            Console.WriteLine("Enter attendees (name, weight, included). Type 'done' to finish.");

            while (true)
            {
                Console.Write("Attendee name (or 'done'): ");
                string name = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(name) || name.ToLower() == "done")
                {
                    break;
                }

                Console.Write("Weight: ");
                string weightInput = Console.ReadLine();
                int weight = 0;
                int.TryParse(weightInput, out weight);

                Console.Write("Included (y/n): ");
                string includedInput = Console.ReadLine();
                bool included = includedInput.Trim().ToLower() == "y";

                Attendee attendee = new Attendee(name, weight, included);
                attendees.Add(attendee);

                Console.WriteLine();
            }

            // Collect shares
            List<Share> shares = new List<Share>();
            Console.WriteLine();
            Console.WriteLine("Enter shares (name, amount). Type 'done' to finish.");

            while (true)
            {
                Console.Write("Share name (or 'done'): ");
                string shareName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(shareName) || shareName.ToLower() == "done")
                {
                    break;
                }

                Console.Write("Amount: ");
                string amountInput = Console.ReadLine();
                decimal amount = 0m;
                decimal.TryParse(amountInput, out amount);

                Share share = new Share(shareName, amount);
                shares.Add(share);

                Console.WriteLine();
            }

            // Build bill
            Bill bill = new Bill(subtotal, tax, tip, attendees, shares);

            // Compute splits
            Splitter splitter = new Splitter();
            Dictionary<Attendee, decimal> splits = splitter.Split(bill);

            // Format output
            Formatter formatter = new Formatter();

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