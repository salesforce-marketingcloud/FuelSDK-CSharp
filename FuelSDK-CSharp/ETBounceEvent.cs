using System;
namespace FuelSDK
{
	/// <summary>
	/// Contains SMTP and other information pertaining to the specific event of an email message bounce.
	/// </summary>
	public class ETBounceEvent : BounceEvent
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:FuelSDK.ETBounceEvent"/> get since last batch.
        /// </summary>
        /// <value><c>true</c> if get since last batch; otherwise, <c>false</c>.</value>
		public bool GetSinceLastBatch { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ETBounceEvent"/> class and set get since last batch to false.
        /// </summary>
		public ETBounceEvent() { GetSinceLastBatch = true; }
		/// <summary>
		/// Get <see cref="T:FuelSDK.GetReturn"/> object after initializaing LastRequestID.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/> object</returns>
		public GetReturn Get() { var r = new GetReturn(this); LastRequestID = r.RequestID; return r; }
		/// <summary>
		/// Gets more results.
		/// </summary>
		/// <returns><see cref="T:FuelSDK.GetReturn"/> object</returns>
		public GetReturn GetMoreResults() { var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r; }
		/// <summary>
		/// Returns the <see cref="T:FuelSDK.InfoReturn"/> object.
		/// </summary>
		/// <returns><see cref="T:FuelSDK.InfoReturn"/> object</returns>
		public InfoReturn Info() { return new InfoReturn(this); }
    }

    [Obsolete("ET_BounceEvent will be removed in future release. Please use ETBounceEvent instead.")]
	public class ET_BounceEvent : ETBounceEvent
	{
		
	}
}
