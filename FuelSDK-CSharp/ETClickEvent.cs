using System;
namespace FuelSDK
{
	/// <summary>
    /// ETClickEvent - Represents ClickEvent Class
	/// Contains time and date information, as well as a URL ID and a URL, regarding a click on a link contained in a message.
	/// </summary>
	public class ETClickEvent : ClickEvent
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:FuelSDK.ETClickEvent"/> get since last batch.
        /// </summary>
        /// <value><c>true</c> if get since last batch; otherwise, <c>false</c>.</value>
        public bool GetSinceLastBatch
        {
            get; set;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ETClickEvent"/> class.
        /// </summary>
        public ETClickEvent()
        {
            GetSinceLastBatch = true;
        }
		/// <summary>
		/// Get this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/>.</returns>
		public GetReturn Get()
        {
            var r = new GetReturn(this); LastRequestID = r.RequestID; return r;
        }
		/// <summary>
		/// Gets the more results.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/>.</returns>
		public GetReturn GetMoreResults()
        {
            var r = new GetReturn(this, true, null); LastRequestID = r.RequestID; return r;
        }
		/// <summary>
		/// Get the Info of this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.InfoReturn"/>.</returns>
		public InfoReturn Info()
        {
            return new InfoReturn(this);
        }
    }

    [Obsolete("ET_ClickEvent will be removed in future release. Please use ETClickEvent instead.")]
    public class ET_ClickEvent : ETClickEvent
    {

    }
}
