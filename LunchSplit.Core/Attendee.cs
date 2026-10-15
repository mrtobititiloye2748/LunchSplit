namespace LunchSplit.Core
{
    // The Attendee class represents one person who may share the bill.
    public class Attendee
    {
        // The attendee's name
        public string Name { get; }

        // The weight used for weighted splitting
        public int Weight { get; }

        // Whether this attendee is included in the split
        public bool Included { get; }

        // Constructor to create an attendee
        public Attendee(string name, int weight, bool included)
        {
            Name = name;
            Weight = weight;
            Included = included;
        }
    }
}