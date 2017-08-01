using System;
namespace FuelSDK
{
    /// <summary>
    /// ExecuteAPIResponse - Represents the response object of an execute operation.
    /// </summary>
	public class ExecuteAPIResponse<TResult>
	{
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
		public TResult[] Results { get; set; }
        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>The request identifier.</value>
		public string RequestID { get; set; }
        /// <summary>
        /// Gets or sets the overall status.
        /// </summary>
        /// <value>The overall status.</value>
		public string OverallStatus { get; set; }
        /// <summary>
        /// Gets or sets the overall status message.
        /// </summary>
        /// <value>The overall status message.</value>
		public string OverallStatusMessage { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ExecuteAPIResponse`1"/> class.
        /// </summary>
        /// <param name="results">Results.</param>
        /// <param name="requestID">Request identifier.</param>
        /// <param name="overallStatus">Overall status.</param>
		public ExecuteAPIResponse(TResult[] results, string requestID, string overallStatus)
		{
			Results = results;
			RequestID = requestID;
			OverallStatus = overallStatus;
		}
	}
}
