using System;
using Newtonsoft.Json.Linq;

namespace FuelSDK
{
    /// <summary>
    /// Represents a program in an account
    /// </summary>
    public class ETCampaign : FuelObject
    {
        /// <summary>
        /// Gets or sets the name of the campaign.
        /// </summary>
        /// <value>The name.</value>
		public string Name { get; set; }
        /// <summary>
        /// Gets or sets the description of the campaign.
        /// </summary>
        /// <value>The description.</value>
		public string Description { get; set; }
        /// <summary>
        /// Gets or sets the campaign code of the campaign.
        /// </summary>
        /// <value>The campaign code.</value>
		public string CampaignCode { get; set; }
        /// <summary>
        /// Gets or sets the color of the campaign.
        /// </summary>
        /// <value>The color.</value>
		public string Color { get; set; }
        /// <summary>
        /// Gets or sets the if the campaign is flagged as favorite.
        /// </summary>
        /// <value>The favorite flag.</value>
		public bool? Favorite { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ETCampaign"/> class.
        /// </summary>
		public ETCampaign()
		{
            Endpoint = ConfigUtil.GetFuelSDKConfigSection().RestEndPoint + "/hub/v1/campaigns/{ID}";
			URLProperties = new[] { "ID" };
			RequiredURLProperties = new string[0];
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ETCampaign"/> class.
        /// </summary>
        /// <param name="obj">Javascript object.</param>
		public ETCampaign(JObject obj)
		{
			if (obj["id"] != null)
				ID = int.Parse(CleanRestValue(obj["id"]));
			if (obj["createdDate"] != null)
				CreatedDate = DateTime.Parse(CleanRestValue(obj["createdDate"]));
			if (obj["modifiedDate"] != null)
				ModifiedDate = DateTime.Parse(CleanRestValue(obj["modifiedDate"]));
			if (obj["name"] != null)
				Name = CleanRestValue(obj["name"]);
			if (obj["description"] != null)
				Description = CleanRestValue(obj["description"]);
			if (obj["campaignCode"] != null)
				CampaignCode = CleanRestValue(obj["campaignCode"]);
			if (obj["color"] != null)
				Color = CleanRestValue(obj["color"]);
			if (obj["favorite"] != null)
				Favorite = bool.Parse(CleanRestValue(obj["favorite"]));
		}
		/// <summary>
		/// Post this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.PostReturn"/>.</returns>
		public PostReturn Post() { return new PostReturn(this); }
		/// <summary>
		/// Delete this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.DeleteReturn"/>.</returns>
		public DeleteReturn Delete() { return new DeleteReturn(this); }
		/// <summary>
		/// Get this instance.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/>.</returns>
		public GetReturn Get() { var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
		/// <summary>
		/// Gets more results.
		/// </summary>
		/// <returns>The <see cref="T:FuelSDK.GetReturn"/>.</returns>
		public GetReturn GetMoreResults() { Page++; var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
    }

    [Obsolete("ET_Campaign will be removed in future release. Please use ETCampaign instead.")]
	public class ET_Campaign : ETCampaign
	{
        public ET_Campaign() : base() {}
        public ET_Campaign(JObject obj) : base(obj) { }
	}
}
