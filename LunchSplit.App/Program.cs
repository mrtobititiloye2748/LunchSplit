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
            Console.Write("Enter the meal subtotal (before tax/tip): ");
            string subtotalInput = Console.ReadLine();
            decimal subtotal = 0m;
            decimal.TryParse(subtotalInput, out subtotal);

            // Collect tax
            Console.Write("Enter the tax amount: ");
            string taxInput = Console.ReadLine();
            decimal tax = 0m;
            decimal.TryParse(taxInput, out tax);

            // Collect tip
            Console.Write("Enter the tip amount: ");
            string tipInput = Console.ReadLine();
            decimal tip = 0m;
            decimal.TryParse(tipInput, out tip);

            // Collect attendees
            List<Attendee> attendees = new List<Attendee>();
            Console.WriteLine();
            Console.WriteLine("Enter each person who is part of the bill.");
            Console.WriteLine("Type 'done' when finished.");
            Console.WriteLine();

            while (true)
            {
                Console.Write("Person's name (or 'done'): ");
                string name = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(name) || name.ToLower() == "done")
                    break;

                Console.Write("How many portions did this person eat? (portion weight): ");
                string weightInput = Console.ReadLine();
                int weight = 0;
                int.TryParse(weightInput, out weight);

                Console.Write("Is this person included in the split? (y/n): ");
                string includedInput = Console.ReadLine();
                bool included = includedInput.Trim().ToLower() == "y";

                attendees.Add(new Attendee(name, weight, included));
                Console.WriteLine();
            }

            // Collect shares
            List<Share> shares = new List<Share>();
            Console.WriteLine();
            Console.WriteLine("Enter any shared items (e.g., fries, nachos).");
            Console.WriteLine("Type 'done' when finished.");
            Console.WriteLine();

            while (true)
            {
                Console.Write("Shared item name (or 'done'): ");
                string shareName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(shareName) || shareName.ToLower() == "done")
                    break;

                Console.Write("Cost of this shared item: ");
                string amountInput = Console.ReadLine();
                decimal amount = 0m;
                decimal.TryParse(amountInput, out amount);

                shares.Add(new Share(shareName, amount));
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