using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace FuelSDK.Test
{
    class ETOpenEventTest
    {
        ETClient client;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            client = new ETClient();
        }

        [Test()]
        public void OpenEvent()
        {
            var filterDate = DateTime.Now.AddDays(-30);
            var oe = new ETOpenEvent
            {
                AuthStub = client,
                SearchFilter = new SimpleFilterPart { Property = "EventDate", SimpleOperator = SimpleOperators.greaterThan, DateValue = new[] { filterDate } },
                Props = new[] { "SendID", "SubscriberKey", "EventDate", "Client.ID", "EventType", "BatchID", "TriggeredSendDefinitionObjectID", "PartnerKey" },
            };
            var response = oe.Get();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            
        }
    }
}
