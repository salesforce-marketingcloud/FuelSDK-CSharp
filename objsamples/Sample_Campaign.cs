using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_Campaign()
        {
            ET_Client myclient = new ET_Client();
            string IDOfpostCampaign = string.Empty;
            string IDOfpostCampaignAsset = string.Empty;
            string ExampleAssetType = "LIST";
            string ExampleAssetItemID = "1953114";

            Console.WriteLine("--- Testing Campaign ---");

            Console.WriteLine("\n Retrieve All Campaigns");
            ET_Campaign getCampaign = new ET_Campaign();
            getCampaign.AuthStub = myclient;            
            GetReturn grCampaign = getCampaign.Get();

            Console.WriteLine("Get Status: " + grCampaign.Status.ToString());
            Console.WriteLine("Message: " + grCampaign.Message.ToString());
            Console.WriteLine("Code: " + grCampaign.Code.ToString());
            Console.WriteLine("Results Length: " + grCampaign.Results.Length);
            Console.WriteLine("MoreResults: " + grCampaign.MoreResults.ToString());

            foreach (ET_Campaign thisCamp in grCampaign.Results)
            {
                Console.WriteLine("Name: " + thisCamp.Name + ",  ID: " + thisCamp.ID + ", Description: " + thisCamp.Description);
            }

            if (grCampaign.MoreResults)
            {
                grCampaign = getCampaign.GetMoreResults();
                Console.WriteLine("Get Status: " + grCampaign.Status.ToString());
                Console.WriteLine("Message: " + grCampaign.Message.ToString());
                Console.WriteLine("Code: " + grCampaign.Code.ToString());
                Console.WriteLine("Results Length: " + grCampaign.Results.Length);
                Console.WriteLine("MoreResults: " + grCampaign.MoreResults.ToString());

                foreach (ET_Campaign thisCamp in grCampaign.Results)
                {
                    Console.WriteLine("Name: " + thisCamp.Name + ",  ID: " + thisCamp.ID + ", Description: " + thisCamp.Description);
                }
            }


            Console.WriteLine("\n Create Campaign");
            ET_Campaign camp = new ET_Campaign();
            camp.AuthStub = myclient;
            camp.Name = "CSharpSDKCreatedForTEST";
            camp.Description = "CSharpSDKCreatedForTEST";

            PostReturn prCampaign = camp.Post();
            Console.WriteLine("Post Status: " + prCampaign.Status.ToString());
            Console.WriteLine("Message: " + prCampaign.Message.ToString());
            Console.WriteLine("Code: " + prCampaign.Code.ToString());
            Console.WriteLine("Results Length: " + prCampaign.Results.Length);

            if (prCampaign.Results.Length > 0)
            {
                ET_Campaign campaign = (ET_Campaign)prCampaign.Results[0].Object;
                Console.WriteLine("--ID: " + campaign.ID + ", CreatedDate: " + campaign.CreatedDate);
                IDOfpostCampaign = campaign.ID.ToString();

                Console.WriteLine("\n Retrieve the new Campaign");
                ET_Campaign singleCampaign = new ET_Campaign();
                singleCampaign.AuthStub = myclient;
                singleCampaign.ID = campaign.ID;
                GetReturn grSingleCamp = singleCampaign.Get();

                Console.WriteLine("Get Status: " + grSingleCamp.Status.ToString());
                Console.WriteLine("Message: " + grSingleCamp.Message.ToString());
                Console.WriteLine("Code: " + grSingleCamp.Code.ToString());
                Console.WriteLine("Results Length: " + grSingleCamp.Results.Length);

                Console.WriteLine("\n Create a new Campaign Asset");
                ET_CampaignAsset postCampAsset = new ET_CampaignAsset();
                postCampAsset.AuthStub = myclient;
                postCampAsset.CampaignID = IDOfpostCampaign;
                postCampAsset.Type = ExampleAssetType;
                postCampAsset.IDs = new string[] { ExampleAssetItemID };
                PostReturn prCampAsset = postCampAsset.Post();
                Console.WriteLine("Post Status: " + prCampAsset.Status.ToString());
                Console.WriteLine("Message: " + prCampAsset.Message.ToString());
                Console.WriteLine("Code: " + prCampAsset.Code.ToString());
                Console.WriteLine("Results Length: " + prCampAsset.Results.Length);

                if (prCampAsset.Status)
                {
                    IDOfpostCampaignAsset = prCampAsset.Results[0].Object.ID.ToString();

                    Console.WriteLine("\n Retrieve a single new Campaign Asset");
                    ET_CampaignAsset singleCampAsset = new ET_CampaignAsset();
                    singleCampAsset.AuthStub = myclient;
                    singleCampAsset.ID = Convert.ToInt16(IDOfpostCampaignAsset);
                    singleCampAsset.CampaignID =  IDOfpostCampaign;
                    GetReturn grSingleCampAsset = singleCampAsset.Get();
                    Console.WriteLine("Get Status: " + grSingleCampAsset.Status.ToString());
                    Console.WriteLine("Message: " + grSingleCampAsset.Message.ToString());
                    Console.WriteLine("Code: " + grSingleCampAsset.Code.ToString());
                    Console.WriteLine("Results Length: " + grSingleCampAsset.Results.Length);


                    Console.WriteLine("\n Delete the new Campaign Asset");
                    ET_CampaignAsset deleteCampAsset = new ET_CampaignAsset();
                    deleteCampAsset.AuthStub = myclient;
                    deleteCampAsset.ID = Convert.ToInt16(IDOfpostCampaignAsset);
                    deleteCampAsset.CampaignID = IDOfpostCampaign;
                    DeleteReturn drSingleCampAsset = deleteCampAsset.Delete();
                    Console.WriteLine("Get Status: " + drSingleCampAsset.Status.ToString());
                    Console.WriteLine("Message: " + drSingleCampAsset.Message.ToString());
                    Console.WriteLine("Code: " + drSingleCampAsset.Code.ToString());
                    Console.WriteLine("Results Length: " + drSingleCampAsset.Results.Length);

                }

                Console.WriteLine("\n Delete Campaign");
                ET_Campaign delCampaign = new ET_Campaign();
                delCampaign.AuthStub = myclient;
                delCampaign.ID = campaign.ID;
                DeleteReturn drCampaign = delCampaign.Delete();
                Console.WriteLine("Delete Status: " + drCampaign.Status.ToString());
                Console.WriteLine("Message: " + drCampaign.Message.ToString());
                Console.WriteLine("Code: " + drCampaign.Code.ToString());
                Console.WriteLine("Results Length: " + drCampaign.Results.Length);
            }         
        }
    }
}
