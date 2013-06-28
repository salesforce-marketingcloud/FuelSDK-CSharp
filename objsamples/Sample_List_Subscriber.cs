using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_ListSubscriber()
        {

            string NewListName = "CSharpSDKListSubscriber";
            string SubscriberTestEmail = "CSharpSDKListSubscriber@bh.exacttarget.com";

            Console.WriteLine("--- Testing ListSubscriber ---");
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
                ET_Subscriber postSub = new ET_Subscriber();
                postSub.Lists = new ET_SubscriberList[] { new ET_SubscriberList() { ID = newListID } };
                postSub.AuthStub = myclient;
                postSub.EmailAddress = SubscriberTestEmail;
                postSub.Attributes = new FuelSDK.ET_ProfileAttribute[] { new ET_ProfileAttribute() { Name = "First Name", Value = "ExactTarget Example" } };
                PostReturn postResponse = postSub.Post();
                Console.WriteLine("Post Status: " + postResponse.Status.ToString());
                Console.WriteLine("Message: " + postResponse.Message.ToString());
                Console.WriteLine("Code: " + postResponse.Code.ToString());
                Console.WriteLine("Results Length: " + postResponse.Results.Length);


                if (!postResponse.Status)
                {
                    if (postResponse.Results.Length > 0 && postResponse.Results[0].ErrorCode == 12014)
                    {
                        // If the subscriber already exists in the account then we need to do an update.
                        //Update Subscriber On List 
                        Console.WriteLine("\n Update Subscriber to add to List");
                        PatchReturn patchResponse = postSub.Patch();
                        Console.WriteLine("Post Status: " + patchResponse.Status.ToString());
                        Console.WriteLine("Message: " + patchResponse.Message.ToString());
                        Console.WriteLine("Code: " + patchResponse.Code.ToString());
                        Console.WriteLine("Results Length: " + patchResponse.Results.Length);
                    }
                }

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
                    Console.WriteLine("--ListID: " + ResultListSub.ID + ", SubscriberKey(EmailAddress): " + ResultListSub.SubscriberKey);
                }
            }


            //Console.WriteLine("Retrieve Filtered ListSubscribers with GetMoreResults");
            //ET_ListSubscriber oe = new ET_ListSubscriber();
            //oe.authStub = myclient;
            //oe.SearchFilter = new SimpleFilterPart() { Property = "EventDate", SimpleOperator = SimpleOperators.greaterThan, DateValue = new DateTime[] { filterDate } };
            //oe.props = new string[] { "ObjectID", "SubscriberKey", "CreatedDate", "Client.ID", "Client.PartnerClientKey", "ListID", "Status" };
            //GetReturn oeGet = oe.Get();

            //Console.WriteLine("Get Status: " + oeGet.Status.ToString());
            //Console.WriteLine("Message: " + oeGet.Message.ToString());
            //Console.WriteLine("Code: " + oeGet.Code.ToString());
            //Console.WriteLine("Results Length: " + oeGet.Results.Length);
            //Console.WriteLine("MoreResults: " + oeGet.MoreResults.ToString());
            //// Since this could potentially return a large number of results, we do not want to print the results
            ////foreach (ET_ListSubscriber ListSubscriber in oeGet.Results)
            ////{
            ////    Console.WriteLine("SubscriberKey: " + ListSubscriber.SubscriberKey + ", EventDate: " + ListSubscriber.EventDate.ToString());
            ////}

            //while (oeGet.MoreResults)
            //{
            //    Console.WriteLine("Continue Retrieve Filtered ListSubscribers with GetMoreResults");
            //    oeGet = oe.GetMoreResults();
            //    Console.WriteLine("Get Status: " + oeGet.Status.ToString());
            //    Console.WriteLine("Message: " + oeGet.Message.ToString());
            //    Console.WriteLine("Code: " + oeGet.Code.ToString());
            //    Console.WriteLine("Results Length: " + oeGet.Results.Length);
            //    Console.WriteLine("MoreResults: " + oeGet.MoreResults.ToString());
            //}


            //The following request could potentially bring back large amounts of data if run against a production account	
            //Console.WriteLine("Retrieve All ListSubscribers with GetMoreResults");
            //ET_ListSubscriber oe = new ET_ListSubscriber();
            //oe.authStub = myclient;
            //oe.props = new string[] { "SendID", "SubscriberKey", "EventDate", "Client.ID", "EventType", "BatchID", "TriggeredSendDefinitionObjectID", "PartnerKey" };
            //GetResponse oeGetAll = oe.Get();

            //Console.WriteLine("Get Status: " + oeGetAll.Status.ToString());
            //Console.WriteLine("Message: " + oeGetAll.Message.ToString());
            //Console.WriteLine("Code: " + oeGetAll.Code.ToString());
            //Console.WriteLine("Results Length: " + oeGetAll.Results.Length);
            //Console.WriteLine("MoreResults: " + oeGetAll.MoreResults.ToString());
            //// Since this could potentially return a large number of results, we do not want to print the results
            ////foreach (ET_ListSubscriber ListSubscriber in oeGet.Results)
            ////{
            ////    Console.WriteLine("SubscriberKey: " + ListSubscriber.SubscriberKey + ", EventDate: " + ListSubscriber.EventDate.ToString());
            ////}

            //while (oeGetAll.MoreResults)
            //{
            //    oeGetAll = oe.GetMoreResults();
            //    Console.WriteLine("Get Status: " + oeGetAll.Status.ToString());
            //    Console.WriteLine("Message: " + oeGetAll.Message.ToString());
            //    Console.WriteLine("Code: " + oeGetAll.Code.ToString());
            //    Console.WriteLine("Results Length: " + oeGetAll.Results.Length);
            //    Console.WriteLine("MoreResults: " + oeGetAll.MoreResults.ToString());
            //}
        }
    }
}
