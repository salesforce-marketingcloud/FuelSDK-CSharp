using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_Email(){
            ET_Client myclient = new ET_Client();
            string NameOfTestEmail = "CSharpSDKEmail";

            Console.WriteLine("--- Testing Email ---");
            Console.WriteLine("\n Retrieve All Email with GetMoreResults");
            ET_Email getAllEmail = new ET_Email();
            getAllEmail.AuthStub = myclient;
            getAllEmail.Props = new string[] { "ID","PartnerKey","CreatedDate","ModifiedDate","Client.ID","Name","Folder","CategoryID","TextBody","Subject","IsActive","IsHTMLPaste","ClonedFromID","Status","EmailType","CharacterSet","HasDynamicSubjectLine","ContentCheckStatus","Client.PartnerClientKey","ContentAreas","CustomerKey" };
            GetReturn grAllEmail = getAllEmail.Get();

            Console.WriteLine("Get Status: " + grAllEmail.Status.ToString());
            Console.WriteLine("Message: " + grAllEmail.Message.ToString());
            Console.WriteLine("Code: " + grAllEmail.Code.ToString());
            Console.WriteLine("Results Length: " + grAllEmail.Results.Length);

            while (grAllEmail.MoreResults)
            {
                Console.WriteLine("\n Continue Retrieve All Email with GetMoreResults");
                grAllEmail = getAllEmail.GetMoreResults();
                Console.WriteLine("Get Status: " + grAllEmail.Status.ToString());
                Console.WriteLine("Message: " + grAllEmail.Message.ToString());
                Console.WriteLine("Code: " + grAllEmail.Code.ToString());
                Console.WriteLine("Results Length: " + grAllEmail.Results.Length);
            }

            Console.WriteLine("\n Create Email");            
            ET_Email postEmail = new ET_Email();
            postEmail.AuthStub = myclient;
            postEmail.Name = NameOfTestEmail;
            postEmail.CustomerKey = NameOfTestEmail;
            postEmail.Subject = "Created Using the Fuel SDK";
            postEmail.HTMLBody =  "<b>Some HTML Goes here</b>";
            //postEmail.FolderID = 898544;            
            PostReturn postResponse = postEmail.Post();
            Console.WriteLine("Post Status: " + postResponse.Status.ToString());
            Console.WriteLine("Message: " + postResponse.Message.ToString());
            Console.WriteLine("Code: " + postResponse.Code.ToString());
            Console.WriteLine("Results Length: " + postResponse.Results.Length);

            if (postResponse.Status)
            {
                Console.WriteLine("\n Retrieve newly create Email");
                ET_Email getEmail = new ET_Email();
                getEmail.AuthStub = myclient;
                getEmail.Props = new string[] { "ID","PartnerKey","CreatedDate","ModifiedDate","Client.ID","Name","Folder","CategoryID","HTMLBody","TextBody","Subject","IsActive","IsHTMLPaste","ClonedFromID","Status","EmailType","CharacterSet","HasDynamicSubjectLine","ContentCheckStatus","Client.PartnerClientKey","ContentAreas","CustomerKey" };
                getEmail.SearchFilter = new SimpleFilterPart() { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new String[] { NameOfTestEmail } };
                GetReturn getResponse = getEmail.Get();
                Console.WriteLine("Get Status: " + getResponse.Status.ToString());
                Console.WriteLine("Message: " + getResponse.Message.ToString());
                Console.WriteLine("Code: " + getResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getResponse.Results.Length);
                foreach (ET_Email ResultEmail in getResponse.Results)
                {
                    Console.WriteLine("--ID: " + ResultEmail.ID + ", Name: " + ResultEmail.Name + ", HTMLBody: " + ResultEmail.HTMLBody);
                }

                Console.WriteLine("\n Update Email");
                ET_Email patchEmail = new ET_Email();
                patchEmail.CustomerKey = NameOfTestEmail;
                patchEmail.HTMLBody = "<b>Some HTML Goes here. NOW WITH NEW CONTENT</b>";
                patchEmail.AuthStub = myclient;
                FuelSDK.PatchReturn patchFR = patchEmail.Patch();
                Console.WriteLine("Patch Status: " + patchFR.Status.ToString());
                Console.WriteLine("Message: " + patchFR.Message.ToString());
                Console.WriteLine("Code: " + patchFR.Code.ToString());
                Console.WriteLine("Results Length: " + patchFR.Results.Length);

                Console.WriteLine("\n Retrieve updated Email");
                getEmail.Props = new string[] { "ID","PartnerKey","CreatedDate","ModifiedDate","Client.ID","Name","Folder","CategoryID","HTMLBody","TextBody","Subject","IsActive","IsHTMLPaste","ClonedFromID","Status","EmailType","CharacterSet","HasDynamicSubjectLine","ContentCheckStatus","Client.PartnerClientKey","ContentAreas","CustomerKey" };
                getEmail.SearchFilter = new SimpleFilterPart() { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new String[] { NameOfTestEmail } };
                getResponse = getEmail.Get();
                Console.WriteLine("Get Status: " + getResponse.Status.ToString());
                Console.WriteLine("Message: " + getResponse.Message.ToString());
                Console.WriteLine("Code: " + getResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getResponse.Results.Length);
                foreach (ET_Email ResultEmail in getResponse.Results)
                {
                    Console.WriteLine("--ID: " + ResultEmail.ID + ", Name: " + ResultEmail.Name + ", HTMLBody: " + ResultEmail.HTMLBody);
                }

                Console.WriteLine("\n Delete Email");
                ET_Email delEmail = new ET_Email();
                delEmail.CustomerKey = NameOfTestEmail;
                delEmail.AuthStub = myclient;
                DeleteReturn deleteResponse = delEmail.Delete();
                Console.WriteLine("Delete Status: " + deleteResponse.Status.ToString());
                Console.WriteLine("Message: " + deleteResponse.Message.ToString());
                Console.WriteLine("Code: " + deleteResponse.Code.ToString());
                Console.WriteLine("Results Length: " + deleteResponse.Results.Length);

                Console.WriteLine("\n Retrieve Email to confirm deletion");
                getEmail.Props = new string[] { "ID","PartnerKey","CreatedDate","ModifiedDate","Client.ID","Name","Folder","CategoryID","HTMLBody","TextBody","Subject","IsActive","IsHTMLPaste","ClonedFromID","Status","EmailType","CharacterSet","HasDynamicSubjectLine","ContentCheckStatus","Client.PartnerClientKey","ContentAreas","CustomerKey" };
                getEmail.SearchFilter = new SimpleFilterPart() { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new String[] { NameOfTestEmail } };
                getResponse = getEmail.Get();
                Console.WriteLine("Get Status: " + getResponse.Status.ToString());
                Console.WriteLine("Message: " + getResponse.Message.ToString());
                Console.WriteLine("Code: " + getResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getResponse.Results.Length);

                Console.WriteLine("\n Info Email");
                ET_Email EmailInfo = new ET_Email();
                EmailInfo.AuthStub = myclient;
                InfoReturn info = EmailInfo.Info();
                Console.WriteLine("Info Status: " + info.Status.ToString());
                Console.WriteLine("Message: " + info.Message.ToString());
                Console.WriteLine("Code: " + info.Code.ToString());
                Console.WriteLine("Results Length: " + info.Results.Length);
                foreach (ET_PropertyDefinition def in info.Results)
                {
                    Console.WriteLine("--Name: " + def.Name + ", IsRetrievable: " + def.IsRetrievable.ToString());
                }
            }
        }
    }
}
