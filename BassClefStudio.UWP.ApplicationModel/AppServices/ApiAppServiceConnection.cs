using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace BassClefStudio.UWP.ApplicationModel.AppServices
{
    public class ApiAppServiceConnection : IAppServiceConnection
    {
        private AppServiceConnection appServiceConnection;
        internal ApiAppServiceConnection(AppServiceConnection serviceConnection)
        {
            appServiceConnection = serviceConnection;
        }

        public async Task<ValueSet> CallAppService(ValueSet inputs)
        {
            // Add the connection.
            if (appServiceConnection == null)
            {
                throw new AppServiceException("No App Service is connected.");
            }

            AppServiceResponse response = await appServiceConnection.SendMessageAsync(inputs);

            if (response.Status == AppServiceResponseStatus.Success)
            {
                return response.Message;
            }
            else
            {
                throw new AppServiceException("App Service call failed.");
            }
        }

        public async Task<AppServiceOutput> CallAppService(AppServiceInput inputs)
        {
            // Add the connection.
            if (appServiceConnection == null)
            {
                throw new AppServiceException("No App Service is connected.");
            }

            AppServiceResponse response = await appServiceConnection.SendMessageAsync(inputs.ToValueSet());

            if (response.Status == AppServiceResponseStatus.Success)
            {
                try
                {
                    return AppServiceOutput.Parse(response.Message);
                }
                catch
                {
                    throw new AppServiceException("Return message parse failed.");
                }
            }
            else
            {
                throw new AppServiceException("App Service call failed.");
            }
        }

        public void Dispose()
        {
            appServiceConnection.Dispose();
        }
    }
}
