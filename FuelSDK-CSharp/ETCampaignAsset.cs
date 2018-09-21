using System;
using Newtonsoft.Json.Linq;

namespace FuelSDK
{
    /// <summary>
    /// ETCampaignAsset - Represents an asset associated with a campaign.
    /// </summary>
    public class ETCampaignAsset : FuelObject
    {
        /// <summary>
        /// Gets or sets the type of the campaign asset.
        /// </summary>
        /// <value>The type.</value>
		public string Type { get; set; }
		/// <summary>
		/// Gets or sets the campaign identifier of the campaign asset.
		/// </summary>
		/// <value>The campaign identifier.</value>
		public string CampaignID { get; set; }
		/// <summary>
		/// Gets or sets the identifier of the campaign asset.
		/// </summary>
		/// <value>The identifier.</value>
		public string[] IDs { get; set; }
		/// <summary>
		/// Gets or sets the item identifier of the campaign asset.
		/// </summary>
		/// <value>The item identifier.</value>
		public string ItemID { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ETCampaignAsset"/> class.
        /// </summary>
		public ETCampaignAsset()
		{
            Endpoint = ConfigUtil.GetFuelSDKConfigSection().RestEndPoint + "/hub/v1/campaigns/{CampaignID}/assets/{ID}";
			URLProperties = new[] { "CampaignID", "ID" };
			RequiredURLProperties = new[] { "CampaignID" };
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.ETCampaignAsset"/> class.
        /// </summary>
        /// <param name="obj">Javascript Object.</param>
		public ETCampaignAsset(JObject obj)
		{
			if (obj["id"] != null)
				ID = int.Parse(CleanRestValue(obj["id"]));
			if (obj["createdDate"] != null)
				CreatedDate = DateTime.Parse(CleanRestValue(obj["createdDate"]));
			if (obj["type"] != null)
				Type = CleanRestValue(obj["type"]);
			if (obj["campaignId"] != null)
				CampaignID = CleanRestValue(obj["campaignId"]);
			if (obj["itemID"] != null)
				ItemID = CleanRestValue(obj["itemID"]);
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
	public class ET_CampaignAsset : ETCampaignAsset
	{
		
	}
}
