using Microsoft.VisualStudio.TestTools.UnitTesting;
using LunchSplit.Core;

namespace LunchSplit.Tests
{
    [TestClass]
    public class AttendeeTests
    {
        [TestMethod]
        public void Attendee_StoresNameWeightAndIncluded()
        {
            var attendee = new Attendee
            {
                Name = "Tobi",
                Weight = 2,
                Included = true
            };

            Assert.AreEqual("Tobi", attendee.Name);
            Assert.AreEqual(2, attendee.Weight);
            Assert.IsTrue(attendee.Included);
        }
    }
}
