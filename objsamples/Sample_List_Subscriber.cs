using FuelSDK;
using System;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_ListSubscriber()
        {
            var newListName = "CSharpSDKListSubscriber";
            var subscriberTestEmail = "CSharpSDKListSubscriber@bh.exacttarget.com";

            Console.WriteLine("--- Testing ListSubscriber ---");
            var myclient = new ET_Client();

            Console.WriteLine("\n Create List");
            var postList = new ET_List
            {
                AuthStub = myclient,
                ListName = newListName,
            };
            var prList = postList.Post();

            if (prList.Status && prList.Results.Length > 0)
            {
                var newListID = prList.Results[0].Object.ID;

                Console.WriteLine("\n Create Subscriber on List");
                var postSub = new ET_Subscriber
                {
                    Lists = new[] { new ET_SubscriberList { ID = newListID } },
                    AuthStub = myclient,
                    EmailAddress = subscriberTestEmail,
                    Attributes = new[] { new ET_ProfileAttribute { Name = "First Name", Value = "ExactTarget Example" } },
                };
                var postResponse = postSub.Post();
                Console.WriteLine("Post Status: " + postResponse.Status.ToString());
                Console.WriteLine("Message: " + postResponse.Message);
                Console.WriteLine("Code: " + postResponse.Code.ToString());
                Console.WriteLine("Results Length: " + postResponse.Results.Length);

                if (!postResponse.Status)
                    if (postResponse.Results.Length > 0 && postResponse.Results[0].ErrorCode == 12014)
                    {
                        // If the subscriber already exists in the account then we need to do an update.
                        // Update Subscriber On List 
                        Console.WriteLine("\n Update Subscriber to add to List");
                        PatchReturn patchResponse = postSub.Patch();
                        Console.WriteLine("Post Status: " + patchResponse.Status.ToString());
                        Console.WriteLine("Message: " + patchResponse.Message);
                        Console.WriteLine("Code: " + patchResponse.Code.ToString());
                        Console.WriteLine("Results Length: " + patchResponse.Results.Length);
                    }

                Console.WriteLine("\n Retrieve all Subscribers on the List");
                var getListSub = new ET_List_Subscriber
                {
                    AuthStub = myclient,
                    Props = new[] { "ObjectID", "SubscriberKey", "CreatedDate", "Client.ID", "Client.PartnerClientKey", "ListID", "Status" },
                    SearchFilter = new SimpleFilterPart { Property = "ListID", SimpleOperator = SimpleOperators.equals, Value = new[] { newListID.ToString() } },
                };
                var getResponse = getListSub.Get();
                Console.WriteLine("Get Status: " + getResponse.Status.ToString());
                Console.WriteLine("Message: " + getResponse.Message);
                Console.WriteLine("Code: " + getResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getResponse.Results.Length);
                foreach (ET_List_Subscriber resultListSub in getResponse.Results)
                    Console.WriteLine("--ListID: " + resultListSub.ID + ", SubscriberKey(EmailAddress): " + resultListSub.SubscriberKey);
            }

#if false
            var filterDate = new DateTime(2013, 1, 15, 13, 0, 0);

            Console.WriteLine("Retrieve Filtered ListSubscribers with GetMoreResults");
            var oe2 = new ET_List_Subscriber
            {
                AuthStub = myclient,
                SearchFilter = new SimpleFilterPart { Property = "EventDate", SimpleOperator = SimpleOperators.greaterThan, DateValue = new [] { filterDate } },
                Props = new [] { "ObjectID", "SubscriberKey", "CreatedDate", "Client.ID", "Client.PartnerClientKey", "ListID", "Status" },
            };
            var oeGet = oe2.Get();

            Console.WriteLine("Get Status: " + oeGet.Status.ToString());
            Console.WriteLine("Message: " + oeGet.Message);
            Console.WriteLine("Code: " + oeGet.Code.ToString());
            Console.WriteLine("Results Length: " + oeGet.Results.Length);
            Console.WriteLine("MoreResults: " + oeGet.MoreResults.ToString());
            // Since this could potentially return a large number of results, we do not want to print the results
            //foreach (ET_ListSubscriber ListSubscriber in oeGet.Results)
            //    Console.WriteLine("SubscriberKey: " + ListSubscriber.SubscriberKey + ", EventDate: " + ListSubscriber.EventDate.ToString());

            while (oeGet.MoreResults)
            {
                Console.WriteLine("Continue Retrieve Filtered ListSubscribers with GetMoreResults");
                oeGet = oe2.GetMoreResults();
                Console.WriteLine("Get Status: " + oeGet.Status.ToString());
                Console.WriteLine("Message: " + oeGet.Message);
                Console.WriteLine("Code: " + oeGet.Code.ToString());
                Console.WriteLine("Results Length: " + oeGet.Results.Length);
                Console.WriteLine("MoreResults: " + oeGet.MoreResults.ToString());
            }

            // The following request could potentially bring back large amounts of data if run against a production account	
            Console.WriteLine("Retrieve All ListSubscribers with GetMoreResults");
            var oe3 = new ET_List_Subscriber
            {
                AuthStub = myclient,
                Props = new string[] { "SendID", "SubscriberKey", "EventDate", "Client.ID", "EventType", "BatchID", "TriggeredSendDefinitionObjectID", "PartnerKey" },
            };
            var oeGetAll = oe3.Get();

            Console.WriteLine("Get Status: " + oeGetAll.Status.ToString());
            Console.WriteLine("Message: " + oeGetAll.Message);
            Console.WriteLine("Code: " + oeGetAll.Code.ToString());
            Console.WriteLine("Results Length: " + oeGetAll.Results.Length);
            Console.WriteLine("MoreResults: " + oeGetAll.MoreResults.ToString());
            // Since this could potentially return a large number of results, we do not want to print the results
            foreach (ET_List_Subscriber ListSubscriber in oeGetAll.Results)
                Console.WriteLine("SubscriberKey: " + ListSubscriber.SubscriberKey + ", EventDate: " + ListSubscriber.EventDate.ToString());

            while (oeGetAll.MoreResults)
            {
                oeGetAll = oe3.GetMoreResults();
                Console.WriteLine("Get Status: " + oeGetAll.Status.ToString());
                Console.WriteLine("Message: " + oeGetAll.Message);
                Console.WriteLine("Code: " + oeGetAll.Code.ToString());
                Console.WriteLine("Results Length: " + oeGetAll.Results.Length);
                Console.WriteLine("MoreResults: " + oeGetAll.MoreResults.ToString());
            }
#endif
        }
    }
}
