using NUnit.Framework;
using System;
namespace FuelSDK.Test
{
    [TestFixture()]
    public class ETContentAreaTest
    {
		ETClient client;
        ETContentArea contentArea;
		string contentAreaName;
		string content;
        string updatedContent;

		[OneTimeSetUp]
		public void Setup()
		{
			client = new ETClient();

		}

		[SetUp]
		public void ContentAreaSetup()
		{
            contentAreaName = Guid.NewGuid().ToString();
            content = "<b>Some HTML Content Goes Here</b>";
            updatedContent = "<b>Updated HTML Cotent Goes Here</b>";
            var contentObj = new ETContentArea
			{
				AuthStub = client,
				Name = contentAreaName,
				CustomerKey = contentAreaName,
				Content = content,
			};

			var result = contentObj.Post();
			contentArea = (ETContentArea)result.Results[0].Object;
		}

		[TearDown]
		public void ContentAreaTearDown()
		{
			if (contentArea != null)
			{
                var contentObj = new ETContentArea();
				contentObj.ID = contentArea.ID;
				contentObj.AuthStub = client;
				contentObj.Delete();
			}
		}

        [Test()]
        public void CreateContentArea()
        {
            Assert.AreNotEqual(contentArea,null);
        }

        [Test()]
        public void GetContentArea()
        {
			var getContentArea = new ETContentArea
			{
				AuthStub = client,
				Props = new[] { "RowObjectID", "ObjectID", "ID", "CustomerKey", "Client.ID", "ModifiedDate", "CreatedDate", "CategoryID", "Name", "Layout", "IsDynamicContent", "Content", "IsSurvey", "IsBlank", "Key" },
				SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { contentAreaName } },
			};
			var response = getContentArea.Get();
            getContentArea = (ETContentArea)response.Results[0];

            Assert.AreEqual(getContentArea.Content,content);
        }

        [Test()]
        public void UpdateContentArea()
        {
            var updateContentArea = new ETContentArea
            {
                AuthStub = client,
                CustomerKey = contentAreaName,
                Content = updatedContent
            };
            var response = updateContentArea.Patch();
            Assert.AreEqual(response.Code, 200);

            var getContentArea = new ETContentArea
            {
                AuthStub = client,
                Props = new[] { "RowObjectID", "ObjectID", "ID", "CustomerKey", "Client.ID", "ModifiedDate", "CreatedDate", "CategoryID", "Name", "Layout", "IsDynamicContent", "Content", "IsSurvey", "IsBlank", "Key" },
                SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { contentAreaName } },
            };
            var getResponse = getContentArea.Get();
            getContentArea = (ETContentArea)getResponse.Results[0];
            Assert.AreEqual(getResponse.Code, 200);
            Assert.AreEqual(getContentArea.Content, updatedContent);

        }

        [Test()]
        public void DeleteContentArea()
        {
            var deleteContentArea = new ETContentArea
            {
                AuthStub = client,
                CustomerKey = contentAreaName
            };
            var response = deleteContentArea.Delete();
            Assert.AreEqual(response.Code, 200);
            Assert.GreaterOrEqual(response.Results.Length, 1);
            Assert.AreEqual(response.Results[0].StatusMessage, "Content deleted");
            var getContentArea = new ETContentArea
            {
                AuthStub = client,
                Props = new[] { "RowObjectID", "ObjectID", "ID", "CustomerKey", "Client.ID", "ModifiedDate", "CreatedDate", "CategoryID", "Name", "Layout", "IsDynamicContent", "Content", "IsSurvey", "IsBlank", "Key" },
                SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { contentAreaName } },
            };
            var getResponse = getContentArea.Get();
            Assert.AreEqual(getResponse.Code, 200);
            Assert.AreEqual(getResponse.Results.Length, 0);
        }

	}
}
