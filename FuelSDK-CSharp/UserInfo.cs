using Newtonsoft.Json.Linq;

namespace FuelSDK
{
    public class UserInfo : FuelObject
    {
        public string StackKey { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.UserInfo"/> class.
        /// </summary>
        /// <param name="authRestEndpoint">The auth rest endpoint.</param>
		public UserInfo(string authRestEndpoint)
        {
            Endpoint = authRestEndpoint + "/v2/userinfo";
            URLProperties = new string[0];
            RequiredURLProperties = new string[0];
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FuelSDK.UserInfo"/> class.
        /// </summary>
        /// <param name="obj">Javascript Object.</param>
		public UserInfo(JObject obj)
        {
            if (obj["organization"]["stack_key"] != null)
            {
                StackKey = obj["organization"]["stack_key"].ToString();
            }
        }
        /// Get this instance.
        /// </summary>
        /// <returns>The <see cref="T:FuelSDK.GetReturn"/> object..</returns>
        public GetReturn Get() {    var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
        /// <summary>
        /// Gets the more results.
        /// </summary>
        /// <returns>The <see cref="T:FuelSDK.GetReturn"/> object..</returns>
        public GetReturn GetMoreResults() { Page++; var r = new GetReturn(this); Page = r.LastPageNumber; return r; }
    }
}