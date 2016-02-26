using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_LinkSend()
        {

            Console.WriteLine("--- Testing LinkSend ---");
            ET_Client myclient = new ET_Client();

            Console.WriteLine("Retrieve Filtered LinkSend");
            ET_LinkSend oe = new ET_LinkSend();
            oe.AuthStub = myclient;
            oe.SearchFilter = new SimpleFilterPart() { Property = "SendID", SimpleOperator = SimpleOperators.equals, Value = new String[] { "11428050" } };
            oe.Props = new string[] { "ID","SendID","PartnerKey","Client.ID","Client.PartnerClientKey","Link.ID","Link.PartnerKey","Link.TotalClicks","Link.UniqueClicks","Link.URL","Link.Alias" };
            GetReturn oeGet = oe.Get();

            Console.WriteLine("Get Status: " + oeGet.Status.ToString());
            Console.WriteLine("Message: " + oeGet.Message.ToString());
            Console.WriteLine("Code: " + oeGet.Code.ToString());
            Console.WriteLine("Results Length: " + oeGet.Results.Length);
            Console.WriteLine("MoreResults: " + oeGet.MoreResults.ToString());
            
            foreach (ET_LinkSend ls in oeGet.Results)
            {
                Console.WriteLine("SendID: " + ls.SendID + ", URL: " + ls.Link.URL + ", UniqueClicks: " + ls.Link.UniqueClicks + ", TotalClicks: " + ls.Link.TotalClicks);               
            }
        }
    }
}
