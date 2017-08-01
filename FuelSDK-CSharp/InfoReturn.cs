using System;
using System.Linq;

namespace FuelSDK
{
	/// <summary>
	/// InfoReturn - Represents a FuelReturn object returns as result of an Info operation.
	/// </summary>
	public class InfoReturn : FuelReturn
	{
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
		public ETPropertyDefinition[] Results { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.InfoReturn"/> class.
        /// </summary>
        /// <param name="objs">Objects.</param>
		public InfoReturn(APIObject objs)
		{
			if (objs == null)
				throw new ArgumentNullException("objs");
			var response = ExecuteAPI(x => new ObjectDefinitionRequest
			{
				ObjectType = TranslateObject(x).GetType().ToString().Replace("FuelSDK.", string.Empty)
			}, (client, o) =>
			{
				string requestID;
				return new ExecuteAPIResponse<ObjectDefinition>(client.SoapClient.Describe(o, out requestID), requestID, "OK");
			}, objs);
			if (response != null)
                if (response.Length > 0)
                {
                    if (objs.GetType().ToString().Contains("ET_"))
                    {
                        Results = response[0].Properties.Select(x => (ET_PropertyDefinition)(TranslateObject2(x))).ToArray();
                    }
                    else
                    {
                        Results = response[0].Properties.Select(x => (ETPropertyDefinition)(TranslateObject(x))).ToArray();
                    }
                    
                }
                else
                {
                    Status = false;
                }
		}
	}
}
