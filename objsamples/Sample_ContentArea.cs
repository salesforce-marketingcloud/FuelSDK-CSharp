using FuelSDK;
using System;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_ContentArea()
        {
            var myclient = new ET_Client();
            var nameOfTestContentArea = "CSharpSDKContentArea";

            Console.WriteLine("--- Testing ContentArea ---");
            Console.WriteLine("\n Retrieve All ContentArea with GetMoreResults");
            var getAllContentArea = new ET_ContentArea
            {
                AuthStub = myclient,
                Props = new[] { "RowObjectID", "ObjectID", "ID", "CustomerKey", "Client.ID", "ModifiedDate", "CreatedDate", "CategoryID", "Name", "Layout", "IsDynamicContent", "Content", "IsSurvey", "IsBlank", "Key" },
            };
            var grAllContent = getAllContentArea.Get();

            Console.WriteLine("Get Status: " + grAllContent.Status.ToString());
            Console.WriteLine("Message: " + grAllContent.Message);
            Console.WriteLine("Code: " + grAllContent.Code.ToString());
            Console.WriteLine("Results Length: " + grAllContent.Results.Length);

            while (grAllContent.MoreResults)
            {
                Console.WriteLine("\n Continue Retrieve All ContentArea with GetMoreResults");
                grAllContent = getAllContentArea.GetMoreResults();
                Console.WriteLine("Get Status: " + grAllContent.Status.ToString());
                Console.WriteLine("Message: " + grAllContent.Message);
                Console.WriteLine("Code: " + grAllContent.Code.ToString());
                Console.WriteLine("Results Length: " + grAllContent.Results.Length);
            }

            Console.WriteLine("\n Create ContentArea");
            var postContentArea = new ET_ContentArea
            {
                AuthStub = myclient,
                Name = nameOfTestContentArea,
                CustomerKey = nameOfTestContentArea,
                Content = "<b>Some HTML Content Goes here</b>",
                //FolderID = 1073161,
            };
            var postResponse = postContentArea.Post();
            Console.WriteLine("Post Status: " + postResponse.Status.ToString());
            Console.WriteLine("Message: " + postResponse.Message);
            Console.WriteLine("Code: " + postResponse.Code.ToString());
            Console.WriteLine("Results Length: " + postResponse.Results.Length);

            if (postResponse.Status)
            {
                Console.WriteLine("\n Retrieve newly create ContentArea");
                var getContentArea = new ET_ContentArea
                {
                    AuthStub = myclient,
                    Props = new[] { "RowObjectID", "ObjectID", "ID", "CustomerKey", "Client.ID", "ModifiedDate", "CreatedDate", "CategoryID", "Name", "Layout", "IsDynamicContent", "Content", "IsSurvey", "IsBlank", "Key" },
                    SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { nameOfTestContentArea } },
                };
                var getResponse = getContentArea.Get();
                Console.WriteLine("Get Status: " + getResponse.Status.ToString());
                Console.WriteLine("Message: " + getResponse.Message);
                Console.WriteLine("Code: " + getResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getResponse.Results.Length);
                foreach (ET_ContentArea ResultContentArea in getResponse.Results)
                    Console.WriteLine("--ID: " + ResultContentArea.ID + ", Name: " + ResultContentArea.Name + ", Content: " + ResultContentArea.Content);

                Console.WriteLine("\n Update ContentArea");
                var patchContentArea = new ET_ContentArea
                {
                    CustomerKey = nameOfTestContentArea,
                    Content = "<b>Some HTML Content Goes here. NOW WITH NEW CONTENT</b>",
                    AuthStub = myclient,
                };
                var patchFR = patchContentArea.Patch();
                Console.WriteLine("Patch Status: " + patchFR.Status.ToString());
                Console.WriteLine("Message: " + patchFR.Message);
                Console.WriteLine("Code: " + patchFR.Code.ToString());
                Console.WriteLine("Results Length: " + patchFR.Results.Length);

                Console.WriteLine("\n Retrieve updated ContentArea");
                getContentArea.Props = new[] { "RowObjectID", "ObjectID", "ID", "CustomerKey", "Client.ID", "ModifiedDate", "CreatedDate", "CategoryID", "Name", "Layout", "IsDynamicContent", "Content", "IsSurvey", "IsBlank", "Key" };
                getContentArea.SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { nameOfTestContentArea } };
                getResponse = getContentArea.Get();
                Console.WriteLine("Get Status: " + getResponse.Status.ToString());
                Console.WriteLine("Message: " + getResponse.Message);
                Console.WriteLine("Code: " + getResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getResponse.Results.Length);
                foreach (ET_ContentArea ResultContentArea in getResponse.Results)
                    Console.WriteLine("--ID: " + ResultContentArea.ID + ", Name: " + ResultContentArea.Name + ", Content: " + ResultContentArea.Content);

                Console.WriteLine("\n Delete ContentArea");
                var delContentArea = new ET_ContentArea
                {
                    CustomerKey = nameOfTestContentArea,
                    AuthStub = myclient,
                };
                var deleteResponse = delContentArea.Delete();
                Console.WriteLine("Delete Status: " + deleteResponse.Status.ToString());
                Console.WriteLine("Message: " + deleteResponse.Message);
                Console.WriteLine("Code: " + deleteResponse.Code.ToString());
                Console.WriteLine("Results Length: " + deleteResponse.Results.Length);

                Console.WriteLine("\n Retrieve ContentArea to confirm deletion");
                getContentArea.Props = new[] { "RowObjectID", "ObjectID", "ID", "CustomerKey", "Client.ID", "ModifiedDate", "CreatedDate", "CategoryID", "Name", "Layout", "IsDynamicContent", "Content", "IsSurvey", "IsBlank", "Key" };
                getContentArea.SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { nameOfTestContentArea } };
                getResponse = getContentArea.Get();
                Console.WriteLine("Get Status: " + getResponse.Status.ToString());
                Console.WriteLine("Message: " + getResponse.Message);
                Console.WriteLine("Code: " + getResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getResponse.Results.Length);

#if false
                Console.WriteLine("\n Info ContentArea");
                var ContentAreaInfo = new ET_ContentArea
                {
                    AuthStub = myclient,
                };
                var info = ContentAreaInfo.Info();
                Console.WriteLine("Info Status: " + info.Status.ToString());
                Console.WriteLine("Message: " + info.Message);
                Console.WriteLine("Code: " + info.Code.ToString());
                Console.WriteLine("Results Length: " + info.Results.Length);
                foreach (ET_PropertyDefinition def in info.Results)
                    Console.WriteLine("--Name: " + def.Name + ", IsRetrievable: " + def.IsRetrievable.ToString());
#endif
            }
        }
    }
}
