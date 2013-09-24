using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_ContentArea(){
            ET_Client myclient = new ET_Client();
            string NameOfTestContentArea = "CSharpSDKContentArea";

            Console.WriteLine("--- Testing ContentArea ---");
            Console.WriteLine("\n Retrieve All ContentArea with GetMoreResults");
            ET_ContentArea getAllContentArea = new ET_ContentArea();
            getAllContentArea.AuthStub = myclient;
            getAllContentArea.Props = new string[] { "RowObjectID", "ObjectID", "ID", "CustomerKey", "Client.ID", "ModifiedDate", "CreatedDate", "CategoryID", "Name", "Layout", "IsDynamicContent", "Content", "IsSurvey", "IsBlank", "Key" };
            GetReturn grAllContent = getAllContentArea.Get();

            Console.WriteLine("Get Status: " + grAllContent.Status.ToString());
            Console.WriteLine("Message: " + grAllContent.Message.ToString());
            Console.WriteLine("Code: " + grAllContent.Code.ToString());
            Console.WriteLine("Results Length: " + grAllContent.Results.Length);

            while (grAllContent.MoreResults)
            {
                Console.WriteLine("\n Continue Retrieve All ContentArea with GetMoreResults");
                grAllContent = getAllContentArea.GetMoreResults();
                Console.WriteLine("Get Status: " + grAllContent.Status.ToString());
                Console.WriteLine("Message: " + grAllContent.Message.ToString());
                Console.WriteLine("Code: " + grAllContent.Code.ToString());
                Console.WriteLine("Results Length: " + grAllContent.Results.Length);
            }

            Console.WriteLine("\n Create ContentArea");            
            ET_ContentArea postContentArea = new ET_ContentArea();
            postContentArea.AuthStub = myclient;
            postContentArea.Name = NameOfTestContentArea;
            postContentArea.CustomerKey = NameOfTestContentArea;
            postContentArea.Content = "<b>Some HTML Content Goes here</b>";
            //postContentArea.FolderID = 1073161;
            PostReturn postResponse = postContentArea.Post();
            Console.WriteLine("Post Status: " + postResponse.Status.ToString());
            Console.WriteLine("Message: " + postResponse.Message.ToString());
            Console.WriteLine("Code: " + postResponse.Code.ToString());
            Console.WriteLine("Results Length: " + postResponse.Results.Length);

            
            if (postResponse.Status)
            {
                Console.WriteLine("\n Retrieve newly create ContentArea");
                ET_ContentArea getContentArea = new ET_ContentArea();
                getContentArea.AuthStub = myclient;
                getContentArea.Props = new string[] { "RowObjectID","ObjectID","ID","CustomerKey","Client.ID","ModifiedDate","CreatedDate","CategoryID","Name","Layout","IsDynamicContent","Content","IsSurvey","IsBlank","Key" };
                getContentArea.SearchFilter = new SimpleFilterPart() { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new String[] { NameOfTestContentArea } };
                GetReturn getResponse = getContentArea.Get();
                Console.WriteLine("Get Status: " + getResponse.Status.ToString());
                Console.WriteLine("Message: " + getResponse.Message.ToString());
                Console.WriteLine("Code: " + getResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getResponse.Results.Length);
                foreach (ET_ContentArea ResultContentArea in getResponse.Results)
                {
                    Console.WriteLine("--ID: " + ResultContentArea.ID + ", Name: " + ResultContentArea.Name + ", Content: " + ResultContentArea.Content);
                }

                Console.WriteLine("\n Update ContentArea");
                ET_ContentArea patchContentArea = new ET_ContentArea();
                patchContentArea.CustomerKey = NameOfTestContentArea;
                patchContentArea.Content = "<b>Some HTML Content Goes here. NOW WITH NEW CONTENT</b>";
                patchContentArea.AuthStub = myclient;
                FuelSDK.PatchReturn patchFR = patchContentArea.Patch();
                Console.WriteLine("Patch Status: " + patchFR.Status.ToString());
                Console.WriteLine("Message: " + patchFR.Message.ToString());
                Console.WriteLine("Code: " + patchFR.Code.ToString());
                Console.WriteLine("Results Length: " + patchFR.Results.Length);

                Console.WriteLine("\n Retrieve updated ContentArea");
                getContentArea.Props = new string[] { "RowObjectID","ObjectID","ID","CustomerKey","Client.ID","ModifiedDate","CreatedDate","CategoryID","Name","Layout","IsDynamicContent","Content","IsSurvey","IsBlank","Key" };
                getContentArea.SearchFilter = new SimpleFilterPart() { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new String[] { NameOfTestContentArea } };
                getResponse = getContentArea.Get();
                Console.WriteLine("Get Status: " + getResponse.Status.ToString());
                Console.WriteLine("Message: " + getResponse.Message.ToString());
                Console.WriteLine("Code: " + getResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getResponse.Results.Length);
                foreach (ET_ContentArea ResultContentArea in getResponse.Results)
                {
                    Console.WriteLine("--ID: " + ResultContentArea.ID + ", Name: " + ResultContentArea.Name + ", Content: " + ResultContentArea.Content);
                }

                Console.WriteLine("\n Delete ContentArea");
                ET_ContentArea delContentArea = new ET_ContentArea();
                delContentArea.CustomerKey = NameOfTestContentArea;
                delContentArea.AuthStub = myclient;
                DeleteReturn deleteResponse = delContentArea.Delete();
                Console.WriteLine("Delete Status: " + deleteResponse.Status.ToString());
                Console.WriteLine("Message: " + deleteResponse.Message.ToString());
                Console.WriteLine("Code: " + deleteResponse.Code.ToString());
                Console.WriteLine("Results Length: " + deleteResponse.Results.Length);

                Console.WriteLine("\n Retrieve ContentArea to confirm deletion");
                getContentArea.Props = new string[] { "RowObjectID","ObjectID","ID","CustomerKey","Client.ID","ModifiedDate","CreatedDate","CategoryID","Name","Layout","IsDynamicContent","Content","IsSurvey","IsBlank","Key" };
                getContentArea.SearchFilter = new SimpleFilterPart() { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new String[] { NameOfTestContentArea } };
                getResponse = getContentArea.Get();
                Console.WriteLine("Get Status: " + getResponse.Status.ToString());
                Console.WriteLine("Message: " + getResponse.Message.ToString());
                Console.WriteLine("Code: " + getResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getResponse.Results.Length);

                //Console.WriteLine("\n Info ContentArea");
                //ET_ContentArea ContentAreaInfo = new ET_ContentArea();
                //ContentAreaInfo.authStub = myclient;
                //InfoReturn info = ContentAreaInfo.Info();
                //Console.WriteLine("Info Status: " + info.Status.ToString());
                //Console.WriteLine("Message: " + info.Message.ToString());
                //Console.WriteLine("Code: " + info.Code.ToString());
                //Console.WriteLine("Results Length: " + info.Results.Length);
                //foreach (ET_PropertyDefinition def in info.Results)
                //{
                //    Console.WriteLine("--Name: " + def.Name + ", IsRetrievable: " + def.IsRetrievable.ToString());
                //}
            }             
        }
    }
}
