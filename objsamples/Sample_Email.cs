using FuelSDK;
using System;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_Email()
        {
            var myclient = new ET_Client();
            var nameOfTestEmail = "CSharpSDKEmail";

            Console.WriteLine("--- Testing Email ---");
            Console.WriteLine("\n Retrieve All Email with GetMoreResults");
            var getAllEmail = new ET_Email
            {
                AuthStub = myclient,
                Props = new[] { "ID", "PartnerKey", "CreatedDate", "ModifiedDate", "Client.ID", "Name", "Folder", "CategoryID", "TextBody", "Subject", "IsActive", "IsHTMLPaste", "ClonedFromID", "Status", "EmailType", "CharacterSet", "HasDynamicSubjectLine", "ContentCheckStatus", "Client.PartnerClientKey", "ContentAreas", "CustomerKey" },
            };
            var grAllEmail = getAllEmail.Get();

            Console.WriteLine("Get Status: " + grAllEmail.Status.ToString());
            Console.WriteLine("Message: " + grAllEmail.Message);
            Console.WriteLine("Code: " + grAllEmail.Code.ToString());
            Console.WriteLine("Results Length: " + grAllEmail.Results.Length);

            while (grAllEmail.MoreResults)
            {
                Console.WriteLine("\n Continue Retrieve All Email with GetMoreResults");
                grAllEmail = getAllEmail.GetMoreResults();
                Console.WriteLine("Get Status: " + grAllEmail.Status.ToString());
                Console.WriteLine("Message: " + grAllEmail.Message);
                Console.WriteLine("Code: " + grAllEmail.Code.ToString());
                Console.WriteLine("Results Length: " + grAllEmail.Results.Length);
            }

            Console.WriteLine("\n Create Email");
            var postEmail = new ET_Email
            {
                AuthStub = myclient,
                Name = nameOfTestEmail,
                CustomerKey = nameOfTestEmail,
                Subject = "Created Using the Fuel SDK",
                HTMLBody = "<b>Some HTML Goes here</b>",
                //FolderID = 898544,
            };
            var postResponse = postEmail.Post();
            Console.WriteLine("Post Status: " + postResponse.Status.ToString());
            Console.WriteLine("Message: " + postResponse.Message);
            Console.WriteLine("Code: " + postResponse.Code.ToString());
            Console.WriteLine("Results Length: " + postResponse.Results.Length);

            if (postResponse.Status)
            {
                Console.WriteLine("\n Retrieve newly create Email");
                var getEmail = new ET_Email
                {
                    AuthStub = myclient,
                    Props = new[] { "ID", "PartnerKey", "CreatedDate", "ModifiedDate", "Client.ID", "Name", "Folder", "CategoryID", "HTMLBody", "TextBody", "Subject", "IsActive", "IsHTMLPaste", "ClonedFromID", "Status", "EmailType", "CharacterSet", "HasDynamicSubjectLine", "ContentCheckStatus", "Client.PartnerClientKey", "ContentAreas", "CustomerKey" },
                    SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { nameOfTestEmail } },
                };
                var getResponse = getEmail.Get();
                Console.WriteLine("Get Status: " + getResponse.Status.ToString());
                Console.WriteLine("Message: " + getResponse.Message);
                Console.WriteLine("Code: " + getResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getResponse.Results.Length);
                foreach (ET_Email ResultEmail in getResponse.Results)
                    Console.WriteLine("--ID: " + ResultEmail.ID + ", Name: " + ResultEmail.Name + ", HTMLBody: " + ResultEmail.HTMLBody);

                Console.WriteLine("\n Update Email");
                var patchEmail = new ET_Email
                {
                    CustomerKey = nameOfTestEmail,
                    HTMLBody = "<b>Some HTML Goes here. NOW WITH NEW CONTENT</b>",
                    AuthStub = myclient,
                };
                var patchFR = patchEmail.Patch();
                Console.WriteLine("Patch Status: " + patchFR.Status.ToString());
                Console.WriteLine("Message: " + patchFR.Message);
                Console.WriteLine("Code: " + patchFR.Code.ToString());
                Console.WriteLine("Results Length: " + patchFR.Results.Length);

                Console.WriteLine("\n Retrieve updated Email");
                getEmail.Props = new[] { "ID", "PartnerKey", "CreatedDate", "ModifiedDate", "Client.ID", "Name", "Folder", "CategoryID", "HTMLBody", "TextBody", "Subject", "IsActive", "IsHTMLPaste", "ClonedFromID", "Status", "EmailType", "CharacterSet", "HasDynamicSubjectLine", "ContentCheckStatus", "Client.PartnerClientKey", "ContentAreas", "CustomerKey" };
                getEmail.SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { nameOfTestEmail } };
                getResponse = getEmail.Get();
                Console.WriteLine("Get Status: " + getResponse.Status.ToString());
                Console.WriteLine("Message: " + getResponse.Message);
                Console.WriteLine("Code: " + getResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getResponse.Results.Length);
                foreach (ET_Email ResultEmail in getResponse.Results)
                    Console.WriteLine("--ID: " + ResultEmail.ID + ", Name: " + ResultEmail.Name + ", HTMLBody: " + ResultEmail.HTMLBody);

                Console.WriteLine("\n Delete Email");
                var delEmail = new ET_Email
                {
                    CustomerKey = nameOfTestEmail,
                    AuthStub = myclient,
                };
                var deleteResponse = delEmail.Delete();
                Console.WriteLine("Delete Status: " + deleteResponse.Status.ToString());
                Console.WriteLine("Message: " + deleteResponse.Message);
                Console.WriteLine("Code: " + deleteResponse.Code.ToString());
                Console.WriteLine("Results Length: " + deleteResponse.Results.Length);

                Console.WriteLine("\n Retrieve Email to confirm deletion");
                getEmail.Props = new[] { "ID", "PartnerKey", "CreatedDate", "ModifiedDate", "Client.ID", "Name", "Folder", "CategoryID", "HTMLBody", "TextBody", "Subject", "IsActive", "IsHTMLPaste", "ClonedFromID", "Status", "EmailType", "CharacterSet", "HasDynamicSubjectLine", "ContentCheckStatus", "Client.PartnerClientKey", "ContentAreas", "CustomerKey" };
                getEmail.SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { nameOfTestEmail } };
                getResponse = getEmail.Get();
                Console.WriteLine("Get Status: " + getResponse.Status.ToString());
                Console.WriteLine("Message: " + getResponse.Message);
                Console.WriteLine("Code: " + getResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getResponse.Results.Length);

                Console.WriteLine("\n Info Email");
                var EmailInfo = new ET_Email
                {
                    AuthStub = myclient,
                };
                var info = EmailInfo.Info();
                Console.WriteLine("Info Status: " + info.Status.ToString());
                Console.WriteLine("Message: " + info.Message);
                Console.WriteLine("Code: " + info.Code.ToString());
                Console.WriteLine("Results Length: " + info.Results.Length);
                foreach (ET_PropertyDefinition def in info.Results)
                    Console.WriteLine("--Name: " + def.Name + ", IsRetrievable: " + def.IsRetrievable.ToString());
            }
        }
    }
}
