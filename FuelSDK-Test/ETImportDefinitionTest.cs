using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelSDK.Test
{
    class ETImportDefinitionTest
    {
        ETClient client;

        string importName;
        string dataExtensionName;
        string dataExtensionId;
        bool deleteDef = false;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            client = new ETClient();
        }

        [SetUp()]
        public void Setup()
        {
            dataExtensionName = Guid.NewGuid().ToString();
            importName = Guid.NewGuid().ToString();
            var deObj = new ETDataExtension
            {
                AuthStub = client,
                Name = dataExtensionName,
                CustomerKey = dataExtensionName,
                IsSendable = true,
                SendableDataExtensionField = new ETDataExtensionColumn { Name = "SubscriberID", FieldType = DataExtensionFieldType.Number },
                SendableSubscriberField = new ETProfileAttribute { Name = "Subscriber ID", Value = "Subscriber ID" },
                Columns = new[] { 
                    new ETDataExtensionColumn { Name = "SubscriberID", FieldType = DataExtensionFieldType.Number, IsPrimaryKey = true, IsRequired = true },
                    new ETDataExtensionColumn { Name = "FirstName", FieldType = DataExtensionFieldType.Text },
                    new ETDataExtensionColumn { Name = "LastName", FieldType = DataExtensionFieldType.Text } 
                }
            };
            var response = deObj.Post();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            dataExtensionId = response.Results[0].NewObjectID;

            var importDefObj = new ETImportDefinition
            {
                AuthStub = client,
                Name = importName,
                CustomerKey = importName,
                Description = "Created with FuelSDK",
                AllowErrors = true,
                DestinationObject = new ETDataExtension { ObjectID = dataExtensionId },
                FieldMappingType = ImportDefinitionFieldMappingType.InferFromColumnHeadings,
                FileSpec = "FuelSDKExample.csv",
                FileType = FileType.CSV,
                Notification = new AsyncResponse { ResponseType = AsyncResponseType.email, ResponseAddress = "example@bh.exacttarget.com" },
                RetrieveFileTransferLocation = new FileTransferLocation { CustomerKey = "ExactTarget Enhanced FTP" },
                UpdateType = ImportDefinitionUpdateType.Overwrite,
            };
            var createresponse = importDefObj.Post();
            Assert.AreEqual(createresponse.Code, 200);
            Assert.AreEqual(createresponse.Status, true);
            Assert.AreEqual(createresponse.Results[0].StatusMessage, "ImportDefinition created.");
            deleteDef = true;
        }

        [TearDown]
        public void TearDown()
        {
            var deObj = new ETDataExtension
            {
                AuthStub = client,
                CustomerKey = dataExtensionName
            };
            var response = deObj.Delete();
            if (deleteDef)
            {
                var importObj = new ETImportDefinition
                {
                    AuthStub = client,
                    CustomerKey = importName
                };
                importObj.Delete();
            }
        }

        [Test()]
        public void ImportDefinitionCreate()
        {
            //if the deletedef flag is set to true means we have successfully created the definition.
            Assert.AreEqual(deleteDef, true);
        }

        [Test()]
        public void ImportDefinitionUpdate()
        {
            var importDefObj = new ETImportDefinition
            {
                AuthStub = client,
                CustomerKey = importName,
                Description = "Updated - Created with FuelSDK",
                AllowErrors = false,
            };
            var updresponse = importDefObj.Patch();
            Assert.AreEqual(updresponse.Code, 200);
            Assert.AreEqual(updresponse.Status, true);
            Assert.AreEqual(updresponse.Results[0].StatusMessage, "ImportDefinition updated");
        }

        [Test()]
        public void ImportDefinitionGet()
        {
            var importDefObj = new ETImportDefinition
            {
                AuthStub = client,
                CustomerKey = importName,
                Props = new[] { "Description", "CustomerKey", "FileSpec" },
                SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { importName } }
            };
            var response = importDefObj.Get();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            Assert.Greater(response.Results.Length, 0);
            ETImportDefinition def = (ETImportDefinition)response.Results[0];
            Assert.AreEqual(def.FileSpec, "FuelSDKExample.csv");
        }

        [Test()]
        public void ImportDefinitionDelete()
        {
            var importDefObj = new ETImportDefinition
            {
                AuthStub = client,
                CustomerKey = importName
            };
            var response = importDefObj.Delete();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            Assert.AreEqual(response.Results[0].StatusMessage, "ImportDefinition deleted");
        }

    }
}
