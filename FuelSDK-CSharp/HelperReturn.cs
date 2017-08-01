using System;
namespace FuelSDK
{
	/// <summary>
	/// HelperReturn - Represent PostReturn object returns as a result of a helper operation.
	/// </summary>
	public class HelperReturn : PostReturn
	{
		public HelperReturn(APIObject obj)
			: base(obj) { }
	}
}
