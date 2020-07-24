using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.System.RemoteSystems;

namespace BassClefStudio.UWP.ApplicationModel.AppServices
{
    public interface IAppServiceConnection : IDisposable
    {
        Task<ValueSet> CallAppService(ValueSet inputs);
        Task<AppServiceOutput> CallAppService(AppServiceInput inputs);
    }

    public static class AppServiceConnectionService
    {
        public static async Task<IAppServiceConnection> CreateConnection(RemoteSystem remoteSystem, string serviceName, bool useApi = true, string clientId = null) => await CreateConnection(remoteSystem, serviceName, Package.Current.Id.FamilyName, useApi, clientId);
        public static async Task<IAppServiceConnection> CreateConnection(RemoteSystem remoteSystem, string serviceName, string packageName, bool useApi = true, string clientId = null)
        {
            if (useApi)
            {
                // Set up a new app service connection.
                AppServiceConnection serviceConnection = new AppServiceConnection
                {
                    AppServiceName = serviceName,
                    PackageFamilyName = packageName
                };

                // a valid RemoteSystem object is needed before going any further
                if (remoteSystem == null)
                {
                    throw new AppServiceException("A valid remote system instance is required to connect.");
                }

                // Create a remote system connection request for the given remote device
                RemoteSystemConnectionRequest connectionRequest = new RemoteSystemConnectionRequest(remoteSystem);
                

                // "open" the AppServiceConnection using the remote request
                AppServiceConnectionStatus status = await serviceConnection.OpenRemoteAsync(connectionRequest);

                // only continue if the connection opened successfully
                if (status != AppServiceConnectionStatus.Success)
                {
                    throw new AppServiceException($"Failed to connect to the service: Status {status}");
                }
                else
                {
                    return new ApiAppServiceConnection(serviceConnection);
                }
            }
            else
            {
                throw new NotImplementedException("Web app service connections are not currently supported.");
                //OAuthApi.Authentication.OAuthAuthenticationService authService =
                //    DefaultAuthenticationService.GetMicrosoftService(
                //        clientId,
                //        new string[][] 
                //        { 
                //            DefaultAuthenticationService.MicrosoftBasicScopes,
                //            new string[] { "Device.Command", "Device.Read" } 
                //        });
                //var account = await authService.TrySignIn();
                //if (account == null)
                //{
                //    throw new AppServiceException("Could not sign in to Microsoft account.");
                //}
                //else
                //{
                //    return new WebAppServiceConnection(remoteSystem, serviceName, packageName, new NET.OAuthApi.Api.ApiService(account));
                //}
            }
        }

        public static async Task<IAppServiceConnection> CreateConnection(string serviceName) => await CreateConnection(serviceName, Package.Current.Id.FamilyName);
        public static async Task<IAppServiceConnection> CreateConnection(string serviceName, string packageName)
        {
            AppServiceConnection serviceConnection = new AppServiceConnection()
            {
                // Here, we use the app service name defined in the app service 
                // provider's Package.appxmanifest file in the <Extension> section.
                AppServiceName = serviceName,

                // Use Windows.ApplicationModel.Package.Current.Id.FamilyName 
                // within the app service provider to get this value.
                //appServiceConnection.PackageFamilyName = packageName.ToString();
                PackageFamilyName = packageName
            };

            var status = await serviceConnection.OpenAsync();

            if (status != AppServiceConnectionStatus.Success)
            {
                throw new AppServiceException($"Failed to connect to the service: Status {status.ToString()}");
            }
            else
            {
                return new ApiAppServiceConnection(serviceConnection);
            }
        }
    }

    public class AppServiceException : Exception
    {
        public AppServiceException() { }
        public AppServiceException(string message) : base(message) { }
        public AppServiceException(string message, Exception inner) : base(message, inner) { }
    }
}
