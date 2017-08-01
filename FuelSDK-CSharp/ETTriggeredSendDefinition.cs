using System;
namespace FuelSDK
{
	/// <summary>
	/// ETTriggeredSendDefinition - Defines a triggered send in the account.
	/// </summary>
	public class ETTriggeredSendDefinition : TriggeredSendDefinition
    {
        /// <summary>
        /// Gets or sets the folder identifier.
        /// </summary>
        /// <value>The folder identifier.</value>
		public int? FolderID { get; set; }
        internal string FolderMediaType = "triggered_send";
        /// <summary>
        /// Gets or sets the subscribers.
        /// </summary>
        /// <value>The subscribers.</value>
		public ETSubscriber[] Subscribers { get; set; }
        /// <summary>
        /// Send this instance.
        /// </summary>
        /// <returns>The <see cref="T:FuelSDK.SendReturn"/></returns>
		public SendReturn Send()
		{
			var ts = new ETTriggerSend
			{
				CustomerKey = CustomerKey,
				TriggeredSendDefinition = this,
				Subscribers = Subscribers,
				AuthStub = AuthStub,
			};
			((ETTriggeredSendDefinition)ts.TriggeredSendDefinition).Subscribers = null;
			return new SendReturn(ts);
		}
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

	[Obsolete("ET_TriggeredSend will be removed in future release. Please use ETTriggeredSendDefinition instead.")]
	public class ET_TriggeredSend : ETTriggeredSendDefinition
	{
		
	}
}
