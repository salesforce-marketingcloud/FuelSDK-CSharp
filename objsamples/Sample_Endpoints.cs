using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_Endpoint()
        {
            ET_Client myclient = new ET_Client();

            Console.WriteLine("--- Testing Endpoint ---");

            Console.WriteLine("\n Retrieve All Endpoints");
            ET_Endpoint getEndpoint = new ET_Endpoint();
            getEndpoint.AuthStub = myclient;
            GetReturn grEndpoint = getEndpoint.Get();

            Console.WriteLine("Get Status: " + grEndpoint.Status.ToString());
            Console.WriteLine("Message: " + grEndpoint.Message.ToString());
            Console.WriteLine("Code: " + grEndpoint.Code.ToString());
            Console.WriteLine("Results Length: " + grEndpoint.Results.Length);
            Console.WriteLine("MoreResults: " + grEndpoint.MoreResults.ToString());

            Console.WriteLine("\n Retrieve Single Endpoint by Type");
            ET_Endpoint getSingleEndpoint = new ET_Endpoint();
            getSingleEndpoint.AuthStub = myclient;
            getSingleEndpoint.Type = "soap";
            GetReturn grSingleEndpoint = getSingleEndpoint.Get();

            Console.WriteLine("Get Status: " + grSingleEndpoint.Status.ToString());
            Console.WriteLine("Message: " + grSingleEndpoint.Message.ToString());
            Console.WriteLine("Code: " + grSingleEndpoint.Code.ToString());
            Console.WriteLine("Results Length: " + grSingleEndpoint.Results.Length);
            Console.WriteLine("MoreResults: " + grSingleEndpoint.MoreResults.ToString());

        }
    }
}
