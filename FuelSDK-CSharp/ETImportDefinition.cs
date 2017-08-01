using System;
namespace FuelSDK
{
	/// <summary>
    /// ETImportDefinition - Defines a reusable pattern of import options.
	/// </summary>
	public class ETImportDefinition : ImportDefinition
    {
		internal string LastTaskID = string.Empty;
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
		/// Status of this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/> object.</returns>
		public GetReturn Status()
		{
			if (LastTaskID == string.Empty)
				throw new Exception("No ID available in order to return status for ETImport");
			var r = new GetReturn(new ETImportResult
			{
				AuthStub = AuthStub,
				Props = new string[] { "ImportDefinitionCustomerKey", "TaskResultID", "ImportStatus", "StartDate", "EndDate", "DestinationID", "NumberSuccessful", "NumberDuplicated", "NumberErrors", "TotalRows", "ImportType" },
				SearchFilter = new SimpleFilterPart { Value = new[] { LastTaskID }, Property = "TaskResultID", SimpleOperator = SimpleOperators.equals },
			});
			LastRequestID = r.RequestID;
			return r;
		}
		/// <summary>
		/// Start this import process.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.PerformReturn"/> object.</returns>
		public PerformReturn Start()
		{
			var r = new PerformReturn(this, "start");
			if (r.Results.Length == 1)
				LastTaskID = ((ResultDetail)r.Results[0]).Task.ID;
			return r;
		}
    }

    [Obsolete("ET_Import will be removed in future release. Please use ETImportDefinition instead.")]
    public class ET_Import : ETImportDefinition
	{
		
	}
}
