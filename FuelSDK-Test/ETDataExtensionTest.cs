using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelSDK.Test
{
    [TestFixture()]
    class ETDataExtensionTest
    {
        ETClient client;
        ETDataExtension dataExtension;
        string dataExtensionName;
        string updatedDataExtensionName;
        [OneTimeSetUp]
        public void Setup()
        {
            client = new ETClient();

        }

        [SetUp]
        public void DataExtensionSetup()
        {
            dataExtensionName = Guid.NewGuid().ToString();
            updatedDataExtensionName = Guid.NewGuid().ToString();
            var deObj = new ETDataExtension
            {
                AuthStub = client,
                Name = dataExtensionName,
                CustomerKey = dataExtensionName,
                Columns = new[] { 
                    new ETDataExtensionColumn { Name = "Field1", FieldType = DataExtensionFieldType.Text, IsPrimaryKey = true, MaxLength = 100, IsRequired = true },
                    new ETDataExtensionColumn { Name = "Field2", FieldType = DataExtensionFieldType.Text } ,
                    new ETDataExtensionColumn { Name = "NumericField", FieldType = DataExtensionFieldType.Number}
                }
            };

            var result = deObj.Post();
            dataExtension = (ETDataExtension)result.Results[0].Object;
        }

        [TearDown]
        public void DataExtensionTearDown()
        {
            if (dataExtension != null)
            {
                var deObj = new ETDataExtension();
                deObj.AuthStub = client;
                deObj.CustomerKey = dataExtensionName;
                deObj.Delete();
            }
        }

        [Test()]
        public void DataExtensionCreate()
        {
            Assert.AreNotEqual(dataExtension, null);
        }

        [Test()]
        public void DataExtensionGet()
        {
            var deObj = new ETDataExtension
            {
                AuthStub = client,
                Props = new[] { "Name" },
                SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { dataExtension.CustomerKey } },
            };

            var getResponse = deObj.Get();
            Assert.AreEqual(getResponse.Code, 200);
            Assert.AreEqual(getResponse.Status, true);
            Assert.GreaterOrEqual(getResponse.Results.Length, 1);
            var getDE = (ETDataExtension)getResponse.Results[0];
            Assert.AreEqual(getDE.Name, dataExtensionName);
        }

        [Test()]
        public void DataExtensionUpdate()
        {
            var deObj = new ETDataExtension
            {
                AuthStub = client,
                Name = updatedDataExtensionName,
                CustomerKey = dataExtensionName
            };

            var response = deObj.Patch();
            Assert.AreEqual(response.Code, 200);
            var degetObj = new ETDataExtension
            {
                AuthStub = client,
                Props = new[] { "Name" },
                SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { dataExtension.CustomerKey } },
            };

            var getResponse = degetObj.Get();
            Assert.AreEqual(getResponse.Code, 200);
            Assert.GreaterOrEqual(getResponse.Results.Length, 1);

            var getDE = (ETDataExtension)getResponse.Results[0];
            Assert.AreEqual(getDE.Name, updatedDataExtensionName);
        }

        [Test()]
        public void DataExtensionDelete()
        {
            var deObj = new ETDataExtension
            {
                AuthStub = client,
                CustomerKey = dataExtensionName
            };
            var response = deObj.Delete();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Results[0].StatusMessage, "Data Extension deleted.");
            var degetObj = new ETDataExtension
            {
                AuthStub = client,
                Props = new[] { "Name" },
                SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { dataExtension.CustomerKey } },
            };

            var getResponse = degetObj.Get();
            Assert.AreEqual(getResponse.Code, 200);
            Assert.AreEqual(getResponse.Results.Length, 0);


        }


    }
}
