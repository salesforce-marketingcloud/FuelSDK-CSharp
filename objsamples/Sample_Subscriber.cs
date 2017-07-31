using FuelSDK;
using System;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_Subscriber()
        {
            var myclient = new ET_Client();

            Console.WriteLine("--- Testing Subscriber ---");
            var subscriberTestEmail = "CSharpSDKExample223@bh.exacttarget.com";

            Console.WriteLine("\n Create Subscriber");
            var postSub = new ET_Subscriber
            {
                AuthStub = myclient,
                EmailAddress = subscriberTestEmail,
                Attributes = new[] { new ET_ProfileAttribute { Name = "First Name", Value = "ExactTarget Example" } },
            };
            var postResponse = postSub.Post();
            Console.WriteLine("Post Status: " + postResponse.Status.ToString());
            Console.WriteLine("Message: " + postResponse.Message);
            Console.WriteLine("Code: " + postResponse.Code.ToString());
            Console.WriteLine("Results Length: " + postResponse.Results.Length);

            if (postResponse.Results.Length > 0)
            {
                Console.WriteLine("--NewID: " + postResponse.Results[0].NewID.ToString());
                foreach (ET_ProfileAttribute attr in ((ET_Subscriber)postResponse.Results[0].Object).Attributes)
                    Console.WriteLine("Name: " + attr.Name + ", Value: " + attr.Value);
            }

            Console.WriteLine("\n Retrieve newly created Subscriber");
            var getSub = new ET_Subscriber
            {
                AuthStub = myclient,
                Props = new[] { "SubscriberKey", "EmailAddress", "Status" },
                SearchFilter = new SimpleFilterPart { Property = "SubscriberKey", SimpleOperator = SimpleOperators.equals, Value = new[] { subscriberTestEmail } },
            };
            var getResponse = getSub.Get();
            Console.WriteLine("Get Status: " + getResponse.Status.ToString());
            Console.WriteLine("Message: " + getResponse.Message);
            Console.WriteLine("Code: " + getResponse.Code.ToString());
            Console.WriteLine("Results Length: " + getResponse.Results.Length);
            foreach (ET_Subscriber sub in getResponse.Results)
                Console.WriteLine("--EmailAddress: " + sub.EmailAddress + " Status: " + sub.Status.ToString());

            Console.WriteLine("\n Update Subscriber");
            var patchSub = new ET_Subscriber
            {
                AuthStub = myclient,
                EmailAddress = subscriberTestEmail,
                Status = SubscriberStatus.Unsubscribed,
                Attributes = new[] { new ET_ProfileAttribute { Name = "First Name", Value = "ExactTarget Example" } },
            };
            var pathResponse = patchSub.Patch();
            Console.WriteLine("Patch Status: " + pathResponse.Status.ToString());
            Console.WriteLine("Message: " + pathResponse.Message);
            Console.WriteLine("Code: " + pathResponse.Code.ToString());
            Console.WriteLine("Results Length: " + pathResponse.Results.Length);
            foreach (ResultDetail rd in pathResponse.Results)
            {
                var sub = (ET_Subscriber)rd.Object;
                Console.WriteLine("--EmailAddress: " + sub.EmailAddress + " Status: " + sub.Status.ToString());
            }

            Console.WriteLine("\n Retrieve Subscriber that should have status unsubscribed now");
            getResponse = getSub.Get();
            Console.WriteLine("Get Status: " + getResponse.Status.ToString());
            Console.WriteLine("Message: " + getResponse.Message);
            Console.WriteLine("Code: " + getResponse.Code.ToString());
            Console.WriteLine("Results Length: " + getResponse.Results.Length);
            foreach (ET_Subscriber sub in getResponse.Results)
                Console.WriteLine("--EmailAddress: " + sub.EmailAddress + " Status: " + sub.Status.ToString());

            Console.WriteLine("\n Delete Subscriber");
            var deleteSub = new ET_Subscriber
            {
                AuthStub = myclient,
                EmailAddress = subscriberTestEmail,
            };
            var deleteResponse = deleteSub.Delete();
            Console.WriteLine("Delete Status: " + deleteResponse.Status.ToString());
            Console.WriteLine("Message: " + deleteResponse.Message);
            Console.WriteLine("Code: " + deleteResponse.Code.ToString());
            Console.WriteLine("Results Length: " + deleteResponse.Results.Length);

            Console.WriteLine("\n Retrieve Subscriber to confirm deletion");
            getResponse = getSub.Get();
            Console.WriteLine("Get Status: " + getResponse.Status.ToString());
            Console.WriteLine("Message: " + getResponse.Message);
            Console.WriteLine("Code: " + getResponse.Code.ToString());
            Console.WriteLine("Results Length: " + getResponse.Results.Length);
            foreach (ET_Subscriber sub in getResponse.Results)
                Console.WriteLine("--EmailAddress: " + sub.EmailAddress + " Status: " + sub.Status.ToString());
        }
    }
}
