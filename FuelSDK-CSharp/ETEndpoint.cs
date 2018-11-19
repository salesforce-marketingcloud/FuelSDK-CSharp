using System;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace FuelSDK
{
    /// <summary>
    /// ETEndpoint - Represent an EndPoint.
    /// </summary>
    public class ETEndpoint : FuelObject
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
		public string Type { get; set; }
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
		public string URL { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ETEndpoint"/> class.
        /// </summary>
		public ETEndpoint()
		{
            Endpoint = ConfigUtil.GetFuelSDKConfigSection().RestEndPoint + "/platform/v1/endpoints/{Type}";
			URLProperties = new[] { "Type" };
			RequiredURLProperties = new string[0];
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ETEndpoint"/> class.
        /// </summary>
        /// <param name="obj">Javascript Object.</param>
		public ETEndpoint(JObject obj)
		{
			if (obj["type"] != null)
				Type = CleanRestValue(obj["type"]).ToString().Trim();
			if (obj["url"] != null)
				URL = CleanRestValue(obj["url"]);
		}
		/// Get this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/> object..</returns>
		public GetReturn Get() { var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
		/// <summary>
		/// Gets the more results.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/> object..</returns>
		public GetReturn GetMoreResults() { Page++; var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
    }

    [Obsolete("ET_Endpoint will be removed in future release. Please use ETEndpoint instead.")]
    public class ET_Endpoint : ETEndpoint
    {

    }
}
