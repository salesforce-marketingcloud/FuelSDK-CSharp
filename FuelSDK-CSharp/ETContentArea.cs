using System;
namespace FuelSDK
{
	/// <summary>
    /// ETContentArea - Represents a ContentArea class
	/// A ContentArea represents a defined section of reusable content. One or many ContentAreas can be defined for an Email object. 
    /// A ContentArea is always acted upon in the context of an Email object.
    /// Valid values for the Layout property include the following:
    /// </summary>
    public class ETContentArea : ContentArea
    {
		internal string FolderMediaType = "content";
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
    }

	[Obsolete("ET_ContentArea will be removed in future release. Please use ETContentArea instead.")]
	public class ET_ContentArea : ETContentArea
	{
		
	}
}
