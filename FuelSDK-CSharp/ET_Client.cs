using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace FuelSDK
{
    #region ET-Client

    public class ET_Client
    {
        public string authToken;
        public SoapClient soapclient;
        private string appSignature = string.Empty;
        private string clientId = string.Empty;
        private string clientSecret = string.Empty;
        private string soapEndPoint = string.Empty;
        public string internalAuthToken = string.Empty;
        private string refreshKey = string.Empty;
        private DateTime authTokenExpiration = DateTime.Now;
        public string SDKVersion = "FuelSDX-C#-V.8";

        public ET_Client(NameValueCollection parameters = null)
        {
            // Get configuration file and set variables
            if (File.Exists(@"FuelSDK_config.xml"))
            {
                var doc = new System.Xml.XPath.XPathDocument(@"FuelSDK_config.xml");
                foreach (XPathNavigator child in doc.CreateNavigator().Select("configuration"))
                {
                    appSignature = child.SelectSingleNode("appSignature").Value.ToString().Trim();
                    clientId = child.SelectSingleNode("clientId").Value.ToString().Trim();
                    clientSecret = child.SelectSingleNode("clientSecret").Value.ToString().Trim();
                    soapEndPoint = child.SelectSingleNode("soapEndPoint").Value.ToString().Trim();
                }
            }

            if (parameters != null)
            {
                if (parameters.AllKeys.Contains("appSignature"))
                    appSignature = parameters["appSignature"];
                if (parameters.AllKeys.Contains("clientId"))
                    clientId = parameters["clientId"];
                if (parameters.AllKeys.Contains("clientSecret"))
                    clientSecret = parameters["clientSecret"];
                if (parameters.AllKeys.Contains("soapEndPoint"))
                    soapEndPoint = parameters["soapEndPoint"];
            }

            if (clientId.Equals(string.Empty) || clientSecret.Equals(string.Empty))
                throw new Exception("clientId or clientSecret is null: Must be provided in config file or passed when instantiating ET_Client");

            //If JWT URL Parameter Used
            if (parameters != null && parameters.AllKeys.Contains("jwt"))
            {
                if (appSignature.Equals(string.Empty))
                    throw new Exception("Unable to utilize JWT for SSO without appSignature: Must be provided in config file or passed when instantiating ET_Client");
                var encodedJWT = parameters["jwt"].ToString().Trim();
                var decodedJWT = JsonWebToken.Decode(encodedJWT, appSignature);
                var parsedJWT = JObject.Parse(decodedJWT);
                authToken = parsedJWT["request"]["user"]["oauthToken"].Value<string>().Trim();
                authTokenExpiration = DateTime.Now.AddSeconds(int.Parse(parsedJWT["request"]["user"]["expiresIn"].Value<string>().Trim()));
                internalAuthToken = parsedJWT["request"]["user"]["internalOauthToken"].Value<string>().Trim();
                refreshKey = parsedJWT["request"]["user"]["refreshToken"].Value<string>().Trim();
            }
            else
                RefreshToken();

            // Find the appropriate endpoint for the acccount
            var grSingleEndpoint = new ET_Endpoint { AuthStub = this, Type = "soap" }.Get();
            if (grSingleEndpoint.Status && grSingleEndpoint.Results.Length == 1)
                soapEndPoint = ((ET_Endpoint)grSingleEndpoint.Results[0]).URL;
            else
                throw new Exception("Unable to determine stack using /platform/v1/endpoints: " + grSingleEndpoint.Message);

            // Create the SOAP binding for call with Oauth.
            var binding = new BasicHttpBinding
            {
                Name = "UserNameSoapBinding",
                MaxReceivedMessageSize = 2147483647,
            };
            binding.Security.Mode = BasicHttpSecurityMode.TransportWithMessageCredential;
            soapclient = new SoapClient(binding, new EndpointAddress(new Uri(soapEndPoint)));
            soapclient.ClientCredentials.UserName.UserName = "*";
            soapclient.ClientCredentials.UserName.Password = "*";
        }

        public void RefreshToken(bool force = false)
        {
            // RefreshToken
            if (!string.IsNullOrEmpty(authToken) && DateTime.Now.AddSeconds(300) <= authTokenExpiration && !force)
                return;

            // Get an internalAuthToken using clientId and clientSecret
            var strURL = "https://auth.exacttargetapis.com/v1/requestToken?legacy=1";

            // Build the request
            var request = (HttpWebRequest)WebRequest.Create(strURL.Trim());
            request.Method = "POST";
            request.ContentType = "application/json";
            request.UserAgent = this.SDKVersion;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json;
                if (!string.IsNullOrEmpty(refreshKey))
                    json = string.Format(@"{""clientId"": ""{0}"", ""clientSecret"": ""{1}"", ""refreshToken"": ""{2}"", ""scope"": ""cas:{3}"" , ""accessType"": ""offline""}", clientId, clientSecret, refreshKey, internalAuthToken);
                else
                    json = string.Format(@"{""clientId"": ""{0}"", ""clientSecret"": ""{1}"", ""accessType"": ""offline""}", clientId, clientSecret);
                streamWriter.Write(json);
            }

            // Get the response
            string responseFromServer = null;
            using (var response = (HttpWebResponse)request.GetResponse())
            using (var dataStream = response.GetResponseStream())
            using (var reader = new StreamReader(dataStream))
                responseFromServer = reader.ReadToEnd();

            // Parse the response
            var parsedResponse = JObject.Parse(responseFromServer);
            internalAuthToken = parsedResponse["legacyToken"].Value<string>().Trim();
            authToken = parsedResponse["accessToken"].Value<string>().Trim();
            authTokenExpiration = DateTime.Now.AddSeconds(int.Parse(parsedResponse["expiresIn"].Value<string>().Trim()));
            refreshKey = parsedResponse["refreshToken"].Value<string>().Trim();
        }

        public FuelReturn AddSubscribersToList(string emailAddress, string subscriberKey, IEnumerable<int> listIDs) { return ProcessAddSubscriberToList(emailAddress, subscriberKey, listIDs); }
        public FuelReturn AddSubscribersToList(string emailAddress, IEnumerable<int> listIDs) { return ProcessAddSubscriberToList(emailAddress, null, listIDs); }
        protected FuelReturn ProcessAddSubscriberToList(string emailAddress, string subscriberKey, IEnumerable<int> listIDs)
        {
            var sub = new ET_Subscriber
            {
                AuthStub = this,
                EmailAddress = emailAddress,
                Lists = listIDs.Select(x => new ET_SubscriberList { ID = x }).ToArray(),
            };
            if (subscriberKey != null)
                sub.SubscriberKey = subscriberKey;
            var prAddSub = sub.Post();
            if (!prAddSub.Status && prAddSub.Results.Length > 0 && prAddSub.Results[0].ErrorCode == 12014)
                return sub.Patch();
            return prAddSub;
        }

        public FuelReturn CreateDataExtensions(ET_DataExtension[] dataExtensions)
        {
            var cleanedArray = new List<ET_DataExtension>();
            foreach (var de in dataExtensions)
            {
                de.AuthStub = this;
                de.Fields = de.Columns;
                de.Columns = null;
                cleanedArray.Add(de);
            }
            return new PostReturn(cleanedArray.ToArray());
        }
    }

    #endregion

    #region Primitives

    public partial class APIObject
    {
        [XmlIgnore()]
        [JsonIgnore]
        public ET_Client AuthStub { get; set; }
        [XmlIgnore()]
        public string[] Props { get; set; }
        [XmlIgnore()]
        public FilterPart SearchFilter { get; set; }
        [XmlIgnore()]
        public String LastRequestID { get; set; }
    }

    public class FuelObject : APIObject
    {
        [JsonIgnore]
        public string Endpoint { get; set; }
        public string[] URLProperties { get; set; }
        public string[] RequiredURLProperties { get; set; }
        public int Page { get; set; }
        protected string CleanRestValue(JToken obj) { return obj.ToString().Replace("\"", "").Trim(); }
    }

    public abstract class FuelReturn
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public bool MoreResults { get; set; }
        public int Code { get; set; }
        public string RequestID { get; set; }

        private Dictionary<Type, Type> _translators = new Dictionary<Type, Type>();

        public FuelReturn()
        {
            _translators.Add(typeof(ET_Folder), typeof(DataFolder));
            _translators.Add(typeof(DataFolder), typeof(ET_Folder));

            _translators.Add(typeof(ET_List), typeof(List));
            _translators.Add(typeof(List), typeof(ET_List));

            _translators.Add(typeof(ET_ContentArea), typeof(ContentArea));
            _translators.Add(typeof(ContentArea), typeof(ET_ContentArea));

            _translators.Add(typeof(ET_ObjectDefinition), typeof(ObjectDefinition));
            _translators.Add(typeof(ObjectDefinition), typeof(ET_ObjectDefinition));

            _translators.Add(typeof(ET_PropertyDefinition), typeof(PropertyDefinition));
            _translators.Add(typeof(PropertyDefinition), typeof(ET_PropertyDefinition));

            _translators.Add(typeof(Subscriber), typeof(ET_Subscriber));
            _translators.Add(typeof(ET_Subscriber), typeof(Subscriber));

            _translators.Add(typeof(ET_ProfileAttribute), typeof(Attribute));
            _translators.Add(typeof(Attribute), typeof(ET_ProfileAttribute));

            _translators.Add(typeof(ET_Email), typeof(Email));
            _translators.Add(typeof(Email), typeof(ET_Email));

            _translators.Add(typeof(ET_SubscriberList), typeof(SubscriberList));
            _translators.Add(typeof(SubscriberList), typeof(ET_SubscriberList));

            _translators.Add(typeof(ET_List_Subscriber), typeof(ListSubscriber));
            _translators.Add(typeof(ListSubscriber), typeof(ET_List_Subscriber));

            _translators.Add(typeof(ET_DataExtension), typeof(DataExtension));
            _translators.Add(typeof(DataExtension), typeof(ET_DataExtension));

            _translators.Add(typeof(ET_DataExtensionColumn), typeof(DataExtensionField));
            _translators.Add(typeof(DataExtensionField), typeof(ET_DataExtensionColumn));

            _translators.Add(typeof(ET_DataExtensionRow), typeof(DataExtensionObject));
            _translators.Add(typeof(DataExtensionObject), typeof(ET_DataExtensionRow));

            _translators.Add(typeof(ET_SendClassification), typeof(SendClassification));
            _translators.Add(typeof(SendClassification), typeof(ET_SendClassification));

            _translators.Add(typeof(ET_SendDefinitionList), typeof(SendDefinitionList));
            _translators.Add(typeof(SendDefinitionList), typeof(ET_SendDefinitionList));

            _translators.Add(typeof(ET_SenderProfile), typeof(SenderProfile));
            _translators.Add(typeof(SenderProfile), typeof(ET_SenderProfile));

            _translators.Add(typeof(ET_DeliveryProfile), typeof(DeliveryProfile));
            _translators.Add(typeof(DeliveryProfile), typeof(ET_DeliveryProfile));

            _translators.Add(typeof(ET_TriggeredSend), typeof(TriggeredSendDefinition));
            _translators.Add(typeof(TriggeredSendDefinition), typeof(ET_TriggeredSend));

            _translators.Add(typeof(ET_EmailSendDefinition), typeof(EmailSendDefinition));
            _translators.Add(typeof(EmailSendDefinition), typeof(ET_EmailSendDefinition));

            _translators.Add(typeof(ET_Send), typeof(Send));
            _translators.Add(typeof(Send), typeof(ET_Send));

            _translators.Add(typeof(ET_Import), typeof(ImportDefinition));
            _translators.Add(typeof(ImportDefinition), typeof(ET_Import));

            _translators.Add(typeof(ET_ImportResult), typeof(ImportResultsSummary));
            _translators.Add(typeof(ImportResultsSummary), typeof(ET_ImportResult));

            // The translation for this is handled in the Get() method for DataExtensionObject so no need to translate it
            _translators.Add(typeof(APIProperty), typeof(APIProperty));

            _translators.Add(typeof(ET_Trigger), typeof(TriggeredSend));
            _translators.Add(typeof(TriggeredSend), typeof(ET_Trigger));

            // Tracking Events
            _translators.Add(typeof(ET_BounceEvent), typeof(BounceEvent));
            _translators.Add(typeof(BounceEvent), typeof(ET_BounceEvent));
            _translators.Add(typeof(OpenEvent), typeof(ET_OpenEvent));
            _translators.Add(typeof(ET_OpenEvent), typeof(OpenEvent));
            _translators.Add(typeof(ET_ClickEvent), typeof(ClickEvent));
            _translators.Add(typeof(ClickEvent), typeof(ET_ClickEvent));
            _translators.Add(typeof(ET_UnsubEvent), typeof(UnsubEvent));
            _translators.Add(typeof(UnsubEvent), typeof(ET_UnsubEvent));
            _translators.Add(typeof(ET_SentEvent), typeof(SentEvent));
            _translators.Add(typeof(SentEvent), typeof(ET_SentEvent));
        }

        public APIObject TranslateObject(APIObject inputObject)
        {
            if (_translators.ContainsKey(inputObject.GetType()))
            {
                var returnObject = (APIObject)Activator.CreateInstance(_translators[inputObject.GetType()]);
                foreach (var prop in inputObject.GetType().GetProperties())
                {
                    var propValue = prop.GetValue(inputObject, null);
                    if ((prop.PropertyType.IsSubclassOf(typeof(APIObject)) || prop.PropertyType == typeof(APIObject)) && propValue != null)
                        prop.SetValue(returnObject, TranslateObject(propValue), null);
                    else if (_translators.ContainsKey(prop.PropertyType) && propValue != null)
                        prop.SetValue(returnObject, TranslateObject(propValue), null);
                    else if (prop.PropertyType.IsArray && propValue != null)
                    {
                        var a = (Array)propValue;
                        Array outArray;
                        if (a.Length > 0)
                            if (_translators.ContainsKey(a.GetValue(0).GetType()))
                            {
                                outArray = Array.CreateInstance(_translators[a.GetValue(0).GetType()], a.Length);
                                for (int i = 0; i < a.Length; i++)
                                    if (_translators.ContainsKey(a.GetValue(i).GetType()))
                                        outArray.SetValue(TranslateObject(a.GetValue(i)), i);
                                if (outArray.Length > 0)
                                    prop.SetValue(returnObject, outArray, null);
                            }
                    }
                    else if (prop.Name == "FolderID" && propValue != null)
                    {
                        if (inputObject.GetType().GetProperty("Category") != null)
                        {
                            var categoryIDProp = inputObject.GetType().GetProperty("Category");
                            categoryIDProp.SetValue(returnObject, propValue, null);
                        }
                        else if (inputObject.GetType().GetProperty("CategoryID") != null)
                        {
                            var categoryIDProp = inputObject.GetType().GetProperty("CategoryID");
                            categoryIDProp.SetValue(returnObject, propValue, null);
                        }
                    }
                    else if ((prop.Name == "CategoryIDSpecified" || prop.Name == "CategorySpecified") && propValue != null)
                    {
                        // Do nothing
                    }
                    else if ((prop.Name == "CategoryID" || prop.Name == "Category") && propValue != null)
                    {
                        if (returnObject.GetType().GetProperty("FolderID") != null)
                        {
                            var folderIDProp = returnObject.GetType().GetProperty("FolderID");
                            folderIDProp.SetValue(returnObject, Convert.ToInt32(propValue), null);
                        }
                    }
                    else if (propValue != null && returnObject.GetType().GetProperty(prop.Name) != null)
                        prop.SetValue(returnObject, propValue, null);
                }
                return returnObject;
            }
            return inputObject;
        }

        protected object TranslateObject(object inputObject)
        {
            if (_translators.ContainsKey(inputObject.GetType()))
            {
                var returnObject = (object)Activator.CreateInstance(_translators[inputObject.GetType()]);
                foreach (var prop in inputObject.GetType().GetProperties())
                    if (prop.GetValue(inputObject, null) != null && returnObject.GetType().GetProperty(prop.Name) != null)
                        prop.SetValue(returnObject, prop.GetValue(inputObject, null), null);
                return returnObject;
            }
            return inputObject;
        }

        public class ExecuteAPIResponse<TResult>
        {
            public TResult[] Results { get; set; }
            public string RequestID { get; set; }
            public string OverallStatus { get; set; }
            public string OverallStatusMessage { get; set; }

            public ExecuteAPIResponse(TResult[] results, string requestID, string overallStatus)
            {
                Results = results;
                RequestID = requestID;
                OverallStatus = overallStatus;
            }
        }

        protected TResult[] ExecuteAPI<TResult>(Func<ET_Client, APIObject[], ExecuteAPIResponse<TResult>> func, params APIObject[] objs) { return ExecuteAPI<TResult, APIObject>(TranslateObject, func, objs); }
        protected TResult[] ExecuteAPI<TResult, TObject>(Func<APIObject, TObject> select, Func<ET_Client, TObject[], ExecuteAPIResponse<TResult>> func, params APIObject[] objs)
        {
            if (objs == null)
                throw new ArgumentNullException("objs");
            var client = objs.Select(x => x.AuthStub).FirstOrDefault();
            if (client == null)
                throw new InvalidOperationException("client");
            client.RefreshToken();
            using (var scope = new OperationContextScope(client.soapclient.InnerChannel))
            {
                // Add oAuth token to SOAP header.
                XNamespace ns = "http://exacttarget.com";
                var oauthElement = new XElement(ns + "oAuthToken", client.internalAuthToken);
                var xmlHeader = MessageHeader.CreateHeader("oAuth", "http://exacttarget.com", oauthElement);
                OperationContext.Current.OutgoingMessageHeaders.Add(xmlHeader);

                var httpRequest = new System.ServiceModel.Channels.HttpRequestMessageProperty();
                OperationContext.Current.OutgoingMessageProperties.Add(System.ServiceModel.Channels.HttpRequestMessageProperty.Name, httpRequest);
                httpRequest.Headers.Add(HttpRequestHeader.UserAgent, client.SDKVersion);

                var response = func(client, objs.Select(select).ToArray());
                RequestID = response.RequestID;
                Status = (response.OverallStatus == "OK" || response.OverallStatus == "MoreDataAvailable");
                Code = 200;
                MoreResults = (response.OverallStatus == "MoreDataAvailable");
                Message = (response.OverallStatusMessage ?? string.Empty);
                return response.Results;
            }
        }

        protected string ExecuteFuel(FuelObject obj, string[] required, string method, bool postValue)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            obj.AuthStub.RefreshToken();

            object propValue;
            string propValueAsString;
            var completeURL = obj.Endpoint;
            if (required != null)
                foreach (string urlProp in required)
                {
                    var match = false;
                    foreach (var prop in obj.GetType().GetProperties())
                        if (obj.URLProperties.Contains(prop.Name) && (propValue = prop.GetValue(obj, null)) != null)
                            if ((propValueAsString = propValue.ToString().Trim()).Length > 0 && propValueAsString != "0")
                                match = true;
                    if (!match)
                        throw new Exception("Unable to process request due to missing required property: " + urlProp);
                }
            foreach (var prop in obj.GetType().GetProperties())
                if (obj.URLProperties.Contains(prop.Name) && (propValue = prop.GetValue(obj, null)) != null)
                    if ((propValueAsString = propValue.ToString().Trim()).Length > 0 && propValueAsString != "0")
                        completeURL = completeURL.Replace("{" + prop.Name + "}", propValueAsString);

            // Clean up not required URL parameters
            if (obj.URLProperties != null)
                foreach (string urlProp in obj.URLProperties)
                    completeURL = completeURL.Replace("{" + urlProp + "}", string.Empty);

            completeURL += "?access_token=" + obj.AuthStub.authToken;
            if (obj.Page != 0)
                completeURL += "&page=" + obj.Page.ToString();

            var request = (HttpWebRequest)WebRequest.Create(completeURL.Trim());
            request.Method = method;
            request.ContentType = "application/json";
            request.UserAgent = obj.AuthStub.SDKVersion;

            if (postValue)
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    streamWriter.Write(JsonConvert.SerializeObject(obj));

            // Get the response
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var dataStream = response.GetResponseStream())
                using (var reader = new StreamReader(dataStream))
                {
                    Code = (int)response.StatusCode;
                    Status = (response.StatusCode == HttpStatusCode.OK);
                    MoreResults = false;
                    Message = (Status ? string.Empty : response.ToString());
                    return (Status ? reader.ReadToEnd() : null);
                }
            }
            catch (WebException we)
            {
                Code = (int)((HttpWebResponse)we.Response).StatusCode;
                Status = false;
                MoreResults = false;
                using (var stream = we.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                    Message = reader.ReadToEnd();
                return null;
            }
        }
    }

    public class ResultDetail
    {
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public int OrdinalID { get; set; }
        public int ErrorCode { get; set; }
        public int NewID { get; set; }
        public string NewObjectID { get; set; }
        public APIObject Object { get; set; }
        public TaskResult Task { get; set; }
    }

    public class PostReturn : FuelReturn
    {
        public ResultDetail[] Results { get; set; }

        public PostReturn(params APIObject[] objs)
        {
            if (objs == null)
                throw new ArgumentNullException("objs");
            var response = ExecuteAPI((c, os) =>
            {
                string requestID;
                string overallStatus;
                return new ExecuteAPIResponse<CreateResult>(c.soapclient.Create(new CreateOptions(), os, out requestID, out overallStatus), requestID, overallStatus);
            }, objs);
            if (response != null)
                if (response.GetType() == typeof(CreateResult[]) && response.Length > 0)
                    Results = response.Cast<CreateResult>().Select(x => new ResultDetail
                    {
                        StatusCode = x.StatusCode,
                        StatusMessage = x.StatusMessage,
                        NewObjectID = x.NewObjectID,
                        Object = (x.Object != null ? TranslateObject(x.Object) : null),
                        OrdinalID = x.OrdinalID,
                        ErrorCode = x.ErrorCode,
                        NewID = x.NewID,
                    }).ToArray();
                else
                    Results = new ResultDetail[0];
        }

        public PostReturn(FuelObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            var response = ExecuteFuel(obj, obj.RequiredURLProperties, "POST", true);
            if (string.IsNullOrEmpty(response))
                return;
            if (response.StartsWith("["))
                Results = JArray.Parse(response)
                    .Select(x => new ResultDetail { Object = (APIObject)Activator.CreateInstance(obj.GetType(), BindingFlags.Public | BindingFlags.Instance, null, new object[] { x }, null) }).ToArray();
            else
            {
                var x = JObject.Parse(response);
                Results = new[] { new ResultDetail { Object = (APIObject)Activator.CreateInstance(obj.GetType(), BindingFlags.Public | BindingFlags.Instance, null, new object[] { x }, null) } };
            }
        }
    }

    public class SendReturn : PostReturn
    {
        public SendReturn(APIObject obj)
            : base(obj) { }
    }

    public class HelperReturn : PostReturn
    {
        public HelperReturn(APIObject obj)
            : base(obj) { }
    }

    public class PatchReturn : FuelReturn
    {
        public ResultDetail[] Results { get; set; }

        public PatchReturn(APIObject objs)
        {
            if (objs == null)
                throw new ArgumentNullException("objs");
            var response = ExecuteAPI((c, os) =>
            {
                string requestID;
                string overallStatus;
                return new ExecuteAPIResponse<UpdateResult>(c.soapclient.Update(new UpdateOptions(), os, out requestID, out overallStatus), requestID, overallStatus);
            }, objs);
            if (response != null)
                if (response.GetType() == typeof(UpdateResult[]) && response.Length > 0)
                    Results = response.Cast<UpdateResult>().Select(x => new ResultDetail
                    {
                        StatusCode = x.StatusCode,
                        StatusMessage = x.StatusMessage,
                        Object = (x.Object != null ? TranslateObject(x.Object) : null),
                        OrdinalID = x.OrdinalID,
                        ErrorCode = x.ErrorCode,
                    }).ToArray();
                else
                    Results = new ResultDetail[0];
        }
    }

    public class DeleteReturn : FuelReturn
    {
        public ResultDetail[] Results { get; set; }

        public DeleteReturn(APIObject objs)
        {
            if (objs == null)
                throw new ArgumentNullException("objs");
            var response = ExecuteAPI((c, os) =>
            {
                string requestID;
                string overallStatus;
                return new ExecuteAPIResponse<DeleteResult>(c.soapclient.Delete(new DeleteOptions(), os, out requestID, out overallStatus), requestID, overallStatus);
            }, objs);
            if (response != null)
                if (response.GetType() == typeof(DeleteResult[]) && response.Length > 0)
                    Results = response.Cast<DeleteResult>().Select(x => new ResultDetail
                    {
                        StatusCode = x.StatusCode,
                        StatusMessage = x.StatusMessage,
                        Object = (x.Object != null ? TranslateObject(x.Object) : null),
                        OrdinalID = x.OrdinalID,
                        ErrorCode = x.ErrorCode,
                    }).ToArray();
                else
                    Results = new ResultDetail[0];
        }

        public DeleteReturn(FuelObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            ExecuteFuel(obj, obj.URLProperties, "DELETE", false);
        }
    }

    public class PerformReturn : FuelReturn
    {
        public ResultDetail[] Results { get; set; }

        public PerformReturn(APIObject objs, string performAction)
        {
            if (objs == null)
                throw new ArgumentNullException("objs");
            var response = ExecuteAPI((c, os) =>
            {
                string requestID;
                string overallStatus;
                string overallStatusMessage;
                return new ExecuteAPIResponse<PerformResult>(c.soapclient.Perform(new PerformOptions(), performAction, os, out overallStatus, out overallStatusMessage, out requestID), requestID, overallStatus) { OverallStatusMessage = overallStatusMessage };
            }, objs);
            if (response != null)
                if (response.GetType() == typeof(PerformResult[]) && response.Length > 0)
                    Results = response.Cast<PerformResult>().Select(cr => new ResultDetail
                    {
                        StatusCode = cr.StatusCode,
                        StatusMessage = cr.StatusMessage,
                        Object = (cr.Object != null ? TranslateObject(cr.Object) : null),
                        Task = cr.Task,
                        OrdinalID = cr.OrdinalID,
                        ErrorCode = cr.ErrorCode,
                    }).ToArray();
                else
                    Results = new ResultDetail[0];
        }
    }

    public class GetReturn : FuelReturn
    {
        public int LastPageNumber { get; set; }
        public APIObject[] Results { get; set; }

        public GetReturn(APIObject objs) : this(objs, false, null) { }
        public GetReturn(APIObject objs, bool continueRequest, string overrideObjectType)
        {
            if (objs == null)
                throw new ArgumentNullException("objs");
            var response = ExecuteAPI(x =>
            {
                var rr = new RetrieveRequest();

                // Handle RetrieveAllSinceLastBatch
                if (x.GetType().GetProperty("GetSinceLastBatch") != null)
                    rr.RetrieveAllSinceLastBatch = (bool)x.GetType().GetProperty("GetSinceLastBatch").GetValue(x, null);

                if (continueRequest)
                {
                    if (x.LastRequestID == null)
                        throw new Exception("Unable to call GetMoreResults without first making successful Get() request");
                    rr.ContinueRequest = x.LastRequestID;
                }
                else
                {
                    if (x.SearchFilter != null)
                        rr.Filter = x.SearchFilter;

                    // Use the name from the object passed in unless an override is passed (Used for DataExtensionObject)
                    if (!string.IsNullOrEmpty(overrideObjectType))
                        rr.ObjectType = TranslateObject(x).GetType().ToString().Replace("FuelSDK.", string.Empty);
                    else
                        rr.ObjectType = overrideObjectType;

                    //If they didn't specify Props then we look them up using Info()
                    if (x.Props == null && x.GetType().GetMethod("Info") != null)
                    {
                        var ir = new InfoReturn(x);
                        if (!ir.Status)
                            throw new Exception("Unable to find properties for object in order to perform Get() request");
                        rr.Properties = ir.Results.Where(y => y.IsRetrievable).Select(y => y.Name).ToArray();
                    }
                    else
                        rr.Properties = x.Props;
                }
                return rr;
            }, (c, x) =>
            {
                string requestID;
                APIObject[] objectResults;
                string overallStatus = c.soapclient.Retrieve(x[0], out requestID, out objectResults);
                return new ExecuteAPIResponse<APIObject>(objectResults, requestID, overallStatus) { OverallStatusMessage = overallStatus };
            }, objs);
            if (response != null)
                if (response.Length > 0)
                    Results = response.Select(TranslateObject).ToArray();
                else
                    Results = new APIObject[0];
        }

        public GetReturn(FuelObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            var response = ExecuteFuel(obj, obj.RequiredURLProperties, "GET", false);
            if (string.IsNullOrEmpty(response))
                return;
            var parsedResponse = JObject.Parse(response);
            // Check on the paging information from response
            if (parsedResponse["page"] != null)
            {
                LastPageNumber = int.Parse(parsedResponse["page"].Value<string>().Trim());
                var pageSize = int.Parse(parsedResponse["pageSize"].Value<string>().Trim());

                var count = -1;
                if (parsedResponse["count"] != null)
                    count = int.Parse(parsedResponse["count"].Value<string>().Trim());
                else if (parsedResponse["totalCount"] != null)
                    count = int.Parse(parsedResponse["totalCount"].Value<string>().Trim());
                if (count != -1 && (count > (LastPageNumber * pageSize)))
                    MoreResults = true;
            }

            // Sub-response
            string subResponse;
            if (parsedResponse["items"] != null)
                subResponse = parsedResponse["items"].ToString().Trim();
            else if (parsedResponse["entities"] != null)
                subResponse = parsedResponse["entities"].ToString().Trim();
            else
                subResponse = response.Trim();
            if (string.IsNullOrEmpty(subResponse))
                return;

            var responseAsJSon = JsonConvert.DeserializeObject(response);
            if (responseAsJSon == null || responseAsJSon.ToString().Length <= 0)
                Results = new APIObject[0];
            else
                if (subResponse.StartsWith("["))
                    Results = JArray.Parse(subResponse)
                        .Select(x => (APIObject)Activator.CreateInstance(obj.GetType(), BindingFlags.Public | BindingFlags.Instance, null, new object[] { x }, null)).ToArray();
                else
                {
                    var x = JObject.Parse(subResponse);
                    Results = new[] { (APIObject)Activator.CreateInstance(obj.GetType(), BindingFlags.Public | BindingFlags.Instance, null, new object[] { x }, null) };
                }
        }
    }

    public class InfoReturn : FuelReturn
    {
        public ET_PropertyDefinition[] Results { get; set; }

        public InfoReturn(APIObject objs)
        {
            if (objs == null)
                throw new ArgumentNullException("objs");
            var response = ExecuteAPI(x => new ObjectDefinitionRequest { ObjectType = TranslateObject(x).GetType().ToString().Replace("FuelSDK.", string.Empty) }, (c, os) =>
            {
                string requestID;
                return new ExecuteAPIResponse<ObjectDefinition>(c.soapclient.Describe(os, out requestID), requestID, "OK");
            }, objs);
            if (response != null)
                if (response.Length > 0)
                    Results = response[0].Properties.Select(x => (ET_PropertyDefinition)(TranslateObject(x))).ToArray();
                else
                    Status = false;
        }
    }

    #endregion

    #region Subscriber Related Objects

    public class ET_Subscriber : Subscriber
    {
        public PostReturn Post() { return new PostReturn(this); }
        public PatchReturn Patch() { return new PatchReturn(this); }
        public DeleteReturn Delete() { return new DeleteReturn(this); }
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
    }

    public class ET_ProfileAttribute : Attribute { }
    public class ET_ImportResult : ImportResultsSummary { }
    public class ET_SubscriberList : SubscriberList { }

    public class ET_List : List
    {
        internal string FolderMediaType = "list";
        public int FolderID { get; set; }
        public PostReturn Post() { return new PostReturn(this); }
        public PatchReturn Patch() { return new PatchReturn(this); }
        public DeleteReturn Delete() { return new DeleteReturn(this); }
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
    }

    public class ET_Send : Send
    {
        public PostReturn Post() { return new PostReturn(this); }
        public PatchReturn Patch() { return new PatchReturn(this); }
        public DeleteReturn Delete() { return new DeleteReturn(this); }
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
    }

    public class ET_List_Subscriber : ListSubscriber
    {
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
    }

    #endregion

    #region Content Related

    public class ET_ContentArea : ContentArea
    {
        internal string FolderMediaType = "content";
        public int FolderID { get; set; }
        public PostReturn Post() { return new PostReturn(this); }
        public PatchReturn Patch() { return new PatchReturn(this); }
        public DeleteReturn Delete() { return new DeleteReturn(this); }
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
    }

    public class ET_Email : Email
    {
        internal string FolderMediaType = "email";
        public int FolderID { get; set; }
        public PostReturn Post() { return new PostReturn(this); }
        public PatchReturn Patch() { return new PatchReturn(this); }
        public DeleteReturn Delete() { return new DeleteReturn(this); }
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
    }

    public class ET_EmailSendDefinition : EmailSendDefinition
    {
        internal string FolderMediaType = "userinitiatedsends";
        internal string LastTaskID = string.Empty;
        public int FolderID { get; set; }
        public PostReturn Post() { return new PostReturn(this); }
        public PatchReturn Patch() { return new PatchReturn(this); }
        public DeleteReturn Delete() { return new DeleteReturn(this); }
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
        public PerformReturn Send()
        {
            var r = new PerformReturn(this, "start");
            if (r.Results.Length == 1)
                LastTaskID = ((ResultDetail)r.Results[0]).Task.ID;
            return r;
        }
        public GetReturn Status()
        {
            if (LastTaskID == string.Empty)
                throw new Exception("No ID available in order to return status for ET_EmailSendDefinition");
            var r = new GetReturn(new ET_Send
            {
                AuthStub = AuthStub,
                SearchFilter = new SimpleFilterPart { Value = new[] { LastTaskID }, Property = "ID", SimpleOperator = SimpleOperators.equals },
            });
            LastRequestID = r.RequestID;
            return r;
        }
    }

    public class ET_Import : ImportDefinition
    {
        internal string LastTaskID = string.Empty;
        public PostReturn Post() { return new PostReturn(this); }
        public PatchReturn Patch() { return new PatchReturn(this); }
        public DeleteReturn Delete() { return new DeleteReturn(this); }
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
        public GetReturn Status()
        {
            if (LastTaskID == string.Empty)
                throw new Exception("No ID available in order to return status for ET_Import");
            var r = new GetReturn(new ET_ImportResult
            {
                AuthStub = AuthStub,
                Props = new string[] { "ImportDefinitionCustomerKey", "TaskResultID", "ImportStatus", "StartDate", "EndDate", "DestinationID", "NumberSuccessful", "NumberDuplicated", "NumberErrors", "TotalRows", "ImportType" },
                SearchFilter = new SimpleFilterPart { Value = new[] { LastTaskID }, Property = "TaskResultID", SimpleOperator = SimpleOperators.equals },
            });
            LastRequestID = r.RequestID;
            return r;
        }
        public PerformReturn Start()
        {
            var r = new PerformReturn(this, "start");
            if (r.Results.Length == 1)
                LastTaskID = ((ResultDetail)r.Results[0]).Task.ID;
            return r;
        }
    }

    #endregion

    #region Data Extension Objects

    public class ET_DataExtension : DataExtension
    {
        internal string FolderMediaType = "dataextension";
        public int FolderID { get; set; }
        public ET_DataExtensionColumn[] Columns { get; set; }
        public PostReturn Post()
        {
            ET_DataExtension de = this;
            de.Fields = Columns;
            de.Columns = null;
            var pr = new PostReturn(de);
            foreach (var rd in pr.Results)
            {
                ((ET_DataExtension)rd.Object).Columns = (ET_DataExtensionColumn[])((ET_DataExtension)rd.Object).Fields;
                ((ET_DataExtension)rd.Object).Fields = null;
            }
            return pr;
        }
        public PatchReturn Patch()
        {
            ET_DataExtension de = this;
            de.Fields = Columns;
            de.Columns = null;
            var pr = new PatchReturn(de);
            foreach (var rd in pr.Results)
            {
                ((ET_DataExtension)rd.Object).Columns = (ET_DataExtensionColumn[])((ET_DataExtension)rd.Object).Fields;
                ((ET_DataExtension)rd.Object).Fields = null;
            }
            return pr;
        }
        public DeleteReturn Delete()
        {
            ET_DataExtension de = this;
            de.Fields = Columns;
            return new DeleteReturn(de);
        }
        public GetReturn Get()
        {
            var r = new GetReturn(this);
            LastRequestID = r.RequestID;
            foreach (ET_DataExtension rd in r.Results)
            {
                rd.Columns = (ET_DataExtensionColumn[])rd.Fields;
                rd.Fields = null;
            }
            return r;
        }
        public GetReturn GetMoreResults()
        {
            var r = new GetReturn(this, true, null);
            LastRequestID = r.RequestID;
            foreach (ET_DataExtension rd in r.Results)
            {
                rd.Columns = (ET_DataExtensionColumn[])rd.Fields;
                rd.Fields = null;
            }
            return r;
        }
        public InfoReturn Info() { return new InfoReturn(this); }
    }

    public class ET_DataExtensionColumn : DataExtensionField
    {
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
    }

    public class ET_DataExtensionRow : DataExtensionObject
    {
        public string DataExtensionName { get; set; }
        public string DataExtensionCustomerKey { get; set; }
        public Dictionary<string, string> ColumnValues { get; set; }
        public ET_DataExtensionRow()
        {
            ColumnValues = new Dictionary<string, string>();
        }
        public PostReturn Post()
        {
            GetDataExtensionCustomerKey();
            ET_DataExtensionRow row = this;
            row.CustomerKey = DataExtensionCustomerKey;
            row.Properties = ColumnValues.Select(x => new APIProperty { Name = x.Key, Value = x.Value }).ToArray();
            row.ColumnValues = null;
            row.DataExtensionName = null;
            row.DataExtensionCustomerKey = null;
            return new PostReturn(row);
        }
        public PatchReturn Patch()
        {
            GetDataExtensionCustomerKey();
            ET_DataExtensionRow row = this;
            row.CustomerKey = DataExtensionCustomerKey;
            row.Properties = ColumnValues.Select(x => new APIProperty { Name = x.Key, Value = x.Value }).ToArray();
            row.ColumnValues = null;
            row.DataExtensionName = null;
            row.DataExtensionCustomerKey = null;
            return new PatchReturn(row);
        }
        public DeleteReturn Delete()
        {
            GetDataExtensionCustomerKey();
            ET_DataExtensionRow row = this;
            row.CustomerKey = DataExtensionCustomerKey;
            row.ColumnValues = null;
            row.Keys = ColumnValues.Select(x => new APIProperty { Name = x.Key, Value = x.Value }).ToArray();
            row.DataExtensionName = null;
            row.DataExtensionCustomerKey = null;
            return new DeleteReturn(row);
        }
        public GetReturn Get()
        {
            GetDataExtensionName();
            var r = new GetReturn(this, false, "DataExtensionObject[" + DataExtensionName + "]");
            LastRequestID = r.RequestID;
            foreach (ET_DataExtensionRow dr in r.Results)
            {
                dr.ColumnValues = dr.Properties.ToDictionary(x => x.Name, x => x.Value);
                dr.Properties = null;
            }
            return r;
        }
        public GetReturn GetMoreResults()
        {
            GetDataExtensionName();
            var r = new GetReturn(this, true, "DataExtensionObject[" + DataExtensionName + "]");
            LastRequestID = r.RequestID;
            foreach (ET_DataExtensionRow dr in r.Results)
            {
                dr.ColumnValues = dr.Properties.ToDictionary(x => x.Name, x => x.Value);
                dr.Properties = null;
            }
            return r;
        }
        public InfoReturn Info() { return new InfoReturn(this); }
        private void GetDataExtensionName()
        {
            if (DataExtensionName == null)
            {
                if (DataExtensionCustomerKey == null)
                    throw new Exception("Unable to process ET_DataExtensionRow request due to DataExtensionCustomerKey or DataExtensionName not being defined on ET_DatExtensionRow");
                var grDEName = new ET_DataExtension
                {
                    AuthStub = AuthStub,
                    Props = new[] { "Name", "CustomerKey" },
                    SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { DataExtensionCustomerKey } },
                }.Get();
                if (grDEName.Status && grDEName.Results.Length > 0)
                    DataExtensionName = ((ET_DataExtension)grDEName.Results[0]).Name;
                else
                    throw new Exception("Unable to process ET_DataExtensionRow request due to unable to find DataExtension based on CustomerKey");
            }
        }
        private void GetDataExtensionCustomerKey()
        {
            if (DataExtensionCustomerKey == null)
            {
                if (DataExtensionName == null)
                    throw new Exception("Unable to process ET_DataExtensionRow request due to DataExtensionCustomerKey or DataExtensionName not being defined on ET_DatExtensionRow");
                var grDEName = new ET_DataExtension
                {
                    AuthStub = AuthStub,
                    Props = new[] { "Name", "CustomerKey" },
                    SearchFilter = new SimpleFilterPart { Property = "Name", SimpleOperator = SimpleOperators.equals, Value = new[] { DataExtensionName } },
                }.Get();
                if (grDEName.Status && grDEName.Results.Length > 0)
                    DataExtensionCustomerKey = ((ET_DataExtension)grDEName.Results[0]).CustomerKey;
                else
                    throw new Exception("Unable to process ET_DataExtensionRow request due to unable to find DataExtension based on DataExtensionName provided.");
            }
        }
    }

    #endregion

    #region Misc Objects

    public class ET_TriggeredSend : TriggeredSendDefinition
    {
        public int FolderID { get; set; }
        internal string FolderMediaType = "triggered_send";
        public ET_Subscriber[] Subscribers { get; set; }
        public SendReturn Send()
        {
            var ts = new ET_Trigger
            {
                CustomerKey = CustomerKey,
                TriggeredSendDefinition = this,
                Subscribers = Subscribers,
                AuthStub = AuthStub,
            };
            ((ET_TriggeredSend)ts.TriggeredSendDefinition).Subscribers = null;
            return new SendReturn(ts);
        }
        public PostReturn Post() { return new PostReturn(this); }
        public PatchReturn Patch() { return new PatchReturn(this); }
        public DeleteReturn Delete() { return new DeleteReturn(this); }
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
    }

    public class ET_Folder : DataFolder
    {
        public PostReturn Post() { return new PostReturn(this); }
        public PatchReturn Patch() { return new PatchReturn(this); }
        public DeleteReturn Delete() { return new DeleteReturn(this); }
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
    }

    public class ET_ObjectDefinition : ObjectDefinition { }
    public class ET_PropertyDefinition : PropertyDefinition { }
    public class ET_SendClassification : SendClassification { }
    public class ET_SenderProfile : SenderProfile { }
    public class ET_DeliveryProfile : DeliveryProfile { }
    public class ET_SendDefinitionList : SendDefinitionList { }
    public class ET_Trigger : TriggeredSend { }

    #endregion

    #region Tracking Events

    public class ET_OpenEvent : OpenEvent
    {
        public Boolean GetSinceLastBatch { get; set; }
        public ET_OpenEvent() { GetSinceLastBatch = true; }
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
    }

    public class ET_BounceEvent : BounceEvent
    {
        public Boolean GetSinceLastBatch { get; set; }
        public ET_BounceEvent() { GetSinceLastBatch = true; }
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
    }

    public class ET_ClickEvent : ClickEvent
    {
        public Boolean GetSinceLastBatch { get; set; }
        public ET_ClickEvent() { GetSinceLastBatch = true; }
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
    }

    public class ET_UnsubEvent : UnsubEvent
    {
        public Boolean GetSinceLastBatch { get; set; }
        public ET_UnsubEvent() { GetSinceLastBatch = true; }
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
    }

    public class ET_SentEvent : SentEvent
    {
        public Boolean GetSinceLastBatch { get; set; }
        public ET_SentEvent() { GetSinceLastBatch = true; }
        public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
        public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
        public InfoReturn Info() { return new InfoReturn(this); }
    }

    #endregion

    #region Campaign

    public class ET_Campaign : FuelObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CampaignCode { get; set; }
        public string Color { get; set; }
        public bool Favorite { get; set; }
        public ET_Campaign()
        {
            Endpoint = "https://www.exacttargetapis.com/hub/v1/campaigns/{ID}";
            URLProperties = new string[] { "ID" };
            RequiredURLProperties = new string[] { };
        }
        public ET_Campaign(JObject obj)
        {
            if (obj["id"] != null)
                ID = int.Parse(CleanRestValue(obj["id"]));
            if (obj["createdDate"] != null)
                CreatedDate = DateTime.Parse(CleanRestValue(obj["createdDate"]));
            if (obj["modifiedDate"] != null)
                ModifiedDate = DateTime.Parse(CleanRestValue(obj["modifiedDate"]));
            if (obj["name"] != null)
                Name = CleanRestValue(obj["name"]);
            if (obj["description"] != null)
                Description = CleanRestValue(obj["description"]);
            if (obj["campaignCode"] != null)
                CampaignCode = CleanRestValue(obj["campaignCode"]);
            if (obj["color"] != null)
                Color = CleanRestValue(obj["color"]);
            if (obj["favorite"] != null)
                Favorite = bool.Parse(CleanRestValue(obj["favorite"]));
        }
        public PostReturn Post() { return new PostReturn(this); }
        public DeleteReturn Delete() { return new DeleteReturn(this); }
        public GetReturn Get() { var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
        public GetReturn GetMoreResults() { Page++; var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
    }

    public class ET_CampaignAsset : FuelObject
    {
        public string Type { get; set; }
        public string CampaignID { get; set; }
        public string[] IDs { get; set; }
        public string ItemID { get; set; }
        public ET_CampaignAsset()
        {
            Endpoint = "https://www.exacttargetapis.com/hub/v1/campaigns/{CampaignID}/assets/{ID}";
            URLProperties = new string[] { "CampaignID", "ID" };
            RequiredURLProperties = new string[] { "CampaignID" };
        }
        public ET_CampaignAsset(JObject obj)
        {
            if (obj["id"] != null)
                ID = int.Parse(CleanRestValue(obj["id"]));
            if (obj["createdDate"] != null)
                CreatedDate = DateTime.Parse(CleanRestValue(obj["createdDate"]));
            if (obj["type"] != null)
                Type = CleanRestValue(obj["type"]);
            if (obj["campaignId"] != null)
                CampaignID = CleanRestValue(obj["campaignId"]);
            if (obj["itemID"] != null)
                ItemID = CleanRestValue(obj["itemID"]);
        }
        public PostReturn Post() { return new PostReturn(this); }
        public DeleteReturn Delete() { return new DeleteReturn(this); }
        public GetReturn Get() { var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
        public GetReturn GetMoreResults() { Page++; var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
    }

    public class ET_Endpoint : FuelObject
    {
        public string Type { get; set; }
        public string URL { get; set; }
        public ET_Endpoint()
        {
            Endpoint = "https://www.exacttargetapis.com/platform/v1/endpoints/{Type}";
            URLProperties = new string[] { "Type" };
            RequiredURLProperties = new string[] { };
        }
        public ET_Endpoint(JObject obj)
        {
            if (obj["type"] != null)
                Type = CleanRestValue(obj["type"]).ToString().Trim();
            if (obj["url"] != null)
                URL = CleanRestValue(obj["url"]);
        }
        public GetReturn Get() { var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
        public GetReturn GetMoreResults() { Page++; var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
    }

    #endregion
}