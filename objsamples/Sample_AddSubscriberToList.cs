using FuelSDK;
using System;

namespace objsamples
{
    partial class Tester
    {
        static void Test_AddSubscriberToList()
        {
            var newListName = "CSharpSDKAddSubscriberToList";
            var subscriberTestEmail = "AddSubToListExample@bh.exacttarget.com";

            Console.WriteLine("--- Testing AddSubscriberToList ---");
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
                var hrAddSub = myclient.AddSubscribersToList(subscriberTestEmail, new[] { newListID });
                Console.WriteLine("Helper Status: " + hrAddSub.Status.ToString());
                Console.WriteLine("Message: " + hrAddSub.Message);
                Console.WriteLine("Code: " + hrAddSub.Code.ToString());

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
                foreach (ET_List_Subscriber ResultListSub in getResponse.Results)
                    Console.WriteLine("--ListID: " + ResultListSub.ListID + ", SubscriberKey(EmailAddress): " + ResultListSub.SubscriberKey);

                Console.WriteLine("\n Delete List");
                postList.ID = newListID;
                var drList = postList.Delete();
                Console.WriteLine("Delete Status: " + drList.Status.ToString());
            }
        }
    }
}
