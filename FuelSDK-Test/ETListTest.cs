using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelSDK.Test
{
    class ETListTest
    {
        ETClient client;
        ETList list;
        string listname;
        string listDesc;
        string listUpdatedDesc;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            client = new ETClient();
        }

        [SetUp]
        public void Setup()
        {
            listname = System.Guid.NewGuid().ToString();
            listDesc = "List Created by C# SDK";
            listUpdatedDesc = "Updated Description created by C# SDK";

            var list = new ETList
            {
                AuthStub = client,
                ListName = listname,
                Description = listDesc
                
            };
            var response = list.Post();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            this.list = (ETList) response.Results[0].Object;

        }

        [TearDown]
        public void TearDown()
        {
            if (this.list != null)
            {
                var list = new ETList
                {
                    AuthStub = client,
                    ListName = listname
                };
                list.Delete();
            }
        }

        [Test()]
        public void ListCreate()
        {
            Assert.AreNotEqual(list, null);
        }

        [Test()]
        public void ListUpdate()
        {
            var list = new ETList
            {
                AuthStub = client,
                ID = this.list.ID,
                Description = listUpdatedDesc
                
            };
            var response = list.Patch();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);

            list = new ETList
            {
                AuthStub = client,
                Props = new string[] { "ID", "ListName", "Description" },
                SearchFilter = new SimpleFilterPart { Property = "ID", SimpleOperator = SimpleOperators.equals, Value = new[] { this.list.ID.ToString() } }

            };

            var getresponse = list.Get();
            Assert.AreEqual(getresponse.Code, 200);
            Assert.AreEqual(getresponse.Status, true);
            var getlist = (ETList)getresponse.Results[0];

            Assert.AreEqual(getlist.Description, listUpdatedDesc);
        }

        [Test()]
        public void ListDelete()
        {
            var list = new ETList
            {
                AuthStub = client,
                ID = this.list.ID
            };
            var response = list.Delete();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            this.list = null;
        }

        [Test()]
        public void ListGet()
        {
            list = new ETList
            {
                AuthStub = client,
                Props = new string[] { "ID", "ListName", "Description" },
                SearchFilter = new SimpleFilterPart { Property = "ID", SimpleOperator = SimpleOperators.equals, Value = new[] { this.list.ID.ToString() } }

            };

            var getresponse = list.Get();
            Assert.AreEqual(getresponse.Code, 200);
            Assert.AreEqual(getresponse.Status, true);
            var getlist = (ETList)getresponse.Results[0];
            Assert.AreEqual(getlist.Description, listDesc);
        }

        [Test()]
        public void ListSubscriberAdd()
        { 
            //list is already created in the setup
            for (int i = 0; i < 10; i++)
            {
                var subsObj = new ETSubscriber
                {
                    AuthStub = client,
                    Lists = new[] {new ETSubscriberList { ID = list.ID }},
                    EmailAddress = string.Format("{0}@gmail.com",System.Guid.NewGuid().ToString()),
                    SubscriberKey = System.Guid.NewGuid().ToString(),
                    Status = SubscriberStatus.Active,
                    Attributes = new[] { new ETProfileAttribute { Name = "FullName", Value = "Test Subscriber_"+i.ToString() } }
                };

                var response = subsObj.Post();
            }

            var listsubs = new ETListSubscriber
            {
                AuthStub = client,
                Props = new string[] { "ObjectID", "SubscriberKey", "CreatedDate", "Client.ID", "Client.PartnerClientKey", "ListID", "Status" },
                SearchFilter = new SimpleFilterPart { Property = "ListID", SimpleOperator = SimpleOperators.equals, Value = new[] { this.list.ID.ToString() } }

            };

            var getresponse = listsubs.Get();
            Assert.AreEqual(getresponse.Code, 200);
            Assert.AreEqual(getresponse.Status, true);
            var getlist = (ETListSubscriber)getresponse.Results[0];
            Assert.AreEqual(getresponse.Results.Length, 10);
        }
    }
}
