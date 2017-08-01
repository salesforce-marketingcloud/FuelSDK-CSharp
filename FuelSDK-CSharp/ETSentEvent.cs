using System;
namespace FuelSDK
{
	/// <summary>
	/// ETSentEvent - Contains tracking data related to a send, including information on individual subscribers.
	/// </summary>
	public class ETSentEvent : SentEvent
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:FuelSDK.ETSentEvent"/> get since last batch.
        /// </summary>
        /// <value><c>true</c> if get since last batch; otherwise, <c>false</c>.</value>
		public bool GetSinceLastBatch { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ETSentEvent"/> class.
        /// </summary>
		public ETSentEvent() { GetSinceLastBatch = true; }
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

	[Obsolete("ET_SentEvent will be removed in future release. Please use ETSentEvent instead.")]
	public class ET_SentEvent : ETSentEvent
	{
		
	}
}
