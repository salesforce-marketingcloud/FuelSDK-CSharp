using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_Send()
        {
            DateTime filterDate = new DateTime(2014, 1, 1, 13, 0, 0);

            Console.WriteLine("--- Testing Send ---");
            ET_Client myclient = new ET_Client();

            Console.WriteLine("Retrieve Filtered Send with GetMoreResults");
            ET_Send s = new ET_Send();
            s.AuthStub = myclient;
            s.SearchFilter = new SimpleFilterPart() { Property = "SendDate", SimpleOperator = SimpleOperators.greaterThan, DateValue = new DateTime[] { filterDate } };
            s.Props = new string[] { "ID", "PartnerKey", "CreatedDate", "ModifiedDate", "Client.ID", "Client.PartnerClientKey", "Email.ID", "Email.PartnerKey", "SendDate", "FromAddress", "FromName", "Duplicates", "InvalidAddresses", "ExistingUndeliverables", "ExistingUnsubscribes", "HardBounces", "SoftBounces", "OtherBounces", "ForwardedEmails", "UniqueClicks", "UniqueOpens", "NumberSent", "NumberDelivered", "NumberTargeted", "NumberErrored", "NumberExcluded", "Unsubscribes", "MissingAddresses", "Subject", "PreviewURL", "SentDate", "EmailName", "Status", "IsMultipart", "SendLimit", "SendWindowOpen", "SendWindowClose", "IsAlwaysOn", "Additional", "BCCEmail", "EmailSendDefinition.ObjectID", "EmailSendDefinition.CustomerKey" };
            GetReturn oeGet = s.Get();

            Console.WriteLine("Get Status: " + oeGet.Status.ToString());
            Console.WriteLine("Message: " + oeGet.Message.ToString());
            Console.WriteLine("Code: " + oeGet.Code.ToString());
            Console.WriteLine("Results Length: " + oeGet.Results.Length);
            Console.WriteLine("MoreResults: " + oeGet.MoreResults.ToString());
             //Since this could potentially return a large number of results, we do not want to print the results
            foreach (ET_Send send in oeGet.Results)
            {
                Console.WriteLine("JobID: " + send.ID + ", SendDate: " + send.SendDate );
            }

            while (oeGet.MoreResults)
            {
                Console.WriteLine("Continue Retrieve Filtered SentEvents with GetMoreResults");
                oeGet = s.GetMoreResults();
                Console.WriteLine("Get Status: " + oeGet.Status.ToString());
                Console.WriteLine("Message: " + oeGet.Message.ToString());
                Console.WriteLine("Code: " + oeGet.Code.ToString());
                Console.WriteLine("Results Length: " + oeGet.Results.Length);
                Console.WriteLine("MoreResults: " + oeGet.MoreResults.ToString());
            }

        }
    }
}
