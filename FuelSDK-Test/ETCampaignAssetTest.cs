using NUnit.Framework;
using System;
namespace FuelSDK.Test
{
    [TestFixture()]
    public class ETCampaignAssetTest
    {
		ETClient client;
        ETCampaignAsset asset;
        ETCampaign campaign;

		[OneTimeSetUp]
		public void Setup()
		{
			client = new ETClient();

			var campObj = new ETCampaign
			{
				AuthStub = client,
				Name = "Created for testing campaign asset test cases in C#",
				Description = "Test Description"
			};

			var result = campObj.Post();
			campaign = (ETCampaign)result.Results[0].Object;
		}

		[SetUp]
		public void CampaignAssetSetup()
		{
			var rnd = new Random(DateTime.Now.Millisecond);
			int ticks = rnd.Next(3000,6000);

            var assetObj = new ETCampaignAsset
            {
                AuthStub = client,
                CustomerKey = Guid.NewGuid().ToString(),
                Type = "Email",
                CampaignID = campaign.ID.ToString(),
                ID = 32798
			};

			var result = assetObj.Post();
            asset = (ETCampaignAsset)result.Results[0].Object;
		}

		[Test()]
		public void CreateCampaignAsset()
		{
			Assert.AreNotEqual(asset, null);
		}

		[TearDown]
		public void CampaignAssetTearDown()
		{
			if (asset != null)
			{
                var assetObj = new ETCampaignAsset();
				assetObj.ID = asset.ID;
				assetObj.AuthStub = client;
				assetObj.Delete();
			}
		}

	}
}
