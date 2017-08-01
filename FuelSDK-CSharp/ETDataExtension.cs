using System;
namespace FuelSDK
{
	/// <summary>
	/// ETDataExtension - Represents a data extension within an account.
	/// </summary>
	public class ETDataExtension : DataExtension
    {
		internal string FolderMediaType = "dataextension";
        /// <summary>
        /// Gets or sets the folder identifier.
        /// </summary>
        /// <value>The folder identifier.</value>
		public int? FolderID { get; set; }
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
		public ETDataExtensionColumn[] Columns { get; set; }
		/// <summary>
		/// Post this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.PostReturn"/> object.</returns>
		public PostReturn Post()
		{
			ETDataExtension de = this;
			de.Fields = Columns;
			de.Columns = null;
			var pr = new PostReturn(de);
			foreach (var rd in pr.Results)
			{
				((ETDataExtension)rd.Object).Columns = (ETDataExtensionColumn[])((ETDataExtension)rd.Object).Fields;
				((ETDataExtension)rd.Object).Fields = null;
			}
			return pr;
		}
		/// <summary>
		/// Patch this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.PatchReturn"/> object..</returns>
		public PatchReturn Patch()
		{
			ETDataExtension de = this;
			de.Fields = Columns;
			de.Columns = null;
			var pr = new PatchReturn(de);
			foreach (var rd in pr.Results)
			{
				((ETDataExtension)rd.Object).Columns = (ETDataExtensionColumn[])((ETDataExtension)rd.Object).Fields;
				((ETDataExtension)rd.Object).Fields = null;
			}
			return pr;
		}
		/// <summary>
		/// Delete this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.DeleteReturn"/> object..</returns>
		public DeleteReturn Delete()
		{
			ETDataExtension de = this;
			de.Fields = Columns;
			return new DeleteReturn(de);
		}
		/// <summary>
		/// Get this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/> object..</returns>
		public GetReturn Get()
		{
			var r = new GetReturn(this);
			LastRequestID = r.RequestID;
			foreach (ETDataExtension rd in r.Results)
			{
				rd.Columns = (ETDataExtensionColumn[])rd.Fields;
				rd.Fields = null;
			}
			return r;
		}
		/// <summary>
		/// Gets more results.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/> object..</returns>
		public GetReturn GetMoreResults()
		{
			var r = new GetReturn(this, true, null);
			LastRequestID = r.RequestID;
			foreach (ETDataExtension rd in r.Results)
			{
				rd.Columns = (ETDataExtensionColumn[])rd.Fields;
				rd.Fields = null;
			}
			return r;
		}
		/// <summary>
		/// Info of this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.InfoReturn"/> object..</returns>
		public InfoReturn Info() { return new InfoReturn(this); }
    }

    [Obsolete("ET_DataExtension will be removed in future release. Please use ETDataExtension instead.")]
	public class ET_DataExtension : ETDataExtension
	{
        
	}
}
