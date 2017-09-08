using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml.Linq;

namespace FuelSDK
{
    /// <summary>
    /// ETDataExtract : Provides methods to send data extract request.
    /// </summary>
    public class ETDataExtract
    {
        /// <summary>
        /// Gets or sets flag to indicate whether to include number of bounced email.
        /// </summary>
        public bool ExtractBounces { get; set; }
        /// <summary>
        /// Gets or sets flag to indicate whether to include number of user click.
        /// </summary>
        public bool ExtractClicks { get; set; }
        /// <summary>
        /// Gets or sets flag to indicate whether to inlcude conversion details.
        /// </summary>
        public bool ExtractConversions { get; set; }
        /// <summary>
        /// Gets or sets falg to indicate whether to include number of opens.
        /// </summary>
        public bool ExtractOpens { get; set; }
        /// <summary>
        /// Gets or sets flag to indicate whether to include number of emails sent.
        /// </summary>
        public bool ExtractSent { get; set; }
        /// <summary>
        /// Gets or sets flag to indicate whether to include send jobs details.
        /// </summary>
        public bool ExtractSendJobs { get; set; }
        /// <summary>
        /// Gets or sets flag to indicate whether to include survey response.
        /// </summary>
        public bool ExtractSurveyResponses { get; set; }
        /// <summary>
        /// Gets or sets flag to indicate whether to include test send email details.
        /// </summary>
        public bool IncludeTestSends { get; set; }
        /// <summary>
        /// Gets or sets flag to indicate whether to include unsubscribed count.
        /// </summary>
        public bool ExtractUnsubs { get; set; }
        /// <summary>
        /// Gets or sets output file name of the data extract request.
        /// </summary>
        public string OutputFileName { get; set; }
        /// <summary>
        /// Gets or sets the start date of the data to be extracted.
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Gets or sets the end date of the data to be extracted.
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Gets or sets the authentication stub.
        /// </summary>
        public ETClient AuthStub { get; set; }
        /// <summary>
        /// Gets or sets the customer key of the requested data extension extract.
        /// </summary>
        public string DECustomerKey { get; set; }
        /// <summary>
        /// Gets the asynchronous operation ID. This is internal use only.
        /// </summary>
        public string _AsyncID { get; private set; }
        /// <summary>
        /// Gets or sets flag to indicate whether extracted data has column headers.
        /// </summary>
        public bool HasColumnHeaders { get; set; }

        private Dictionary<string, string> extractTypes;

        /// <summary>
        /// Default constructor. Initialize various properties with default value.
        /// </summary>
        public ETDataExtract()
        {
            ExtractBounces = true;
            ExtractClicks = true;
            ExtractConversions = true;
            ExtractOpens = true;
            ExtractSendJobs = true;
            ExtractSent = true;
            ExtractSurveyResponses = true;
            IncludeTestSends = false;
            ExtractUnsubs = true;
            HasColumnHeaders = true;
            _AsyncID = "0";
        }

        /// <summary>
        /// Sends an data extension extract request. 
        /// </summary>
        /// <param name="deCustomerKey">Customer key of the data extension to be extracted.</param>
        /// <param name="outputFileName">Name of the output file.</param>
        /// <param name="hasColumnHeaders">flag to indicate whether extracted data has column headers.</param>
        /// <returns>The <see cref="T:FuelSDK.ExtractResponse"/> object.</returns>
        public ExtractResponse ExtractDataExtension(string deCustomerKey, string outputFileName, bool hasColumnHeaders = true)
        {
            DECustomerKey = deCustomerKey;
            HasColumnHeaders = hasColumnHeaders;
            OutputFileName = outputFileName;
            return ExtractDataExtension();
        }
        /// <summary>
        /// Sends an data extension extract request. 
        /// </summary>
        /// <returns>The <see cref="T:FuelSDK.ExtractResponse"/> object.</returns>
        public ExtractResponse ExtractDataExtension()
        {
            ValidateExtractDataExtensionParameters();
            StartDate = Convert.ToDateTime("01/01/1900 01:00 AM");
            EndDate = Convert.ToDateTime("01/01/1900 01:00 AM");
            string[] paramNames = new[] { "DECustomerKey", "HasColumnHeaders", "_AsyncID", "OutputFileName", "StartDate", "EndDate" };
            return PerformDataExtract("Data Extension Extract", paramNames);
        }

