using System;
using System.Linq;

namespace FuelSDK
{
	/// <summary>
	/// PerformReturn - Represents a FuelReturn object returns as result of an Perform operation.
	/// </summary>
	public class PerformReturn : FuelReturn
	{
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
		public ResultDetail[] Results { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.PerformReturn"/> class.
        /// </summary>
        /// <param name="objs">Objects.</param>
        /// <param name="performAction">Perform action.</param>
		public PerformReturn(APIObject objs, string performAction)
		{
			if (objs == null)
				throw new ArgumentNullException("objs");
			var response = ExecuteAPI((client, o) =>
			{
				string requestID;
				string overallStatus;
				string overallStatusMessage;
				return new ExecuteAPIResponse<PerformResult>(client.SoapClient.Perform(new PerformOptions(), performAction, o, out overallStatus, out overallStatusMessage, out requestID), requestID, overallStatus) { OverallStatusMessage = overallStatusMessage };
			}, objs);
			if (response != null)
				if (response.GetType() == typeof(PerformResult[]) && response.Length > 0)
					Results = response.Cast<PerformResult>().Select(cr => new ResultDetail
					{
						StatusCode = cr.StatusCode,
						StatusMessage = cr.StatusMessage,
						Object = (cr.Object != null ? (objs.GetType().ToString().Contains("ET_") ? TranslateObject2(cr.Object) : TranslateObject(cr.Object) ): null),
						Task = cr.Task,
						OrdinalID = cr.OrdinalID,
						ErrorCode = cr.ErrorCode,
					}).ToArray();
				else
					Results = new ResultDetail[0];
		}
	}
}
