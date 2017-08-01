using System;
namespace FuelSDK
{
    /// <summary>
    /// ResultDetails - Represents details of the result object.
    /// </summary>
	public class ResultDetail
	{
        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>The status code.</value>
		public string StatusCode { get; set; }
        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        /// <value>The status message.</value>
		public string StatusMessage { get; set; }
        /// <summary>
        /// Gets or sets the ordinal identifier.
        /// </summary>
        /// <value>The ordinal identifier.</value>
		public int OrdinalID { get; set; }
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>The error code.</value>
		public int ErrorCode { get; set; }
        /// <summary>
        /// Gets or sets the new identifier.
        /// </summary>
        /// <value>The new identifier.</value>
		public int NewID { get; set; }
        /// <summary>
        /// Gets or sets the new object identifier.
        /// </summary>
        /// <value>The new object identifier.</value>
		public string NewObjectID { get; set; }
        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        /// <value><see cref="T:FuelSDK.APIObject"/> object.</value>
		public APIObject Object { get; set; }
        /// <summary>
        /// Gets or sets the task.
        /// </summary>
        /// <value>The task.</value>
		public TaskResult Task { get; set; }
	}
}
