using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void Test_AddSubscriberToList()
        {

            string NewListName = "CSharpSDKAddSubscriberToList";
            string SubscriberTestEmail = "AddSubToListExample@bh.exacttarget.com";

            Console.WriteLine("--- Testing AddSubscriberToList ---");
            ET_Client myclient = new ET_Client();

            Console.WriteLine("\n Create List");
            ET_List postList = new ET_List();
            postList.AuthStub = myclient;
            postList.ListName = NewListName;
            PostReturn prList = postList.Post();

            if (prList.Status && prList.Results.Length > 0)
            {
                int newListID = prList.Results[0].Object.ID;

                Console.WriteLine("\n Create Subscriber on List");
                FuelReturn hrAddSub = myclient.AddSubscribersToList(SubscriberTestEmail, new List<int>() { newListID });
                Console.WriteLine("Helper Status: " + hrAddSub.Status.ToString());
                Console.WriteLine("Message: " + hrAddSub.Message.ToString());
                Console.WriteLine("Code: " + hrAddSub.Code.ToString());

                Console.WriteLine("\n Retrieve all Subscribers on the List");
                ET_List_Subscriber getListSub = new ET_List_Subscriber();
                getListSub.AuthStub = myclient;
                getListSub.Props = new string[] { "ObjectID", "SubscriberKey", "CreatedDate", "Client.ID", "Client.PartnerClientKey", "ListID", "Status" };
                getListSub.SearchFilter = new SimpleFilterPart() { Property = "ListID", SimpleOperator = SimpleOperators.equals, Value = new string[] { newListID.ToString() } };
                GetReturn getResponse = getListSub.Get();
                Console.WriteLine("Get Status: " + getResponse.Status.ToString());
                Console.WriteLine("Message: " + getResponse.Message.ToString());
                Console.WriteLine("Code: " + getResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getResponse.Results.Length);
                foreach (ET_List_Subscriber ResultListSub in getResponse.Results)
                {
                    Console.WriteLine("--ListID: " + ResultListSub.ListID + ", SubscriberKey(EmailAddress): " + ResultListSub.SubscriberKey);
                }

                Console.WriteLine("\n Delete List");
                postList.ID = newListID;
                DeleteReturn drList = postList.Delete();
                Console.WriteLine("Delete Status: " + drList.Status.ToString());
            }
        }
    }
}
