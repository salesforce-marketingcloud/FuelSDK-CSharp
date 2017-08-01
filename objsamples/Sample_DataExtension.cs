using FuelSDK;
using System;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_DataExtension()
        {
            var myclient = new ET_Client();

            var nameOfTestDataExtension = "CSharpCreatedDE";

            Console.WriteLine("--- Testing DataExtension ---");
            Console.WriteLine("\n Get all of the DataExtensions in an Account");
            var getAllDataExtension = new ET_DataExtension
            {
                AuthStub = myclient,
                Props = new[] { "CustomerKey", "Name" },
            };
            var grAllDataExtension = getAllDataExtension.Get();

            Console.WriteLine("Get Status: " + grAllDataExtension.Status.ToString());
            Console.WriteLine("Message: " + grAllDataExtension.Message);
            Console.WriteLine("Code: " + grAllDataExtension.Code.ToString());
            Console.WriteLine("Results Length: " + grAllDataExtension.Results.Length);

            while (grAllDataExtension.MoreResults)
            {
                Console.WriteLine("\n Continue Retrieve All DataExtension with GetMoreResults");
                grAllDataExtension = getAllDataExtension.GetMoreResults();
                Console.WriteLine("Get Status: " + grAllDataExtension.Status.ToString());
                Console.WriteLine("Message: " + grAllDataExtension.Message);
                Console.WriteLine("Code: " + grAllDataExtension.Code.ToString());
                Console.WriteLine("Results Length: " + grAllDataExtension.Results.Length);
            }

            Console.WriteLine("\n Create DataExtension");
            var postDataExtension = new ET_DataExtension
            {
                AuthStub = myclient,
                Name = nameOfTestDataExtension,
                CustomerKey = nameOfTestDataExtension,
                Columns = new[] { 
                    new ET_DataExtensionColumn { Name = "Name", FieldType = DataExtensionFieldType.Text, IsPrimaryKey = true, MaxLength = 100, IsRequired = true },
                    new ET_DataExtensionColumn { Name = "OtherColumn", FieldType = DataExtensionFieldType.Text } },
            };
            var postResponse = postDataExtension.Post();
            Console.WriteLine("Post Status: " + postResponse.Status.ToString());
            Console.WriteLine("Message: " + postResponse.Message);
            Console.WriteLine("Code: " + postResponse.Code.ToString());
            Console.WriteLine("Results Length: " + postResponse.Results.Length);

            if (postResponse.Status)
            {
                Console.WriteLine("\n Update DE to add new field");
                var patchDataExtension = new ET_DataExtension
                {
                    AuthStub = myclient,
                    CustomerKey = nameOfTestDataExtension,
                    Columns = new[] {
                        new ET_DataExtensionColumn { Name = "AddedField", FieldType = DataExtensionFieldType.Text } },
                };
                var patchFR = patchDataExtension.Patch();
                Console.WriteLine("Patch Status: " + patchFR.Status.ToString());
                Console.WriteLine("Message: " + patchFR.Message);
                Console.WriteLine("Code: " + patchFR.Code.ToString());
                Console.WriteLine("Results Length: " + patchFR.Results.Length);

                Console.WriteLine("\n Retrieve All Columns for a data extension");
                var getColumn = new ET_DataExtensionColumn
                {
                    AuthStub = myclient,
                    Props = new[] { "Name", "FieldType" },
                    SearchFilter = new SimpleFilterPart { Property = "DataExtension.CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { nameOfTestDataExtension } },
                };
                var getColumnResponse = getColumn.Get();
                Console.WriteLine("Get Status: " + getColumnResponse.Status.ToString());
                Console.WriteLine("Message: " + getColumnResponse.Message);
                Console.WriteLine("Code: " + getColumnResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getColumnResponse.Results.Length);

                if (getColumnResponse.Status)
                    foreach (ET_DataExtensionColumn column in getColumnResponse.Results)
                        Console.WriteLine("-- Name: " + column.Name + "  Type: " + column.FieldType.ToString());

                Console.WriteLine("\n Add a row to a data extension (using CustomerKey)");
                var deRowPost = new ET_DataExtensionRow
                {
                    AuthStub = myclient,
                    DataExtensionCustomerKey = nameOfTestDataExtension,
                };
                deRowPost.ColumnValues.Add("Name", "Example Name");
                deRowPost.ColumnValues.Add("OtherColumn", "Different Example Text");
                var prRowResponse = deRowPost.Post();
                Console.WriteLine("Post Status: " + prRowResponse.Status.ToString());
                Console.WriteLine("Message: " + prRowResponse.Message);
                Console.WriteLine("Code: " + prRowResponse.Code.ToString());
                Console.WriteLine("Results Length: " + prRowResponse.Results.Length);

                Console.WriteLine("\n Add a row to a data extension (using Name)");
                var deRowPost2 = new ET_DataExtensionRow
                {
                    AuthStub = myclient,
                    DataExtensionName = nameOfTestDataExtension,
                };
                deRowPost2.ColumnValues.Add("Name", "Example Name3");
                deRowPost2.ColumnValues.Add("OtherColumn", "Different Example Text");
                var prRowResponse2 = deRowPost2.Post();
                Console.WriteLine("Post Status: " + prRowResponse2.Status.ToString());
                Console.WriteLine("Message: " + prRowResponse2.Message);
                Console.WriteLine("Code: " + prRowResponse2.Code.ToString());
                Console.WriteLine("Results Length: " + prRowResponse2.Results.Length);

                Console.WriteLine("\n Retrieve All Rows from DataExtension");
                var deRowGet = new ET_DataExtensionRow
                {
                    AuthStub = myclient,
                    DataExtensionName = nameOfTestDataExtension,
                    Props = new[] { "Name", "OtherColumn" },
                };
                var grRow = deRowGet.Get();
                Console.WriteLine("Post Status: " + grRow.Status.ToString());
                Console.WriteLine("Message: " + grRow.Message);
                Console.WriteLine("Code: " + grRow.Code.ToString());
                Console.WriteLine("Results Length: " + grRow.Results.Length);

                if (getColumnResponse.Status)
                    foreach (ET_DataExtensionRow column in grRow.Results)
                        Console.WriteLine("--Name: " + column.ColumnValues["Name"] + " - OtherColumn: " + column.ColumnValues["OtherColumn"]);

                Console.WriteLine("\n Update a row in  a data extension");
                var deRowPatch = new ET_DataExtensionRow
                {
                    AuthStub = myclient,
                    DataExtensionCustomerKey = nameOfTestDataExtension,
                };
                deRowPatch.ColumnValues.Add("Name", "Example Name");
                deRowPatch.ColumnValues.Add("OtherColumn", "New Value for First Column");
                var patchRowResponse = deRowPatch.Patch();
                Console.WriteLine("Post Status: " + patchRowResponse.Status.ToString());
                Console.WriteLine("Message: " + patchRowResponse.Message);
                Console.WriteLine("Code: " + patchRowResponse.Code.ToString());
                Console.WriteLine("Results Length: " + patchRowResponse.Results.Length);

                Console.WriteLine("\n Retrieve only updated row");
                var deRowGetSingle = new ET_DataExtensionRow
                {
                    AuthStub = myclient,
                    DataExtensionName = nameOfTestDataExtension,
                    Props = new[] { "Name", "OtherColumn" },
                    SearchFilter = new SimpleFilterPart { Property = "Name", SimpleOperator = SimpleOperators.equals, Value = new[] { "Example Name" } },
                };
                var grSingleRow = deRowGetSingle.Get();
                Console.WriteLine("Post Status: " + grSingleRow.Status.ToString());
                Console.WriteLine("Message: " + grSingleRow.Message);
                Console.WriteLine("Code: " + grSingleRow.Code.ToString());
                Console.WriteLine("Results Length: " + grSingleRow.Results.Length);

                if (getColumnResponse.Status)
                    foreach (ET_DataExtensionRow column in grSingleRow.Results)
                        Console.WriteLine("--Name: " + column.ColumnValues["Name"] + " - OtherColumn: " + column.ColumnValues["OtherColumn"]);

                Console.WriteLine("\n Delete a row from a data extension)");
                var deRowDelete = new ET_DataExtensionRow
                {
                    AuthStub = myclient,
                    DataExtensionCustomerKey = nameOfTestDataExtension,
                };
                deRowDelete.ColumnValues.Add("Name", "Example Name");
                var drRowResponse = deRowDelete.Delete();
                Console.WriteLine("Post Status: " + drRowResponse.Status.ToString());
                Console.WriteLine("Message: " + drRowResponse.Message);
                Console.WriteLine("Code: " + drRowResponse.Code.ToString());
                Console.WriteLine("Results Length: " + drRowResponse.Results.Length);

                Console.WriteLine("\n Delete DataExtension");
                var delDataExtension = new ET_DataExtension
                {
                    CustomerKey = nameOfTestDataExtension,
                    AuthStub = myclient,
                };
                var deleteResponse = delDataExtension.Delete();
                Console.WriteLine("Delete Status: " + deleteResponse.Status.ToString());
                Console.WriteLine("Message: " + deleteResponse.Message);
                Console.WriteLine("Code: " + deleteResponse.Code.ToString());
                Console.WriteLine("Results Length: " + deleteResponse.Results.Length);

                Console.WriteLine("\n Info DataExtension");
                var DataExtensionInfo = new ET_DataExtension
                {
                    AuthStub = myclient,
                };
                var info = DataExtensionInfo.Info();
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

