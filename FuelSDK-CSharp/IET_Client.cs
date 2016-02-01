using System;
using System.Collections.Generic;

namespace FuelSDK
{
    public interface IET_Client
    {
        string authToken { get; set; }
        SoapClient soapclient { get; set; }
        string internalAuthToken { get; set; }
        string SDKVersion { get; set; }

        void refreshToken(bool force = false);
        FuelReturn AddSubscribersToList(string EmailAddress, string SubscriberKey, List<int> ListIDs);
        FuelReturn AddSubscribersToList(string EmailAddress, List<int> ListIDs);
        FuelReturn CreateDataExtensions(ET_DataExtension[] ArrayOfET_DataExtension);
    }
}
