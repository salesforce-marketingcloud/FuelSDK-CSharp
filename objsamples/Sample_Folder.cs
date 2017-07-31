using FuelSDK;
using System;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_Folder()
        {
            var newFolderName = "Testing C Sharp SDK";
            var parentIDForEmail = 0;
            var contentType = "email";

            var myclient = new ET_Client();

            Console.WriteLine("--- Testing Folder ---");

            Console.WriteLine("\n Retrieve All Folder with GetMoreResults");
            var getAllFolder = new ET_Folder
            {
                AuthStub = myclient,
                Props = new[] { "ID", "Client.ID", "ParentFolder.ID", "ParentFolder.CustomerKey", "ParentFolder.ObjectID", "ParentFolder.Name", "ParentFolder.Description", "ParentFolder.ContentType", "ParentFolder.IsActive", "ParentFolder.IsEditable", "ParentFolder.AllowChildren", "Name", "Description", "ContentType", "IsActive", "IsEditable", "AllowChildren", "CreatedDate", "ModifiedDate", "Client.ModifiedBy", "ObjectID", "CustomerKey", "Client.EnterpriseID", "Client.CreatedBy" },
            };
            var grAllFolder = getAllFolder.Get();

            Console.WriteLine("Get Status: " + grAllFolder.Status.ToString());
            Console.WriteLine("Message: " + grAllFolder.Message);
            Console.WriteLine("Code: " + grAllFolder.Code.ToString());
            Console.WriteLine("Results Length: " + grAllFolder.Results.Length);

            while (grAllFolder.MoreResults)
            {
                Console.WriteLine("\n Continue Retrieve All Folders with GetMoreResults");
                grAllFolder = getAllFolder.GetMoreResults();
                Console.WriteLine("Get Status: " + grAllFolder.Status.ToString());
                Console.WriteLine("Message: " + grAllFolder.Message);
                Console.WriteLine("Code: " + grAllFolder.Code.ToString());
                Console.WriteLine("Results Length: " + grAllFolder.Results.Length);
            }

            Console.WriteLine("\n Retrieve Specific Folder for Email Folder ParentID");
            var getFolder = new ET_Folder
            {
                AuthStub = myclient,
                SearchFilter = new ComplexFilterPart
                {
                    LeftOperand = new SimpleFilterPart { Property = "ParentFolder.ID", SimpleOperator = SimpleOperators.equals, Value = new[] { "0" } },
                    RightOperand = new SimpleFilterPart { Property = "ContentType", SimpleOperator = SimpleOperators.equals, Value = new[] { "Email" } },
                    LogicalOperator = LogicalOperators.AND
                },
                Props = new[] { "ID", "Name", "Description" },
            };
            var grFolder = getFolder.Get();

            Console.WriteLine("Get Status: " + grFolder.Status.ToString());
            Console.WriteLine("Message: " + grFolder.Message);
            Console.WriteLine("Code: " + grFolder.Code.ToString());
            Console.WriteLine("Results Length: " + grFolder.Results.Length);

            foreach (ET_Folder ef in grFolder.Results)
                parentIDForEmail = ef.ID;

            if (parentIDForEmail != 0)
            {
                Console.WriteLine("\n Create Folder");
                var fold = new ET_Folder
                {
                    Name = newFolderName,
                    Description = newFolderName,
                    CustomerKey = newFolderName,
                    AuthStub = myclient,
                    ParentFolder = new ET_Folder { ID = parentIDForEmail },
                    ContentType = contentType,
                    IsEditable = true,
                };
                var prFolder = fold.Post();

                Console.WriteLine("Post Status: " + prFolder.Status.ToString());
                Console.WriteLine("Message: " + prFolder.Message);
                Console.WriteLine("Code: " + prFolder.Code.ToString());
                Console.WriteLine("Results Length: " + prFolder.Results.Length);
                foreach (ResultDetail rd in prFolder.Results)
                {
                    Console.WriteLine("--Status Code: " + rd.StatusCode);
                    Console.WriteLine("--Status Message: " + rd.StatusMessage);
                }

                Console.WriteLine("\n Retrieve newly created Folder");
                var getNewFolder = new ET_Folder
                {
                    AuthStub = myclient,
                    SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { newFolderName } },
                    Props = new[] { "ID", "Name", "Description" },
                };
                var grNewFolder = getNewFolder.Get();

                Console.WriteLine("Get Status: " + grNewFolder.Status.ToString());
                Console.WriteLine("Message: " + grFolder.Message);
                Console.WriteLine("Code: " + grNewFolder.Code.ToString());
                Console.WriteLine("Results Length: " + grNewFolder.Results.Length);
                foreach (ET_Folder ef in grNewFolder.Results)
                    Console.WriteLine("--Name: " + ef.Name + " - Description:" + ef.Description);

                Console.WriteLine("\n UpdateFolder");
                var patchFolder = new ET_Folder
                {
                    AuthStub = myclient,
                    CustomerKey = newFolderName,
                    Description = "Now with a new Description",
                };
                var patchResponse = patchFolder.Patch();

                Console.WriteLine("Patch Status: " + patchResponse.Status.ToString());
                Console.WriteLine("Message: " + patchResponse.Message);
                Console.WriteLine("Code: " + patchResponse.Code.ToString());
                Console.WriteLine("Results Length: " + patchResponse.Results.Length);

                if (patchResponse.Status)
                {
                    Console.WriteLine("\n Retrieve updated Folder");
                    grNewFolder = getNewFolder.Get();
                    Console.WriteLine("Get Status: " + grNewFolder.Status.ToString());
                    Console.WriteLine("Message: " + grFolder.Message);
                    Console.WriteLine("Code: " + grNewFolder.Code.ToString());
                    Console.WriteLine("Results Length: " + grNewFolder.Results.Length);
                    foreach (ET_Folder ef in grNewFolder.Results)
                        Console.WriteLine("--Name: " + ef.Name + " - Description:" + ef.Description);
                }

                Console.WriteLine("\n Delete Folder");
                var delFolder = new ET_Folder
                {
                    AuthStub = myclient,
                    CustomerKey = newFolderName,
                };
                var drFolder = delFolder.Delete();
                Console.WriteLine("Delete Status: " + drFolder.Status.ToString());
                Console.WriteLine("Message: " + drFolder.Message);
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
                Console.WriteLine("Message: " + grFolder.Message);
                Console.WriteLine("Code: " + grNewFolder.Code.ToString());
                Console.WriteLine("Results Length: " + grNewFolder.Results.Length);
                foreach (ET_Folder ef in grNewFolder.Results)
                    Console.WriteLine("--Name: " + ef.Name + " - Description:" + ef.Description);
            }
        }
    }
}

