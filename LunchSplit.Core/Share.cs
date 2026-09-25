namespace LunchSplit.Core
{
    public class Share
    {
        public string Name { get; }
        public decimal Amount { get; }

        public Share(string name, decimal amount)
        {
            Name = name;
            Amount = amount;
        }
    }
}
