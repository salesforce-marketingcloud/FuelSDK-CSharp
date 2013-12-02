using FuelSDK;
using System;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_TriggeredSend()
        {
            var myclient = new ET_Client();

            Console.WriteLine("--- Testing TriggeredSend ---");
            var tsNameForCreateThenDelete = Guid.NewGuid().ToString();
            var existingTSDCustomerKey = "TEXTEXT";

            Console.WriteLine("\n Send Using an Existing Definition ");
            var tsdSend = new ET_TriggeredSend
            {
                AuthStub = myclient,
                CustomerKey = existingTSDCustomerKey,
                Subscribers = new[] { new ET_Subscriber { EmailAddress = "example@bh.exacttarget.com", SubscriberKey = "example@bh.exacttarget.com" } },
            };
            var srSend = tsdSend.Send();
            Console.WriteLine("Send Status: " + srSend.Status.ToString());
            Console.WriteLine("Message: " + srSend.Message);
            Console.WriteLine("Code: " + srSend.Code.ToString());
            Console.WriteLine("Results Length: " + srSend.Results.Length);

            Console.WriteLine("\n Retrieve All TriggeredSend Definitions");
            var tsdGetAll = new ET_TriggeredSend
            {
                AuthStub = myclient,
                Props = new[] { "CustomerKey", "Name", "TriggeredSendStatus" },
                SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { tsNameForCreateThenDelete } },
            };
            var grAllTSD = tsdGetAll.Get();
            Console.WriteLine("Get Status: " + grAllTSD.Status.ToString());
            Console.WriteLine("Message: " + grAllTSD.Message);
            Console.WriteLine("Code: " + grAllTSD.Code.ToString());
            Console.WriteLine("Results Length: " + grAllTSD.Results.Length);
            foreach (ET_TriggeredSend result in grAllTSD.Results)
                Console.WriteLine("--CustomerKey: " + result.CustomerKey + ", Name: " + result.Name + ", Status: " + result.TriggeredSendStatus.ToString());

            Console.WriteLine("\n Create a TriggeredSend Definition");
            var tsd = new ET_TriggeredSend
            {
                AuthStub = myclient,
                Name = tsNameForCreateThenDelete,
                CustomerKey = tsNameForCreateThenDelete,
                Email = new ET_Email { ID = 3113962 },
                SendClassification = new ET_SendClassification { CustomerKey = "2240" },
            };
            var prTSD = tsd.Post();
            Console.WriteLine("Post Status: " + prTSD.Status.ToString());
            Console.WriteLine("Message: " + prTSD.Message);
            Console.WriteLine("Code: " + prTSD.Code.ToString());
            Console.WriteLine("Results Length: " + prTSD.Results.Length);

            Console.WriteLine("\n Retrieve Single TriggeredSend");
            var tsdGet = new ET_TriggeredSend
            {
                AuthStub = myclient,
                Props = new[] { "CustomerKey", "Name", "TriggeredSendStatus" },
                SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { tsNameForCreateThenDelete } },
            };
            var tsdGetSingle = tsdGet.Get();
            Console.WriteLine("Get Status: " + tsdGetSingle.Status.ToString());
            Console.WriteLine("Message: " + tsdGetSingle.Message);
            Console.WriteLine("Code: " + tsdGetSingle.Code.ToString());
            Console.WriteLine("Results Length: " + tsdGetSingle.Results.Length);
            foreach (ET_TriggeredSend result in tsdGetSingle.Results)
                Console.WriteLine("--CustomerKey: " + result.CustomerKey + ", Name: " + result.Name + ", Status: " + result.TriggeredSendStatus.ToString());

            Console.WriteLine("\n Start a TriggeredSend by setting to Active");
            var tsdPatch = new ET_TriggeredSend
            {
                AuthStub = myclient,
                CustomerKey = tsNameForCreateThenDelete,
                TriggeredSendStatus = TriggeredSendStatusEnum.Active,
            };
            var patchrTSD = tsdPatch.Patch();
            Console.WriteLine("Patch Status: " + patchrTSD.Status.ToString());
            Console.WriteLine("Message: " + patchrTSD.Message);
            Console.WriteLine("Code: " + patchrTSD.Code.ToString());
            Console.WriteLine("Results Length: " + patchrTSD.Results.Length);

            Console.WriteLine("\n  Retrieve Single TriggeredSend After setting to active");
            tsdGetSingle = tsdGet.Get();
            Console.WriteLine("Get Status: " + tsdGetSingle.Status.ToString());
            Console.WriteLine("Message: " + tsdGetSingle.Message);
            Console.WriteLine("Code: " + tsdGetSingle.Code.ToString());
            Console.WriteLine("Results Length: " + tsdGetSingle.Results.Length);
            foreach (ET_TriggeredSend result in tsdGetSingle.Results)
                Console.WriteLine("--CustomerKey: " + result.CustomerKey + ", Name: " + result.Name + ", Status: " + result.TriggeredSendStatus.ToString());

            Console.WriteLine("\n  Send using new definition");
            var tsdSendNew = new ET_TriggeredSend
            {
                AuthStub = myclient,
                CustomerKey = tsNameForCreateThenDelete,
                Subscribers = new[] { new ET_Subscriber { EmailAddress = "example@bh.exacttarget.com", SubscriberKey = "example@bh.exacttarget.com" } },
            };
            var srSendnew = tsdSendNew.Send();
            Console.WriteLine("Send Status: " + srSendnew.Status.ToString());
            Console.WriteLine("Message: " + srSendnew.Message);
            Console.WriteLine("Code: " + srSendnew.Code.ToString());
            Console.WriteLine("Results Length: " + srSendnew.Results.Length);

            Console.WriteLine("\n Pause a TriggeredSend by setting to Inactive");
            tsdPatch.TriggeredSendStatus = TriggeredSendStatusEnum.Inactive;
            patchrTSD = tsdPatch.Patch();
            Console.WriteLine("Patch Status: " + patchrTSD.Status.ToString());
            Console.WriteLine("Message: " + patchrTSD.Message);
            Console.WriteLine("Code: " + patchrTSD.Code.ToString());
            Console.WriteLine("Results Length: " + patchrTSD.Results.Length);

            Console.WriteLine("\n Delete a TriggeredSend Definition");
            var tsdDelete = new ET_TriggeredSend
            {
                AuthStub = myclient,
                CustomerKey = tsNameForCreateThenDelete,
            };
            var drTSD = tsdDelete.Delete();
            Console.WriteLine("Delete Status: " + drTSD.Status.ToString());
            Console.WriteLine("Message: " + drTSD.Message);
            Console.WriteLine("Code: " + drTSD.Code.ToString());
            Console.WriteLine("Results Length: " + drTSD.Results.Length);
        }
    }
}
