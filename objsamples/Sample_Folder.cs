using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_Folder()
        {
            string NewFolderName = "Testing C Sharp SDK";
            int ParentIDForEmail = 0;
            string ContentType = "email";

            ET_Client myclient = new ET_Client();

            Console.WriteLine("--- Testing Folder ---");

            Console.WriteLine("\n Retrieve All Folder with GetMoreResults");
            ET_Folder getAllFolder = new ET_Folder();
            getAllFolder.AuthStub = myclient;
            getAllFolder.Props = new string[] { "ID", "Client.ID", "ParentFolder.ID", "ParentFolder.CustomerKey", "ParentFolder.ObjectID", "ParentFolder.Name", "ParentFolder.Description", "ParentFolder.ContentType", "ParentFolder.IsActive", "ParentFolder.IsEditable", "ParentFolder.AllowChildren", "Name", "Description", "ContentType", "IsActive", "IsEditable", "AllowChildren", "CreatedDate", "ModifiedDate", "Client.ModifiedBy", "ObjectID", "CustomerKey", "Client.EnterpriseID", "Client.CreatedBy" };
            GetReturn grAllFolder = getAllFolder.Get();

            Console.WriteLine("Get Status: " + grAllFolder.Status.ToString());
            Console.WriteLine("Message: " + grAllFolder.Message.ToString());
            Console.WriteLine("Code: " + grAllFolder.Code.ToString());
            Console.WriteLine("Results Length: " + grAllFolder.Results.Length);

            while (grAllFolder.MoreResults)
            {
                Console.WriteLine("\n Continue Retrieve All Folders with GetMoreResults");
                grAllFolder = getAllFolder.GetMoreResults();
                Console.WriteLine("Get Status: " + grAllFolder.Status.ToString());
                Console.WriteLine("Message: " + grAllFolder.Message.ToString());
                Console.WriteLine("Code: " + grAllFolder.Code.ToString());
                Console.WriteLine("Results Length: " + grAllFolder.Results.Length);
            }


            Console.WriteLine("\n Retrieve Specific Folder for Email Folder ParentID");
            ET_Folder getFolder = new ET_Folder();
            getFolder.AuthStub = myclient;
            SimpleFilterPart ParentFolderFilter = new SimpleFilterPart() { Property = "ParentFolder.ID", SimpleOperator = SimpleOperators.equals, Value = new string[] { "0" } };
            SimpleFilterPart ContentTypeFilter = new SimpleFilterPart() { Property = "ContentType", SimpleOperator = SimpleOperators.equals, Value = new string[] { "Email" } };
            getFolder.SearchFilter = new ComplexFilterPart() { LeftOperand = ParentFolderFilter, RightOperand = ContentTypeFilter, LogicalOperator = LogicalOperators.AND };
            getFolder.Props = new string[] { "ID", "Name", "Description" };
            GetReturn grFolder = getFolder.Get();

            Console.WriteLine("Get Status: " + grFolder.Status.ToString());
            Console.WriteLine("Message: " + grFolder.Message.ToString());
            Console.WriteLine("Code: " + grFolder.Code.ToString());
            Console.WriteLine("Results Length: " + grFolder.Results.Length);

            foreach (ET_Folder ef in grFolder.Results)
            {
                ParentIDForEmail = ef.ID;
            }


            if (ParentIDForEmail != 0)
            {
                Console.WriteLine("\n Create Folder");
                ET_Folder fold = new ET_Folder();
                fold.Name = NewFolderName;
                fold.Description = NewFolderName;
                fold.CustomerKey = NewFolderName;
                fold.AuthStub = myclient;
                fold.ParentFolder = new ET_Folder();
                fold.ParentFolder.ID = ParentIDForEmail;
                fold.ContentType = ContentType;
                fold.IsEditable = true;
                PostReturn prFolder = fold.Post();

                Console.WriteLine("Post Status: " + prFolder.Status.ToString());
                Console.WriteLine("Message: " + prFolder.Message.ToString());
                Console.WriteLine("Code: " + prFolder.Code.ToString());
                Console.WriteLine("Results Length: " + prFolder.Results.Length);

                foreach (ResultDetail rd in prFolder.Results)
                {
                    Console.WriteLine("--Status Code: " + rd.StatusCode);
                    Console.WriteLine("--Status Message: " + rd.StatusMessage);
                }

                Console.WriteLine("\n Retrieve newly created Folder");
                ET_Folder getNewFolder = new ET_Folder();
                getNewFolder.AuthStub = myclient;
                getNewFolder.SearchFilter = new SimpleFilterPart() { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new string[] { NewFolderName } };
                getNewFolder.Props = new string[] { "ID", "Name", "Description" };
                GetReturn grNewFolder = getNewFolder.Get();

                Console.WriteLine("Get Status: " + grNewFolder.Status.ToString());
                Console.WriteLine("Message: " + grFolder.Message.ToString());
                Console.WriteLine("Code: " + grNewFolder.Code.ToString());
                Console.WriteLine("Results Length: " + grNewFolder.Results.Length);

                foreach (ET_Folder ef in grNewFolder.Results)
                {
                    Console.WriteLine("--Name: " + ef.Name + " - Description:" + ef.Description);
                }

                Console.WriteLine("\n UpdateFolder");
                ET_Folder patchFolder = new ET_Folder();
                patchFolder.AuthStub = myclient;
                patchFolder.CustomerKey = NewFolderName;
                patchFolder.Description = "Now with a new Description";
                PatchReturn patchResponse = patchFolder.Patch();

                Console.WriteLine("Patch Status: " + patchResponse.Status.ToString());
                Console.WriteLine("Message: " + patchResponse.Message.ToString());
                Console.WriteLine("Code: " + patchResponse.Code.ToString());
                Console.WriteLine("Results Length: " + patchResponse.Results.Length);

                if (patchResponse.Status)
                {
                    Console.WriteLine("\n Retrieve updated Folder");
                    grNewFolder = getNewFolder.Get();

                    Console.WriteLine("Get Status: " + grNewFolder.Status.ToString());
                    Console.WriteLine("Message: " + grFolder.Message.ToString());
                    Console.WriteLine("Code: " + grNewFolder.Code.ToString());
                    Console.WriteLine("Results Length: " + grNewFolder.Results.Length);

                    foreach (ET_Folder ef in grNewFolder.Results)
                    {
                        Console.WriteLine("--Name: " + ef.Name + " - Description:" + ef.Description);
                    }
                }

                Console.WriteLine("\n Delete Folder");
                ET_Folder delFolder = new ET_Folder();
                delFolder.AuthStub = myclient;
                delFolder.CustomerKey = NewFolderName;
                FuelSDK.DeleteReturn drFolder = delFolder.Delete();
                Console.WriteLine("Delete Status: " + drFolder.Status.ToString());
                Console.WriteLine("Message: " + drFolder.Message.ToString());
                Console.WriteLine("Code: " + drFolder.Code.ToString());
                Console.WriteLine("Results Length: " + drFolder.Results.Length);

                foreach (ResultDetail rd in drFolder.Results)
                {
                    Console.WriteLine("--Status Code: " + rd.StatusCode);
                    Console.WriteLine("--Status Message: " + rd.StatusMessage);
                }

                Console.WriteLine("\n Retrieve Folder to confirm deletion");
                grNewFolder = getNewFolder.Get();

                Console.WriteLine("Get Status: " + grNewFolder.Status.ToString());
                Console.WriteLine("Message: " + grFolder.Message.ToString());
                Console.WriteLine("Code: " + grNewFolder.Code.ToString());
                Console.WriteLine("Results Length: " + grNewFolder.Results.Length);

                foreach (ET_Folder ef in grNewFolder.Results)
                {
                    Console.WriteLine("--Name: " + ef.Name + " - Description:" + ef.Description);
                }


            }
        }
    }
}

