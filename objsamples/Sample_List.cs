using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_List()
        {
            ET_Client myclient = new ET_Client();

            Console.WriteLine("--- Testing List ---");
            int MyNewListID = 0;

            Console.WriteLine("\n Create List");
            ET_List list = new ET_List();
            list.AuthStub = myclient;
            list.ListName = "C# SDK Rules!!";
            list.Description = "This is my SDK Created List";
            //list.FolderID = 1083760;            
            PostReturn postFR = list.Post();
            Console.WriteLine("Post Status: " + postFR.Status.ToString());
            Console.WriteLine("Message: " + postFR.Message.ToString());
            Console.WriteLine("Code: " + postFR.Code.ToString());
            Console.WriteLine("Results Length: " + postFR.Results.Length);

            if (postFR.Results.Length > 0)
            {
                MyNewListID = postFR.Results[0].NewID;
            }

            if (MyNewListID > 0)
            {
                Console.WriteLine("\n Retrieve newly create list");
                list.Props = new string[] { "ID", "ListName", "Description" };
                list.SearchFilter = new SimpleFilterPart() { Property = "ID", SimpleOperator = SimpleOperators.equals, Value = new String[] { MyNewListID.ToString() } };
                GetReturn getFR = list.Get();
                Console.WriteLine("Get Status: " + getFR.Status.ToString());
                Console.WriteLine("Message: " + getFR.Message.ToString());
                Console.WriteLine("Code: " + getFR.Code.ToString());
                Console.WriteLine("Results Length: " + getFR.Results.Length);
                foreach (ET_List ResultList in getFR.Results)
                {
                    Console.WriteLine("--ID: " + ResultList.ID + ", Name: " + ResultList.ListName + ", Description: " + ResultList.Description);
                }

                Console.WriteLine("\n Update list");
                ET_List patchList = new ET_List();
                patchList.ID = MyNewListID;
                patchList.Description = "This is the new description";
                patchList.AuthStub = myclient;
                FuelSDK.PatchReturn patchFR = patchList.Patch();
                Console.WriteLine("Patch Status: " + patchFR.Status.ToString());
                Console.WriteLine("Message: " + patchFR.Message.ToString());
                Console.WriteLine("Code: " + patchFR.Code.ToString());
                Console.WriteLine("Results Length: " + patchFR.Results.Length);

                Console.WriteLine("\n Retrieve List that should have description updated");
                list.Props = new string[] { "ID", "ListName", "Description" };
                list.SearchFilter = new SimpleFilterPart() { Property = "ID", SimpleOperator = SimpleOperators.equals, Value = new String[] { MyNewListID.ToString() } };
                getFR = list.Get();
                Console.WriteLine("Get Status: " + getFR.Status.ToString());
                Console.WriteLine("Message: " + getFR.Message.ToString());
                Console.WriteLine("Code: " + getFR.Code.ToString());
                Console.WriteLine("Results Length: " + getFR.Results.Length);
                foreach (ET_List ResultList in getFR.Results)
                {
                    Console.WriteLine("--ID: " + ResultList.ID + ", Name: " + ResultList.ListName + ", Description: " + ResultList.Description);
                }

                Console.WriteLine("\n Delete List");
                ET_List delList = new ET_List();
                delList.ID = MyNewListID;
                delList.AuthStub = myclient;
                FuelSDK.DeleteReturn fr = delList.Delete();
                Console.WriteLine("Delete Status: " + fr.Status.ToString());
                Console.WriteLine("Message: " + fr.Message.ToString());
                Console.WriteLine("Code: " + fr.Code.ToString());
                Console.WriteLine("Results Length: " + fr.Results.Length);

                Console.WriteLine("\n Retrieve List to confirm deletion");
                list.Props = new string[] { "ID", "ListName", "Description" };
                list.SearchFilter = new SimpleFilterPart() { Property = "ID", SimpleOperator = SimpleOperators.equals, Value = new String[] { MyNewListID.ToString() } };
                getFR = list.Get();
                Console.WriteLine("Get Status: " + getFR.Status.ToString());
                Console.WriteLine("Message: " + getFR.Message.ToString());
                Console.WriteLine("Code: " + getFR.Code.ToString());
                Console.WriteLine("Results Length: " + getFR.Results.Length);
                foreach (ET_List ResultList in getFR.Results)
                {
                    Console.WriteLine("--ID: " + ResultList.ID + ", Name: " + ResultList.ListName + ", Description: " + ResultList.Description);
                }

                Console.WriteLine("\n Info List");
                ET_List listInfo = new ET_List();
                listInfo.AuthStub = myclient;
                InfoReturn info = listInfo.Info();
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
