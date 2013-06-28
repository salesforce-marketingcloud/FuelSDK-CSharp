using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_TriggeredSend(){
            ET_Client myclient = new ET_Client();

            Console.WriteLine("--- Testing TriggeredSend ---");
            string TSNameForCreateThenDelete = Guid.NewGuid().ToString();
            string ExistingTSDCustomerKey = "TEXTEXT";

            Console.WriteLine("\n Send Using an Existing Definition ");
            ET_TriggeredSend tsdSend = new ET_TriggeredSend();
            tsdSend.AuthStub = myclient;
            tsdSend.CustomerKey = ExistingTSDCustomerKey;
            tsdSend.Subscribers = new ET_Subscriber[] { new ET_Subscriber() { EmailAddress = "example@bh.exacttarget.com", SubscriberKey = "example@bh.exacttarget.com" } };
            SendReturn srSend = tsdSend.Send();
            Console.WriteLine("Send Status: " + srSend.Status.ToString());
            Console.WriteLine("Message: " + srSend.Message.ToString());
            Console.WriteLine("Code: " + srSend.Code.ToString());
            Console.WriteLine("Results Length: " + srSend.Results.Length);

            Console.WriteLine("\n Retrieve All TriggeredSend Definitions");
            ET_TriggeredSend tsdGetAll = new ET_TriggeredSend();
            tsdGetAll.AuthStub = myclient;
            tsdGetAll.Props = new string[] { "CustomerKey", "Name", "TriggeredSendStatus" };
            tsdGetAll.SearchFilter = new SimpleFilterPart() { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new string[] { TSNameForCreateThenDelete } };
            GetReturn grAllTSD = tsdGetAll.Get();
            Console.WriteLine("Get Status: " + grAllTSD.Status.ToString());
            Console.WriteLine("Message: " + grAllTSD.Message.ToString());
            Console.WriteLine("Code: " + grAllTSD.Code.ToString());
            Console.WriteLine("Results Length: " + grAllTSD.Results.Length);

            foreach (ET_TriggeredSend result in grAllTSD.Results)
            {
                Console.WriteLine("--CustomerKey: " + result.CustomerKey + ", Name: " + result.Name + ", Status: " + result.TriggeredSendStatus.ToString());
            }

            Console.WriteLine("\n Create a TriggeredSend Definition");
            ET_TriggeredSend tsd = new ET_TriggeredSend();
            tsd.AuthStub = myclient;
            tsd.Name = TSNameForCreateThenDelete;
            tsd.CustomerKey = TSNameForCreateThenDelete;
            tsd.Email = new ET_Email() { ID = 3113962 };
            tsd.SendClassification = new ET_SendClassification() { CustomerKey = "2240" };
            PostReturn prTSD = tsd.Post();
            Console.WriteLine("Post Status: " + prTSD.Status.ToString());
            Console.WriteLine("Message: " + prTSD.Message.ToString());
            Console.WriteLine("Code: " + prTSD.Code.ToString());
            Console.WriteLine("Results Length: " + prTSD.Results.Length);

            Console.WriteLine("\n Retrieve Single TriggeredSend");
            ET_TriggeredSend tsdGet = new ET_TriggeredSend();
            tsdGet.AuthStub = myclient;
            tsdGet.Props = new string[] { "CustomerKey", "Name", "TriggeredSendStatus" };
            tsdGet.SearchFilter = new SimpleFilterPart() { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new string[] { TSNameForCreateThenDelete } };
            GetReturn tsdGetSingle = tsdGet.Get();
            Console.WriteLine("Get Status: " + tsdGetSingle.Status.ToString());
            Console.WriteLine("Message: " + tsdGetSingle.Message.ToString());
            Console.WriteLine("Code: " + tsdGetSingle.Code.ToString());
            Console.WriteLine("Results Length: " + tsdGetSingle.Results.Length);

            foreach (ET_TriggeredSend result in tsdGetSingle.Results)
            {
                Console.WriteLine("--CustomerKey: " + result.CustomerKey + ", Name: " + result.Name + ", Status: " + result.TriggeredSendStatus.ToString());
            }

            Console.WriteLine("\n Start a TriggeredSend by setting to Active");
            ET_TriggeredSend tsdPatch = new ET_TriggeredSend();
            tsdPatch.AuthStub = myclient;
            tsdPatch.CustomerKey = TSNameForCreateThenDelete;
            tsdPatch.TriggeredSendStatus = TriggeredSendStatusEnum.Active;
            PatchReturn patchrTSD = tsdPatch.Patch();
            Console.WriteLine("Patch Status: " + patchrTSD.Status.ToString());
            Console.WriteLine("Message: " + patchrTSD.Message.ToString());
            Console.WriteLine("Code: " + patchrTSD.Code.ToString());
            Console.WriteLine("Results Length: " + patchrTSD.Results.Length);

            Console.WriteLine("\n  Retrieve Single TriggeredSend After setting to active");
            tsdGetSingle = tsdGet.Get();
            Console.WriteLine("Get Status: " + tsdGetSingle.Status.ToString());
            Console.WriteLine("Message: " + tsdGetSingle.Message.ToString());
            Console.WriteLine("Code: " + tsdGetSingle.Code.ToString());
            Console.WriteLine("Results Length: " + tsdGetSingle.Results.Length);

            foreach (ET_TriggeredSend result in tsdGetSingle.Results)
            {
                Console.WriteLine("--CustomerKey: " + result.CustomerKey + ", Name: " + result.Name + ", Status: " + result.TriggeredSendStatus.ToString());
            }

            Console.WriteLine("\n  Send using new definition");
            ET_TriggeredSend tsdSendNew = new ET_TriggeredSend();
            tsdSendNew.AuthStub = myclient;
            tsdSendNew.CustomerKey = TSNameForCreateThenDelete;
            tsdSendNew.Subscribers = new ET_Subscriber[] { new ET_Subscriber() { EmailAddress = "example@bh.exacttarget.com", SubscriberKey = "example@bh.exacttarget.com" } };
            SendReturn srSendnew = tsdSendNew.Send();
            Console.WriteLine("Send Status: " + srSendnew.Status.ToString());
            Console.WriteLine("Message: " + srSendnew.Message.ToString());
            Console.WriteLine("Code: " + srSendnew.Code.ToString());
            Console.WriteLine("Results Length: " + srSendnew.Results.Length);

            Console.WriteLine("\n Pause a TriggeredSend by setting to Inactive");
            tsdPatch.TriggeredSendStatus = TriggeredSendStatusEnum.Inactive;
            patchrTSD = tsdPatch.Patch();
            Console.WriteLine("Patch Status: " + patchrTSD.Status.ToString());
            Console.WriteLine("Message: " + patchrTSD.Message.ToString());
            Console.WriteLine("Code: " + patchrTSD.Code.ToString());
            Console.WriteLine("Results Length: " + patchrTSD.Results.Length);

            Console.WriteLine("\n Delete a TriggeredSend Definition");
            ET_TriggeredSend tsdDelete = new ET_TriggeredSend();
            tsdDelete.AuthStub = myclient;
            tsdDelete.CustomerKey = TSNameForCreateThenDelete;
            DeleteReturn drTSD = tsdDelete.Delete();
            Console.WriteLine("Delete Status: " + drTSD.Status.ToString());
            Console.WriteLine("Message: " + drTSD.Message.ToString());
            Console.WriteLine("Code: " + drTSD.Code.ToString());
            Console.WriteLine("Results Length: " + drTSD.Results.Length);

        }
    }
}
