using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelSDK.Test
{
    class ETFolderTest
    {
        ETClient client;
        int folderId;
        ETFolder parentFolder;
        string folderName;
        string folderDesc;
        string updatedFolderDesc;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            client = new ETClient();
        }

        [SetUp]
        public void Setup()
        {
            folderName = Guid.NewGuid().ToString();
            folderDesc = "Test Folder C# SDK";
            updatedFolderDesc = "Updated Test Folder C# SDK";

            //get the parent folder
            var getFolder = new ETFolder
            {
                AuthStub = client,
                SearchFilter = new ComplexFilterPart
                {
                    LeftOperand = new SimpleFilterPart { Property = "ParentFolder.ID", SimpleOperator = SimpleOperators.equals, Value = new[] { "0" } },
                    RightOperand = new SimpleFilterPart { Property = "ContentType", SimpleOperator = SimpleOperators.equals, Value = new[] { "Email" } },
                    LogicalOperator = LogicalOperators.AND
                },
                Props = new[] { "ID", "Name", "Description" },
            };
            var getresponse = getFolder.Get();
            Assert.AreEqual(getresponse.Code, 200);
            Assert.AreEqual(getresponse.Status, true);
            parentFolder = (ETFolder)getresponse.Results[0];
            var fold = new ETFolder
            {
                Name = folderName,
                Description = folderDesc,
                CustomerKey = folderName,
                AuthStub = client,
                ParentFolder = parentFolder,
                ContentType = "email",
                IsEditable = true,
            };
            var response = fold.Post();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            folderId = response.Results[0].NewID;
        }

        [TearDown]
        public void TearDown()
        {
                var fold = new ETFolder
                {
                    AuthStub = client,
                    ID = folderId
                };
                var response = fold.Delete();
        }

        [Test()]
        public void FolderCreate()
        {
            Assert.AreNotEqual(folderId, null);
        }

        [Test()]
        public void FolderUpdate()
        {
            var fold = new ETFolder
            {
                Description = updatedFolderDesc,
                ID = folderId,
                AuthStub = client
            };

            var response = fold.Patch();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            Assert.AreEqual(response.Results[0].StatusMessage, "Folder updated successfully.");

            var getFolder = new ETFolder
            {
                AuthStub = client,
                SearchFilter = new SimpleFilterPart
                {
                    Property = "ID",
                    Value = new[] { folderId.ToString() },
                    SimpleOperator = SimpleOperators.equals
                },
                Props = new[] { "ID", "Name", "Description" },
            };
            var getresponse = getFolder.Get();
            Assert.AreEqual(getresponse.Code, 200);
            Assert.AreEqual(getresponse.Status, true);
            ETFolder folder = (ETFolder)getresponse.Results[0];
            Assert.AreEqual(folder.Description, updatedFolderDesc);

        }

        [Test()]
        public void FolderGet()
        {
            var getFolder = new ETFolder
            {
                AuthStub = client,
                SearchFilter = new SimpleFilterPart
                {
                    Property = "ID",
                    Value = new[] { folderId.ToString() },
                    SimpleOperator = SimpleOperators.equals
                },
                Props = new[] { "ID", "Name", "Description" },
            };
            var getresponse = getFolder.Get();
            Assert.AreEqual(getresponse.Code, 200);
            Assert.AreEqual(getresponse.Status, true);
            ETFolder folder = (ETFolder)getresponse.Results[0];
            Assert.AreEqual(folder.Description, folderDesc);
        }

        [Test()]
        public void FolderDelete()
        {
            var fold = new ETFolder
            {
                AuthStub = client,
                ID = folderId
            };
            var response = fold.Delete();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            Assert.AreEqual(response.Results[0].StatusMessage, "Folder deleted successfully.");

        }
    }
}
