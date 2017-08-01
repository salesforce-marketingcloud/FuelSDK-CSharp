using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelSDK.Test
{
    class ETQueryDefinitionTest
    {
        ETClient client;
        ETQueryDefinition queryDef;
        ETDataExtension dataExtension;
        string dataExtensionName;
        string targetDEKey;
        string queryDefKey;
        string desc;
        string updatedDesc;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            client = new ETClient();
        }

        [SetUp]
        public void Setup()
        {
            dataExtensionName = Guid.NewGuid().ToString();
            targetDEKey = Guid.NewGuid().ToString();
            desc = "Query definition created by C# SDK";
            updatedDesc = "Updated Query definition created by C# SDK";
            var deObj = new ETDataExtension
            {
                AuthStub = client,
                Name = dataExtensionName,
                CustomerKey = dataExtensionName,
                Columns = new[] { 
                    new ETDataExtensionColumn { Name = "CustomerNumber", FieldType = DataExtensionFieldType.Text, IsPrimaryKey = true, MaxLength = 100, IsRequired = true },
                    new ETDataExtensionColumn { Name = "StoreId", FieldType = DataExtensionFieldType.Number },
                    new ETDataExtensionColumn { Name = "FirstName", FieldType = DataExtensionFieldType.Text, MaxLength=256 } ,
                    new ETDataExtensionColumn { Name = "LastName", FieldType = DataExtensionFieldType.Text, MaxLength=256 } 
                }
            };

            var result = deObj.Post();
            dataExtension = (ETDataExtension)result.Results[0].Object;

            for (int i = 0; i < 3; i++)
            {
                var deRow = new ETDataExtensionRow
                {
                    AuthStub = client,
                    DataExtensionName = dataExtensionName,
                };
                deRow.ColumnValues.Add("CustomerNumber", i.ToString());
                deRow.ColumnValues.Add("StoreId", i % 2 == 0 ? "101" : "102");
                deRow.ColumnValues.Add("FirstName", "FirstName_" + i.ToString());
                deRow.ColumnValues.Add("LastName", "LastName_" + i.ToString());

                var derResponse = deRow.Post();
                Assert.AreEqual(derResponse.Code, 200);
                Assert.AreEqual(derResponse.Results[0].StatusMessage, "Created DataExtensionObject");

            }

            queryDefKey = Guid.NewGuid().ToString();
            var qDef = new ETQueryDefinition
            {
                AuthStub = client,
                Name = queryDefKey,
                Description = desc,
                CustomerKey = queryDefKey,
                DataExtensionTarget = new InteractionBaseObject { CustomerKey = targetDEKey, Name = "Store102DEx" },
                QueryText = string.Format("SELECT * FROM [{0}] WHERE STOREID=102", dataExtensionName),
                TargetType = "DE",
                TargetUpdateType = "Overwrite"


            };
            var qdefResponse = qDef.Post();
            Assert.AreEqual(qdefResponse.Code, 200);
            Assert.AreEqual(qdefResponse.Status, true);
            Assert.AreEqual(qdefResponse.Results[0].StatusMessage, "QueryDefinition created");
            queryDef = (ETQueryDefinition)qdefResponse.Results[0].Object;

        }

        [TearDown]
        public void TearDown()
        {
            var qDef = new ETQueryDefinition
            {
                AuthStub = client,
                CustomerKey = queryDefKey,
                ObjectID = queryDef.ObjectID

            };
            var qdefResponse = qDef.Delete();

            var deObj = new ETDataExtension
            {
                AuthStub = client,
                CustomerKey = dataExtensionName,

            };
            var result = deObj.Delete();

        }

        [Test()]
        public void QueryDefinitionCreate()
        {
            Assert.AreNotEqual(queryDef, null);
        }

        [Test()]
        public void QueryDefinitionGet()
        {
            var qDef = new ETQueryDefinition
            {
                AuthStub = client,
                CustomerKey = queryDefKey,
                ObjectID = queryDef.ObjectID,
                Props = new[] { "Name", "ObjectID", "Description" },
                SearchFilter = new SimpleFilterPart { Property = "ObjectID", Value = new[] { queryDef.ObjectID.ToString() }, SimpleOperator = SimpleOperators.equals }

            };
            var qdefResponse = qDef.Get();
            Assert.AreEqual(qdefResponse.Code, 200);
            Assert.AreEqual(qdefResponse.Status, true);
            var def = (ETQueryDefinition)qdefResponse.Results[0];
            Assert.AreEqual(def.Name, queryDefKey);
            Assert.AreEqual(def.Description, desc);

        }

        [Test()]
        public void QueryDefinitionUpdate()
        {

            var qDef = new ETQueryDefinition
            {
                AuthStub = client,
                CustomerKey = queryDefKey,
                ObjectID = queryDef.ObjectID,
                Description = updatedDesc,
                DataExtensionTarget = new InteractionBaseObject { CustomerKey = targetDEKey, Name = "Store102DEx" },
                QueryText = string.Format("SELECT * FROM [{0}] WHERE STOREID=102", dataExtensionName),
                TargetType = "DE",
                TargetUpdateType = "Overwrite"

            };
            var qdefResponse = qDef.Patch();
            Assert.AreEqual(qdefResponse.Code, 200);
            Assert.AreEqual(qdefResponse.Status, true);
            Assert.AreEqual(qdefResponse.Results[0].StatusMessage, "QueryDefinition updated");

            qDef = new ETQueryDefinition
            {
                AuthStub = client,
                CustomerKey = queryDefKey,
                ObjectID = queryDef.ObjectID,
                Props = new[] { "Name", "ObjectID", "Description" },
                SearchFilter = new SimpleFilterPart { Property = "ObjectID", Value = new[] { queryDef.ObjectID.ToString() }, SimpleOperator = SimpleOperators.equals }

            };
            var getResponse = qDef.Get();
            Assert.AreEqual(getResponse.Code, 200);
            Assert.AreEqual(getResponse.Status, true);
            var def = (ETQueryDefinition)getResponse.Results[0];
            Assert.AreEqual(def.Name, queryDefKey);
            Assert.AreEqual(def.Description, updatedDesc);

        }

        [Test()]
        public void QueryDefinitionDelete()
        {
            var qDef = new ETQueryDefinition
            {
                AuthStub = client,
                CustomerKey = queryDefKey,
                ObjectID = queryDef.ObjectID
            };
            var qdefResponse = qDef.Delete();
            Assert.AreEqual(qdefResponse.Code, 200);
            Assert.AreEqual(qdefResponse.Status, true);
            Assert.AreEqual(qdefResponse.Results[0].StatusMessage, "QueryDefinition deleted");
        }


    }
}
