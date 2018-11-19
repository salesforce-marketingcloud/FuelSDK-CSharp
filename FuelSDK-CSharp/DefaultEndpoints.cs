namespace FuelSDK
{
    /// <summary>
    /// Contains the default endpoints for the SDK.
    /// </summary>
    public static class DefaultEndpoints
    {
        /// <summary>
        /// The default SOAP endpoint
        /// </summary>
        public static string Soap => "https://webservice.s4.exacttarget.com/Service.asmx";

        /// <summary>
        /// The default authentication endpoint
        /// </summary>
        public static string Auth => "https://auth.exacttargetapis.com/v1/requestToken?legacy=1";

        /// <summary>
        /// The default REST endpoint
        /// </summary>
        public static string Rest => "https://www.exacttargetapis.com";
    }
}
