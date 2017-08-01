using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace FuelSDK
{
	/// <summary>
	/// The APIObject is the object from which all SOAP API base objects inherit. The APIObject allows all base objects to
	/// - Refer to a specific account.
	/// - Have an associated creation date.
	/// - Have a Marketing Cloud identifier.
	/// - Have an associated key from an external system.
	/// </summary>
	public partial class APIObject
	{
        /// <summary>
        /// Gets or sets the authentication stub.
        /// </summary>
        /// <value>The authentication stub.</value>
		[XmlIgnore, JsonIgnore]
		public ETClient AuthStub { get; set; }
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
		[XmlIgnore]
		public string[] Props { get; set; }
        /// <summary>
        /// Gets or sets the search filter.
        /// </summary>
        /// <value>The search filter.</value>
		[XmlIgnore]
		public FilterPart SearchFilter { get; set; }
        /// <summary>
        /// Gets or sets the last request identifier.
        /// </summary>
        /// <value>The last request identifier.</value>
		[XmlIgnore]
		public string LastRequestID { get; set; }
        /// <summary>
        /// Gets or sets the directory path.
        /// </summary>
        /// <value>The directory path.</value>
		[XmlElementAttribute(Order = 10000), JsonIgnore]
		public string DirectoryPath { get; set; }
        /// <summary>
        /// Gets the unique identifier.
        /// Returns the ID if the value is greater than 0.
        /// If not and Object ID is not null returns Object ID.
        /// If not and Customer Key is not null returns Customer Key.
        /// If not throw  throws error.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// <value>The unique identifier.</value>
		[XmlIgnore, JsonIgnore]
		public string UniqueID
		{
			get
			{
				if (ID > 0) return ID.ToString();
				if (!string.IsNullOrEmpty(ObjectID)) return ObjectID;
				if (!string.IsNullOrEmpty(CustomerKey)) return CustomerKey;
				throw new InvalidOperationException("Unable to generate UniqueID");
			}
		}
	}
}
