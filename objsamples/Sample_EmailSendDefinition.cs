using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;
using System.Threading;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_EmailSendDefinition()
        {
            ET_Client myclient = new ET_Client();
            string NewSendDefinitionName = "CSharpSDKSendDefinition";
            string SendableDataExtensionCustomerKey = "F6F3871A-D124-499B-BBF5-3EFC0E827A51";
            int EmailIDForSendDefinition = 3113962;
            int ListIDForSendDefinition = 1729515;
            string SendClassificationCustomerKey = "2239";


            Console.WriteLine("--- Testing EmailSendDefinition ---");
            Console.WriteLine("\n Retrieve All EmailSendDefinition with GetMoreResults");
            ET_EmailSendDefinition getAllESD = new ET_EmailSendDefinition();
            getAllESD.AuthStub = myclient;
            getAllESD.Props = new string[] {"Client.ID", "CreatedDate","ModifiedDate","ObjectID","CustomerKey","Name","CategoryID","Description","SendClassification.CustomerKey","SenderProfile.CustomerKey","SenderProfile.FromName","SenderProfile.FromAddress","DeliveryProfile.CustomerKey","DeliveryProfile.SourceAddressType","DeliveryProfile.PrivateIP","DeliveryProfile.DomainType","DeliveryProfile.PrivateDomain","DeliveryProfile.HeaderSalutationSource","DeliveryProfile.FooterSalutationSource","SuppressTracking","IsSendLogging","Email.ID","BccEmail","AutoBccEmail","TestEmailAddr","EmailSubject","DynamicEmailSubject","IsMultipart","IsWrapped","SendLimit","SendWindowOpen","SendWindowClose","DeduplicateByEmail","ExclusionFilter","Additional"};
            GetReturn grAllEmail = getAllESD.Get();

            Console.WriteLine("Get Status: " + grAllEmail.Status.ToString());
            Console.WriteLine("Message: " + grAllEmail.Message.ToString());
            Console.WriteLine("Code: " + grAllEmail.Code.ToString());
            Console.WriteLine("Results Length: " + grAllEmail.Results.Length);

            while (grAllEmail.MoreResults)
            {
                Console.WriteLine("\n Continue Retrieve All Email with GetMoreResults");
                grAllEmail = getAllESD.GetMoreResults();
                Console.WriteLine("Get Status: " + grAllEmail.Status.ToString());
                Console.WriteLine("Message: " + grAllEmail.Message.ToString());
                Console.WriteLine("Code: " + grAllEmail.Code.ToString());
                Console.WriteLine("Results Length: " + grAllEmail.Results.Length);
            }

            Console.WriteLine("\n Create SendDefinition to DataExtension");
            ET_EmailSendDefinition postESDDE = new ET_EmailSendDefinition();
            postESDDE.AuthStub = myclient;
            postESDDE.Name = NewSendDefinitionName;
            postESDDE.CustomerKey = NewSendDefinitionName;
            postESDDE.Description = "Created with Fuel SDK";
            postESDDE.SendClassification = new ET_SendClassification() { CustomerKey = SendClassificationCustomerKey };
            postESDDE.SendDefinitionList = new ET_SendDefinitionList[] {new ET_SendDefinitionList() {CustomerKey = SendableDataExtensionCustomerKey, DataSourceTypeID = DataSourceTypeEnum.CustomObject}};
            postESDDE.Email = new ET_Email() {ID = EmailIDForSendDefinition };         
            PostReturn postResponse = postESDDE.Post();
            Console.WriteLine("Post Status: " + postResponse.Status.ToString());
            Console.WriteLine("Message: " + postResponse.Message.ToString());
            Console.WriteLine("Code: " + postResponse.Code.ToString());
            Console.WriteLine("Results Length: " + postResponse.Results.Length);

            Console.WriteLine("\n Delete SendDefinition");
            ET_EmailSendDefinition deleteESDDE = new ET_EmailSendDefinition();
            deleteESDDE.CustomerKey = NewSendDefinitionName;
            deleteESDDE.AuthStub = myclient;
            DeleteReturn deleteESDDEResponse = deleteESDDE.Delete();
            Console.WriteLine("Delete Status: " + deleteESDDEResponse.Status.ToString());
            Console.WriteLine("Message: " + deleteESDDEResponse.Message.ToString());
            Console.WriteLine("Code: " + deleteESDDEResponse.Code.ToString());
            Console.WriteLine("Results Length: " + deleteESDDEResponse.Results.Length);

            Console.WriteLine("\n Create SendDefinition to List");
            ET_EmailSendDefinition postESDL = new ET_EmailSendDefinition();
            postESDL.AuthStub = myclient;
            postESDL.Name = NewSendDefinitionName;
            postESDL.CustomerKey = NewSendDefinitionName;
            postESDL.Description = "Created with Fuel SDK";
            postESDL.SendClassification = new ET_SendClassification() { CustomerKey = SendClassificationCustomerKey };
            postESDL.SendDefinitionList = new ET_SendDefinitionList[] { new ET_SendDefinitionList() { List = new ET_List() { ID = ListIDForSendDefinition }, DataSourceTypeID = DataSourceTypeEnum.List } };
            postESDL.Email = new ET_Email() { ID = EmailIDForSendDefinition };
            PostReturn postESDLResponse = postESDDE.Post();
            Console.WriteLine("Post Status: " + postESDLResponse.Status.ToString());
            Console.WriteLine("Message: " + postESDLResponse.Message.ToString());
            Console.WriteLine("Code: " + postESDLResponse.Code.ToString());
            Console.WriteLine("Results Length: " + postESDLResponse.Results.Length);

            Console.WriteLine("\n Send using Definition ");
            ET_EmailSendDefinition sendESD = new ET_EmailSendDefinition();
            sendESD.AuthStub = myclient;
            sendESD.CustomerKey = NewSendDefinitionName;
            PerformReturn sendESDResponse = sendESD.Send();
            Console.WriteLine("Send Status: " + sendESDResponse.Status.ToString());
            Console.WriteLine("Message: " + sendESDResponse.Message.ToString());
            Console.WriteLine("Code: " + sendESDResponse.Code.ToString());
            Console.WriteLine("Results Length: " + sendESDResponse.Results.Length);

            Console.WriteLine("\n Retrieve All EmailSendDefinition with GetMoreResults");
            string EmailStatus = string.Empty;
            while(sendESDResponse.Status && EmailStatus != "Canceled" && EmailStatus != "Complete")
            {
                Console.WriteLine("\n Checking Status in Loop");
                GetReturn getESDStatusReturn = sendESD.Status();
                Console.WriteLine("Get Status: " + getESDStatusReturn.Status.ToString());
                Console.WriteLine("Message: " + getESDStatusReturn.Message.ToString());
                Console.WriteLine("Code: " + getESDStatusReturn.Code.ToString());
                Console.WriteLine("Results Length: " + getESDStatusReturn.Results.Length);
                EmailStatus = ((ET_Send)getESDStatusReturn.Results[0]).Status;
                Console.WriteLine("Send Status: " + EmailStatus);
                Thread.Sleep(5000);
            }

            Console.WriteLine("\n Delete SendDefinition");
            ET_EmailSendDefinition deleteESDL = new ET_EmailSendDefinition();
            deleteESDL.CustomerKey = NewSendDefinitionName;
            deleteESDL.AuthStub = myclient;
            DeleteReturn deleteESDLResponse = deleteESDDE.Delete();
            Console.WriteLine("Delete Status: " + deleteESDLResponse.Status.ToString());
            Console.WriteLine("Message: " + deleteESDLResponse.Message.ToString());
            Console.WriteLine("Code: " + deleteESDLResponse.Code.ToString());
            Console.WriteLine("Results Length: " + deleteESDLResponse.Results.Length);
        }
    }
}
