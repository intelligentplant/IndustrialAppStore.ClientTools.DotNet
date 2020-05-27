using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using IntelligentPlant.DataCore.Client.Clients;

namespace IntelligentPlant.IndustrialAppStore.Client.Clients {
    public abstract class IasClientBase : ClientBase<IndustrialAppStoreHttpClientOptions> {

        protected override Uri BaseUrl {
            get { return Options.AppStoreUrl; }
        }


        protected IasClientBase(HttpClient httpClient, IndustrialAppStoreHttpClientOptions options)
            : base(httpClient, options) { }

    }
}
