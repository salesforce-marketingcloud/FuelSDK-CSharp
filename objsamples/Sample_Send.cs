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
            GetReturn sGet = s.Get();

            Console.WriteLine("Get Status: " + sGet.Status.ToString());
            Console.WriteLine("Message: " + sGet.Message.ToString());
            Console.WriteLine("Code: " + sGet.Code.ToString());
            Console.WriteLine("Results Length: " + sGet.Results.Length);
            Console.WriteLine("MoreResults: " + sGet.MoreResults.ToString());
            foreach (ET_Send send in sGet.Results)
            {
                Console.WriteLine("JobID: " + send.ID + ", SendDate: " + send.SendDate );
            }

            while (sGet.MoreResults)
            {
                Console.WriteLine("Continue Retrieve Filtered Send with GetMoreResults");
                sGet = s.GetMoreResults();
                Console.WriteLine("Get Status: " + sGet.Status.ToString());
                Console.WriteLine("Message: " + sGet.Message.ToString());
                Console.WriteLine("Code: " + sGet.Code.ToString());
                Console.WriteLine("Results Length: " + sGet.Results.Length);
                Console.WriteLine("MoreResults: " + sGet.MoreResults.ToString());
            }

        }
    }
}
