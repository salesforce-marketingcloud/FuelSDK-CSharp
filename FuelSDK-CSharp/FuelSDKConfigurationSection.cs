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
		[ConfigurationProperty("clientSecret")]
		public string ClientSecret
		{
			get { return (string)this["clientSecret"]; }
			set { this["clientSecret"] = value; }
		}
        /// <summary>
        /// Gets or sets the SOAP end point.
        /// </summary>
        /// <value>The SOAP end point.</value>
		[ConfigurationProperty("soapEndPoint")]
		public string SoapEndPoint
		{
			get { return (string)this["soapEndPoint"]; }
			set { this["soapEndPoint"] = value; }
		}
        /// <summary>
        /// Gets or sets the authentification end point.
        /// </summary>
        /// <value>The authentification end point.</value>
        [ConfigurationProperty("authEndPoint")]
		public string AuthenticationEndPoint
		{
			get { return (string)this["authEndPoint"]; }
			set { this["authEndPoint"] = value; }
		}
        /// <summary>
        /// Gets or sets the REST end point.
        /// </summary>
        /// <value>The REST end point.</value>
        [ConfigurationProperty("restEndPoint")]
        public string RestEndPoint
        {
            get { return (string)this["restEndPoint"]; }
            set { this["restEndPoint"] = value; }
        }
	    /// <summary>
	    /// Gets or sets the Authenticaton Mode.
	    /// </summary>
	    /// <value>Authenticaton Mode</value>
        [ConfigurationProperty("useOAuth2Authentication", DefaultValue = "false")]
	    public string UseOAuth2Authentication
	    {
            get { return (string)this["useOAuth2Authentication"]; }
            set { this["useOAuth2Authentication"] = value; }
	    }

	    /// <summary>
	    /// Gets or sets the Account Id.
	    /// </summary>
	    /// <value>Authenticaton Mode</value>
        [ConfigurationProperty("accountId", DefaultValue = "")]
	    public string AccountId
	    {
	        get { return (string)this["accountId"]; }
            set { this["accountId"] = value; }
	    }
	    /// <summary>
	    /// Gets or sets the Authenticaton Mode.
	    /// </summary>
	    /// <value>Authenticaton Mode</value>
	    [ConfigurationProperty("scope", DefaultValue = "")]
	    public string Scope
	    {
            get { return (string)this["scope"]; }
            set { this["scope"] = value; }
	    }

        /// <summary>
        /// Gets or sets the Application Type.
        /// </summary>
        /// <value>Application Type</value>
        [ConfigurationProperty("applicationType", DefaultValue = "server")]
        public string ApplicationType
        {
            get { return (string)this["applicationType"]; }
            set { this["applicationType"] = value; }
        }

        /// <summary>
        /// Gets or sets the Authorization Code.
        /// </summary>
        /// <value>Authorization Code</value>
        [ConfigurationProperty("authorizationCode", DefaultValue = "")]
        public string AuthorizationCode
        {
            get { return (string)this["authorizationCode"]; }
            set { this["authorizationCode"] = value; }
        }

        /// <summary>
        /// Gets or sets the Redirect URL.
        /// </summary>
        /// <value>Authorization Code</value>
        [ConfigurationProperty("redirectURI", DefaultValue = "")]
        public string RedirectURI
        {
            get { return (string)this["redirectURI"]; }
            set { this["redirectURI"] = value; }
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

        /// <summary>
        /// Sets the AuthenticationEndPoint to the default value if it is not set and returns the updated instance.
        /// </summary>
        /// <param name="defaultAuthEndpoint">The default auth endpoint</param>
        /// <returns>The updated <see cref="FuelSDKConfigurationSection"/> instance</returns>
	    public FuelSDKConfigurationSection WithDefaultAuthEndpoint(string defaultAuthEndpoint)
	    {
	        if (string.IsNullOrEmpty(AuthenticationEndPoint))
	        {
	            AuthenticationEndPoint = defaultAuthEndpoint;
	        }

	        return this;
	    }

	    public FuelSDKConfigurationSection WithDefaultRestEndpoint(string defaultRestEndpoint)
	    {
	        if (string.IsNullOrEmpty(RestEndPoint))
	        {
	            this.RestEndPoint = defaultRestEndpoint;
	        }

	        return this;
	    }
	}
}
