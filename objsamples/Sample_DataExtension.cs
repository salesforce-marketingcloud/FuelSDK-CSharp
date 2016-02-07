using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_DataExtension()
        {
            ET_Client myclient = new ET_Client();

            string NameOfTestDataExtension = "CSharpSDKExample";

            Console.WriteLine("--- Testing DataExtension ---");
            Console.WriteLine("\n Get all of the DataExtensions in an Account");
            ET_DataExtension getAllDataExtension = new ET_DataExtension();
            getAllDataExtension.AuthStub = myclient;
            getAllDataExtension.Props = new string[] { "CustomerKey", "Name" };
            GetReturn grAllDataExtension = getAllDataExtension.Get();

            Console.WriteLine("Get Status: " + grAllDataExtension.Status.ToString());
            Console.WriteLine("Message: " + grAllDataExtension.Message.ToString());
            Console.WriteLine("Code: " + grAllDataExtension.Code.ToString());
            Console.WriteLine("Results Length: " + grAllDataExtension.Results.Length);

            while (grAllDataExtension.MoreResults)
            {
                Console.WriteLine("\n Continue Retrieve All DataExtension with GetMoreResults");
                grAllDataExtension = getAllDataExtension.GetMoreResults();
                Console.WriteLine("Get Status: " + grAllDataExtension.Status.ToString());
                Console.WriteLine("Message: " + grAllDataExtension.Message.ToString());
                Console.WriteLine("Code: " + grAllDataExtension.Code.ToString());
                Console.WriteLine("Results Length: " + grAllDataExtension.Results.Length);
            }



            Console.WriteLine("\n Create DataExtension");
            ET_DataExtension postDataExtension = new ET_DataExtension();
            postDataExtension.AuthStub = myclient;
            postDataExtension.Name = NameOfTestDataExtension;
            postDataExtension.CustomerKey = NameOfTestDataExtension;
            ET_DataExtensionColumn nameColumn = new ET_DataExtensionColumn() { Name = "Name", FieldType = DataExtensionFieldType.Text, IsPrimaryKey = true, MaxLength = 100, IsRequired = true };
            ET_DataExtensionColumn otherColumn = new ET_DataExtensionColumn() { Name = "OtherColumn", FieldType = DataExtensionFieldType.Text };
            postDataExtension.Columns = new ET_DataExtensionColumn[] { nameColumn, otherColumn };
            PostReturn postResponse = postDataExtension.Post();
            Console.WriteLine("Post Status: " + postResponse.Status.ToString());
            Console.WriteLine("Message: " + postResponse.Message.ToString());
            Console.WriteLine("Code: " + postResponse.Code.ToString());
            Console.WriteLine("Results Length: " + postResponse.Results.Length);

            if (postResponse.Status)
            {
                Console.WriteLine("\n Update DE to add new field");
                ET_DataExtension patchDataExtension = new ET_DataExtension();
                patchDataExtension.AuthStub = myclient;
                patchDataExtension.CustomerKey = NameOfTestDataExtension;
                ET_DataExtensionColumn addedField = new ET_DataExtensionColumn() { Name = "AddedField", FieldType = DataExtensionFieldType.Text };
                patchDataExtension.Columns = new ET_DataExtensionColumn[] { addedField };
                FuelSDK.PatchReturn patchFR = patchDataExtension.Patch();
                Console.WriteLine("Patch Status: " + patchFR.Status.ToString());
                Console.WriteLine("Message: " + patchFR.Message.ToString());
                Console.WriteLine("Code: " + patchFR.Code.ToString());
                Console.WriteLine("Results Length: " + patchFR.Results.Length);

                Console.WriteLine("\n Retrieve All Columns for a data extension");
                ET_DataExtensionColumn getColumn = new ET_DataExtensionColumn();
                getColumn.AuthStub = myclient;
                getColumn.Props = new string[] { "Name", "FieldType" };
                getColumn.SearchFilter = new SimpleFilterPart() { Property = "DataExtension.CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new string[] { NameOfTestDataExtension } };
                GetReturn getColumnResponse = getColumn.Get();
                Console.WriteLine("Get Status: " + getColumnResponse.Status.ToString());
                Console.WriteLine("Message: " + getColumnResponse.Message.ToString());
                Console.WriteLine("Code: " + getColumnResponse.Code.ToString());
                Console.WriteLine("Results Length: " + getColumnResponse.Results.Length);

                if (getColumnResponse.Status)
                {
                    foreach (ET_DataExtensionColumn column in getColumnResponse.Results)
                    {
                        Console.WriteLine("-- Name: " + column.Name + "  Type: " + column.FieldType.ToString());
                    }
                }

                Console.WriteLine("\n Add a row to a data extension (using CustomerKey)");
                ET_DataExtensionRow deRowPost = new ET_DataExtensionRow();
                deRowPost.AuthStub = myclient;
                deRowPost.DataExtensionCustomerKey = NameOfTestDataExtension;
                deRowPost.ColumnValues.Add("Name", "Example Name");
                deRowPost.ColumnValues.Add("OtherColumn", "Different Example Text");
                PostReturn prRowResponse = deRowPost.Post();
                Console.WriteLine("Post Status: " + prRowResponse.Status.ToString());
                Console.WriteLine("Message: " + prRowResponse.Message.ToString());
                Console.WriteLine("Code: " + prRowResponse.Code.ToString());
                Console.WriteLine("Results Length: " + prRowResponse.Results.Length);

                Console.WriteLine("\n Add a row to a data extension (using Name)");
                ET_DataExtensionRow deRowPost2 = new ET_DataExtensionRow();
                deRowPost2.AuthStub = myclient;
                deRowPost2.DataExtensionName = NameOfTestDataExtension;
                deRowPost2.ColumnValues.Add("Name", "Example Name3");
                deRowPost2.ColumnValues.Add("OtherColumn", "Different Example Text");
                PostReturn prRowResponse2 = deRowPost2.Post();
                Console.WriteLine("Post Status: " + prRowResponse2.Status.ToString());
                Console.WriteLine("Message: " + prRowResponse2.Message.ToString());
                Console.WriteLine("Code: " + prRowResponse2.Code.ToString());
                Console.WriteLine("Results Length: " + prRowResponse2.Results.Length);

                Console.WriteLine("\n Retrieve All Rows from DataExtension");
                ET_DataExtensionRow deRowGet = new ET_DataExtensionRow();
                deRowGet.AuthStub = myclient;
                deRowGet.DataExtensionName = NameOfTestDataExtension;
                deRowGet.Props = new string[] { "Name", "OtherColumn" };
                GetReturn grRow = deRowGet.Get();
                Console.WriteLine("Post Status: " + grRow.Status.ToString());
                Console.WriteLine("Message: " + grRow.Message.ToString());
                Console.WriteLine("Code: " + grRow.Code.ToString());
                Console.WriteLine("Results Length: " + grRow.Results.Length);

                if (getColumnResponse.Status)
                {
                    foreach (ET_DataExtensionRow column in grRow.Results)
                    {
                        Console.WriteLine("--Name: " + column.ColumnValues["Name"] + " - OtherColumn: " + column.ColumnValues["OtherColumn"]);
                    }
                }

                Console.WriteLine("\n Update a row in  a data extension");
                ET_DataExtensionRow deRowPatch = new ET_DataExtensionRow();
                deRowPatch.AuthStub = myclient;
                deRowPatch.DataExtensionCustomerKey = NameOfTestDataExtension;
                deRowPatch.ColumnValues.Add("Name", "Example Name");
                deRowPatch.ColumnValues.Add("OtherColumn", "New Value for First Column");
                PatchReturn patchRowResponse = deRowPatch.Patch();
                Console.WriteLine("Post Status: " + patchRowResponse.Status.ToString());
                Console.WriteLine("Message: " + patchRowResponse.Message.ToString());
                Console.WriteLine("Code: " + patchRowResponse.Code.ToString());
                Console.WriteLine("Results Length: " + patchRowResponse.Results.Length);

                Console.WriteLine("\n Retrieve only updated row");
                ET_DataExtensionRow deRowGetSingle = new ET_DataExtensionRow();
                deRowGetSingle.AuthStub = myclient;
                deRowGetSingle.DataExtensionName = NameOfTestDataExtension;
                deRowGetSingle.Props = new string[] { "Name", "OtherColumn" };
                deRowGetSingle.SearchFilter = new SimpleFilterPart() { Property = "Name", SimpleOperator = SimpleOperators.equals, Value = new string[] { "Example Name" } };
                GetReturn grSingleRow = deRowGetSingle.Get();
                Console.WriteLine("Post Status: " + grSingleRow.Status.ToString());
                Console.WriteLine("Message: " + grSingleRow.Message.ToString());
                Console.WriteLine("Code: " + grSingleRow.Code.ToString());
                Console.WriteLine("Results Length: " + grSingleRow.Results.Length);

                if (getColumnResponse.Status)
                {
                    foreach (ET_DataExtensionRow column in grSingleRow.Results)
                    {
                        Console.WriteLine("--Name: " + column.ColumnValues["Name"] + " - OtherColumn: " + column.ColumnValues["OtherColumn"]);
                    }
                }

                Console.WriteLine("\n Delete a row from a data extension)");
                ET_DataExtensionRow deRowDelete = new ET_DataExtensionRow();
                deRowDelete.AuthStub = myclient;
                deRowDelete.DataExtensionCustomerKey = NameOfTestDataExtension;
                deRowDelete.ColumnValues.Add("Name", "Example Name");
                DeleteReturn drRowResponse = deRowDelete.Delete();
                Console.WriteLine("Post Status: " + drRowResponse.Status.ToString());
                Console.WriteLine("Message: " + drRowResponse.Message.ToString());
                Console.WriteLine("Code: " + drRowResponse.Code.ToString());
                Console.WriteLine("Results Length: " + drRowResponse.Results.Length);


                Console.WriteLine("\n Delete DataExtension");
                ET_DataExtension delDataExtension = new ET_DataExtension();
                delDataExtension.CustomerKey = NameOfTestDataExtension;
                delDataExtension.AuthStub = myclient;
                DeleteReturn deleteResponse = delDataExtension.Delete();
                Console.WriteLine("Delete Status: " + deleteResponse.Status.ToString());
                Console.WriteLine("Message: " + deleteResponse.Message.ToString());
                Console.WriteLine("Code: " + deleteResponse.Code.ToString());
                Console.WriteLine("Results Length: " + deleteResponse.Results.Length);

                Console.WriteLine("\n Create Sendable Data Extension ");
                ET_DataExtension postSendableDataExtension = new ET_DataExtension();
                postSendableDataExtension.AuthStub = myclient;
                postSendableDataExtension.Name = NameOfTestDataExtension + "Sendable";
                postSendableDataExtension.CustomerKey = NameOfTestDataExtension + "Sendable";
                postSendableDataExtension.IsSendable = true;
                postSendableDataExtension.SendableDataExtensionField = new ET_DataExtensionColumn() { Name = "Email" };
                postSendableDataExtension.SendableSubscriberField = new ET_ProfileAttribute() { Name = "Email Address" }; // This value would need to be "Subscriber Key" if not using Email Address.
                ET_DataExtensionColumn nameSColumn = new ET_DataExtensionColumn() { Name = "Email", FieldType = DataExtensionFieldType.EmailAddress, IsPrimaryKey = true, MaxLength = 100, IsRequired = true };
                ET_DataExtensionColumn otherSColumn = new ET_DataExtensionColumn() { Name = "OtherColumn", FieldType = DataExtensionFieldType.Text };
                postSendableDataExtension.Columns = new ET_DataExtensionColumn[] { nameSColumn, otherSColumn };
                PostReturn postSendableResponse = postSendableDataExtension.Post();
                Console.WriteLine("Post Status: " + postSendableResponse.Status.ToString());
                Console.WriteLine("Message: " + postSendableResponse.Message.ToString());
                Console.WriteLine("Code: " + postSendableResponse.Code.ToString());
                Console.WriteLine("Results Length: " + postSendableResponse.Results.Length);

                Console.WriteLine("\n Delete DataExtension");
                ET_DataExtension delSendableDataExtension = new ET_DataExtension();
                delSendableDataExtension.CustomerKey = NameOfTestDataExtension + "Sendable";
                delSendableDataExtension.AuthStub = myclient;
                DeleteReturn deleteSendableResponse = delSendableDataExtension.Delete();
                Console.WriteLine("Delete Status: " + deleteSendableResponse.Status.ToString());
                Console.WriteLine("Message: " + deleteSendableResponse.Message.ToString());
                Console.WriteLine("Code: " + deleteSendableResponse.Code.ToString());
                Console.WriteLine("Results Length: " + deleteSendableResponse.Results.Length);


                Console.WriteLine("\n Info DataExtension");
                ET_DataExtension DataExtensionInfo = new ET_DataExtension();
                DataExtensionInfo.AuthStub = myclient;
                InfoReturn info = DataExtensionInfo.Info();
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
