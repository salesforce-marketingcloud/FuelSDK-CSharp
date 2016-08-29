using FuelSDK;
using System;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_BounceEvent()
        {
            var filterDate = new DateTime(2011, 1, 15, 13, 0, 0);

            Console.WriteLine("--- Testing BounceEvent ---");
            var myclient = new ET_Client();

            Console.WriteLine("Retrieve Filtered BounceEvents with GetMoreResults");
            var oe = new ET_BounceEvent
            {
                AuthStub = myclient,
                SearchFilter = new SimpleFilterPart { Property = "EventDate", SimpleOperator = SimpleOperators.greaterThan, DateValue = new[] { filterDate } },
                Props = new string[] { "SendID", "SubscriberKey", "EventDate", "Client.ID", "EventType", "BatchID", "TriggeredSendDefinitionObjectID", "PartnerKey" },
            };
            var oeGet = oe.Get();

            Console.WriteLine("Get Status: " + oeGet.Status.ToString());
            Console.WriteLine("Message: " + oeGet.Message);
            Console.WriteLine("Code: " + oeGet.Code.ToString());
            Console.WriteLine("Results Length: " + oeGet.Results.Length);
            Console.WriteLine("MoreResults: " + oeGet.MoreResults.ToString());
            // Since this could potentially return a large number of results, we do not want to print the results
            //foreach (ET_BounceEvent BounceEvent in oeGet.Results)
            //    Console.WriteLine("SubscriberKey: " + BounceEvent.SubscriberKey + ", EventDate: " + BounceEvent.EventDate.ToString());
            while (oeGet.MoreResults)
            {
                Console.WriteLine("Continue Retrieve Filtered BounceEvents with GetMoreResults");
                oeGet = oe.GetMoreResults();
                Console.WriteLine("Get Status: " + oeGet.Status.ToString());
                Console.WriteLine("Message: " + oeGet.Message);
                Console.WriteLine("Code: " + oeGet.Code.ToString());
                Console.WriteLine("Results Length: " + oeGet.Results.Length);
                Console.WriteLine("MoreResults: " + oeGet.MoreResults.ToString());
            }

#if false
            // The following request could potentially bring back large amounts of data if run against a production account	
            Console.WriteLine("Retrieve All BounceEvents with GetMoreResults");
            var oe2 = new ET_BounceEvent
            {
                AuthStub = myclient,
                Props = new string[] { "SendID", "SubscriberKey", "EventDate", "Client.ID", "EventType", "BatchID", "TriggeredSendDefinitionObjectID", "PartnerKey" },
            };
            var oeGetAll = oe2.Get();

            Console.WriteLine("Get Status: " + oeGetAll.Status.ToString());
            Console.WriteLine("Message: " + oeGetAll.Message);
            Console.WriteLine("Code: " + oeGetAll.Code.ToString());
            Console.WriteLine("Results Length: " + oeGetAll.Results.Length);
            Console.WriteLine("MoreResults: " + oeGetAll.MoreResults.ToString());
            // Since this could potentially return a large number of results, we do not want to print the results
            //foreach (ET_BounceEvent BounceEvent in oeGet.Results)
            //    Console.WriteLine("SubscriberKey: " + BounceEvent.SubscriberKey + ", EventDate: " + BounceEvent.EventDate.ToString());
            while (oeGetAll.MoreResults)
            {
                oeGetAll = oe2.GetMoreResults();
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
