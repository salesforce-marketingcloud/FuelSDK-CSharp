using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_Subscriber()
        {
            ET_Client myclient = new ET_Client();

            Console.WriteLine("--- Testing Subscriber ---");
            string SubscriberTestEmail = "CSharpSDKExample223@bh.exacttarget.com";

            Console.WriteLine("\n Create Subscriber");
            ET_Subscriber postSub = new ET_Subscriber();
            postSub.AuthStub = myclient;
            postSub.EmailAddress = SubscriberTestEmail;
            postSub.Attributes = new FuelSDK.ET_ProfileAttribute[] {new ET_ProfileAttribute(){ Name= "First Name", Value = "ExactTarget Example"} };
            PostReturn postResponse = postSub.Post();
            Console.WriteLine("Post Status: " + postResponse.Status.ToString());
            Console.WriteLine("Message: " + postResponse.Message.ToString());
            Console.WriteLine("Code: " + postResponse.Code.ToString());
            Console.WriteLine("Results Length: " + postResponse.Results.Length);

            if (postResponse.Results.Length > 0)
            {
                Console.WriteLine("--NewID: " + postResponse.Results[0].NewID.ToString());
                foreach (ET_ProfileAttribute attr in ((ET_Subscriber)postResponse.Results[0].Object).Attributes) 
                {
                    Console.WriteLine("Name: " + attr.Name + ", Value: " + attr.Value);
                }
            }

            Console.WriteLine("\n Retrieve newly created Subscriber");
            ET_Subscriber getSub = new ET_Subscriber();
            getSub.AuthStub = myclient;
            getSub.Props = new string[] { "SubscriberKey", "EmailAddress", "Status" };
            getSub.SearchFilter = new SimpleFilterPart() { Property = "SubscriberKey", SimpleOperator = SimpleOperators.equals, Value = new string[] { SubscriberTestEmail } };
            GetReturn getResponse = getSub.Get();
            Console.WriteLine("Get Status: " + getResponse.Status.ToString());
            Console.WriteLine("Message: " + getResponse.Message.ToString());
            Console.WriteLine("Code: " + getResponse.Code.ToString());
            Console.WriteLine("Results Length: " + getResponse.Results.Length);

            foreach (ET_Subscriber sub in getResponse.Results) {
                Console.WriteLine("--EmailAddress: " + sub.EmailAddress + " Status: " + sub.Status.ToString());
            }


            Console.WriteLine("\n Update Subscriber");
            ET_Subscriber patchSub = new ET_Subscriber();
            patchSub.AuthStub = myclient;
            patchSub.EmailAddress = SubscriberTestEmail;
            patchSub.Status = SubscriberStatus.Unsubscribed;
            patchSub.Attributes = new FuelSDK.ET_ProfileAttribute[] { new ET_ProfileAttribute() { Name = "First Name", Value = "ExactTarget Example" } };
            PatchReturn pathResponse = patchSub.Patch();
            Console.WriteLine("Patch Status: " + pathResponse.Status.ToString());
            Console.WriteLine("Message: " + pathResponse.Message.ToString());
            Console.WriteLine("Code: " + pathResponse.Code.ToString());
            Console.WriteLine("Results Length: " + pathResponse.Results.Length);
            
            foreach (ResultDetail rd in pathResponse.Results){
                ET_Subscriber sub = (ET_Subscriber)rd.Object;
                Console.WriteLine("--EmailAddress: " + sub.EmailAddress + " Status: " + sub.Status.ToString());
            }

            Console.WriteLine("\n Retrieve Subscriber that should have status unsubscribed now");
            getResponse = getSub.Get();
            Console.WriteLine("Get Status: " + getResponse.Status.ToString());
            Console.WriteLine("Message: " + getResponse.Message.ToString());
            Console.WriteLine("Code: " + getResponse.Code.ToString());
            Console.WriteLine("Results Length: " + getResponse.Results.Length);

            foreach (ET_Subscriber sub in getResponse.Results)
            {
                Console.WriteLine("--EmailAddress: " + sub.EmailAddress + " Status: " + sub.Status.ToString());
            }

            Console.WriteLine("\n Delete Subscriber");
            ET_Subscriber deleteSub = new ET_Subscriber();
            deleteSub.AuthStub = myclient;
            deleteSub.EmailAddress = SubscriberTestEmail;
            DeleteReturn deleteResponse = deleteSub.Delete();
            Console.WriteLine("Delete Status: " + deleteResponse.Status.ToString());
            Console.WriteLine("Message: " + deleteResponse.Message.ToString());
            Console.WriteLine("Code: " + deleteResponse.Code.ToString());
            Console.WriteLine("Results Length: " + deleteResponse.Results.Length);

            Console.WriteLine("\n Retrieve Subscriber to confirm deletion");
            getResponse = getSub.Get();
            Console.WriteLine("Get Status: " + getResponse.Status.ToString());
            Console.WriteLine("Message: " + getResponse.Message.ToString());
            Console.WriteLine("Code: " + getResponse.Code.ToString());
            Console.WriteLine("Results Length: " + getResponse.Results.Length);

            foreach (ET_Subscriber sub in getResponse.Results)
            {
                Console.WriteLine("--EmailAddress: " + sub.EmailAddress + " Status: " + sub.Status.ToString());
            }           
        }
    }
}
