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
            Console.WriteLine("--- Testing Campaign ---");

            Console.WriteLine("\n Retrieve All Campaigns");
            ET_Campaign getCampaign = new ET_Campaign();
            getCampaign.authStub = myclient;
            //getCampaign.Page = 2;
            GetReturn grCampaign = getCampaign.Get();

            Console.WriteLine("Get Status: " + grCampaign.Status.ToString());
            Console.WriteLine("Message: " + grCampaign.Message.ToString());
            Console.WriteLine("Code: " + grCampaign.Code.ToString());
            Console.WriteLine("Results Length: " + grCampaign.Results.Length);
            Console.WriteLine("MoreResults: " + grCampaign.MoreResults.ToString());


            foreach (ET_Campaign thisCamp in grCampaign.Results)
            {
                Console.WriteLine("--Name: " + thisCamp.Name + ",  ID: " + thisCamp.ID + ", Description: " + thisCamp.Description);
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
            camp.authStub = myclient;
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


                Console.WriteLine("\n Delete Campaign");
                ET_Campaign delCampaign = new ET_Campaign();
                delCampaign.authStub = myclient;
                delCampaign.ID = campaign.ID;
                FuelSDK.DeleteReturn drCampaign = delCampaign.Delete();

                Console.WriteLine("Delete Status: " + drCampaign.Status.ToString());
                Console.WriteLine("Message: " + drCampaign.Message.ToString());
                Console.WriteLine("Code: " + drCampaign.Code.ToString());
                Console.WriteLine("Results Length: " + drCampaign.Results.Length);
            }         
        }
    }
}
