using System;
namespace FuelSDK
{
	/// <summary>
	/// SendReturn - Represents a FuelReturn object returns as result of an Send operation.
	/// </summary>
	public class SendReturn : PostReturn
	{
		public SendReturn(APIObject obj)
			: base(obj) { }
	}
}
