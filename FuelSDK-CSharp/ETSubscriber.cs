using System;
namespace FuelSDK
{
	/// <summary>
	/// ETSubscriber - A person subscribed to receive email or SMS communication.
	/// </summary>
	public class ETSubscriber : Subscriber
    {
		/// <summary>
		/// Post this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.PostReturn"/> object.</returns>
		public PostReturn Post() { return new PostReturn(this); }
		/// <summary>
		/// Patch this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.PatchReturn"/> object..</returns>
		public PatchReturn Patch() { return new PatchReturn(this); }
		/// <summary>
		/// Delete this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.DeleteReturn"/> object..</returns>
		public DeleteReturn Delete() { return new DeleteReturn(this); }
		/// <summary>
		/// Get this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/> object..</returns>
		public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
		/// <summary>
		/// Gets the more results.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/> object..</returns>
		public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
		/// <summary>
		/// Info of this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.InfoReturn"/> object..</returns>
		public InfoReturn Info() { return new InfoReturn(this); }
    }

    [Obsolete("ET_Subscriber will be removed in future release. Please use ETSubscriber instead.")]
	public class ET_Subscriber : ETSubscriber
	{
		
	}
}
