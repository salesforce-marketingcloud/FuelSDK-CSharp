using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace FuelSDK
{
	/// <summary>
	/// PostReturn - Represents a FuelReturn object returns as result of an Post operation.
	/// </summary>
	public class PostReturn : FuelReturn
	{
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
		public ResultDetail[] Results { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.PostReturn"/> class.
        /// </summary>
        /// <param name="objs">Objects.</param>
		public PostReturn(params APIObject[] objs)
		{
			if (objs == null)
				throw new ArgumentNullException("objs");
			var response = ExecuteAPI((client, o) =>
			{
				string requestID;
				string overallStatus;
				return new ExecuteAPIResponse<CreateResult>(client.SoapClient.Create(new CreateOptions(), o, out requestID, out overallStatus), requestID, overallStatus);
			}, objs);
			if (response != null)
				if (response.GetType() == typeof(CreateResult[]) && response.Length > 0)
					Results = response.Cast<CreateResult>().Select(x => new ResultDetail
					{
						StatusCode = x.StatusCode,
						StatusMessage = x.StatusMessage,
						NewObjectID = x.NewObjectID,
						Object = (x.Object != null ? (objs[0].GetType().ToString().Contains("ET_") ? TranslateObject2(x.Object) :TranslateObject(x.Object)) : null),
						OrdinalID = x.OrdinalID,
						ErrorCode = x.ErrorCode,
						NewID = x.NewID,
					}).ToArray();
				else
					Results = new ResultDetail[0];
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.PostReturn"/> class.
        /// </summary>
        /// <param name="obj">Object.</param>
		public PostReturn(FuelObject obj)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");
			var response = ExecuteFuel(obj, obj.RequiredURLProperties, "POST", true);
			if (string.IsNullOrEmpty(response))
				Results = new ResultDetail[0];
			else if (response.StartsWith("["))
				Results = JArray.Parse(response)
					.Select(x => new ResultDetail { Object = (APIObject)Activator.CreateInstance(obj.GetType(), BindingFlags.Public | BindingFlags.Instance, null, new object[] { x }, null) }).ToArray();
			else
			{
				var x = JObject.Parse(response);
				Results = new[] { new ResultDetail { Object = (APIObject)Activator.CreateInstance(obj.GetType(), BindingFlags.Public | BindingFlags.Instance, null, new object[] { x }, null) } };
			}
		}
	}
}
