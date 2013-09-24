using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelSDK;
using System.Threading;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_Import()
        {
            ET_Client myclient = new ET_Client();
            string NewImportName = "FuelSDKImportExample";
            string SendableDataExtensionObjectID = "62476204-bfd3-de11-95ca-001e0bbae8cc";
            int ListIDForImport = 1768161;

            Console.WriteLine("--- Testing Import ---");
            Console.WriteLine("\n Create Import to DataExtension");
            ET_Import postImport = new ET_Import();
            postImport.AuthStub = myclient;
            postImport.Name = NewImportName;
            postImport.CustomerKey = NewImportName;
            postImport.Description = "Created with FuelSDK";
            postImport.AllowErrors = true;
            postImport.DestinationObject = new ET_DataExtension() { ObjectID = SendableDataExtensionObjectID};
            postImport.FieldMappingType = ImportDefinitionFieldMappingType.InferFromColumnHeadings;
            postImport.FileSpec = "FuelSDKExample.csv";
            postImport.FileType = FileType.CSV;
            postImport.Notification = new AsyncResponse() { ResponseType = AsyncResponseType.email, ResponseAddress = "example@bh.exacttarget.com" };
            postImport.RetrieveFileTransferLocation = new FileTransferLocation() { CustomerKey = "ExactTarget Enhanced FTP" };
            postImport.UpdateType = ImportDefinitionUpdateType.Overwrite;
            PostReturn prImport = postImport.Post();
            Console.WriteLine("Post Status: " + prImport.Status.ToString());
            Console.WriteLine("Message: " + prImport.Message.ToString());
            Console.WriteLine("Code: " + prImport.Code.ToString());
            Console.WriteLine("Results Length: " + prImport.Results.Length);

            Console.WriteLine("\n Delete Import");
            ET_Import deleteImport = new ET_Import();
            deleteImport.AuthStub = myclient;
            deleteImport.CustomerKey = NewImportName;
            DeleteReturn drImport = deleteImport.Delete();
            Console.WriteLine("Delete Status: " + drImport.Status.ToString());
            Console.WriteLine("Message: " + drImport.Message.ToString());
            Console.WriteLine("Code: " + drImport.Code.ToString());
            Console.WriteLine("Results Length: " + drImport.Results.Length);


            Console.WriteLine("--- Testing Import ---");
            Console.WriteLine("\n Create Import to List");
            ET_Import postListImport = new ET_Import();
            postListImport.AuthStub = myclient;
            postListImport.Name = NewImportName;
            postListImport.CustomerKey = NewImportName;
            postListImport.Description = "Created with FuelSDK";
            postListImport.AllowErrors = true;
            postListImport.DestinationObject = new ET_List() { ID = ListIDForImport };
            postListImport.FieldMappingType = ImportDefinitionFieldMappingType.InferFromColumnHeadings;
            postListImport.FileSpec = "FuelSDKExample.csv";
            postListImport.FileType = FileType.CSV;
            postListImport.Notification = new AsyncResponse() { ResponseType = AsyncResponseType.email, ResponseAddress = "example@bh.exacttarget.com" };
            postListImport.RetrieveFileTransferLocation = new FileTransferLocation() { CustomerKey = "ExactTarget Enhanced FTP" };
            postListImport.UpdateType = ImportDefinitionUpdateType.AddAndUpdate;
            PostReturn prListImport = postListImport.Post();
            Console.WriteLine("Post Status: " + prListImport.Status.ToString());
            Console.WriteLine("Message: " + prListImport.Message.ToString());
            Console.WriteLine("Code: " + prListImport.Code.ToString());
            Console.WriteLine("Results Length: " + prListImport.Results.Length);

            Console.WriteLine("\n Start Import To List");
            ET_Import startImport = new ET_Import();
            startImport.AuthStub = myclient;
            startImport.CustomerKey = NewImportName;
            PerformReturn perListImport = startImport.Start();
            Console.WriteLine("Start Status: " + perListImport.Status.ToString());
            Console.WriteLine("Message: " + perListImport.Message.ToString());
            Console.WriteLine("Code: " + perListImport.Code.ToString());
            Console.WriteLine("Results Length: " + perListImport.Results.Length);

            if (perListImport.Status)
            {
                Console.WriteLine("\n Check Status using the same instance of ET_Import as used for start");
                string CurrentImportStatus = "";
                while (CurrentImportStatus != "Error" && CurrentImportStatus != "Completed")
                {
                    Console.WriteLine("Checking status in loop " + CurrentImportStatus);
                    //Wait a bit before checking the status to give it time to process
                    Thread.Sleep(15000);
                    GetReturn statusListImport = startImport.Status();
                    Console.WriteLine("Status Status: " + statusListImport.Status.ToString());
                    Console.WriteLine("Message: " + statusListImport.Message.ToString());
                    Console.WriteLine("Code: " + statusListImport.Code.ToString());
                    Console.WriteLine("Results Length: " + statusListImport.Results.Length);
                    CurrentImportStatus = ((ET_ImportResult)statusListImport.Results[0]).ImportStatus;
                }
                Console.WriteLine("Final Status: " + CurrentImportStatus);
            }

            Console.WriteLine("\n Delete Import");
            ET_Import deleteListImport = new ET_Import();
            deleteListImport.AuthStub = myclient;
            deleteListImport.CustomerKey = NewImportName;
            DeleteReturn drListImport = deleteImport.Delete();
            Console.WriteLine("Delete Status: " + drListImport.Status.ToString());
            Console.WriteLine("Message: " + drListImport.Message.ToString());
            Console.WriteLine("Code: " + drListImport.Code.ToString());
            Console.WriteLine("Results Length: " + drListImport.Results.Length);
        }
    }
}
