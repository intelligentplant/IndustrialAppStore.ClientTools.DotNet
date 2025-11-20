namespace IntelligentPlant.DataCore.Client.Clients {

    /// <summary>
    /// Client for querying the App Store Connect adapters.
    /// </summary>
    /// <remarks>
    ///   App Store Connect adapters are next-generation data connectors for the Industrial App Store Data API.
    /// </remarks>
    public sealed partial class AdaptersClient : ClientBase {

        /// <summary>
        /// The adapter custom functions client.
        /// </summary>
        public CustomFunctionsClient CustomFunctions { get; }

        
        internal AdaptersClient(HttpClient httpClient, DataCoreHttpClientOptions options) : base(httpClient, options) { 
            CustomFunctions = new CustomFunctionsClient(this);
        }

    }

}
