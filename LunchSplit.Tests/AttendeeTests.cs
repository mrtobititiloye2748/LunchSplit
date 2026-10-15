using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core;

namespace LunchSplit.Tests
{
    // Tests for the Attendee class to make sure it stores values correctly
    [TestClass]
    public class AttendeeTests
    {
        [TestMethod]
        public void Attendee_StoresNameWeightAndIncluded()
        {
            // Arrange:
            // Create an attendee with known values.
            // Name = "Jack"
            // Weight = 2
            // Included = true
            Attendee attendee = new Attendee("Jack", 2, true);

            // Act:
            // Read back the stored values from the Attendee object.
            string name = attendee.Name;
            int weight = attendee.Weight;
            bool included = attendee.Included;

            // Assert:
            // Verify that the values match exactly what we passed in.
            Assert.AreEqual("Jack", name);
            Assert.AreEqual(2, weight);
            Assert.IsTrue(included);
        }
    }
}