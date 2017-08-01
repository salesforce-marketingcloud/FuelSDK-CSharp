using System;
namespace FuelSDK
{
	/// <summary>
	/// ETListSubscriber - The ListSubscriber object retrieves subscribers for a list or lists for a subscriber.
	/// </summary>
	public class ETListSubscriber : ListSubscriber
    {
        internal string FolderMediaType = "listsubscriber";
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

    [Obsolete("ET_List_Subscriber will be removed in future release. Please use ETListSubscriber instead.")]
	public class ET_List_Subscriber : ETListSubscriber
	{
		
	}
}
