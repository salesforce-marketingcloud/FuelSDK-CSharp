using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelSDK.Test
{
    /// <summary>
    /// Marketing cloud API only supports Data Extension Column get operation.
    /// </summary>
    class ETDataExtensionColumnTest
    {
        ETClient client;
        ETDataExtension dataExtension;
        string dataExtensionName;
        string updatedDataExtensionName;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            client = new ETClient();

        }

        [SetUp]
        public void Setup()
        {
            dataExtensionName = Guid.NewGuid().ToString();
            updatedDataExtensionName = Guid.NewGuid().ToString();
            var deObj = new ETDataExtension
            {
                AuthStub = client,
                Name = dataExtensionName,
                CustomerKey = dataExtensionName,
                Columns = new[] { 
                    new ETDataExtensionColumn { Name = "Field1", FieldType = DataExtensionFieldType.Text, IsPrimaryKey = true, MaxLength = 100, IsRequired = true }
                }
            };

            var result = deObj.Post();
            dataExtension = (ETDataExtension)result.Results[0].Object;
        }

        [TearDown]
        public void TearDown()
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
        public void DataExtensionColumnGet()
        {
            var getColumn = new ETDataExtensionColumn
               {
                   AuthStub = client,
                   Props = new[] { "Name", "FieldType" },
                   SearchFilter = new SimpleFilterPart { Property = "DataExtension.CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { dataExtensionName } },
               };
            var getColumnResponse = getColumn.Get();
            Assert.AreEqual(getColumnResponse.Code, 200);
            var getDEColumn = (ETDataExtensionColumn) getColumnResponse.Results[0];
            Assert.AreEqual(getDEColumn.Name, "Field1");
            Assert.AreEqual(getDEColumn.FieldType, DataExtensionFieldType.Text);
            
        }
    }
}
