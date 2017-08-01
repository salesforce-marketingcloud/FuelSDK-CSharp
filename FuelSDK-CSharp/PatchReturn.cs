using System;
using System.Linq;

namespace FuelSDK
{
	/// <summary>
	/// PatchReturn - Represents a FuelReturn object returns as result of an Patch operation.
	/// </summary>
	public class PatchReturn : FuelReturn
	{
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
		public ResultDetail[] Results { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.PatchReturn"/> class.
        /// </summary>
        /// <param name="objs">Objects.</param>
		public PatchReturn(APIObject objs)
		{
			if (objs == null)
				throw new ArgumentNullException("objs");
			var response = ExecuteAPI((client, o) =>
			{
				string requestID;
				string overallStatus;
				return new ExecuteAPIResponse<UpdateResult>(client.SoapClient.Update(new UpdateOptions(), o, out requestID, out overallStatus), requestID, overallStatus);
			}, objs);
			if (response != null)
				if (response.GetType() == typeof(UpdateResult[]) && response.Length > 0)
					Results = response.Cast<UpdateResult>().Select(x => new ResultDetail
					{
						StatusCode = x.StatusCode,
						StatusMessage = x.StatusMessage,
						Object = (x.Object != null ? (objs.GetType().ToString().Contains("ET_") ? TranslateObject2(x.Object) : TranslateObject(x.Object)) : null),
						OrdinalID = x.OrdinalID,
						ErrorCode = x.ErrorCode,
					}).ToArray();
				else
					Results = new ResultDetail[0];
		}
	}
}
