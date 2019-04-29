using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace FuelSDK
{
    /// <summary>
    /// FuelReturn - Represent abstract class for return values.
    /// </summary>
	public abstract class FuelReturn
	{
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:FuelSDK.FuelReturn"/> is status.
        /// </summary>
        /// <value><c>true</c> if status; otherwise, <c>false</c>.</value>
		public bool Status { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
		public string Message { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:FuelSDK.FuelReturn"/> has more results.
        /// </summary>
        /// <value><c>true</c> if more results; otherwise, <c>false</c>.</value>
		public bool MoreResults { get; set; }
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
		public int Code { get; set; }
        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>The request identifier.</value>
		public string RequestID { get; set; }

		private Dictionary<Type, Type> _translators = new Dictionary<Type, Type>();
        private Dictionary<Type, Type> _translators2 = new Dictionary<Type, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.FuelReturn"/> class.
        /// </summary>
		public FuelReturn()
		{
			_translators.Add(typeof(ETFolder), typeof(DataFolder));
			_translators.Add(typeof(DataFolder), typeof(ETFolder));

			_translators.Add(typeof(ETList), typeof(List));
			_translators.Add(typeof(List), typeof(ETList));

			_translators.Add(typeof(ETContentArea), typeof(ContentArea));
			_translators.Add(typeof(ContentArea), typeof(ETContentArea));

			_translators.Add(typeof(ETObjectDefinition), typeof(ObjectDefinition));
			_translators.Add(typeof(ObjectDefinition), typeof(ETObjectDefinition));

			_translators.Add(typeof(ETPropertyDefinition), typeof(PropertyDefinition));
			_translators.Add(typeof(PropertyDefinition), typeof(ETPropertyDefinition));

			_translators.Add(typeof(Subscriber), typeof(ETSubscriber));
			_translators.Add(typeof(ETSubscriber), typeof(Subscriber));

			_translators.Add(typeof(ETProfileAttribute), typeof(Attribute));
			_translators.Add(typeof(Attribute), typeof(ETProfileAttribute));

			_translators.Add(typeof(ETEmail), typeof(Email));
			_translators.Add(typeof(Email), typeof(ETEmail));

			_translators.Add(typeof(ETSubscriberList), typeof(SubscriberList));
			_translators.Add(typeof(SubscriberList), typeof(ETSubscriberList));

			_translators.Add(typeof(ETListSubscriber), typeof(ListSubscriber));
			_translators.Add(typeof(ListSubscriber), typeof(ETListSubscriber));

			_translators.Add(typeof(ETDataExtension), typeof(DataExtension));
			_translators.Add(typeof(DataExtension), typeof(ETDataExtension));

			_translators.Add(typeof(ETDataExtensionColumn), typeof(DataExtensionField));
			_translators.Add(typeof(DataExtensionField), typeof(ETDataExtensionColumn));

			_translators.Add(typeof(ETDataExtensionRow), typeof(DataExtensionObject));
			_translators.Add(typeof(DataExtensionObject), typeof(ETDataExtensionRow));

			_translators.Add(typeof(ETSendClassification), typeof(SendClassification));
			_translators.Add(typeof(SendClassification), typeof(ETSendClassification));

			_translators.Add(typeof(ETSendDefinitionList), typeof(SendDefinitionList));
			_translators.Add(typeof(SendDefinitionList), typeof(ETSendDefinitionList));

			_translators.Add(typeof(ETSenderProfile), typeof(SenderProfile));
			_translators.Add(typeof(SenderProfile), typeof(ETSenderProfile));

			_translators.Add(typeof(ETDeliveryProfile), typeof(DeliveryProfile));
			_translators.Add(typeof(DeliveryProfile), typeof(ETDeliveryProfile));

			_translators.Add(typeof(ETTriggeredSendDefinition), typeof(TriggeredSendDefinition));
			_translators.Add(typeof(TriggeredSendDefinition), typeof(ETTriggeredSendDefinition));

			_translators.Add(typeof(ETEmailSendDefinition), typeof(EmailSendDefinition));
			_translators.Add(typeof(EmailSendDefinition), typeof(ETEmailSendDefinition));

			_translators.Add(typeof(ETQueryDefinition), typeof(QueryDefinition));
			_translators.Add(typeof(QueryDefinition), typeof(ETQueryDefinition));

			_translators.Add(typeof(ETSend), typeof(Send));
			_translators.Add(typeof(Send), typeof(ETSend));

            _translators.Add(typeof(ETImportDefinition), typeof(ImportDefinition));
            _translators.Add(typeof(ImportDefinition), typeof(ETImportDefinition));

			_translators.Add(typeof(ETImportResult), typeof(ImportResultsSummary));
			_translators.Add(typeof(ImportResultsSummary), typeof(ETImportResult));

			// The translation for this is handled in the Get() method for DataExtensionObject so no need to translate it
			_translators.Add(typeof(APIProperty), typeof(APIProperty));

			_translators.Add(typeof(ETTriggerSend), typeof(TriggeredSend));
			_translators.Add(typeof(TriggeredSend), typeof(ETTriggerSend));

			// Tracking Events
			_translators.Add(typeof(ETBounceEvent), typeof(BounceEvent));
			_translators.Add(typeof(BounceEvent), typeof(ETBounceEvent));
			_translators.Add(typeof(OpenEvent), typeof(ETOpenEvent));
			_translators.Add(typeof(ETOpenEvent), typeof(OpenEvent));
			_translators.Add(typeof(ETClickEvent), typeof(ClickEvent));
			_translators.Add(typeof(ClickEvent), typeof(ETClickEvent));
			_translators.Add(typeof(ETUnsubEvent), typeof(UnsubEvent));
			_translators.Add(typeof(UnsubEvent), typeof(ETUnsubEvent));
			_translators.Add(typeof(ETSentEvent), typeof(SentEvent));
			_translators.Add(typeof(SentEvent), typeof(ETSentEvent));


            //obselete class translation
            _translators.Add(typeof(ET_Folder), typeof(DataFolder));
            _translators2.Add(typeof(DataFolder), typeof(ET_Folder));

            _translators.Add(typeof(ET_List), typeof(List));
            _translators2.Add(typeof(List), typeof(ET_List));

            _translators.Add(typeof(ET_ContentArea), typeof(ContentArea));
            _translators2.Add(typeof(ContentArea), typeof(ET_ContentArea));

            _translators.Add(typeof(ET_ObjectDefinition), typeof(ObjectDefinition));
            _translators2.Add(typeof(ObjectDefinition), typeof(ET_ObjectDefinition));

            _translators.Add(typeof(ET_PropertyDefinition), typeof(PropertyDefinition));
            _translators2.Add(typeof(PropertyDefinition), typeof(ET_PropertyDefinition));

            _translators2.Add(typeof(Subscriber), typeof(ET_Subscriber));
            _translators.Add(typeof(ET_Subscriber), typeof(Subscriber));

            _translators.Add(typeof(ET_ProfileAttribute), typeof(Attribute));
            _translators2.Add(typeof(Attribute), typeof(ET_ProfileAttribute));

            _translators.Add(typeof(ET_Email), typeof(Email));
            _translators2.Add(typeof(Email), typeof(ET_Email));

            _translators.Add(typeof(ET_SubscriberList), typeof(SubscriberList));
            _translators2.Add(typeof(SubscriberList), typeof(ET_SubscriberList));

            _translators.Add(typeof(ET_List_Subscriber), typeof(ListSubscriber));
            _translators2.Add(typeof(ListSubscriber), typeof(ET_List_Subscriber));

            _translators.Add(typeof(ET_DataExtension), typeof(DataExtension));
            _translators2.Add(typeof(DataExtension), typeof(ET_DataExtension));

            _translators.Add(typeof(ET_DataExtensionColumn), typeof(DataExtensionField));
            _translators2.Add(typeof(DataExtensionField), typeof(ET_DataExtensionColumn));

            _translators.Add(typeof(ET_DataExtensionRow), typeof(DataExtensionObject));
            _translators2.Add(typeof(DataExtensionObject), typeof(ET_DataExtensionRow));

            _translators.Add(typeof(ET_SendClassification), typeof(SendClassification));
            _translators2.Add(typeof(SendClassification), typeof(ET_SendClassification));

            _translators.Add(typeof(ET_SendDefinitionList), typeof(SendDefinitionList));
            _translators2.Add(typeof(SendDefinitionList), typeof(ET_SendDefinitionList));

            _translators.Add(typeof(ET_SenderProfile), typeof(SenderProfile));
            _translators2.Add(typeof(SenderProfile), typeof(ET_SenderProfile));

            _translators.Add(typeof(ET_DeliveryProfile), typeof(DeliveryProfile));
            _translators2.Add(typeof(DeliveryProfile), typeof(ET_DeliveryProfile));

            _translators.Add(typeof(ET_TriggeredSend), typeof(TriggeredSendDefinition));
            _translators2.Add(typeof(TriggeredSendDefinition), typeof(ET_TriggeredSend));

            _translators.Add(typeof(ET_EmailSendDefinition), typeof(EmailSendDefinition));
            _translators2.Add(typeof(EmailSendDefinition), typeof(ET_EmailSendDefinition));

            _translators.Add(typeof(ET_QueryDefinition), typeof(QueryDefinition));
            _translators2.Add(typeof(QueryDefinition), typeof(ET_QueryDefinition));

            _translators.Add(typeof(ET_Send), typeof(Send));
            _translators2.Add(typeof(Send), typeof(ET_Send));

            _translators.Add(typeof(ET_Import), typeof(ImportDefinition));
            _translators2.Add(typeof(ImportDefinition), typeof(ET_Import));

            _translators.Add(typeof(ET_ImportResult), typeof(ImportResultsSummary));
            _translators2.Add(typeof(ImportResultsSummary), typeof(ET_ImportResult));

            // The translation for this is handled in the GET_() mET_hod for DataExtensionObject so no need to translate it
            _translators2.Add(typeof(APIProperty), typeof(APIProperty));

            _translators.Add(typeof(ET_Trigger), typeof(TriggeredSend));
            _translators2.Add(typeof(TriggeredSend), typeof(ET_Trigger));

            // Tracking Events
            _translators.Add(typeof(ET_BounceEvent), typeof(BounceEvent));
            _translators2.Add(typeof(BounceEvent), typeof(ET_BounceEvent));
            _translators2.Add(typeof(OpenEvent), typeof(ET_OpenEvent));
            _translators.Add(typeof(ET_OpenEvent), typeof(OpenEvent));
            _translators.Add(typeof(ET_ClickEvent), typeof(ClickEvent));
            _translators2.Add(typeof(ClickEvent), typeof(ET_ClickEvent));
            _translators.Add(typeof(ET_UnsubEvent), typeof(UnsubEvent));
            _translators2.Add(typeof(UnsubEvent), typeof(ET_UnsubEvent));
            _translators.Add(typeof(ET_SentEvent), typeof(SentEvent));
            _translators2.Add(typeof(SentEvent), typeof(ET_SentEvent));
		}

        /// <summary>
        /// Translates the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="inputObject">Input <see cref="T:FuelSDK.APIObject"/> object.</param>
		public APIObject TranslateObject(APIObject inputObject)
		{
			if (_translators.ContainsKey(inputObject.GetType()))
			{
				var returnObject = (APIObject)Activator.CreateInstance(_translators[inputObject.GetType()]);
				foreach (var prop in inputObject.GetType().GetProperties())
				{
					if (prop.Name == "UniqueID")
						continue;
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

        /// <summary>
        /// Translates the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="inputObject">Input <see cref="T:FuelSDK.APIObject"/> object.</param>
        public APIObject TranslateObject2(APIObject inputObject)
        {
            if (_translators2.ContainsKey(inputObject.GetType()))
            {
                var returnObject = (APIObject)Activator.CreateInstance(_translators2[inputObject.GetType()]);
                foreach (var prop in inputObject.GetType().GetProperties())
                {
                    if (prop.Name == "UniqueID")
                        continue;
                    var propValue = prop.GetValue(inputObject, null);
                    if ((prop.PropertyType.IsSubclassOf(typeof(APIObject)) || prop.PropertyType == typeof(APIObject)) && propValue != null)
                        prop.SetValue(returnObject, TranslateObject2(propValue), null);
                    else if (_translators2.ContainsKey(prop.PropertyType) && propValue != null)
                        prop.SetValue(returnObject, TranslateObject2(propValue), null);
                    else if (prop.PropertyType.IsArray && propValue != null)
                    {
                        var a = (Array)propValue;
                        Array outArray;
                        if (a.Length > 0)
                            if (_translators2.ContainsKey(a.GetValue(0).GetType()))
                            {
                                outArray = Array.CreateInstance(_translators2[a.GetValue(0).GetType()], a.Length);
                                for (int i = 0; i < a.Length; i++)
                                    if (_translators2.ContainsKey(a.GetValue(i).GetType()))
                                        outArray.SetValue(TranslateObject2(a.GetValue(i)), i);
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
				{
					if (prop.Name == "UniqueID")
						continue;
					if (prop.GetValue(inputObject, null) != null && returnObject.GetType().GetProperty(prop.Name) != null)
						prop.SetValue(returnObject, prop.GetValue(inputObject, null), null);
				}
				return returnObject;
			}
			return inputObject;


		}

        protected object TranslateObject2(object inputObject)
        {
            if (_translators2.ContainsKey(inputObject.GetType()))
            {
                var returnObject = (object)Activator.CreateInstance(_translators2[inputObject.GetType()]);
                foreach (var prop in inputObject.GetType().GetProperties())
                {
                    if (prop.Name == "UniqueID")
                        continue;
                    if (prop.GetValue(inputObject, null) != null && returnObject.GetType().GetProperty(prop.Name) != null)
                        prop.SetValue(returnObject, prop.GetValue(inputObject, null), null);
                }
                return returnObject;
            }
            return inputObject;


        }
		

		protected TResult[] ExecuteAPI<TResult>(Func<ETClient, APIObject[], ExecuteAPIResponse<TResult>> func, params APIObject[] objs) { return ExecuteAPI<TResult, APIObject>(TranslateObject, func, objs); }
		protected TResult[] ExecuteAPI<TResult, TObject>(Func<APIObject, TObject> select, Func<ETClient, TObject[], ExecuteAPIResponse<TResult>> func, params APIObject[] objs)
		{
			if (objs == null)
				throw new ArgumentNullException("objs");
			var client = objs.Select(x => x.AuthStub).FirstOrDefault();
			if (client == null)
				throw new InvalidOperationException("client");
			client.RefreshToken();
			using (var scope = new OperationContextScope(client.SoapClient.InnerChannel))
			{
                // Add oAuth token to SOAP header.
                XNamespace ns = "http://exacttarget.com";
                var oauthElement = new XElement(ns + "oAuthToken", client.InternalAuthToken);
			    var xmlHeader = client.UseOAuth2Authentication ? MessageHeader.CreateHeader("fueloauth", "http://exacttarget.com", client.AuthToken)
			         :  MessageHeader.CreateHeader("oAuth", "http://exacttarget.com", oauthElement);
               
                OperationContext.Current.OutgoingMessageHeaders.Add(xmlHeader);

				var httpRequest = new System.ServiceModel.Channels.HttpRequestMessageProperty();
				OperationContext.Current.OutgoingMessageProperties.Add(System.ServiceModel.Channels.HttpRequestMessageProperty.Name, httpRequest);
				httpRequest.Headers.Add(HttpRequestHeader.UserAgent, ETClient.SDKVersion);

				var response = func(client, objs.Select(select).ToArray());
				RequestID = response.RequestID;
				Status = (response.OverallStatus == "OK" || response.OverallStatus == "MoreDataAvailable");
				Code = (Status ? 200 : 0);
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
					{
						if (prop.Name == "UniqueID")
							continue;
						if (obj.URLProperties.Contains(prop.Name) && (propValue = prop.GetValue(obj, null)) != null)
							if ((propValueAsString = propValue.ToString().Trim()).Length > 0 && propValueAsString != "0")
								match = true;
					}
					if (!match)
						throw new Exception("Unable to process request due to missing required property: " + urlProp);
				}
			foreach (var prop in obj.GetType().GetProperties())
			{
				if (prop.Name == "UniqueID")
					continue;
				if (obj.URLProperties.Contains(prop.Name) && (propValue = prop.GetValue(obj, null)) != null)
					if ((propValueAsString = propValue.ToString().Trim()).Length > 0 && propValueAsString != "0")
						completeURL = completeURL.Replace("{" + prop.Name + "}", propValueAsString);
			}

			// Clean up not required URL parameters
			if (obj.URLProperties != null)
				foreach (string urlProp in obj.URLProperties)
					completeURL = completeURL.Replace("{" + urlProp + "}", string.Empty);

            if (obj.Page.HasValue && obj.Page.Value > 0)
                completeURL += "?page=" + obj.Page.ToString();

            var request = (HttpWebRequest)WebRequest.Create(completeURL.Trim());
            request.Headers.Add("Authorization", "Bearer " + obj.AuthStub.AuthToken);
            request.Method = method;
			request.ContentType = "application/json";
			request.UserAgent = ETClient.SDKVersion;

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
}
