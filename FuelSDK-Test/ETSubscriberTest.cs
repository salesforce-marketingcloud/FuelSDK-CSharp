using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelSDK.Test
{
    class ETSubscriberTest
    {
        ETClient client;
        ETSubscriber subscriber;
        string subsKey;
        string subsEmail;


        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            client = new ETClient();
        }
        [SetUp]
        public void Setup()
        {
            subsEmail = "testcsharp@gmail.com";
            subsKey = System.Guid.NewGuid().ToString();

            var subsObj = new ETSubscriber
            {
                AuthStub = client,
                EmailAddress = subsEmail,
                SubscriberKey = subsKey,
                Status = SubscriberStatus.Active,
                Attributes = new[] { new ETProfileAttribute { Name = "FullName", Value = "Test Subscriber" } }
            };

            var response = subsObj.Post();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            subscriber = (ETSubscriber)response.Results[0].Object;
        }

        [TearDown]
        public void TearDown()
        {
            if (subscriber != null)
            {
                var subsObj = new ETSubscriber
                {
                    AuthStub = client,
                    EmailAddress = subsEmail,
                };
                var response = subsObj.Delete();
            }
        }

        [Test()]
        public void SubscriberCreate()
        {
            Assert.AreNotEqual(subscriber, null);
            Assert.AreEqual(subscriber.EmailAddress, subsEmail);
        }

        [Test()]
        public void SubscriberGet()
        {
            var subsObj = new ETSubscriber
            {
                AuthStub = client,
                Props = new[] { "EmailAddress", "ID" },
                SearchFilter = new SimpleFilterPart { Property = "ID", Value = new[] { subscriber.ID.ToString() }, SimpleOperator = SimpleOperators.equals }
            };
            var response = subsObj.Get();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            var subs = (ETSubscriber)response.Results[0];
            Assert.AreEqual(subs.EmailAddress, subsEmail);
        }

        [Test()]
        public void SubscriberUpdate()
        {
            var subsObj = new ETSubscriber
            {
                AuthStub = client,
                EmailAddress = subsEmail,
                SubscriberKey = subsKey,
                Attributes = new[] { new ETProfileAttribute { Name = "FullName", Value = "Updated FullName" } }
            };
            var response = subsObj.Patch();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            Assert.AreEqual(response.Results[0].StatusMessage, "Updated Subscriber.");
        }

        [Test()]
        public void SubscriberDelete()
        {
            var subsObj = new ETSubscriber
            {
                AuthStub = client,
                SubscriberKey = subsKey
            };
            var response = subsObj.Delete();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            Assert.AreEqual(response.Results[0].StatusMessage, "Subscriber deleted");
            subscriber = null;
        }

        


    }
}
