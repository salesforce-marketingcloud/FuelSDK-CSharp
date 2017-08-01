using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FuelSDK.Test
{
    class ETDataExtensionRowTest
    {
        ETClient client;
        ETDataExtension dataExtension;
        string dataExtensionName;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            client = new ETClient();

        }

        [SetUp]
        public void Setup()
        {
            dataExtensionName = Guid.NewGuid().ToString();
            var deObj = new ETDataExtension
            {
                AuthStub = client,
                Name = dataExtensionName,
                CustomerKey = dataExtensionName,
                Columns = new[] { 
                    new ETDataExtensionColumn { Name = "CustomerNumber", FieldType = DataExtensionFieldType.Text, IsPrimaryKey = true, MaxLength = 100, IsRequired = true },
                    new ETDataExtensionColumn { Name = "FirstName", FieldType = DataExtensionFieldType.Text, MaxLength=256 } ,
                    new ETDataExtensionColumn { Name = "LastName", FieldType = DataExtensionFieldType.Text, MaxLength=256 } 
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
        public void DataExtensionRowCreate()
        {
            for (int i = 0; i < 50; i++)
            {
                var deRow = new ETDataExtensionRow
                {
                    AuthStub = client,
                    DataExtensionName = dataExtensionName,
                };
                deRow.ColumnValues.Add("CustomerNumber", "Customer_" + i.ToString());
                deRow.ColumnValues.Add("FirstName", "FirstName_" + i.ToString());
                deRow.ColumnValues.Add("LastName", "LastName_" + i.ToString());

                var derResponse = deRow.Post();
                Assert.AreEqual(derResponse.Code, 200);
                Assert.AreEqual(derResponse.Results[0].StatusMessage, "Created DataExtensionObject");

            }
        }

        [Test()]
        public void DataExtensionRowUpdate()
        {
            var deRow = new ETDataExtensionRow
            {
                AuthStub = client,
                DataExtensionName = dataExtensionName,
            };
            deRow.ColumnValues.Add("CustomerNumber", "Customer_");
            deRow.ColumnValues.Add("FirstName", "FirstName_");
            deRow.ColumnValues.Add("LastName", "LastName_");

            var derResponse = deRow.Post();
            Assert.AreEqual(derResponse.Code, 200);
            Assert.AreEqual(derResponse.Results[0].StatusMessage, "Created DataExtensionObject");
            var custKey = ((ETDataExtensionRow)derResponse.Results[0].Object).CustomerKey;

            deRow = new ETDataExtensionRow
            {
                AuthStub = client,
                DataExtensionName = dataExtensionName,

            };
            deRow.CustomerKey = custKey;
            //for any update/delete operation, make sure to set the primary key value
            deRow.ColumnValues.Add("CustomerNumber", "Customer_");
            deRow.ColumnValues.Add("FirstName", "Changed First Name");
            var derUpdateResponse = deRow.Patch();
            Assert.AreEqual(derUpdateResponse.Code, 200);

            deRow = new ETDataExtensionRow
            {
                AuthStub = client,
                DataExtensionName = dataExtensionName,
                Props = new[] { "CustomerNumber", "FirstName" },
                SearchFilter = new SimpleFilterPart { Property = "CustomerNumber", SimpleOperator = SimpleOperators.equals, Value = new[] { "Customer_" } },
            };
            var derGetResponse = deRow.Get();
            Assert.AreEqual(derGetResponse.Code, 200);
            Assert.AreEqual(((ETDataExtensionRow)derGetResponse.Results[0]).ColumnValues["FirstName"], "Changed First Name");
            
        }

        [Test()]
        public void DataExtensionRowDelete()
        {
            var deRow = new ETDataExtensionRow
            {
                AuthStub = client,
                DataExtensionName = dataExtensionName,
            };
            deRow.ColumnValues.Add("CustomerNumber", "Customer_");
            deRow.ColumnValues.Add("FirstName", "FirstName_");
            deRow.ColumnValues.Add("LastName", "LastName_");

            var derResponse = deRow.Post();
            Assert.AreEqual(derResponse.Code, 200);
            Assert.AreEqual(derResponse.Results[0].StatusMessage, "Created DataExtensionObject");
            var custKey = ((ETDataExtensionRow)derResponse.Results[0].Object).CustomerKey;
            deRow = new ETDataExtensionRow
            {
                AuthStub = client,
                DataExtensionName = dataExtensionName,
                Props = new[] { "CustomerNumber", "FirstName" },
                SearchFilter = new SimpleFilterPart { Property = "CustomerNumber", SimpleOperator = SimpleOperators.equals, Value = new[] { "Customer_" } },
            };
            var derGetResponse = deRow.Get();
            Assert.AreEqual(derGetResponse.Code, 200);
            Assert.AreEqual(((ETDataExtensionRow)derGetResponse.Results[0]).ColumnValues["FirstName"], "FirstName_");

            deRow = new ETDataExtensionRow
            {
                AuthStub = client,
                DataExtensionName = dataExtensionName
            };
            deRow.CustomerKey = custKey;
            //for any update/delete operation, make sure to set the primary key value
            deRow.ColumnValues.Add("CustomerNumber", "Customer_");
            deRow.Delete();

            deRow = new ETDataExtensionRow
            {
                AuthStub = client,
                DataExtensionName = dataExtensionName,
                Props = new[] { "CustomerNumber", "FirstName" },
                SearchFilter = new SimpleFilterPart { Property = "CustomerNumber", SimpleOperator = SimpleOperators.equals, Value = new[] { "Customer_" } },
            };
            derGetResponse = deRow.Get();
            Assert.AreEqual(derGetResponse.Code, 200);
            Assert.AreEqual(derGetResponse.Results.Length, 0);

        }

    }
}
