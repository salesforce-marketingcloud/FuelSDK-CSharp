using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ServiceModel;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.ServiceModel.Channels;



namespace FuelSDK
{
    public class ET_Client
    {
        //Variables
        public string authToken;
        public SoapClient soapclient;
        private string appSignature = string.Empty;
        private string clientId = string.Empty;
        private string clientSecret = string.Empty;
        private string soapEndPoint = string.Empty;
        private string sandbox = string.Empty;
        public string internalAuthToken = string.Empty;
        private string refreshKey = string.Empty;
        private DateTime authTokenExpiration = DateTime.Now;
        public string SDKVersion = "FuelSDX-C#-V.9";

        //Constructor
        public ET_Client(NameValueCollection parameters = null)
        {
            //Get configuration file and set variables
            if (File.Exists(@"FuelSDK_config.xml"))
            {
                System.Xml.XPath.XPathDocument doc = new System.Xml.XPath.XPathDocument(@"FuelSDK_config.xml");

                foreach (System.Xml.XPath.XPathNavigator child in doc.CreateNavigator().Select("configuration"))
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
                if (parameters.AllKeys.Contains("sandbox"))
                    sandbox = parameters["sandbox"];
            }

            if (clientId.Equals(string.Empty) || clientSecret.Equals(string.Empty))
                throw new Exception("clientId or clientSecret is null: Must be provided in config file or passed when instantiating ET_Client");

            //If JWT URL Parameter Used
            if (parameters != null && parameters.AllKeys.Contains("jwt"))
            {
                if (appSignature.Equals(string.Empty))
                    throw new Exception("Unable to utilize JWT for SSO without appSignature: Must be provided in config file or passed when instantiating ET_Client");
                string encodedJWT = parameters["jwt"].ToString().Trim();
                String decodedJWT = JsonWebToken.Decode(encodedJWT, appSignature);
                JObject parsedJWT = JObject.Parse(decodedJWT);
                authToken = parsedJWT["request"]["user"]["oauthToken"].Value<string>().Trim();
                authTokenExpiration = DateTime.Now.AddSeconds(int.Parse(parsedJWT["request"]["user"]["expiresIn"].Value<string>().Trim()));
                internalAuthToken = parsedJWT["request"]["user"]["internalOauthToken"].Value<string>().Trim();
                refreshKey = parsedJWT["request"]["user"]["refreshToken"].Value<string>().Trim();
            }
            else
            {
                refreshToken();
            }

            // Find the appropriate endpoint for the acccount
            ET_Endpoint getSingleEndpoint = new ET_Endpoint();
            getSingleEndpoint.AuthStub = this;
            getSingleEndpoint.Type = "soap";
            GetReturn grSingleEndpoint = getSingleEndpoint.Get();

            if (grSingleEndpoint.Status && grSingleEndpoint.Results.Length == 1)
            {
                soapEndPoint = ((ET_Endpoint)grSingleEndpoint.Results[0]).URL;
            }
            else
            {
                throw new Exception("Unable to determine stack using /platform/v1/endpoints: " + grSingleEndpoint.Message);
            }

            //Create the SOAP binding for call with Oauth.
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Name = "UserNameSoapBinding";
            binding.Security.Mode = BasicHttpSecurityMode.TransportWithMessageCredential;
            binding.MaxReceivedMessageSize = 2147483647;
            soapclient = new SoapClient(binding, new EndpointAddress(new Uri(soapEndPoint)));
            soapclient.ClientCredentials.UserName.UserName = "*";
            soapclient.ClientCredentials.UserName.Password = "*";

        }

