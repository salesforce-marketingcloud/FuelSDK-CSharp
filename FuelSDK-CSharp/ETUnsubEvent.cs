using System;
namespace FuelSDK
{
	/// <summary>
	/// ETUnsubEvent - Contains information regarding a specific unsubscription action taken by a subscriber.
	/// </summary>
	public class ETUnsubEvent : UnsubEvent
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:FuelSDK.ETUnsubEvent"/> get since last batch.
        /// </summary>
        /// <value><c>true</c> if get since last batch; otherwise, <c>false</c>.</value>
		public bool GetSinceLastBatch { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ETUnsubEvent"/> class.
        /// </summary>
		public ETUnsubEvent() { GetSinceLastBatch = true; }
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

    [Obsolete("ET_UnsubEvent will be removed in future release. Please use ETUnsubEvent instead.")]
	public class ET_UnsubEvent : ETUnsubEvent
	{
		
	}
}
