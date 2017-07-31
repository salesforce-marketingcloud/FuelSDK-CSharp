using FuelSDK;
using System;
using System.Threading;

namespace objsamples
{
    partial class Tester
    {
        static void TestET_Import()
        {
            var myclient = new ET_Client();
            var newImportName = "FuelSDKImportExample";
            var SendableDataExtensionObjectID = "62476204-bfd3-de11-95ca-001e0bbae8cc";
            var ListIDForImport = 1768161;

            Console.WriteLine("--- Testing Import ---");
            Console.WriteLine("\n Create Import to DataExtension");
            var postImport = new ET_Import
            {
                AuthStub = myclient,
                Name = newImportName,
                CustomerKey = newImportName,
                Description = "Created with FuelSDK",
                AllowErrors = true,
                DestinationObject = new ET_DataExtension { ObjectID = SendableDataExtensionObjectID },
                FieldMappingType = ImportDefinitionFieldMappingType.InferFromColumnHeadings,
                FileSpec = "FuelSDKExample.csv",
                FileType = FileType.CSV,
                Notification = new AsyncResponse { ResponseType = AsyncResponseType.email, ResponseAddress = "example@bh.exacttarget.com" },
                RetrieveFileTransferLocation = new FileTransferLocation { CustomerKey = "ExactTarget Enhanced FTP" },
                UpdateType = ImportDefinitionUpdateType.Overwrite,
            };
            var prImport = postImport.Post();
            Console.WriteLine("Post Status: " + prImport.Status.ToString());
            Console.WriteLine("Message: " + prImport.Message);
            Console.WriteLine("Code: " + prImport.Code.ToString());
            Console.WriteLine("Results Length: " + prImport.Results.Length);

            Console.WriteLine("\n Delete Import");
            var deleteImport = new ET_Import
            {
                AuthStub = myclient,
                CustomerKey = newImportName,
            };
            DeleteReturn drImport = deleteImport.Delete();
            Console.WriteLine("Delete Status: " + drImport.Status.ToString());
            Console.WriteLine("Message: " + drImport.Message);
            Console.WriteLine("Code: " + drImport.Code.ToString());
            Console.WriteLine("Results Length: " + drImport.Results.Length);

            Console.WriteLine("--- Testing Import ---");
            Console.WriteLine("\n Create Import to List");
            var postListImport = new ET_Import
            {
                AuthStub = myclient,
                Name = newImportName,
                CustomerKey = newImportName,
                Description = "Created with FuelSDK",
                AllowErrors = true,
                DestinationObject = new ET_List { ID = ListIDForImport },
                FieldMappingType = ImportDefinitionFieldMappingType.InferFromColumnHeadings,
                FileSpec = "FuelSDKExample.csv",
                FileType = FileType.CSV,
                Notification = new AsyncResponse { ResponseType = AsyncResponseType.email, ResponseAddress = "example@bh.exacttarget.com" },
                RetrieveFileTransferLocation = new FileTransferLocation { CustomerKey = "ExactTarget Enhanced FTP" },
                UpdateType = ImportDefinitionUpdateType.AddAndUpdate,
            };
            var prListImport = postListImport.Post();
            Console.WriteLine("Post Status: " + prListImport.Status.ToString());
            Console.WriteLine("Message: " + prListImport.Message);
            Console.WriteLine("Code: " + prListImport.Code.ToString());
            Console.WriteLine("Results Length: " + prListImport.Results.Length);

            Console.WriteLine("\n Start Import To List");
            var startImport = new ET_Import
            {
                AuthStub = myclient,
                CustomerKey = newImportName,
            };
            var perListImport = startImport.Start();
            Console.WriteLine("Start Status: " + perListImport.Status.ToString());
            Console.WriteLine("Message: " + perListImport.Message);
            Console.WriteLine("Code: " + perListImport.Code.ToString());
            Console.WriteLine("Results Length: " + perListImport.Results.Length);

            if (perListImport.Status)
            {
                Console.WriteLine("\n Check Status using the same instance of ET_Import as used for start");
                var CurrentImportStatus = string.Empty;
                while (CurrentImportStatus != "Error" && CurrentImportStatus != "Completed")
                {
                    Console.WriteLine("Checking status in loop " + CurrentImportStatus);
                    //Wait a bit before checking the status to give it time to process
                    Thread.Sleep(15000);
                    var statusListImport = startImport.Status();
                    Console.WriteLine("Status Status: " + statusListImport.Status.ToString());
                    Console.WriteLine("Message: " + statusListImport.Message);
                    Console.WriteLine("Code: " + statusListImport.Code.ToString());
                    Console.WriteLine("Results Length: " + statusListImport.Results.Length);
                    CurrentImportStatus = ((ET_ImportResult)statusListImport.Results[0]).ImportStatus;
                }
                Console.WriteLine("Final Status: " + CurrentImportStatus);
            }

            Console.WriteLine("\n Delete Import");
            var deleteListImport = new ET_Import
            {
                AuthStub = myclient,
                CustomerKey = newImportName,
            };
            var drListImport = deleteImport.Delete();
            Console.WriteLine("Delete Status: " + drListImport.Status.ToString());
            Console.WriteLine("Message: " + drListImport.Message);
            Console.WriteLine("Code: " + drListImport.Code.ToString());
            Console.WriteLine("Results Length: " + drListImport.Results.Length);
        }
    }
}
