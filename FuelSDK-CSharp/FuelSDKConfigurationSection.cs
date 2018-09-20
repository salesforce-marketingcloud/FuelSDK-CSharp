using System;
using System.Configuration;

namespace FuelSDK
{
	/// <summary>
	/// FuelSDKConfigurationSection - Represents ConfigurationSection
	/// </summary>
	public class FuelSDKConfigurationSection : ConfigurationSection
	{
        /// <summary>
        /// Gets or sets the app signature.
        /// </summary>
        /// <value>The app signature.</value>
		[ConfigurationProperty("appSignature", IsRequired = true)]
		public string AppSignature
		{
			get { return (string)this["appSignature"]; }
			set { this["appSignature"] = value; }
		}
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
		[ConfigurationProperty("clientId", IsRequired = true)]
		public string ClientId
		{
			get { return (string)this["clientId"]; }
			set { this["clientId"] = value; }
		}
        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>The client secret.</value>
		[ConfigurationProperty("clientSecret", IsRequired = true)]
		public string ClientSecret
		{
			get { return (string)this["clientSecret"]; }
			set { this["clientSecret"] = value; }
		}
        /// <summary>
        /// Gets or sets the SOAP end point.
        /// </summary>
        /// <value>The SOAP end point.</value>
		[ConfigurationProperty("soapEndPoint", DefaultValue = "https://webservice.s4.exacttarget.com/Service.asmx")]
		public string SoapEndPoint
		{
			get { return (string)this["soapEndPoint"]; }
			set { this["soapEndPoint"] = value; }
		}
		[ConfigurationProperty("authEndPoint", DefaultValue = "https://auth.exacttargetapis.com/v1/requestToken?legacy=1")]
		public string AuthenticationEndPoint
		{
			get { return (string)this["authEndPoint"]; }
			set { this["authEndPoint"] = value; }
		}
        /// <summary>
        /// Gets or sets the REST end point.
        /// </summary>
        /// <value>The REST end point.</value>
        [ConfigurationProperty("restEndPoint", DefaultValue = "https://www.exacttargetapis.com")]
        public string RestEndPoint
        {
            get { return (string)this["restEndPoint"]; }
            set { this["restEndPoint"] = value; }
        }
        /// <summary>
        /// Clone this instance.
        /// </summary>
        /// <returns>The clone.</returns>
		public object Clone()
		{
			return MemberwiseClone();
		}
        /// <summary>
        /// Ises the read only.
        /// </summary>
        /// <returns><c>true</c>, if read only, <c>false</c> otherwise.</returns>
		public override bool IsReadOnly()
		{
			return false;
		}
	}
}
