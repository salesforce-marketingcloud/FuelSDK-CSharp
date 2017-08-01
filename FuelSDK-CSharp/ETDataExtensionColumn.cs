using System;
namespace FuelSDK
{
    /// <summary>
    /// ETDataExtensionColumn - Represents Data Extension Field.
    /// </summary>
    public class ETDataExtensionColumn : DataExtensionField
    {
		/// <summary>
		/// Get this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/> object..</returns>
		public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
		/// <summary>
		/// Gets more results.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/> object..</returns>
		public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
		/// <summary>
		/// Info of this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.InfoReturn"/> object..</returns>
		public InfoReturn Info() { return new InfoReturn(this); }
    }

	[Obsolete("ET_DataExtensionColumn will be removed in future release. Please use ETDataExtensionColumn instead.")]
	public class ET_DataExtensionColumn : ETDataExtensionColumn
	{
		
	}
}