        public void refreshToken(bool force = false)
        {
            //RefreshToken
            if ((authToken == null || authToken.Length == 0 || DateTime.Now.AddSeconds(300) > authTokenExpiration) || force)
            {
                string strURL = "https://auth.exacttargetapis.com/v1/requestToken?legacy=1";
                if (sandbox == "true")
                    strURL = "https://auth-test.exacttargetapis.com/v1/requestToken?legacy=1";                    
                

                //Build the request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL.Trim());
                request.Method = "POST";
                request.ContentType = "application/json";
                request.UserAgent = this.SDKVersion;

                string json;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {

                    if (refreshKey.Length > 0)
                        json = @"{""clientId"": """ + clientId + @""", ""clientSecret"": """ + clientSecret + @""", ""refreshToken"": """ + refreshKey + @""", ""scope"": ""cas:" + internalAuthToken + @""" , ""accessType"": ""offline""}";
                    else
                        json = @"{""clientId"": """ + clientId + @""", ""clientSecret"": """ + clientSecret + @""", ""accessType"": ""offline""}";
                    streamWriter.Write(json);
                }

                //Get the response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                //Parse the response
                JObject parsedResponse = JObject.Parse(responseFromServer);
                internalAuthToken = parsedResponse["legacyToken"].Value<string>().Trim();
                authToken = parsedResponse["accessToken"].Value<string>().Trim();
                authTokenExpiration = DateTime.Now.AddSeconds(int.Parse(parsedResponse["expiresIn"].Value<string>().Trim()));
                refreshKey = parsedResponse["refreshToken"].Value<string>().Trim();
            }
        }

        public FuelReturn AddSubscribersToList(string EmailAddress, string SubscriberKey, List<int> ListIDs)
        {
            return this.ProcessAddSubscriberToList(EmailAddress, SubscriberKey, ListIDs);
        }

        public FuelReturn AddSubscribersToList(string EmailAddress, List<int> ListIDs)
        {
            return this.ProcessAddSubscriberToList(EmailAddress, null, ListIDs);
        }

        protected FuelReturn ProcessAddSubscriberToList(string EmailAddress, string SubscriberKey, List<int> ListIDs)
        {
            ET_Subscriber sub = new ET_Subscriber();
            sub.EmailAddress = EmailAddress;
            if (SubscriberKey != null)
                sub.SubscriberKey = SubscriberKey;
            List<ET_SubscriberList> lLists = new List<ET_SubscriberList>();
            foreach (int listID in ListIDs)
            {
                ET_SubscriberList feList = new ET_SubscriberList();
                feList.ID = listID;
                lLists.Add(feList);
            }
            sub.AuthStub = this;
            sub.Lists = lLists.ToArray();
            PostReturn prAddSub = sub.Post();
            if (!prAddSub.Status && prAddSub.Results.Length > 0 && prAddSub.Results[0].ErrorCode == 12014)
            {
                return sub.Patch();
            }
            else
            {
                return prAddSub;
            }
        }

        public FuelReturn CreateDataExtensions(ET_DataExtension[] ArrayOfET_DataExtension)
        {

            List<ET_DataExtension> cleanedArray = new List<ET_DataExtension>();

            foreach (ET_DataExtension de in ArrayOfET_DataExtension)
            {
                de.Fields = de.Columns;
                de.Columns = null;
                cleanedArray.Add(de);
            }
            return new PostReturn(cleanedArray.ToArray(), this);
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
        public TaskResult Task {get;set; }        
    }

    public class PostReturn : FuelReturn
    {
        public ResultDetail[] Results { get; set; }

        public PostReturn(APIObject[] theObjects, ET_Client theClient)
        {
            this.Message = "";
            this.Status = true;
            this.MoreResults = false;
            string OverallStatus = string.Empty, RequestID = string.Empty;
            Result[] requestResults = new Result[0];

            theClient.refreshToken();
            using (var scope = new OperationContextScope(theClient.soapclient.InnerChannel))
            {
                //Add oAuth token to SOAP header.
                XNamespace ns = "http://exacttarget.com";
                var oauthElement = new XElement(ns + "oAuthToken", theClient.internalAuthToken);
                var xmlHeader = MessageHeader.CreateHeader("oAuth", "http://exacttarget.com", oauthElement);
                OperationContext.Current.OutgoingMessageHeaders.Add(xmlHeader);

                var httpRequest = new System.ServiceModel.Channels.HttpRequestMessageProperty();
                OperationContext.Current.OutgoingMessageProperties.Add(System.ServiceModel.Channels.HttpRequestMessageProperty.Name, httpRequest);
                httpRequest.Headers.Add(HttpRequestHeader.UserAgent, theClient.SDKVersion);

                List<APIObject> lObjects = new List<APIObject>();
                foreach (APIObject ao in theObjects)
                {
                    lObjects.Add(this.TranslateObject(ao));
                }

                requestResults = theClient.soapclient.Create(new CreateOptions(), lObjects.ToArray(), out RequestID, out OverallStatus);

                this.Status = true;
                this.Code = 200;
                this.MoreResults = false;
                this.Message = "";

                if (OverallStatus != "OK")
                {
                    this.Status = false;
                }

                if (requestResults.GetType() == typeof(CreateResult[]) && requestResults.Length > 0)
                {
                    List<ResultDetail> results = new List<ResultDetail>();
                    foreach (CreateResult cr in requestResults)
                    {
                        ResultDetail detail = new ResultDetail();
                        if (cr.StatusCode != null)
                            detail.StatusCode = cr.StatusCode;
                        if (cr.StatusMessage != null)
                            detail.StatusMessage = cr.StatusMessage;
                        if (cr.NewObjectID != null)
                            detail.NewObjectID = cr.NewObjectID;
                        if (cr.Object != null)
                            detail.Object = this.TranslateObject(cr.Object);
                        detail.OrdinalID = cr.OrdinalID;
                        detail.ErrorCode = cr.ErrorCode;
                        detail.NewID = cr.NewID;
                        results.Add(detail);
                    }
                    this.Results = results.ToArray();
                }
            }
        }


        public PostReturn(APIObject theObject)
        {
            this.Message = "";
            this.Status = true;
            this.MoreResults = false;
            string OverallStatus = string.Empty, RequestID = string.Empty;
            Result[] requestResults = new Result[0];

            theObject.AuthStub.refreshToken();
            using (var scope = new OperationContextScope(theObject.AuthStub.soapclient.InnerChannel))
            {
                //Add oAuth token to SOAP header.
                XNamespace ns = "http://exacttarget.com";
                var oauthElement = new XElement(ns + "oAuthToken", theObject.AuthStub.internalAuthToken);
                var xmlHeader = MessageHeader.CreateHeader("oAuth", "http://exacttarget.com", oauthElement);
                OperationContext.Current.OutgoingMessageHeaders.Add(xmlHeader);

                var httpRequest = new System.ServiceModel.Channels.HttpRequestMessageProperty();
                OperationContext.Current.OutgoingMessageProperties.Add(System.ServiceModel.Channels.HttpRequestMessageProperty.Name, httpRequest);
                httpRequest.Headers.Add(HttpRequestHeader.UserAgent, theObject.AuthStub.SDKVersion);

                theObject = this.TranslateObject(theObject);

                requestResults = theObject.AuthStub.soapclient.Create(new CreateOptions(), new APIObject[] { theObject }, out RequestID, out OverallStatus);

                this.Status = true;
                this.Code = 200;
                this.MoreResults = false;
                this.Message = "";

                if (OverallStatus != "OK")
                {
                    this.Status = false;
                }

                if (requestResults.GetType() == typeof(CreateResult[]) && requestResults.Length > 0)
                {
                    List<ResultDetail> results = new List<ResultDetail>();
                    foreach (CreateResult cr in requestResults)
                    {
                        ResultDetail detail = new ResultDetail();
                        if (cr.StatusCode != null)
                            detail.StatusCode = cr.StatusCode;
                        if (cr.StatusMessage != null)
                            detail.StatusMessage = cr.StatusMessage;
                        if (cr.NewObjectID != null)
                            detail.NewObjectID = cr.NewObjectID;
                        if (cr.Object != null)
                            detail.Object = this.TranslateObject(cr.Object);
                        detail.OrdinalID = cr.OrdinalID;
                        detail.ErrorCode = cr.ErrorCode;
                        detail.NewID = cr.NewID;
                        results.Add(detail);
                    }
                    this.Results = results.ToArray();
                }
            }
        }


        public PostReturn(FuelObject theObject)
        {
            this.Message = "";
            this.Status = true;
            this.MoreResults = false;
            string completeURL = theObject.Endpoint;
            string additionalQS;

            theObject.AuthStub.refreshToken();

            foreach (PropertyInfo prop in theObject.GetType().GetProperties())
            {
                if (theObject.URLProperties.Contains(prop.Name) && prop.GetValue(theObject, null) != null)
                    if (prop.GetValue(theObject, null).ToString().Trim() != "" && prop.GetValue(theObject, null).ToString().Trim() != "0")
                        completeURL = completeURL.Replace("{" + prop.Name + "}", prop.GetValue(theObject, null).ToString());
            }

            bool match;
            if (theObject.RequiredURLProperties != null)
            {
                foreach (string urlProp in theObject.RequiredURLProperties)
                {
                    match = false;

                    foreach (PropertyInfo prop in theObject.GetType().GetProperties())
                    {
                        if (theObject.URLProperties.Contains(prop.Name))
                            if (prop.GetValue(theObject, null) != null)
                            {
                                if (prop.GetValue(theObject, null).ToString().Trim() != "" && prop.GetValue(theObject, null).ToString().Trim() != "0")
                                    match = true;
                            }
                    }
                    if (match == false)
                        throw new Exception("Unable to process request due to missing required property: " + urlProp);

                }
            }

            // Clean up not required URL parameters
            int j = 0;
            if (theObject.URLProperties != null)
            {
                foreach (string urlProp in theObject.URLProperties)
                {
                    completeURL = completeURL.Replace("{" + urlProp + "}", "");
                    j++;
                }
            }

            additionalQS = "access_token=" + theObject.AuthStub.authToken;
            completeURL = completeURL + "?" + additionalQS;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(completeURL.Trim());
            request.Method = "POST";
            request.ContentType = "application/json";
            request.UserAgent = theObject.AuthStub.SDKVersion;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string jsonPayload = JsonConvert.SerializeObject(theObject);
                streamWriter.Write(jsonPayload);
            }

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();

                if (response != null)
                    this.Code = (int)response.StatusCode;
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        this.Status = true;
                        List<ResultDetail> AllResults = new List<ResultDetail>();

                        if (responseFromServer.ToString().StartsWith("["))
                        {
                            JArray jsonArray = JArray.Parse(responseFromServer.ToString());
                            foreach (JObject obj in jsonArray)
                            {
                                APIObject currentObject = (APIObject)Activator.CreateInstance(theObject.GetType(), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, new object[] { obj }, null);
                                ResultDetail result = new ResultDetail();
                                result.Object = currentObject;
                                AllResults.Add(result);
                            }

                            this.Results = AllResults.ToArray();
                        }
                        else
                        {
                            JObject jsonObject = JObject.Parse(responseFromServer.ToString());
                            ResultDetail result = new ResultDetail();
                            APIObject currentObject = (APIObject)Activator.CreateInstance(theObject.GetType(), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, new object[] { jsonObject }, null);
                            result.Object = currentObject;
                            AllResults.Add(result);
                            this.Results = AllResults.ToArray();
                        }


                    }
                    else
                    {
                        this.Status = false;
                        this.Message = response.ToString();
                    }
                }

                response.Close();
            }
            catch (WebException we)
            {
                this.Code = (int)((HttpWebResponse)we.Response).StatusCode;
                this.Status = false;
                this.Results = new ResultDetail[] { };
                using (var stream = we.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    Message = reader.ReadToEnd();
                }
            }


        }

    }

    public class SendReturn : PostReturn
    {
        public SendReturn(APIObject theObject)
            : base(theObject)
        {

        }
    }

    public class HelperReturn : PostReturn
    {
        public HelperReturn(APIObject theObject)
            : base(theObject)
        {

        }
    }

    public class PatchReturn : FuelReturn
    {
        public ResultDetail[] Results { get; set; }

        public PatchReturn(APIObject theObject)
        {
            string OverallStatus = string.Empty, RequestID = string.Empty;
            Result[] requestResults = new Result[0];

            theObject.AuthStub.refreshToken();
            using (var scope = new OperationContextScope(theObject.AuthStub.soapclient.InnerChannel))
            {
                //Add oAuth token to SOAP header.
                XNamespace ns = "http://exacttarget.com";
                var oauthElement = new XElement(ns + "oAuthToken", theObject.AuthStub.internalAuthToken);
                var xmlHeader = MessageHeader.CreateHeader("oAuth", "http://exacttarget.com", oauthElement);
                OperationContext.Current.OutgoingMessageHeaders.Add(xmlHeader);

                var httpRequest = new System.ServiceModel.Channels.HttpRequestMessageProperty();
                OperationContext.Current.OutgoingMessageProperties.Add(System.ServiceModel.Channels.HttpRequestMessageProperty.Name, httpRequest);
                httpRequest.Headers.Add(HttpRequestHeader.UserAgent, theObject.AuthStub.SDKVersion);

                theObject = this.TranslateObject(theObject);
                requestResults = theObject.AuthStub.soapclient.Update(new UpdateOptions(), new APIObject[] { theObject }, out RequestID, out OverallStatus);

                this.Status = true;
                this.Code = 200;
                this.MoreResults = false;
                this.Message = "";

                if (OverallStatus != "OK")
                {
                    this.Status = false;
                }

                if (requestResults.GetType() == typeof(UpdateResult[]) && requestResults.Length > 0)
                {
                    List<ResultDetail> results = new List<ResultDetail>();
                    foreach (UpdateResult cr in requestResults)
                    {
                        ResultDetail detail = new ResultDetail();
                        if (cr.StatusCode != null)
                            detail.StatusCode = cr.StatusCode;
                        if (cr.StatusMessage != null)
                            detail.StatusMessage = cr.StatusMessage;
                        if (cr.Object != null)
                            detail.Object = this.TranslateObject(cr.Object);
                        detail.OrdinalID = cr.OrdinalID;
                        detail.ErrorCode = cr.ErrorCode;
                        results.Add(detail);
                    }
                    this.Results = results.ToArray();
                }
            }
        }
    }

    public class DeleteReturn : FuelReturn
    {
        public ResultDetail[] Results { get; set; }

        public DeleteReturn(APIObject theObject)
        {
            string OverallStatus = string.Empty, RequestID = string.Empty;
            Result[] requestResults = new Result[0];

            theObject.AuthStub.refreshToken();
            using (var scope = new OperationContextScope(theObject.AuthStub.soapclient.InnerChannel))
            {
                //Add oAuth token to SOAP header.
                XNamespace ns = "http://exacttarget.com";
                var oauthElement = new XElement(ns + "oAuthToken", theObject.AuthStub.internalAuthToken);
                var xmlHeader = MessageHeader.CreateHeader("oAuth", "http://exacttarget.com", oauthElement);
                OperationContext.Current.OutgoingMessageHeaders.Add(xmlHeader);

                var httpRequest = new System.ServiceModel.Channels.HttpRequestMessageProperty();
                OperationContext.Current.OutgoingMessageProperties.Add(System.ServiceModel.Channels.HttpRequestMessageProperty.Name, httpRequest);
                httpRequest.Headers.Add(HttpRequestHeader.UserAgent, theObject.AuthStub.SDKVersion);
                theObject = this.TranslateObject(theObject);
                requestResults = theObject.AuthStub.soapclient.Delete(new DeleteOptions(), new APIObject[] { theObject }, out RequestID, out OverallStatus);

                this.Status = true;
                this.Code = 200;
                this.MoreResults = false;
                this.Message = "";

                if (OverallStatus != "OK")
                {
                    this.Status = false;
                }

                if (requestResults.GetType() == typeof(DeleteResult[]) && requestResults.Length > 0)
                {
                    List<ResultDetail> results = new List<ResultDetail>();
                    foreach (DeleteResult cr in requestResults)
                    {
                        ResultDetail detail = new ResultDetail();
                        if (cr.StatusCode != null)
                            detail.StatusCode = cr.StatusCode;
                        if (cr.StatusMessage != null)
                            detail.StatusMessage = cr.StatusMessage;
                        if (cr.Object != null)
                            detail.Object = this.TranslateObject(cr.Object);
                        detail.OrdinalID = cr.OrdinalID;
                        detail.ErrorCode = cr.ErrorCode;
                        results.Add(detail);
                    }
                    this.Results = results.ToArray();
                }
            }
        }

        public DeleteReturn(FuelObject theObject)
        {

            this.Message = "";
            this.Status = true;
            this.MoreResults = false;
            this.Results = new ResultDetail[] { };


            theObject.AuthStub.refreshToken();

            string completeURL = theObject.Endpoint;
            string additionalQS;

            // All URL Props are required when doing Delete	
            bool match;
            if (theObject.URLProperties != null)
            {
                foreach (string urlProp in theObject.URLProperties)
                {
                    match = false;
                    if (theObject != null)
                    {
                        foreach (PropertyInfo prop in theObject.GetType().GetProperties())
                        {
                            if (theObject.URLProperties.Contains(prop.Name) && prop.GetValue(theObject, null) != null)
                                if (prop.GetValue(theObject, null).ToString().Trim() != "" && prop.GetValue(theObject, null).ToString().Trim() != "0")
                                    match = true;
                        }
                        if (match == false)
                            throw new Exception("Unable to process request due to missing required prop: " + urlProp);
                    }
                    else
                        throw new Exception("Unable to process request due to missing required prop: " + urlProp);
                }
            }

            if (theObject != null)
            {
                foreach (PropertyInfo prop in theObject.GetType().GetProperties())
                {
                    if (theObject.URLProperties.Contains(prop.Name) && prop.GetValue(theObject, null) != null)
                        if (prop.GetValue(theObject, null).ToString().Trim() != "" && prop.GetValue(theObject, null).ToString().Trim() != "0")
                            completeURL = completeURL.Replace("{" + prop.Name + "}", prop.GetValue(theObject, null).ToString());
                }
            }

            additionalQS = "access_token=" + theObject.AuthStub.authToken;
            completeURL = completeURL + "?" + additionalQS;
            restDelete(theObject, completeURL);
        }




        private void restDelete(FuelObject theObject, string url)
        {
            //Build the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url.Trim());
            request.Method = "DELETE";
            request.ContentType = "application/json";
            request.UserAgent = theObject.AuthStub.SDKVersion;

            //Get the response
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();

                if (response != null)
                    this.Code = (int)response.StatusCode;
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        this.Status = true;
                    }
                    else
                    {
                        this.Status = false;
                        this.Message = response.ToString();
                    }
                }

                response.Close();
            }
            catch (WebException we)
            {
                this.Code = (int)((HttpWebResponse)we.Response).StatusCode;
                this.Status = false;
                using (var stream = we.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    this.Message = reader.ReadToEnd();
                }
            }
        }
    }


    public class PerformReturn : FuelReturn
    {
        public ResultDetail[] Results { get; set; }

        public PerformReturn(APIObject theObject, string PerformAction)
        {
            string OverallStatus = string.Empty, RequestID = string.Empty, OverallStatusMessage = string.Empty;
            Result[] requestResults = new Result[0];

            theObject.AuthStub.refreshToken();
            using (var scope = new OperationContextScope(theObject.AuthStub.soapclient.InnerChannel))
            {
                //Add oAuth token to SOAP header.
                XNamespace ns = "http://exacttarget.com";
                var oauthElement = new XElement(ns + "oAuthToken", theObject.AuthStub.internalAuthToken);
                var xmlHeader = MessageHeader.CreateHeader("oAuth", "http://exacttarget.com", oauthElement);
                OperationContext.Current.OutgoingMessageHeaders.Add(xmlHeader);

                var httpRequest = new System.ServiceModel.Channels.HttpRequestMessageProperty();
                OperationContext.Current.OutgoingMessageProperties.Add(System.ServiceModel.Channels.HttpRequestMessageProperty.Name, httpRequest);
                httpRequest.Headers.Add(HttpRequestHeader.UserAgent, theObject.AuthStub.SDKVersion);
                theObject = this.TranslateObject(theObject);
                requestResults = theObject.AuthStub.soapclient.Perform(new PerformOptions(), PerformAction, new APIObject[] { theObject }, out OverallStatus, out OverallStatusMessage, out RequestID);
                    

                this.Status = true;
                this.Code = 200;
                this.MoreResults = false;
                this.Message = OverallStatusMessage;

                if (OverallStatus != "OK")
                {
                    this.Status = false;
                }

                if (requestResults.GetType() == typeof(PerformResult[]) && requestResults.Length > 0)
                {
                    List<ResultDetail> results = new List<ResultDetail>();
                    foreach (PerformResult cr in requestResults)
                    {
                        ResultDetail detail = new ResultDetail();
                        if (cr.StatusCode != null)
                            detail.StatusCode = cr.StatusCode;
                        if (cr.StatusMessage != null)
                            detail.StatusMessage = cr.StatusMessage;
                        if (cr.Object != null)
                            detail.Object = this.TranslateObject(cr.Object);
                        if (cr.Task != null)
                            detail.Task = cr.Task;
                        detail.OrdinalID = cr.OrdinalID;
                        detail.ErrorCode = cr.ErrorCode;
                        results.Add(detail);
                    }
                    this.Results = results.ToArray();
                }
            }
        }
    }


    public class GetReturn : FuelReturn
    {
        public int LastPageNumber { get; set; }

        public APIObject[] Results { get; set; }

        public GetReturn(APIObject theObject) : this(theObject, false, null) { }
        public GetReturn(APIObject theObject, Boolean Continue, String OverrideObjectType)
        {
            string OverallStatus = string.Empty, RequestID = string.Empty;
            APIObject[] objectResults = new APIObject[0];
            theObject.AuthStub.refreshToken();
            this.Results = new APIObject[0];
            using (var scope = new OperationContextScope(theObject.AuthStub.soapclient.InnerChannel))
            {
                //Add oAuth token to SOAP header.
                XNamespace ns = "http://exacttarget.com";
                var oauthElement = new XElement(ns + "oAuthToken", theObject.AuthStub.internalAuthToken);
                var xmlHeader = MessageHeader.CreateHeader("oAuth", "http://exacttarget.com", oauthElement);
                OperationContext.Current.OutgoingMessageHeaders.Add(xmlHeader);

                var httpRequest = new System.ServiceModel.Channels.HttpRequestMessageProperty();
                OperationContext.Current.OutgoingMessageProperties.Add(System.ServiceModel.Channels.HttpRequestMessageProperty.Name, httpRequest);
                httpRequest.Headers.Add(HttpRequestHeader.UserAgent, theObject.AuthStub.SDKVersion);

                RetrieveRequest rr = new RetrieveRequest();

                // Handle RetrieveAllSinceLastBatch
                if (theObject.GetType().GetProperty("GetSinceLastBatch") != null)
                {
                    rr.RetrieveAllSinceLastBatch = (Boolean)theObject.GetType().GetProperty("GetSinceLastBatch").GetValue(theObject, null);
                }


                if (Continue)
                {
                    if (theObject.LastRequestID == null)
                    {
                        throw new Exception("Unable to call GetMoreResults without first making successful Get() request");
                    }
                    rr.ContinueRequest = theObject.LastRequestID;
                }
                else
                {
                    if (theObject.SearchFilter != null)
                    {
                        rr.Filter = theObject.SearchFilter;
                    }

                    // Use the name from the object passed in unless an override is passed (Used for DataExtensionObject)
                    if (OverrideObjectType == null)
                        rr.ObjectType = this.TranslateObject(theObject).GetType().ToString().Replace("FuelSDK.", "");
                    else
                        rr.ObjectType = OverrideObjectType;

                    //If they didn't specify Props then we look them up using Info()
                    if (theObject.Props == null && theObject.GetType().GetMethod("Info") != null)
                    {
                        InfoReturn ir = new InfoReturn(theObject);
                        List<string> lProps = new List<string>();
                        if (ir.Status)
                        {
                            foreach (ET_PropertyDefinition pd in ir.Results)
                            {
                                if (pd.IsRetrievable)
                                    lProps.Add(pd.Name);
                            }
                        }
                        else
                        {
                            throw new Exception("Unable to find properties for object in order to perform Get() request");
                        }
                        rr.Properties = lProps.ToArray();
                    }
                    else
                        rr.Properties = theObject.Props;
                }
                OverallStatus = theObject.AuthStub.soapclient.Retrieve(rr, out RequestID, out objectResults);

                this.RequestID = RequestID;

                if (objectResults.Length > 0)
                {
                    List<APIObject> cleanedObjectResults = new List<APIObject>();
                    foreach (APIObject obj in objectResults)
                    {
                        cleanedObjectResults.Add(this.TranslateObject(obj));
                    }
                    this.Results = cleanedObjectResults.ToArray();
                }

                this.Status = true;
                this.Code = 200;
                this.MoreResults = false;
                this.Message = "";

                if (OverallStatus != "OK" && OverallStatus != "MoreDataAvailable")
                {
                    this.Status = false;
                    this.Message = OverallStatus;
                }
                else if (OverallStatus == "MoreDataAvailable")
                {
                    this.MoreResults = true;
                }
            }
        }

        public GetReturn(FuelObject theObject)
        {
            this.Message = "";
            this.Status = true;
            this.MoreResults = false;
            this.Results = new APIObject[] { };

            theObject.AuthStub.refreshToken();

            string completeURL = theObject.Endpoint;
            string additionalQS = "";
            bool boolAdditionalQS = false;

            if (theObject != null)
            {
                foreach (PropertyInfo prop in theObject.GetType().GetProperties())
                {
                    if (theObject.URLProperties.Contains(prop.Name) && prop.GetValue(theObject, null) != null)
                        if (prop.GetValue(theObject, null).ToString().Trim() != "" && prop.GetValue(theObject, null).ToString().Trim() != "0")
                            completeURL = completeURL.Replace("{" + prop.Name + "}", prop.GetValue(theObject, null).ToString());
                }
            }

            ////props code for paging
            if (theObject.Page != 0)
            {
                additionalQS += "$page=" + theObject.Page;
                boolAdditionalQS = true;
            }

            bool match;
            if (theObject.RequiredURLProperties != null)
            {
                foreach (string urlProp in theObject.RequiredURLProperties)
                {
                    match = false;
                    if (theObject != null)
                    {
                        foreach (PropertyInfo prop in theObject.GetType().GetProperties())
                        {
                            if (theObject.URLProperties.Contains(prop.Name) && prop.GetValue(theObject, null) != null)
                                if (prop.GetValue(theObject, null).ToString().Trim() != "" && prop.GetValue(theObject, null).ToString().Trim() != "0")
                                    match = true;
                        }

                        if (match == false)
                            throw new Exception("Unable to process request due to missing required prop: " + urlProp);
                    }
                    else
                        throw new Exception("Unable to process request due to missing required prop: " + urlProp);
                }
            }

            //Clean up not required URL parameters
            int j = 0;
            if (theObject.URLProperties != null)
            {
                foreach (string urlProp in theObject.URLProperties)
                {
                    completeURL = completeURL.Replace("{" + urlProp + "}", "");
                    j++;
                }
            }

            if (!boolAdditionalQS)
                additionalQS += "access_token=" + theObject.AuthStub.authToken;
            else
                additionalQS += "&access_token=" + theObject.AuthStub.authToken;

            completeURL = completeURL + "?" + additionalQS;
            restGet(ref theObject, completeURL);
        }

        private void restGet(ref FuelObject theObject, string url)
        {
            //Build the request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url.Trim());
            request.Method = "GET";
            request.ContentType = "application/json";
            request.UserAgent = theObject.AuthStub.SDKVersion;

            //Get the response
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();

                if (response != null)
                    this.Code = (int)response.StatusCode;
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        this.Status = true;
                        if (responseFromServer != null)
                        {
                            JObject parsedResponse = JObject.Parse(responseFromServer);
                            //Check on the paging information from response
                            if (parsedResponse["page"] != null)
                            {
                                this.LastPageNumber = int.Parse(parsedResponse["page"].Value<string>().Trim());
                                int pageSize = int.Parse(parsedResponse["pageSize"].Value<string>().Trim());

                                int count = -1;
                                if (parsedResponse["count"] != null)
                                {
                                    count = int.Parse(parsedResponse["count"].Value<string>().Trim());
                                }
                                else if (parsedResponse["totalCount"] != null)
                                {
                                    count = int.Parse(parsedResponse["totalCount"].Value<string>().Trim());
                                }

                                if (count != -1 && (count > (this.LastPageNumber * pageSize)))
                                {
                                    this.MoreResults = true;
                                }
                            }

                            APIObject[] getResults = new APIObject[] { };

                            if (parsedResponse["items"] != null)
                                getResults = processResults(parsedResponse["items"].ToString().Trim(), theObject.GetType());
                            else if (parsedResponse["entities"] != null)
                                getResults = processResults(parsedResponse["entities"].ToString().Trim(), theObject.GetType());
                            else
                                getResults = processResults(responseFromServer.Trim(), theObject.GetType());

                            this.Results = getResults.ToArray();
                        }
                    }
                    else
                    {
                        this.Status = false;
                        this.Message = response.ToString();
                    }
                }
                response.Close();
            }
            catch (WebException we)
            {
                this.Code = (int)((HttpWebResponse)we.Response).StatusCode;
                this.Status = false;
                using (var stream = we.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    this.Message = reader.ReadToEnd();
                }
            }
        }

        protected APIObject[] processResults(string restResponse, Type fuelType)
        {
            List<APIObject> allObjects = new System.Collections.Generic.List<APIObject>();

            if (restResponse != null)
            {
                if (JsonConvert.DeserializeObject(restResponse.ToString()) != null && JsonConvert.DeserializeObject(restResponse.ToString()).ToString() != "")
                {
                    if (restResponse.ToString().StartsWith("["))
                    {
                        JArray jsonArray = JArray.Parse(restResponse.ToString());
                        foreach (JObject obj in jsonArray)
                        {
                            APIObject currentObject = (APIObject)Activator.CreateInstance(fuelType, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, new object[] { obj }, null);
                            allObjects.Add(currentObject);
                        }

                        return allObjects.ToArray();
                    }
                    else
                    {
                        JObject jsonObject = JObject.Parse(restResponse.ToString());
                        ResultDetail result = new ResultDetail();
                        APIObject currentObject = (APIObject)Activator.CreateInstance(fuelType, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, null, new object[] { jsonObject }, null);
                        allObjects.Add(currentObject);
                        return allObjects.ToArray();
                    }
                }
                else
                    return allObjects.ToArray();
            }
            else
                return allObjects.ToArray();
        }
    }

    public class InfoReturn : FuelReturn
    {
        public ET_PropertyDefinition[] Results { get; set; }
        public InfoReturn(APIObject theObject)
        {
            string RequestID = string.Empty;
            theObject.AuthStub.refreshToken();
            this.Results = new ET_PropertyDefinition[0];
            using (var scope = new OperationContextScope(theObject.AuthStub.soapclient.InnerChannel))
            {
                //Add oAuth token to SOAP header.
                XNamespace ns = "http://exacttarget.com";
                var oauthElement = new XElement(ns + "oAuthToken", theObject.AuthStub.internalAuthToken);
                var xmlHeader = MessageHeader.CreateHeader("oAuth", "http://exacttarget.com", oauthElement);
                OperationContext.Current.OutgoingMessageHeaders.Add(xmlHeader);

                var httpRequest = new System.ServiceModel.Channels.HttpRequestMessageProperty();
                OperationContext.Current.OutgoingMessageProperties.Add(System.ServiceModel.Channels.HttpRequestMessageProperty.Name, httpRequest);
                httpRequest.Headers.Add(HttpRequestHeader.UserAgent, theObject.AuthStub.SDKVersion);

                ObjectDefinitionRequest odr = new ObjectDefinitionRequest();
                odr.ObjectType = this.TranslateObject(theObject).GetType().ToString().Replace("FuelSDK.", "");
                ObjectDefinition[] definitionResults = theObject.AuthStub.soapclient.Describe(new ObjectDefinitionRequest[] { odr }, out RequestID);

                this.RequestID = RequestID;
                this.Status = true;
                this.Code = 200;
                this.MoreResults = false;
                this.Message = "";


                if (definitionResults.Length > 0)
                {

                    List<ET_PropertyDefinition> cleanedObjectResults = new List<ET_PropertyDefinition>();
                    foreach (PropertyDefinition obj in definitionResults[0].Properties)
                    {
                        cleanedObjectResults.Add((ET_PropertyDefinition)(this.TranslateObject(obj)));
                    }
                    this.Results = cleanedObjectResults.ToArray();
                }
                else
                {
                    this.Status = false;
                }
            }
        }
    }

    public abstract class FuelReturn
    {
        public Boolean Status { get; set; }
        public String Message { get; set; }
        public Boolean MoreResults { get; set; }
        public int Code { get; set; }
        public string RequestID { get; set; }

        private Dictionary<Type, Type> translator = new Dictionary<Type, Type>();

        public FuelReturn()
        {
            translator.Add(typeof(ET_Folder), typeof(DataFolder));
            translator.Add(typeof(DataFolder), typeof(ET_Folder));

            translator.Add(typeof(ET_List), typeof(List));
            translator.Add(typeof(List), typeof(ET_List));

            translator.Add(typeof(ET_ContentArea), typeof(ContentArea));
            translator.Add(typeof(ContentArea), typeof(ET_ContentArea));

            translator.Add(typeof(ET_ObjectDefinition), typeof(ObjectDefinition));
            translator.Add(typeof(ObjectDefinition), typeof(ET_ObjectDefinition));

            translator.Add(typeof(ET_PropertyDefinition), typeof(PropertyDefinition));
            translator.Add(typeof(PropertyDefinition), typeof(ET_PropertyDefinition));

            translator.Add(typeof(Subscriber), typeof(ET_Subscriber));
            translator.Add(typeof(ET_Subscriber), typeof(Subscriber));

            translator.Add(typeof(ET_ProfileAttribute), typeof(FuelSDK.Attribute));
            translator.Add(typeof(FuelSDK.Attribute), typeof(ET_ProfileAttribute));

            translator.Add(typeof(ET_Email), typeof(FuelSDK.Email));
            translator.Add(typeof(FuelSDK.Email), typeof(ET_Email));

            translator.Add(typeof(ET_SubscriberList), typeof(FuelSDK.SubscriberList));
            translator.Add(typeof(FuelSDK.SubscriberList), typeof(ET_SubscriberList));

            translator.Add(typeof(ET_List_Subscriber), typeof(FuelSDK.ListSubscriber));
            translator.Add(typeof(FuelSDK.ListSubscriber), typeof(ET_List_Subscriber));

            translator.Add(typeof(ET_DataExtension), typeof(FuelSDK.DataExtension));
            translator.Add(typeof(FuelSDK.DataExtension), typeof(ET_DataExtension));

            translator.Add(typeof(ET_DataExtensionColumn), typeof(FuelSDK.DataExtensionField));
            translator.Add(typeof(FuelSDK.DataExtensionField), typeof(ET_DataExtensionColumn));

            translator.Add(typeof(ET_DataExtensionRow), typeof(FuelSDK.DataExtensionObject));
            translator.Add(typeof(FuelSDK.DataExtensionObject), typeof(ET_DataExtensionRow));

            translator.Add(typeof(ET_SendClassification), typeof(FuelSDK.SendClassification));
            translator.Add(typeof(FuelSDK.SendClassification), typeof(ET_SendClassification));

            translator.Add(typeof(ET_SendDefinitionList), typeof(FuelSDK.SendDefinitionList));
            translator.Add(typeof(FuelSDK.SendDefinitionList), typeof(ET_SendDefinitionList));

            translator.Add(typeof(ET_SenderProfile), typeof(FuelSDK.SenderProfile));
            translator.Add(typeof(FuelSDK.SenderProfile), typeof(ET_SenderProfile));

            translator.Add(typeof(ET_DeliveryProfile), typeof(FuelSDK.DeliveryProfile));
            translator.Add(typeof(FuelSDK.DeliveryProfile), typeof(ET_DeliveryProfile));

            translator.Add(typeof(ET_TriggeredSend), typeof(FuelSDK.TriggeredSendDefinition));
            translator.Add(typeof(FuelSDK.TriggeredSendDefinition), typeof(ET_TriggeredSend));

            translator.Add(typeof(ET_EmailSendDefinition), typeof(FuelSDK.EmailSendDefinition));
            translator.Add(typeof(FuelSDK.EmailSendDefinition), typeof(ET_EmailSendDefinition));

            translator.Add(typeof(ET_Send), typeof(FuelSDK.Send));
            translator.Add(typeof(FuelSDK.Send), typeof(ET_Send));

            translator.Add(typeof(ET_Import), typeof(FuelSDK.ImportDefinition));
            translator.Add(typeof(FuelSDK.ImportDefinition), typeof(ET_Import));

            translator.Add(typeof(ET_ImportResult), typeof(FuelSDK.ImportResultsSummary));
            translator.Add(typeof(FuelSDK.ImportResultsSummary), typeof(ET_ImportResult));

            // The translation for this is handled in the Get() method for DataExtensionObject so no need to translate it
            translator.Add(typeof(APIProperty), typeof(APIProperty));

            translator.Add(typeof(ET_Trigger), typeof(FuelSDK.TriggeredSend));
            translator.Add(typeof(FuelSDK.TriggeredSend), typeof(ET_Trigger));

            // Tracking Events
            translator.Add(typeof(ET_BounceEvent), typeof(BounceEvent));
            translator.Add(typeof(BounceEvent), typeof(ET_BounceEvent));
            translator.Add(typeof(OpenEvent), typeof(ET_OpenEvent));
            translator.Add(typeof(ET_OpenEvent), typeof(OpenEvent));
            translator.Add(typeof(ET_ClickEvent), typeof(ClickEvent));
            translator.Add(typeof(ClickEvent), typeof(ET_ClickEvent));
            translator.Add(typeof(ET_UnsubEvent), typeof(UnsubEvent));
            translator.Add(typeof(UnsubEvent), typeof(ET_UnsubEvent));
            translator.Add(typeof(ET_SentEvent), typeof(SentEvent));
            translator.Add(typeof(SentEvent), typeof(ET_SentEvent));

        }


        public APIObject TranslateObject(APIObject inputObject)
        {

            if (this.translator.ContainsKey(inputObject.GetType()))
            {
                APIObject returnObject = (APIObject)Activator.CreateInstance(translator[inputObject.GetType()]);

                foreach (PropertyInfo prop in inputObject.GetType().GetProperties())
                {
                    if ((prop.PropertyType.IsSubclassOf(typeof(APIObject)) || prop.PropertyType == typeof(APIObject)) && prop.GetValue(inputObject, null) != null)
                    {
                        prop.SetValue(returnObject, this.TranslateObject(prop.GetValue(inputObject, null)), null);
                    }
                    else if (translator.ContainsKey(prop.PropertyType) && prop.GetValue(inputObject, null) != null)
                    {
                        prop.SetValue(returnObject, this.TranslateObject(prop.GetValue(inputObject, null)), null);
                    }
                    else if (prop.PropertyType.IsArray && prop.GetValue(inputObject, null) != null)
                    {
                        Array a = (Array)prop.GetValue(inputObject, null);
                        Array outArray;

                        if (a.Length > 0)
                        {
                            if (translator.ContainsKey(a.GetValue(0).GetType()))
                            {
                                outArray = Array.CreateInstance(translator[a.GetValue(0).GetType()], a.Length);

                                for (int i = 0; i < a.Length; i++)
                                {
                                    if (translator.ContainsKey(a.GetValue(i).GetType()))
                                    {
                                        outArray.SetValue(TranslateObject(a.GetValue(i)), i);
                                    }
                                }
                                if (outArray.Length > 0)
                                {
                                    prop.SetValue(returnObject, outArray, null);
                                }
                            }
                        }
                    }
                    else if (prop.Name == "FolderID" && prop.GetValue(inputObject, null) != null)
                    {
                        if (inputObject.GetType().GetProperty("Category") != null)
                        {
                            PropertyInfo categoryIDProp = inputObject.GetType().GetProperty("Category");
                            categoryIDProp.SetValue(returnObject, prop.GetValue(inputObject, null), null);
                        }
                        else if (inputObject.GetType().GetProperty("CategoryID") != null)
                        {
                            PropertyInfo categoryIDProp = inputObject.GetType().GetProperty("CategoryID");
                            categoryIDProp.SetValue(returnObject, prop.GetValue(inputObject, null), null);
                        }
                    }
                    else if ((prop.Name == "CategoryIDSpecified" || prop.Name == "CategorySpecified") && prop.GetValue(inputObject, null) != null)
                    {
                        // Do nothing
                    }
                    else if ((prop.Name == "CategoryID" || prop.Name == "Category") && prop.GetValue(inputObject, null) != null)
                    {
                        if (returnObject.GetType().GetProperty("FolderID") != null)
                        {
                            PropertyInfo folderIDProp = returnObject.GetType().GetProperty("FolderID");
                            folderIDProp.SetValue(returnObject, Convert.ToInt32(prop.GetValue(inputObject, null)), null);

                        }
                    }
                    else if (prop.GetValue(inputObject, null) != null && returnObject.GetType().GetProperty(prop.Name) != null)
                    {
                        prop.SetValue(returnObject, prop.GetValue(inputObject, null), null);
                    }

                }
                return returnObject;

            }
            else
            {
                return inputObject;
            }
        }

        protected object TranslateObject(object inputObject)
        {

            if (this.translator.ContainsKey(inputObject.GetType()))
            {
                object returnObject = (object)Activator.CreateInstance(translator[inputObject.GetType()]);
                foreach (PropertyInfo prop in inputObject.GetType().GetProperties())
                {
                    if (prop.GetValue(inputObject, null) != null && returnObject.GetType().GetProperty(prop.Name) != null)
                    {
                        prop.SetValue(returnObject, prop.GetValue(inputObject, null), null);
                    }
                }
                return returnObject;

            }
            else
            {
                return inputObject;
            }
        }
    }

    // Subscriber Related Objects
    public class ET_Subscriber : FuelSDK.Subscriber
    {
        public FuelSDK.PostReturn Post()
        {
            return new FuelSDK.PostReturn(this);
        }
        public FuelSDK.PatchReturn Patch()
        {
            return new FuelSDK.PatchReturn(this);
        }
        public FuelSDK.DeleteReturn Delete()
        {
            return new FuelSDK.DeleteReturn(this);
        }
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }

        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }
    }

    public class ET_ProfileAttribute : FuelSDK.Attribute { }
    public class ET_ImportResult : FuelSDK.ImportResultsSummary { }    
    public class ET_SubscriberList : FuelSDK.SubscriberList { }

    public class ET_List : List
    {
        public int? FolderID { get; set; }
        internal string FolderMediaType = "list";
        public FuelSDK.PostReturn Post()
        {
            return new FuelSDK.PostReturn(this);
        }
        public FuelSDK.PatchReturn Patch()
        {
            return new FuelSDK.PatchReturn(this);
        }
        public FuelSDK.DeleteReturn Delete()
        {
            return new FuelSDK.DeleteReturn(this);
        }
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }
    }

    public class ET_Send : Send
    {               
        public FuelSDK.PostReturn Post()
        {
            return new FuelSDK.PostReturn(this);
        }
        public FuelSDK.PatchReturn Patch()
        {
            return new FuelSDK.PatchReturn(this);
        }
        public FuelSDK.DeleteReturn Delete()
        {
            return new FuelSDK.DeleteReturn(this);
        }
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }
    }

    public class ET_List_Subscriber : ListSubscriber
    {
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }
    }

    //Content Related

    public class ET_ContentArea : ContentArea
    {
        public int? FolderID { get; set; }
        internal string FolderMediaType = "content";
        public FuelSDK.PostReturn Post()
        {
            return new FuelSDK.PostReturn(this);
        }
        public FuelSDK.PatchReturn Patch()
        {
            return new FuelSDK.PatchReturn(this);
        }
        public FuelSDK.DeleteReturn Delete()
        {
            return new FuelSDK.DeleteReturn(this);
        }
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }
    }

    public class ET_Email : Email
    {
        public int? FolderID { get; set; }
        internal string FolderMediaType = "email";
        public FuelSDK.PostReturn Post()
        {
            return new FuelSDK.PostReturn(this);
        }
        public FuelSDK.PatchReturn Patch()
        {
            return new FuelSDK.PatchReturn(this);
        }
        public FuelSDK.DeleteReturn Delete()
        {
            return new FuelSDK.DeleteReturn(this);
        }
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }
    }

    public class ET_EmailSendDefinition : EmailSendDefinition
    {
        public int? FolderID { get; set; }
        internal string FolderMediaType = "userinitiatedsends";
        internal string LastTaskID = string.Empty;
        public FuelSDK.PostReturn Post()
        {
            return new FuelSDK.PostReturn(this);
        }
        public FuelSDK.PatchReturn Patch()
        {
            return new FuelSDK.PatchReturn(this);
        }
        public FuelSDK.DeleteReturn Delete()
        {
            return new FuelSDK.DeleteReturn(this);
        }
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }

        public FuelSDK.PerformReturn Send()
        {
            PerformReturn thisPerformResult = new FuelSDK.PerformReturn(this, "start");
            if (thisPerformResult.Results.Length == 1)
            {
                this.LastTaskID = ((ResultDetail)thisPerformResult.Results[0]).Task.ID;
            }
            return thisPerformResult;
        }

        public FuelSDK.GetReturn Status()
        {
            if (LastTaskID == string.Empty)
            {
                throw new Exception("No ID available in order to return status for ET_EmailSendDefinition");
            }
            else
            {
                ET_Send thisSend = new ET_Send();
                thisSend.AuthStub = this.AuthStub;
                thisSend.SearchFilter = new SimpleFilterPart() { Value = new string[] { LastTaskID.ToString() }, Property = "ID", SimpleOperator = SimpleOperators.equals };
                FuelSDK.GetReturn response = new GetReturn(thisSend);
                this.LastRequestID = response.RequestID;
                return response;
            }
        }        
    }

    public class ET_Import : ImportDefinition
    {
        internal string LastTaskID = string.Empty;
        public FuelSDK.PostReturn Post()
        {
            return new FuelSDK.PostReturn(this);
        }
        public FuelSDK.PatchReturn Patch()
        {
            return new FuelSDK.PatchReturn(this);
        }
        public FuelSDK.DeleteReturn Delete()
        {
            return new FuelSDK.DeleteReturn(this);
        }
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }

        public FuelSDK.GetReturn Status()
        {
            if (LastTaskID == string.Empty)
            {
                throw new Exception("No ID available in order to return status for ET_Import");
            }
            else
            {
                ET_ImportResult thisSend = new ET_ImportResult();
                thisSend.AuthStub = this.AuthStub;
                thisSend.Props = new string[] { "ImportDefinitionCustomerKey", "TaskResultID", "ImportStatus", "StartDate", "EndDate", "DestinationID", "NumberSuccessful", "NumberDuplicated", "NumberErrors", "TotalRows", "ImportType" };
                thisSend.SearchFilter = new SimpleFilterPart() { Value = new string[] { LastTaskID.ToString() }, Property = "TaskResultID", SimpleOperator = SimpleOperators.equals };
                FuelSDK.GetReturn response = new GetReturn(thisSend);
                this.LastRequestID = response.RequestID;
                return response;
            }
        }

        public FuelSDK.PerformReturn Start()
        {
            PerformReturn thisPerformResult = new FuelSDK.PerformReturn(this, "start");
            if (thisPerformResult.Results.Length == 1)
            {
                this.LastTaskID = ((ResultDetail)thisPerformResult.Results[0]).Task.ID;
            }
            return thisPerformResult;
        }
    }


    // Data Extension Objects 

    public class ET_DataExtension : DataExtension
    {
        public int? FolderID { get; set; }
        internal string FolderMediaType = "dataextension";
        public ET_DataExtensionColumn[] Columns { get; set; }
        public FuelSDK.PostReturn Post()
        {
            ET_DataExtension tempDE = this;
            tempDE.Fields = this.Columns;
            tempDE.Columns = null;
            PostReturn tempPR = new FuelSDK.PostReturn(tempDE);
            foreach (ResultDetail rd in tempPR.Results)
            {
                ((ET_DataExtension)rd.Object).Columns = (ET_DataExtensionColumn[])((ET_DataExtension)rd.Object).Fields;
                ((ET_DataExtension)rd.Object).Fields = null;
            }
            return tempPR;
        }
        public FuelSDK.PatchReturn Patch()
        {
            ET_DataExtension tempDE = this;
            tempDE.Fields = this.Columns;
            tempDE.Columns = null;
            PatchReturn tempPR = new FuelSDK.PatchReturn(tempDE);
            foreach (ResultDetail rd in tempPR.Results)
            {
                ((ET_DataExtension)rd.Object).Columns = (ET_DataExtensionColumn[])((ET_DataExtension)rd.Object).Fields;
                ((ET_DataExtension)rd.Object).Fields = null;
            }
            return tempPR;
        }
        public FuelSDK.DeleteReturn Delete()
        {
            ET_DataExtension tempDE = this;
            tempDE.Fields = this.Columns;
            return new FuelSDK.DeleteReturn(tempDE);
        }
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            foreach (ET_DataExtension rd in response.Results)
            {
                rd.Columns = (ET_DataExtensionColumn[])rd.Fields;
                rd.Fields = null;
            }
            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            foreach (ET_DataExtension rd in response.Results)
            {
                rd.Columns = (ET_DataExtensionColumn[])rd.Fields;
                rd.Fields = null;
            }
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }
    }

    public class ET_DataExtensionColumn : FuelSDK.DataExtensionField
    {
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }
    }

    public class ET_DataExtensionRow : FuelSDK.DataExtensionObject
    {
        public string DataExtensionName { get; set; }
        public string DataExtensionCustomerKey { get; set; }
        public Dictionary<string, string> ColumnValues { get; set; }

        public ET_DataExtensionRow()
        {
            ColumnValues = new Dictionary<string, string>();
        }

        public FuelSDK.PostReturn Post()
        {
            this.GetDataExtensionCustomerKey();
            ET_DataExtensionRow tempRow = this;
            tempRow.CustomerKey = this.DataExtensionCustomerKey;
            List<APIProperty> lProperties = new List<APIProperty>();
            foreach (KeyValuePair<string, string> kvp in this.ColumnValues)
            {
                APIProperty tempAPIProp = new APIProperty() { Name = kvp.Key, Value = kvp.Value };
                lProperties.Add(tempAPIProp);
            }
            tempRow.ColumnValues = null;
            tempRow.Properties = lProperties.ToArray();
            tempRow.DataExtensionName = null;
            tempRow.DataExtensionCustomerKey = null;
            return new FuelSDK.PostReturn(tempRow);
        }
        public FuelSDK.PatchReturn Patch()
        {
            this.GetDataExtensionCustomerKey();
            ET_DataExtensionRow tempRow = this;
            tempRow.CustomerKey = this.DataExtensionCustomerKey;
            List<APIProperty> lProperties = new List<APIProperty>();
            foreach (KeyValuePair<string, string> kvp in this.ColumnValues)
            {
                APIProperty tempAPIProp = new APIProperty() { Name = kvp.Key, Value = kvp.Value };
                lProperties.Add(tempAPIProp);
            }
            tempRow.ColumnValues = null;
            tempRow.Properties = lProperties.ToArray();
            tempRow.DataExtensionName = null;
            tempRow.DataExtensionCustomerKey = null;
            return new FuelSDK.PatchReturn(tempRow);
        }
        public FuelSDK.DeleteReturn Delete()
        {
            this.GetDataExtensionCustomerKey();
            ET_DataExtensionRow tempRow = this;
            tempRow.CustomerKey = this.DataExtensionCustomerKey;
            List<APIProperty> lProperties = new List<APIProperty>();
            foreach (KeyValuePair<string, string> kvp in this.ColumnValues)
            {
                APIProperty tempAPIProp = new APIProperty() { Name = kvp.Key, Value = kvp.Value };
                lProperties.Add(tempAPIProp);
            }
            tempRow.ColumnValues = null;
            tempRow.Keys = lProperties.ToArray();
            tempRow.DataExtensionName = null;
            tempRow.DataExtensionCustomerKey = null;
            return new FuelSDK.DeleteReturn(tempRow);
        }
        public FuelSDK.GetReturn Get()
        {
            this.GetDataExtensionName();
            FuelSDK.GetReturn response = new GetReturn(this, false, "DataExtensionObject[" + this.DataExtensionName + "]");
            this.LastRequestID = response.RequestID;

            foreach (ET_DataExtensionRow dr in response.Results)
            {
                Dictionary<string, string> returnColumns = new Dictionary<string, string>();
                foreach (APIProperty ap in dr.Properties)
                {
                    returnColumns.Add(ap.Name, ap.Value);
                }
                dr.ColumnValues = returnColumns;
                dr.Properties = null;
            }

            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            this.GetDataExtensionName();
            FuelSDK.GetReturn response = new GetReturn(this, true, "DataExtensionObject[" + this.DataExtensionName + "]");
            this.LastRequestID = response.RequestID;

            foreach (ET_DataExtensionRow dr in response.Results)
            {
                Dictionary<string, string> returnColumns = new Dictionary<string, string>();
                foreach (APIProperty ap in dr.Properties)
                {
                    returnColumns.Add(ap.Name, ap.Value);
                }
                dr.ColumnValues = returnColumns;
                dr.Properties = null;
            }

            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }

        private void GetDataExtensionName()
        {
            if (this.DataExtensionName == null)
            {
                if (this.DataExtensionCustomerKey != null)
                {
                    ET_DataExtension lookupDE = new ET_DataExtension();
                    lookupDE.AuthStub = this.AuthStub;
                    lookupDE.Props = new string[] { "Name", "CustomerKey" };
                    lookupDE.SearchFilter = new SimpleFilterPart() { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new string[] { this.DataExtensionCustomerKey } };
                    GetReturn grDEName = lookupDE.Get();

                    if (grDEName.Status && grDEName.Results.Length > 0)
                    {
                        this.DataExtensionName = ((ET_DataExtension)grDEName.Results[0]).Name;
                    }
                    else
                    {
                        throw new Exception("Unable to process ET_DataExtensionRow request due to unable to find DataExtension based on CustomerKey");
                    }
                }
                else
                {
                    throw new Exception("Unable to process ET_DataExtensionRow request due to DataExtensionCustomerKey or DataExtensionName not being defined on ET_DatExtensionRow");
                }
            }
        }

        private void GetDataExtensionCustomerKey()
        {
            if (this.DataExtensionCustomerKey == null)
            {
                if (this.DataExtensionName != null)
                {
                    ET_DataExtension lookupDE = new ET_DataExtension();
                    lookupDE.AuthStub = this.AuthStub;
                    lookupDE.Props = new string[] { "Name", "CustomerKey" };
                    lookupDE.SearchFilter = new SimpleFilterPart() { Property = "Name", SimpleOperator = SimpleOperators.equals, Value = new string[] { this.DataExtensionName } };
                    GetReturn grDEName = lookupDE.Get();

                    if (grDEName.Status && grDEName.Results.Length > 0)
                    {
                        this.DataExtensionCustomerKey = ((ET_DataExtension)grDEName.Results[0]).CustomerKey;
                    }
                    else
                    {
                        throw new Exception("Unable to process ET_DataExtensionRow request due to unable to find DataExtension based on DataExtensionName provided.");
                    }
                }
                else
                {
                    throw new Exception("Unable to process ET_DataExtensionRow request due to DataExtensionCustomerKey or DataExtensionName not being defined on ET_DatExtensionRow");
                }
            }
        }
    }


    // Misc Objects

    public class ET_TriggeredSend : FuelSDK.TriggeredSendDefinition
    {
        public int? FolderID { get; set; }
        internal string FolderMediaType = "triggered_send";
        public ET_Subscriber[] Subscribers { get; set; }

        public FuelSDK.SendReturn Send()
        {
            ET_Trigger ts = new ET_Trigger();
            ts.CustomerKey = this.CustomerKey;
            ts.TriggeredSendDefinition = this;
            ts.Subscribers = this.Subscribers;
            ((ET_TriggeredSend)ts.TriggeredSendDefinition).Subscribers = null;
            ts.AuthStub = this.AuthStub;

            return new FuelSDK.SendReturn(ts);
        }

        public FuelSDK.PostReturn Post()
        {
            return new FuelSDK.PostReturn(this);
        }
        public FuelSDK.PatchReturn Patch()
        {
            return new FuelSDK.PatchReturn(this);
        }
        public FuelSDK.DeleteReturn Delete()
        {
            return new FuelSDK.DeleteReturn(this);
        }
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }
    }

    public class ET_Folder : FuelSDK.DataFolder
    {
        public FuelSDK.PostReturn Post()
        {
            return new FuelSDK.PostReturn(this);
        }
        public FuelSDK.PatchReturn Patch()
        {
            return new FuelSDK.PatchReturn(this);
        }
        public FuelSDK.DeleteReturn Delete()
        {
            return new FuelSDK.DeleteReturn(this);
        }
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }

        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }
    }

    public class ET_ObjectDefinition : FuelSDK.ObjectDefinition { }

    public class ET_PropertyDefinition : FuelSDK.PropertyDefinition { }

    public class ET_SendClassification : FuelSDK.SendClassification { }

    public class ET_SenderProfile : FuelSDK.SenderProfile { }

    public class ET_DeliveryProfile : FuelSDK.DeliveryProfile { }

    public class ET_SendDefinitionList : FuelSDK.SendDefinitionList { }

    public class ET_Trigger : FuelSDK.TriggeredSend { }


    // Tracking Events

    public class ET_OpenEvent : OpenEvent
    {
        public Boolean GetSinceLastBatch { get; set; }
        public ET_OpenEvent()
        {
            this.GetSinceLastBatch = true;
        }
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }
    }

    public class ET_BounceEvent : BounceEvent
    {
        public Boolean GetSinceLastBatch { get; set; }
        public ET_BounceEvent()
        {
            this.GetSinceLastBatch = true;
        }
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }
    }

    public class ET_ClickEvent : ClickEvent
    {
        public Boolean GetSinceLastBatch { get; set; }
        public ET_ClickEvent()
        {
            this.GetSinceLastBatch = true;
        }
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }
    }

    public class ET_UnsubEvent : UnsubEvent
    {
        public Boolean GetSinceLastBatch { get; set; }
        public ET_UnsubEvent()
        {
            this.GetSinceLastBatch = true;
        }
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }
    }

    public class ET_SentEvent : SentEvent
    {
        public Boolean GetSinceLastBatch { get; set; }
        public ET_SentEvent()
        {
            this.GetSinceLastBatch = true;
        }
        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn response = new GetReturn(this);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.GetReturn GetMoreResults()
        {
            FuelSDK.GetReturn response = new GetReturn(this, true, null);
            this.LastRequestID = response.RequestID;
            return response;
        }
        public FuelSDK.InfoReturn Info()
        {
            return new FuelSDK.InfoReturn(this);
        }
    }

    public partial class APIObject
    {
        [System.Xml.Serialization.XmlIgnore()]
        [JsonIgnore]
        public FuelSDK.ET_Client AuthStub { get; set; }
        [System.Xml.Serialization.XmlIgnore()]
        public string[] Props { get; set; }
        [System.Xml.Serialization.XmlIgnore()]
        public FilterPart SearchFilter { get; set; }
        [System.Xml.Serialization.XmlIgnore()]
        public String LastRequestID { get; set; }
    }

    public class FuelObject : APIObject
    {
        [JsonIgnore]
        public string Endpoint { get; set; }
        public string[] URLProperties { get; set; }
        public string[] RequiredURLProperties { get; set; }
        public int Page { get; set; }

        protected string cleanRestValue(JToken jobj)
        {
            return jobj.ToString().Replace("\"", "").Trim();
        }
    }

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

        public ET_Campaign(JObject jObject)
        {
            if (jObject["id"] != null)
                this.ID = int.Parse(cleanRestValue(jObject["id"]));
            if (jObject["createdDate"] != null)
                this.CreatedDate = DateTime.Parse(cleanRestValue(jObject["createdDate"]));
            if (jObject["modifiedDate"] != null)
                this.ModifiedDate = DateTime.Parse(cleanRestValue(jObject["modifiedDate"]));
            if (jObject["name"] != null)
                this.Name = cleanRestValue(jObject["name"]);
            if (jObject["description"] != null)
                this.Description = cleanRestValue(jObject["description"]);
            if (jObject["campaignCode"] != null)
                this.CampaignCode = cleanRestValue(jObject["campaignCode"]);
            if (jObject["color"] != null)
                this.Color = cleanRestValue(jObject["color"]);
            if (jObject["favorite"] != null)
                this.Favorite = bool.Parse(cleanRestValue(jObject["favorite"]));
        }

        public FuelSDK.PostReturn Post()
        {
            return new FuelSDK.PostReturn(this);
        }

        public FuelSDK.DeleteReturn Delete()
        {
            return new FuelSDK.DeleteReturn(this);
        }

        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn gr = new FuelSDK.GetReturn(this);
            this.Page = gr.LastPageNumber;
            return gr;
        }

        public FuelSDK.GetReturn GetMoreResults()
        {
            this.Page = this.Page + 1;
            FuelSDK.GetReturn gr = new FuelSDK.GetReturn(this);
            this.Page = gr.LastPageNumber;
            return gr;
        }
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

        public ET_CampaignAsset(JObject jObject)
        {
            if (jObject["id"] != null)
                this.ID = int.Parse(cleanRestValue(jObject["id"]));
            if (jObject["createdDate"] != null)
                this.CreatedDate = DateTime.Parse(cleanRestValue(jObject["createdDate"]));
            if (jObject["type"] != null)
                this.Type = cleanRestValue(jObject["type"]);
            if (jObject["campaignId"] != null)
                this.CampaignID = cleanRestValue(jObject["campaignId"]);
            if (jObject["itemID"] != null)
                this.ItemID = cleanRestValue(jObject["itemID"]);
        }

        public FuelSDK.PostReturn Post()
        {
            return new FuelSDK.PostReturn(this);
        }

        public FuelSDK.DeleteReturn Delete()
        {
            return new FuelSDK.DeleteReturn(this);
        }

        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn gr = new FuelSDK.GetReturn(this);
            this.Page = gr.LastPageNumber;
            return gr;
        }

        public FuelSDK.GetReturn GetMoreResults()
        {
            this.Page = this.Page + 1;
            FuelSDK.GetReturn gr = new FuelSDK.GetReturn(this);
            this.Page = gr.LastPageNumber;
            return gr;
        }
    }

    //
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

        public ET_Endpoint(JObject jObject)
        {
            if (jObject["type"] != null)
                this.Type = cleanRestValue(jObject["type"]).ToString().Trim();
            if (jObject["url"] != null)
                this.URL = cleanRestValue(jObject["url"]);
        }

        public FuelSDK.GetReturn Get()
        {
            FuelSDK.GetReturn gr = new FuelSDK.GetReturn(this);
            this.Page = gr.LastPageNumber;
            return gr;
        }

        public FuelSDK.GetReturn GetMoreResults()
        {
            this.Page = this.Page + 1;
            FuelSDK.GetReturn gr = new FuelSDK.GetReturn(this);
            this.Page = gr.LastPageNumber;
            return gr;
        }
    }

}