        private void ValidateExtractDataExtensionParameters()
        {
            if (string.IsNullOrWhiteSpace(DECustomerKey))
            {
                throw new ApplicationException("Data extension customer key is empty or null. Please fill data extension customer key.");
            }
            if (string.IsNullOrWhiteSpace(OutputFileName))
            {
                throw new ApplicationException("Output file name is empty or null. Please fill output file name.");
            }
            if (!OutputFileName.ToLower().EndsWith(".csv") && !OutputFileName.ToLower().EndsWith(".zip"))
            {
                throw new ApplicationException("Invalid file extension. Only csv or zip allowed.");
            }
        }

        private ExtractResponse PerformDataExtract(string extractType, string[] paramNames)
        {
            //Check AuthStub is not null before performing any operation.
            if (AuthStub == null)
            {
                throw new ApplicationException("Auth Stub not initialized. Initialize AuthStub before performing extract operation.");
            }
            AuthStub.RefreshToken();
            if (extractTypes == null)
            {
                PopulateExtractTypes();
            }
            var eparams = new List<ExtractParameter>();
            var type = this.GetType();
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            var props = type.GetProperties(flags);
            foreach (var pname in paramNames)
            {
                var prop = type.GetProperty(pname);
                if (prop == null)
                {
                    throw new ApplicationException(string.Format("Invalid property name passed : {0}", pname));
                }
                if (pname == "OutputFileName" && string.IsNullOrWhiteSpace(prop.GetValue(this, null).ToString()))
                {
                    throw new ApplicationException("Output file name not initialized. Initialize Output file name before performing extract operation.");
                }
                var val = prop.GetValue(this, null).ToString();
                if (prop.PropertyType == typeof(DateTime))
                {
                    val = ((DateTime)prop.GetValue(this, null)).ToString("yyyy-MM-dd hh:mm tt");
                }
                var eparam = new ExtractParameter
                                {
                                    Name = prop.Name,
                                    Value = val
                                };
                eparams.Add(eparam);

            }
           
            using (var scope = new OperationContextScope(AuthStub.SoapClient.InnerChannel))
            {
                string requestId;
                ExtractResult[] results;
                ExtractRequest er = new ExtractRequest();
                er.ID = extractTypes[extractType];
                er.Parameters = eparams.ToArray(); ;
                er.Options = new ExtractOptions();

                // Add oAuth token to SOAP header.
                XNamespace ns = "http://exacttarget.com";
                var oauthElement = new XElement(ns + "oAuthToken", AuthStub.InternalAuthToken);
                var xmlHeader = MessageHeader.CreateHeader("oAuth", "http://exacttarget.com", oauthElement);
                OperationContext.Current.OutgoingMessageHeaders.Add(xmlHeader);

                var httpRequest = new HttpRequestMessageProperty();
                OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, httpRequest);
                httpRequest.Headers.Add(HttpRequestHeader.UserAgent, ETClient.SDKVersion);

                var response = AuthStub.SoapClient.Extract(new ExtractRequest[] { er }, out requestId, out results);
                return new ExtractResponse
                {
                    Results = results,
                    OverallStatus = response,
                    RequestID = requestId
                };
            }
        }

        private void PopulateExtractTypes()
        {
            extractTypes = new Dictionary<string, string>();
            ETExtractDescription eed = new ETExtractDescription
            {
                AuthStub = this.AuthStub
            };
            var resp = eed.Get();
            if (resp.Results != null && resp.Results.Length > 0)
            {
                foreach (var etype in resp.Results)
                {
                    extractTypes.Add(((ExtractTemplate)etype).Name, etype.ObjectID);
                }
            }
        }

    }
}
