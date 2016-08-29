using FuelSDK;
using System;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_List()
        {
            var myclient = new ET_Client();

            Console.WriteLine("--- Testing List ---");
            var myNewListID = 0;

            Console.WriteLine("\n Create List");
            var list = new ET_List
            {
                AuthStub = myclient,
                ListName = "C# SDK Rules!!",
                Description = "This is my SDK Created List",
                //FolderID = 1083760,
            };
            var postFR = list.Post();
            Console.WriteLine("Post Status: " + postFR.Status.ToString());
            Console.WriteLine("Message: " + postFR.Message);
            Console.WriteLine("Code: " + postFR.Code.ToString());
            Console.WriteLine("Results Length: " + postFR.Results.Length);

            if (postFR.Results.Length > 0)
                myNewListID = postFR.Results[0].NewID;

            if (myNewListID > 0)
            {
                Console.WriteLine("\n Retrieve newly create list");
                list.Props = new[] { "ID", "ListName", "Description" };
                list.SearchFilter = new SimpleFilterPart { Property = "ID", SimpleOperator = SimpleOperators.equals, Value = new[] { myNewListID.ToString() } };
                var getFR = list.Get();
                Console.WriteLine("Get Status: " + getFR.Status.ToString());
                Console.WriteLine("Message: " + getFR.Message);
                Console.WriteLine("Code: " + getFR.Code.ToString());
                Console.WriteLine("Results Length: " + getFR.Results.Length);
                foreach (ET_List ResultList in getFR.Results)
                    Console.WriteLine("--ID: " + ResultList.ID + ", Name: " + ResultList.ListName + ", Description: " + ResultList.Description);

                Console.WriteLine("\n Update list");
                var patchList = new ET_List
                {
                    ID = myNewListID,
                    Description = "This is the new description",
                    AuthStub = myclient,
                };
                var patchFR = patchList.Patch();
                Console.WriteLine("Patch Status: " + patchFR.Status.ToString());
                Console.WriteLine("Message: " + patchFR.Message);
                Console.WriteLine("Code: " + patchFR.Code.ToString());
                Console.WriteLine("Results Length: " + patchFR.Results.Length);

                Console.WriteLine("\n Retrieve List that should have description updated");
                list.Props = new[] { "ID", "ListName", "Description" };
                list.SearchFilter = new SimpleFilterPart { Property = "ID", SimpleOperator = SimpleOperators.equals, Value = new[] { myNewListID.ToString() } };
                getFR = list.Get();
                Console.WriteLine("Get Status: " + getFR.Status.ToString());
                Console.WriteLine("Message: " + getFR.Message);
                Console.WriteLine("Code: " + getFR.Code.ToString());
                Console.WriteLine("Results Length: " + getFR.Results.Length);
                foreach (ET_List ResultList in getFR.Results)
                    Console.WriteLine("--ID: " + ResultList.ID + ", Name: " + ResultList.ListName + ", Description: " + ResultList.Description);

                Console.WriteLine("\n Delete List");
                var delList = new ET_List
                {
                    ID = myNewListID,
                    AuthStub = myclient,
                };
                var fr = delList.Delete();
                Console.WriteLine("Delete Status: " + fr.Status.ToString());
                Console.WriteLine("Message: " + fr.Message);
                Console.WriteLine("Code: " + fr.Code.ToString());
                Console.WriteLine("Results Length: " + fr.Results.Length);

                Console.WriteLine("\n Retrieve List to confirm deletion");
                list.Props = new[] { "ID", "ListName", "Description" };
                list.SearchFilter = new SimpleFilterPart { Property = "ID", SimpleOperator = SimpleOperators.equals, Value = new[] { myNewListID.ToString() } };
                getFR = list.Get();
                Console.WriteLine("Get Status: " + getFR.Status.ToString());
                Console.WriteLine("Message: " + getFR.Message);
                Console.WriteLine("Code: " + getFR.Code.ToString());
                Console.WriteLine("Results Length: " + getFR.Results.Length);
                foreach (ET_List ResultList in getFR.Results)
                    Console.WriteLine("--ID: " + ResultList.ID + ", Name: " + ResultList.ListName + ", Description: " + ResultList.Description);

                Console.WriteLine("\n Info List");
                var listInfo = new ET_List
                {
                    AuthStub = myclient
                };
                var info = listInfo.Info();
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
