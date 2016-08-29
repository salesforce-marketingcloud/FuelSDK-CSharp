using FuelSDK;
using System;
using System.Threading;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_EmailSendDefinition()
        {
            var myclient = new ET_Client();
            var newSendDefinitionName = "CSharpSDKSendDefinition";
            var sendableDataExtensionCustomerKey = "F6F3871A-D124-499B-BBF5-3EFC0E827A51";
            var emailIDForSendDefinition = 3113962;
            var listIDForSendDefinition = 1729515;
            var sendClassificationCustomerKey = "2239";

            Console.WriteLine("--- Testing EmailSendDefinition ---");
            Console.WriteLine("\n Retrieve All EmailSendDefinition with GetMoreResults");
            var getAllESD = new ET_EmailSendDefinition
            {
                AuthStub = myclient,
                Props = new[] { "Client.ID", "CreatedDate", "ModifiedDate", "ObjectID", "CustomerKey", "Name", "CategoryID", "Description", "SendClassification.CustomerKey", "SenderProfile.CustomerKey", "SenderProfile.FromName", "SenderProfile.FromAddress", "DeliveryProfile.CustomerKey", "DeliveryProfile.SourceAddressType", "DeliveryProfile.PrivateIP", "DeliveryProfile.DomainType", "DeliveryProfile.PrivateDomain", "DeliveryProfile.HeaderSalutationSource", "DeliveryProfile.FooterSalutationSource", "SuppressTracking", "IsSendLogging", "Email.ID", "BccEmail", "AutoBccEmail", "TestEmailAddr", "EmailSubject", "DynamicEmailSubject", "IsMultipart", "IsWrapped", "SendLimit", "SendWindowOpen", "SendWindowClose", "DeduplicateByEmail", "ExclusionFilter", "Additional" },
            };
            var grAllEmail = getAllESD.Get();

            Console.WriteLine("Get Status: " + grAllEmail.Status.ToString());
            Console.WriteLine("Message: " + grAllEmail.Message);
            Console.WriteLine("Code: " + grAllEmail.Code.ToString());
            Console.WriteLine("Results Length: " + grAllEmail.Results.Length);

            while (grAllEmail.MoreResults)
            {
                Console.WriteLine("\n Continue Retrieve All Email with GetMoreResults");
                grAllEmail = getAllESD.GetMoreResults();
                Console.WriteLine("Get Status: " + grAllEmail.Status.ToString());
                Console.WriteLine("Message: " + grAllEmail.Message);
                Console.WriteLine("Code: " + grAllEmail.Code.ToString());
                Console.WriteLine("Results Length: " + grAllEmail.Results.Length);
            }

            Console.WriteLine("\n Create SendDefinition to DataExtension");
            var postESDDE = new ET_EmailSendDefinition
            {
                AuthStub = myclient,
                Name = newSendDefinitionName,
                CustomerKey = newSendDefinitionName,
                Description = "Created with Fuel SDK",
                SendClassification = new ET_SendClassification { CustomerKey = sendClassificationCustomerKey },
                SendDefinitionList = new[] { new ET_SendDefinitionList { CustomerKey = sendableDataExtensionCustomerKey, DataSourceTypeID = DataSourceTypeEnum.CustomObject } },
                Email = new ET_Email { ID = emailIDForSendDefinition },
            };
            var postResponse = postESDDE.Post();
            Console.WriteLine("Post Status: " + postResponse.Status.ToString());
            Console.WriteLine("Message: " + postResponse.Message);
            Console.WriteLine("Code: " + postResponse.Code.ToString());
            Console.WriteLine("Results Length: " + postResponse.Results.Length);

            Console.WriteLine("\n Delete SendDefinition");
            var deleteESDDE = new ET_EmailSendDefinition
            {
                CustomerKey = newSendDefinitionName,
                AuthStub = myclient,
            };
            var deleteESDDEResponse = deleteESDDE.Delete();
            Console.WriteLine("Delete Status: " + deleteESDDEResponse.Status.ToString());
            Console.WriteLine("Message: " + deleteESDDEResponse.Message);
            Console.WriteLine("Code: " + deleteESDDEResponse.Code.ToString());
            Console.WriteLine("Results Length: " + deleteESDDEResponse.Results.Length);

            Console.WriteLine("\n Create SendDefinition to List");
            var postESDL = new ET_EmailSendDefinition
            {
                AuthStub = myclient,
                Name = newSendDefinitionName,
                CustomerKey = newSendDefinitionName,
                Description = "Created with Fuel SDK",
                SendClassification = new ET_SendClassification { CustomerKey = sendClassificationCustomerKey },
                SendDefinitionList = new[] { new ET_SendDefinitionList() { List = new ET_List { ID = listIDForSendDefinition }, DataSourceTypeID = DataSourceTypeEnum.List } },
                Email = new ET_Email { ID = emailIDForSendDefinition },
            };
            var postESDLResponse = postESDDE.Post();
            Console.WriteLine("Post Status: " + postESDLResponse.Status.ToString());
            Console.WriteLine("Message: " + postESDLResponse.Message);
            Console.WriteLine("Code: " + postESDLResponse.Code.ToString());
            Console.WriteLine("Results Length: " + postESDLResponse.Results.Length);

            Console.WriteLine("\n Send using Definition ");
            var sendESD = new ET_EmailSendDefinition
            {
                AuthStub = myclient,
                CustomerKey = newSendDefinitionName,
            };
            var sendESDResponse = sendESD.Send();
            Console.WriteLine("Send Status: " + sendESDResponse.Status.ToString());
            Console.WriteLine("Message: " + sendESDResponse.Message);
            Console.WriteLine("Code: " + sendESDResponse.Code.ToString());
            Console.WriteLine("Results Length: " + sendESDResponse.Results.Length);

            Console.WriteLine("\n Retrieve All EmailSendDefinition with GetMoreResults");
            var emailStatus = string.Empty;
            while (sendESDResponse.Status && emailStatus != "Canceled" && emailStatus != "Complete")
            {
                Console.WriteLine("\n Checking Status in Loop");
                var getESDStatusReturn = sendESD.Status();
                Console.WriteLine("Get Status: " + getESDStatusReturn.Status.ToString());
                Console.WriteLine("Message: " + getESDStatusReturn.Message);
                Console.WriteLine("Code: " + getESDStatusReturn.Code.ToString());
                Console.WriteLine("Results Length: " + getESDStatusReturn.Results.Length);
                emailStatus = ((ET_Send)getESDStatusReturn.Results[0]).Status;
                Console.WriteLine("Send Status: " + emailStatus);
                Thread.Sleep(5000);
            }

            Console.WriteLine("\n Delete SendDefinition");
            var deleteESDL = new ET_EmailSendDefinition
            {
                CustomerKey = newSendDefinitionName,
                AuthStub = myclient,
            };
            var deleteESDLResponse = deleteESDDE.Delete();
            Console.WriteLine("Delete Status: " + deleteESDLResponse.Status.ToString());
            Console.WriteLine("Message: " + deleteESDLResponse.Message);
            Console.WriteLine("Code: " + deleteESDLResponse.Code.ToString());
            Console.WriteLine("Results Length: " + deleteESDLResponse.Results.Length);
        }
    }
}
