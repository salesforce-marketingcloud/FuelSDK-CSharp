using FuelSDK;
using System;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_Campaign()
        {
            var myclient = new ET_Client();
            var idOfpostCampaign = string.Empty;
            var idOfpostCampaignAsset = string.Empty;
            var exampleAssetType = "LIST";
            var exampleAssetItemID = "1953114";

            Console.WriteLine("--- Testing Campaign ---");

            Console.WriteLine("\n Retrieve All Campaigns");
            var getCampaign = new ET_Campaign
            {
                AuthStub = myclient,
            };
            var grCampaign = getCampaign.Get();

            Console.WriteLine("Get Status: " + grCampaign.Status.ToString());
            Console.WriteLine("Message: " + grCampaign.Message);
            Console.WriteLine("Code: " + grCampaign.Code.ToString());
            Console.WriteLine("Results Length: " + grCampaign.Results.Length);
            Console.WriteLine("MoreResults: " + grCampaign.MoreResults.ToString());
            foreach (ET_Campaign thisCamp in grCampaign.Results)
                Console.WriteLine("Name: " + thisCamp.Name + ",  ID: " + thisCamp.ID + ", Description: " + thisCamp.Description);

            if (grCampaign.MoreResults)
            {
                grCampaign = getCampaign.GetMoreResults();
                Console.WriteLine("Get Status: " + grCampaign.Status.ToString());
                Console.WriteLine("Message: " + grCampaign.Message);
                Console.WriteLine("Code: " + grCampaign.Code.ToString());
                Console.WriteLine("Results Length: " + grCampaign.Results.Length);
                Console.WriteLine("MoreResults: " + grCampaign.MoreResults.ToString());
                foreach (ET_Campaign thisCamp in grCampaign.Results)
                    Console.WriteLine("Name: " + thisCamp.Name + ",  ID: " + thisCamp.ID + ", Description: " + thisCamp.Description);
            }

            Console.WriteLine("\n Create Campaign");
            var camp = new ET_Campaign
            {
                AuthStub = myclient,
                Name = "CSharpSDKCreatedForTEST",
                Description = "CSharpSDKCreatedForTEST",
            };
            var prCampaign = camp.Post();

            Console.WriteLine("Post Status: " + prCampaign.Status.ToString());
            Console.WriteLine("Message: " + prCampaign.Message);
            Console.WriteLine("Code: " + prCampaign.Code.ToString());
            Console.WriteLine("Results Length: " + prCampaign.Results.Length);

            if (prCampaign.Results.Length > 0)
            {
                var campaign = (ET_Campaign)prCampaign.Results[0].Object;
                Console.WriteLine("--ID: " + campaign.ID + ", CreatedDate: " + campaign.CreatedDate);
                idOfpostCampaign = campaign.ID.ToString();

                Console.WriteLine("\n Retrieve the new Campaign");
                var singleCampaign = new ET_Campaign
                {
                    AuthStub = myclient,
                    ID = campaign.ID,
                };
                var grSingleCamp = singleCampaign.Get();

                Console.WriteLine("Get Status: " + grSingleCamp.Status.ToString());
                Console.WriteLine("Message: " + grSingleCamp.Message);
                Console.WriteLine("Code: " + grSingleCamp.Code.ToString());
                Console.WriteLine("Results Length: " + grSingleCamp.Results.Length);

                Console.WriteLine("\n Create a new Campaign Asset");
                var postCampAsset = new ET_CampaignAsset
                {
                    AuthStub = myclient,
                    CampaignID = idOfpostCampaign,
                    Type = exampleAssetType,
                    IDs = new[] { exampleAssetItemID },
                };
                var prCampAsset = postCampAsset.Post();
                Console.WriteLine("Post Status: " + prCampAsset.Status.ToString());
                Console.WriteLine("Message: " + prCampAsset.Message);
                Console.WriteLine("Code: " + prCampAsset.Code.ToString());
                Console.WriteLine("Results Length: " + prCampAsset.Results.Length);

                if (prCampAsset.Status)
                {
                    idOfpostCampaignAsset = prCampAsset.Results[0].Object.ID.ToString();

                    Console.WriteLine("\n Retrieve a single new Campaign Asset");
                    var singleCampAsset = new ET_CampaignAsset
                    {
                        AuthStub = myclient,
                        ID = Convert.ToInt16(idOfpostCampaignAsset),
                        CampaignID = idOfpostCampaign,
                    };
                    var grSingleCampAsset = singleCampAsset.Get();
                    Console.WriteLine("Get Status: " + grSingleCampAsset.Status.ToString());
                    Console.WriteLine("Message: " + grSingleCampAsset.Message);
                    Console.WriteLine("Code: " + grSingleCampAsset.Code.ToString());
                    Console.WriteLine("Results Length: " + grSingleCampAsset.Results.Length);

                    Console.WriteLine("\n Delete the new Campaign Asset");
                    var deleteCampAsset = new ET_CampaignAsset
                    {
                        AuthStub = myclient,
                        ID = Convert.ToInt16(idOfpostCampaignAsset),
                        CampaignID = idOfpostCampaign,
                    };
                    var drSingleCampAsset = deleteCampAsset.Delete();
                    Console.WriteLine("Get Status: " + drSingleCampAsset.Status.ToString());
                    Console.WriteLine("Message: " + drSingleCampAsset.Message);
                    Console.WriteLine("Code: " + drSingleCampAsset.Code.ToString());
                    Console.WriteLine("Results Length: " + drSingleCampAsset.Results.Length);
                }

                Console.WriteLine("\n Delete Campaign");
                var delCampaign = new ET_Campaign
                {
                    AuthStub = myclient,
                    ID = campaign.ID,
                };
                var drCampaign = delCampaign.Delete();
                Console.WriteLine("Delete Status: " + drCampaign.Status.ToString());
                Console.WriteLine("Message: " + drCampaign.Message);
                Console.WriteLine("Code: " + drCampaign.Code.ToString());
                //Console.WriteLine("Results Length: " + drCampaign.Results.Length);
            }
        }
    }
}
