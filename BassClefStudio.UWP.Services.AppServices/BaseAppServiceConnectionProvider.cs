using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using BassClefStudio.UWP.Background.AppServices;
using Windows.ApplicationModel.AppService;

namespace BassClefStudio.UWP.Services.AppServices
{
    /// <summary>
    /// Represents a basic <see cref="AppServiceConnectionInfo"/> for use with the UWP APIs where the sender and receiver are on the same device.
    /// </summary>
    public class BaseAppServiceConnectionInfo : AppServiceConnectionInfo
    {
        /// <summary>
        /// Represents the family name of the app package that wishes to 
        /// </summary>
        public string PackageFamilyName { get; }

        public BaseAppServiceConnectionInfo(string packageFamilyName, string appServiceName) : base(appServiceName)
        {
            PackageFamilyName = packageFamilyName;
        }
    }

    /// <summary>
    /// Represents a simple <see cref="IAppServiceConnectionProvider"/> which uses the built-in UWP APIs to connect to <see cref="IAppService"/>s on the same device.
    /// </summary>
    public class BaseAppServiceConnectionProvider : IAppServiceConnectionProvider
    {
        /// <summary>
        /// The <see cref="AppServiceConnection"/> object managed by the system to connect this <see cref="IAppServiceConnectionProvider"/> to an <see cref="IAppService"/>.
        /// </summary>
        public AppServiceConnection Connection { get; private set; }

        /// <inheritdoc/>
        public bool CanConnect(AppServiceConnectionInfo info)
        {
            return info != null && info is BaseAppServiceConnectionInfo;
        }

        /// <inheritdoc/>
        public async Task Connect(AppServiceConnectionInfo info)
        {
            if(info is BaseAppServiceConnectionInfo connectionInfo)
            {
                Connection = new AppServiceConnection();
                Connection.AppServiceName = connectionInfo.AppServiceName;
                Connection.PackageFamilyName = connectionInfo.PackageFamilyName;
                var status = await Connection.OpenAsync();
                if(status != AppServiceConnectionStatus.Success)
                {
                    throw new AppServiceConnectionException($"Failed to connect to {connectionInfo}, error code {status}.");
                }
            }
            else
            {
                throw new ArgumentException("Expected a non-null BaseAppServiceConnectionInfo as connection info.", "info");
            }
        }

        /// <inheritdoc/>
        public async Task<AppServiceOutput> SendMessage(AppServiceInput input)
        {
            var output = await Connection.SendMessageAsync(input.CreateInput());
            if (output.Status == AppServiceResponseStatus.Success)
            {
                return new AppServiceOutput(output.Message);
            }
            else
            {
                throw new AppServiceConnectionException($"Message failed to send: error code {output.Status}.");
            }
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            Connection.Dispose();
        }
    }

    public static class AppServiceConnectionExtensions
    {
        /// <summary>
        /// Registers the <see cref="BaseAppServiceConnectionProvider"/> as an <see cref="IAppServiceConnectionProvider"/> to provide a way to connect to basic <see cref="IAppService"/>s and send messages.
        /// </summary>
        public static void AddBaseAppServices(this ContainerBuilder builder)
        {
            builder.RegisterType<BaseAppServiceConnectionProvider>().AsImplementedInterfaces();
        }
    }
}
