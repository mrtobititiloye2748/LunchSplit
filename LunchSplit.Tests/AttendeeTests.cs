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
            var attendee = new Attendee("Tobi", 2, true);

            Assert.AreEqual("Tobi", attendee.Name);
            Assert.AreEqual(2, attendee.Weight);
            Assert.IsTrue(attendee.Included);
        }
    }
}
