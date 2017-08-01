using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FuelSDK
{
    /// <summary>
    /// GetReturn - Represents FuelReturn object returned as a result of a Get operation.
    /// </summary>
    public class GetReturn : FuelReturn
    {
        /// <summary>
        /// Gets or sets the last page number.
        /// </summary>
        /// <value>The last page number.</value>
        public int LastPageNumber { get; set; }
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        public APIObject[] Results { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.GetReturn"/> class.
        /// </summary>
        /// <param name="objs">Objects.</param>
        public GetReturn(APIObject objs) : this(objs, false, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.GetReturn"/> class.
        /// </summary>
        /// <param name="objs">Objects.</param>
        /// <param name="continueRequest">If set to <c>true</c> continue request.</param>
        /// <param name="overrideObjectType">Override object type.</param>
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
                    if (string.IsNullOrEmpty(overrideObjectType))
                    {
                        if (x.GetType().ToString().Contains("ET_"))
                        {
                            rr.ObjectType = TranslateObject2(x).GetType().ToString().Replace("FuelSDK.", string.Empty);
                        }
                        else
                        {
                            rr.ObjectType = TranslateObject(x).GetType().ToString().Replace("FuelSDK.", string.Empty);
                        }
                        
                    }
                    else
                    {
                        rr.ObjectType = overrideObjectType;
                    }

                    // If they didn't specify Props then we look them up using Info()
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
            }, (client, o) =>
            {
                string requestID;
                APIObject[] objectResults;
                var overallStatus = client.SoapClient.Retrieve(o[0], out requestID, out objectResults);
                return new ExecuteAPIResponse<APIObject>(objectResults, requestID, overallStatus) { OverallStatusMessage = overallStatus };
            }, objs);
            if (response != null)
            {
                if (response.Length > 0)
                {
                    if (objs.GetType().ToString().Contains("ET_"))
                    {
                        Results = response.Select(TranslateObject2).ToArray();
                    }
                    else
                    {
                        Results = response.Select(TranslateObject).ToArray();
                    }

                }
                else
                {
                    Results = new APIObject[0];
                }
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.GetReturn"/> class.
        /// </summary>
        /// <param name="obj">Object.</param>
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
}
