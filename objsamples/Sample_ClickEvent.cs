using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_ClickEvent()
        {

            DateTime filterDate = new DateTime(2013, 1, 15, 13, 0, 0);

            Console.WriteLine("--- Testing ClickEvent ---");
            ET_Client myclient = new ET_Client();

            Console.WriteLine("Retrieve Filtered ClickEvents with GetMoreResults");
            ET_ClickEvent oe = new ET_ClickEvent();
            oe.AuthStub = myclient;
            oe.SearchFilter = new SimpleFilterPart() { Property = "EventDate", SimpleOperator = SimpleOperators.greaterThan, DateValue = new DateTime[] { filterDate } };
            oe.Props = new string[] { "SendID", "SubscriberKey", "EventDate", "Client.ID", "EventType", "BatchID", "TriggeredSendDefinitionObjectID", "PartnerKey" };
            GetReturn oeGet = oe.Get();

            Console.WriteLine("Get Status: " + oeGet.Status.ToString());
            Console.WriteLine("Message: " + oeGet.Message.ToString());
            Console.WriteLine("Code: " + oeGet.Code.ToString());
            Console.WriteLine("Results Length: " + oeGet.Results.Length);
            Console.WriteLine("MoreResults: " + oeGet.MoreResults.ToString());
            // Since this could potentially return a large number of results, we do not want to print the results
            //foreach (ET_ClickEvent ClickEvent in oeGet.Results)
            //{
            //    Console.WriteLine("SubscriberKey: " + ClickEvent.SubscriberKey + ", EventDate: " + ClickEvent.EventDate.ToString());
            //}

            while (oeGet.MoreResults)
            {
                Console.WriteLine("Continue Retrieve Filtered ClickEvents with GetMoreResults");
                oeGet = oe.GetMoreResults();
                Console.WriteLine("Get Status: " + oeGet.Status.ToString());
                Console.WriteLine("Message: " + oeGet.Message.ToString());
                Console.WriteLine("Code: " + oeGet.Code.ToString());
                Console.WriteLine("Results Length: " + oeGet.Results.Length);
                Console.WriteLine("MoreResults: " + oeGet.MoreResults.ToString());
            }


            //The following request could potentially bring back large amounts of data if run against a production account	
            //Console.WriteLine("Retrieve All ClickEvents with GetMoreResults");
            //ET_ClickEvent oe = new ET_ClickEvent();
            //oe.authStub = myclient;
            //oe.props = new string[] { "SendID", "SubscriberKey", "EventDate", "Client.ID", "EventType", "BatchID", "TriggeredSendDefinitionObjectID", "PartnerKey" };
            //GetResponse oeGetAll = oe.Get();

            //Console.WriteLine("Get Status: " + oeGetAll.Status.ToString());
            //Console.WriteLine("Message: " + oeGetAll.Message.ToString());
            //Console.WriteLine("Code: " + oeGetAll.Code.ToString());
            //Console.WriteLine("Results Length: " + oeGetAll.Results.Length);
            //Console.WriteLine("MoreResults: " + oeGetAll.MoreResults.ToString());
            //// Since this could potentially return a large number of results, we do not want to print the results
            ////foreach (ET_ClickEvent ClickEvent in oeGet.Results)
            ////{
            ////    Console.WriteLine("SubscriberKey: " + ClickEvent.SubscriberKey + ", EventDate: " + ClickEvent.EventDate.ToString());
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
