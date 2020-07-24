//using BassClefStudio.NET.OAuthApi.Api;
//using BassClefStudio.NET.OAuthApi.Authentication;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Windows.Foundation.Collections;
//using Windows.System.RemoteSystems;

//namespace BassClefStudio.UWP.ApplicationModel.AppServices
//{
//    public class WebAppServiceConnection : IAppServiceConnection
//    {
//        private RemoteSystem RemoteSystem;
//        private string ServiceName;
//        private string PackageName;
//        private ApiService ApiService;

//        internal WebAppServiceConnection(RemoteSystem remoteSystem, string serviceName, string packageName, ApiService apiService)
//        {
//            RemoteSystem = remoteSystem;
//            ServiceName = serviceName;
//            PackageName = packageName;
//            ApiService = apiService;
//        }

//        public async Task<ValueSet> CallAppService(ValueSet inputs)
//        {
//            WebAppServiceCall call = new WebAppServiceCall()
//            {
//                Type = "appService",
//                Payload = inputs,
//                PackageFamilyName = PackageName,
//                AppServiceName = ServiceName
//            };
//            string callJson = JsonConvert.SerializeObject(call);

//            var response = await ApiService.PostAsync(
//                $"https://graph.microsoft.com/beta/me/devices/{RemoteSystem.Id}/commands",
//                callJson);

//            return response.Value<ValueSet>("responsePayload");
//        }

//        public async Task<AppServiceOutput> CallAppService(AppServiceInput inputs)
//        {
//            WebAppServiceCall call = new WebAppServiceCall()
//            {
//                Type = "appService",
//                Payload = inputs.ToValueSet(),
//                PackageFamilyName = PackageName,
//                AppServiceName = ServiceName
//            };
//            string callJson = JsonConvert.SerializeObject(call);

//            var response = await ApiService.PostAsync(
//                $"https://graph.microsoft.com/beta/me/devices/{RemoteSystem.Id}/commands",
//                callJson);

//            return AppServiceOutput.Parse(response.Value<ValueSet>("responsePayload"));
//        }

//        public void Dispose()
//        {
//            return;
//        }
//    }

//    public class WebAppServiceCall
//    {
//        [JsonProperty(PropertyName = "type")]
//        public string Type { get; set; }

//        [JsonProperty(PropertyName = "payload")]
//        public ValueSet Payload { get; set; }

//        [JsonProperty(PropertyName = "packageFamilyName")]
//        public string PackageFamilyName { get; set; }

//        [JsonProperty(PropertyName = "appServiceName")]
//        public string AppServiceName { get; set; }
//    }
//}
