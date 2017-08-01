using System;
using Newtonsoft.Json.Linq;

namespace FuelSDK
{
	/// <summary>
	/// FuelObject - Represents APIObject
	/// </summary>
	public class FuelObject : APIObject
	{
        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>The endpoint.</value>
		[Newtonsoft.Json.JsonIgnore]
		public string Endpoint { get; set; }
        /// <summary>
        /// Gets or sets the URL Properties.
        /// </summary>
        /// <value>The URLP roperties.</value>
		public string[] URLProperties { get; set; }
        /// <summary>
        /// Gets or sets the required URL properties.
        /// </summary>
        /// <value>The required URLP roperties.</value>
		public string[] RequiredURLProperties { get; set; }
        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>The page.</value>
		public int? Page { get; set; }

		protected string CleanRestValue(JToken obj) { return obj.ToString().Replace("\"", "").Trim(); }
	}
}
