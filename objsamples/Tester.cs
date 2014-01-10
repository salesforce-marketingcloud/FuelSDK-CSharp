using System;

namespace objsamples
{
    partial class Tester
    {
        static void Main(string[] args)
        {
            //args = new[] { "list", "campaign", "contentarea", "dataextension", "email", "folder", "list", "subscriber", "triggeredsend", "listsubscriber", "addsubscribertolist", "createdataextensions", "openevent", "bounceevent", "sentevent", "clickevent", "unsubevent" };
            if (args.Length == 0)
            {
                Console.WriteLine("Input Object to test:");
                TestObject(Console.ReadLine());
            }
            else
                foreach (string objectName in args)
                    TestObject(objectName);
            Console.WriteLine("Press Enter to Exit");
            Console.ReadLine();
        }

        static void TestObject(string objectName)
        {
            switch (objectName.ToUpper())
            {
                case "CAMPAIGN":
                    TestET_Campaign();
                    break;
                case "CONTENTAREA":
                    TestET_ContentArea();
                    break;
                case "DATAEXTENSION":
                    TestET_DataExtension();
                    break;
                case "EMAIL":
                    TestET_Email();
                    break;
                case "FOLDER":
                    TestET_Folder();
                    break;
                case "LIST":
                    TestET_List();
                    break;
                case "SUBSCRIBER":
                    TestET_Subscriber();
                    break;
                case "TRIGGEREDSEND":
                    TestET_TriggeredSend();
                    break;
                case "LISTSUBSCRIBER":
                    TestET_ListSubscriber();
                    break;
                case "EMAILSENDDEFINITION":
                    TestET_EmailSendDefinition();
                    break;
                case "ENDPOINT":
                    TestET_Endpoint();
                    break;
                case "IMPORT":
                    TestET_Import();
                    break;

                // Helper Methods
                case "ADDSUBSCRIBERTOLIST":
                    Test_AddSubscriberToList();
                    break;
                case "CREATEDATAEXTENSIONS":
                    Test_CreateDataExtensions();
                    break;

                // Tracking Events
                case "OPENEVENT":
                    TestET_OpenEvent();
                    break;
                case "BOUNCEEVENT":
                    TestET_BounceEvent();
                    break;
                case "SENTEVENT":
                    TestET_SentEvent();
                    break;
                case "CLICKEVENT":
                    TestET_ClickEvent();
                    break;
                case "UNSUBEVENT":
                    TestET_UnsubEvent();
                    break;
                default:
                    Console.WriteLine("Unrecognized Object: " + objectName.ToString());
                    break;
            }
        }
    }
}
