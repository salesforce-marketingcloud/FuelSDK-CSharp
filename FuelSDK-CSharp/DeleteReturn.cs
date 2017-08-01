using System;
using System.Linq;

namespace FuelSDK
{
    /// <summary>
    /// Return object by delete operation.
    /// </summary>
	public class DeleteReturn : FuelReturn
	{
        /// <summary>
        /// Gets or sets the results as an ResultDetail array.
        /// </summary>
        /// <value>Array of ResultDetail.</value>
		public ResultDetail[] Results { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.DeleteReturn"/> class.
        /// </summary>
        /// <param name="objs">APIObject</param>
		public DeleteReturn(APIObject objs)
		{
			if (objs == null)
				throw new ArgumentNullException("objs");
			var response = ExecuteAPI((client, o) =>
			{
				string requestID;
				string overallStatus;
				return new ExecuteAPIResponse<DeleteResult>(client.SoapClient.Delete(new DeleteOptions(), o, out requestID, out overallStatus), requestID, overallStatus);
			}, objs);
			if (response != null)
				if (response.GetType() == typeof(DeleteResult[]) && response.Length > 0)
					Results = response.Cast<DeleteResult>().Select(x => new ResultDetail
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
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.DeleteReturn"/> class.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// <param name="obj">FuelObject</param>
		public DeleteReturn(FuelObject obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");
			ExecuteFuel(obj, obj.URLProperties, "DELETE", false);
		}
	}
}
