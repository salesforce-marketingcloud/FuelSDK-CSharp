using System;
namespace FuelSDK
{
	/// <summary>
	/// ETEmailSendDefinition - Record that contains the message information, sender profile, delivery profile, and audience information..
	/// </summary>
	public class ETEmailSendDefinition : EmailSendDefinition
    {
		internal string FolderMediaType = "userinitiatedsends";
        /// <summary>
        /// The last task identifier.
        /// </summary>
		internal string LastTaskID = string.Empty;
        /// <summary>
        /// Gets or sets the folder identifier.
        /// </summary>
        /// <value>The folder identifier.</value>
		public int? FolderID { get; set; }
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
        /// <summary>
        /// Send this instance.
        /// </summary>
        /// <returns>The <see cref="T:FuelSDK.PerformReturn"/> object.</returns>
		public PerformReturn Send()
		{
			var r = new PerformReturn(this, "start");
			if (r.Results.Length == 1)
				LastTaskID = ((ResultDetail)r.Results[0]).Task.ID;
			return r;
		}
        /// <summary>
        /// Status of this instance.
        /// </summary>
        /// <returns>The <see cref="T:FuelSDK.GetReturn"/> object.</returns>
		public GetReturn Status()
		{
			if (LastTaskID == string.Empty)
				throw new Exception("No ID available in order to return status for ETEmailSendDefinition");
			var r = new GetReturn(new ETSend
			{
				AuthStub = AuthStub,
				SearchFilter = new SimpleFilterPart { Value = new[] { LastTaskID }, Property = "ID", SimpleOperator = SimpleOperators.equals },
			});
			LastRequestID = r.RequestID;
			return r;
		}
    }

    [Obsolete("ET_EmailSendDefinition will be removed in future release. Please use ETEmailSendDefinition instead.")]
	public class ET_EmailSendDefinition : ETEmailSendDefinition
	{
		
	}
}
