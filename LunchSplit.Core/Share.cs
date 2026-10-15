namespace LunchSplit.Core
{
    // The Share class represents an extra cost item added to the bill.
    public class Share
    {
        // The name of the share item (for example: "Fries", "Drink", etc.)
        public string Name { get; }

        // The cost of this share item
        public decimal Amount { get; }

        // Constructor to create a share item
        public Share(string name, decimal amount)
        {
            Name = name;
            Amount = amount;
        }
    }
}