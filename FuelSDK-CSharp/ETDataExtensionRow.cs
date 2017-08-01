using System;
using System.Collections.Generic;
using System.Linq;

namespace FuelSDK
{
    /// <summary>
    /// ETDataExtensionRow - Represents each row within a data extension.
    /// </summary>
    public class ETDataExtensionRow : DataExtensionObject
    {
        /// <summary>
        /// Gets or sets the name of the data extension.
        /// </summary>
        /// <value>The name of the data extension.</value>
		public string DataExtensionName { get; set; }
        /// <summary>
        /// Gets or sets the data extension customer key.
        /// </summary>
        /// <value>The data extension customer key.</value>
		public string DataExtensionCustomerKey { get; set; }
        /// <summary>
        /// Gets or sets the column values.
        /// </summary>
        /// <value>The column values.</value>
		public Dictionary<string, string> ColumnValues { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ETDataExtensionRow"/> class.
        /// </summary>
		public ETDataExtensionRow()
		{
			ColumnValues = new Dictionary<string, string>();
		}
		/// <summary>
		/// Post this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.PostReturn"/> object.</returns>
		public PostReturn Post()
		{
			if (ColumnValues == null)
				throw new ArgumentNullException("ColumnValues");
			GetDataExtensionCustomerKey();
			ETDataExtensionRow row = this;
			row.CustomerKey = DataExtensionCustomerKey;
			row.Properties = ColumnValues.Select(x => new APIProperty { Name = x.Key, Value = x.Value }).ToArray();
			row.ColumnValues = null;
			row.DataExtensionName = null;
			row.DataExtensionCustomerKey = null;
			return new PostReturn(row);
		}
		/// <summary>
		/// Patch this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.PatchReturn"/> object..</returns>
		public PatchReturn Patch()
		{
			if (ColumnValues == null)
				throw new ArgumentNullException("ColumnValues");
			GetDataExtensionCustomerKey();
			ETDataExtensionRow row = this;
			row.CustomerKey = DataExtensionCustomerKey;
			row.Properties = ColumnValues.Select(x => new APIProperty { Name = x.Key, Value = x.Value }).ToArray();
			row.ColumnValues = null;
			row.DataExtensionName = null;
			row.DataExtensionCustomerKey = null;
			return new PatchReturn(row);
		}
		/// <summary>
		/// Delete this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.DeleteReturn"/> object..</returns>
		public DeleteReturn Delete()
		{
			GetDataExtensionCustomerKey();
			ETDataExtensionRow row = this;
			row.CustomerKey = DataExtensionCustomerKey;
			row.Keys = (ColumnValues != null ? ColumnValues.Select(x => new APIProperty { Name = x.Key, Value = x.Value }).ToArray() : null);
			row.ColumnValues = null;
			row.DataExtensionName = null;
			row.DataExtensionCustomerKey = null;
			return new DeleteReturn(row);
		}
		/// <summary>
		/// Get this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/> object..</returns>
		public GetReturn Get()
		{
			GetDataExtensionName();
			var r = new GetReturn(this, false, "DataExtensionObject[" + DataExtensionName + "]");
			LastRequestID = r.RequestID;
			foreach (ETDataExtensionRow dr in r.Results)
			{
				dr.ColumnValues = dr.Properties.ToDictionary(x => x.Name, x => x.Value);
				dr.Properties = null;
			}
			return r;
		}
		/// <summary>
		/// Gets more results.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/> object..</returns>
		public GetReturn GetMoreResults()
		{
			GetDataExtensionName();
			var r = new GetReturn(this, true, "DataExtensionObject[" + DataExtensionName + "]");
			LastRequestID = r.RequestID;
			foreach (ETDataExtensionRow dr in r.Results)
			{
				dr.ColumnValues = dr.Properties.ToDictionary(x => x.Name, x => x.Value);
				dr.Properties = null;
			}
			return r;
		}
		/// <summary>
		/// Info of this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.InfoReturn"/> object..</returns>
		public InfoReturn Info() { return new InfoReturn(this); }

		private void GetDataExtensionName()
		{
			if (DataExtensionName == null)
			{
				if (DataExtensionCustomerKey == null)
					throw new Exception("Unable to process ETDataExtensionRow request due to DataExtensionCustomerKey or DataExtensionName not being defined on ETDatExtensionRow");
				var grDEName = new ETDataExtension
				{
					AuthStub = AuthStub,
					Props = new[] { "Name", "CustomerKey" },
					SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { DataExtensionCustomerKey } },
				}.Get();
				if (grDEName.Status && grDEName.Results.Length > 0)
					DataExtensionName = ((ETDataExtension)grDEName.Results[0]).Name;
				else
					throw new Exception("Unable to process ETDataExtensionRow request due to unable to find DataExtension based on CustomerKey");
			}
		}
		private void GetDataExtensionCustomerKey()
		{
			if (DataExtensionCustomerKey == null)
			{
				if (DataExtensionName == null)
					throw new Exception("Unable to process ETDataExtensionRow request due to DataExtensionCustomerKey or DataExtensionName not being defined on ETDatExtensionRow");
				var grDEName = new ETDataExtension
				{
					AuthStub = AuthStub,
					Props = new[] { "Name", "CustomerKey" },
					SearchFilter = new SimpleFilterPart { Property = "Name", SimpleOperator = SimpleOperators.equals, Value = new[] { DataExtensionName } },
				}.Get();
				if (grDEName.Status && grDEName.Results.Length > 0)
					DataExtensionCustomerKey = ((ETDataExtension)grDEName.Results[0]).CustomerKey;
				else
					throw new Exception("Unable to process ETDataExtensionRow request due to unable to find DataExtension based on DataExtensionName provided.");
			}
		}
    }

    [Obsolete("ET_DataExtensionRow will be removed in future release. Please use ETDataExtensionRow instead.")]
	public class ET_DataExtensionRow : ETDataExtensionRow
	{

	}
}
