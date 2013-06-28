using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_OpenEvent()
        {

            DateTime filterDate = new DateTime(2013, 1, 15, 13, 0, 0);

            Console.WriteLine("--- Testing OpenEvent ---");
            ET_Client myclient = new ET_Client();

            Console.WriteLine("Retrieve Filtered OpenEvents with GetMoreResults");
            ET_OpenEvent oe = new ET_OpenEvent();
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
            //foreach (ET_OpenEvent openEvent in oeGet.Results)
            //{
            //    Console.WriteLine("SubscriberKey: " + openEvent.SubscriberKey + ", EventDate: " + openEvent.EventDate.ToString());
            //}

            while (oeGet.MoreResults)
            {
                Console.WriteLine("Continue Retrieve Filtered OpenEvents with GetMoreResults");
                oeGet = oe.GetMoreResults();
                Console.WriteLine("Get Status: " + oeGet.Status.ToString());
                Console.WriteLine("Message: " + oeGet.Message.ToString());
                Console.WriteLine("Code: " + oeGet.Code.ToString());
                Console.WriteLine("Results Length: " + oeGet.Results.Length);
                Console.WriteLine("MoreResults: " + oeGet.MoreResults.ToString());
            }


            //The following request could potentially bring back large amounts of data if run against a production account	
            //Console.WriteLine("Retrieve All OpenEvents with GetMoreResults");
            //ET_OpenEvent oe = new ET_OpenEvent();
            //oe.authStub = myclient;
            //oe.props = new string[] { "SendID", "SubscriberKey", "EventDate", "Client.ID", "EventType", "BatchID", "TriggeredSendDefinitionObjectID", "PartnerKey" };
            //GetResponse oeGetAll = oe.Get();

            //Console.WriteLine("Get Status: " + oeGetAll.Status.ToString());
            //Console.WriteLine("Message: " + oeGetAll.Message.ToString());
            //Console.WriteLine("Code: " + oeGetAll.Code.ToString());
            //Console.WriteLine("Results Length: " + oeGetAll.Results.Length);
            //Console.WriteLine("MoreResults: " + oeGetAll.MoreResults.ToString());
            //// Since this could potentially return a large number of results, we do not want to print the results
            ////foreach (ET_OpenEvent openEvent in oeGet.Results)
            ////{
            ////    Console.WriteLine("SubscriberKey: " + openEvent.SubscriberKey + ", EventDate: " + openEvent.EventDate.ToString());
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
