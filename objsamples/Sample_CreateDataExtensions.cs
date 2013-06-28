using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;

namespace objsamples
{
    partial class Tester
    {
        static void Test_CreateDataExtensions()
        {
            ET_Client myclient = new ET_Client();

            
            Console.WriteLine("--- Testing CreateDataExtensions ---");

            ET_DataExtension DEOne = new ET_DataExtension() {Name = "HelperDEOne",CustomerKey = "HelperDEOne"};
            ET_DataExtensionColumn DEOneColumnOne = new ET_DataExtensionColumn() {Name = "Name", FieldType = DataExtensionFieldType.Text, IsPrimaryKey = true, MaxLength = 100, IsRequired = true};
            ET_DataExtensionColumn DEOneColumnTwo = new ET_DataExtensionColumn() {Name = "OtherField", FieldType = DataExtensionFieldType.Text};
            DEOne.Columns = new ET_DataExtensionColumn[] {DEOneColumnOne, DEOneColumnTwo};

            ET_DataExtension DETwo = new ET_DataExtension() {Name = "HelperDETwo",CustomerKey = "HelperDETwo"};
            ET_DataExtensionColumn DETwoColumnOne = new ET_DataExtensionColumn() {Name = "Name", FieldType = DataExtensionFieldType.Text, IsPrimaryKey = true, MaxLength = 100, IsRequired = true};
            ET_DataExtensionColumn DETwoColumnTwo = new ET_DataExtensionColumn() {Name = "OtherField", FieldType = DataExtensionFieldType.Text};
            DETwo.Columns = new ET_DataExtensionColumn[] {DETwoColumnOne, DETwoColumnTwo};

            ET_DataExtension[] ACoupleDEs = new ET_DataExtension[] { DEOne, DETwo };

            FuelReturn createReturn = myclient.CreateDataExtensions(ACoupleDEs);
            Console.WriteLine("Helper Status: " + createReturn.Status.ToString());
            Console.WriteLine("Message: " + createReturn.Message.ToString());
            Console.WriteLine("Code: " + createReturn.Code.ToString());

            if (createReturn.Status)
            {
                Console.WriteLine("\n Delete DEOne");
                ET_DataExtension delDataExtension = new ET_DataExtension();
                delDataExtension.CustomerKey = "HelperDEOne"; ;
                delDataExtension.AuthStub = myclient;
                DeleteReturn deleteResponse = delDataExtension.Delete();
                Console.WriteLine("Delete Status: " + deleteResponse.Status.ToString());
                Console.WriteLine("Message: " + deleteResponse.Message.ToString());
                Console.WriteLine("Code: " + deleteResponse.Code.ToString());
                Console.WriteLine("Results Length: " + deleteResponse.Results.Length);

                Console.WriteLine("\n Delete DETwo");
                ET_DataExtension delDataExtension2 = new ET_DataExtension();
                delDataExtension2.CustomerKey = "HelperDETwo"; ;
                delDataExtension2.AuthStub = myclient;
                DeleteReturn deleteResponse2 = delDataExtension2.Delete();
                Console.WriteLine("Delete Status: " + deleteResponse2.Status.ToString());
                Console.WriteLine("Message: " + deleteResponse2.Message.ToString());
                Console.WriteLine("Code: " + deleteResponse2.Code.ToString());
                Console.WriteLine("Results Length: " + deleteResponse2.Results.Length);
            }
        }
    }
}
