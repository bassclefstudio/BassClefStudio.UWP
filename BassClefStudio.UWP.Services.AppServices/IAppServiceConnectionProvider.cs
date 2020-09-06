using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BassClefStudio.UWP.Background.AppServices;

namespace BassClefStudio.UWP.Services.AppServices
{
    /// <summary>
    /// Represents a provider that can connect to an <see cref="IAppService"/> on another app and call methods from that app.
    /// </summary>
    public interface IAppServiceConnectionProvider
    {
        /// <summary>
        /// Returns a <see cref="bool"/> value indicating whether this <see cref="IAppServiceConnectionProvider"/> can connect using the given <see cref="AppServiceConnectionInfo"/>.
        /// </summary>
        /// <param name="info">The information about the connection.</param>
        bool CanConnect(AppServiceConnectionInfo info);

        /// <summary>
        /// Connects to an app service as specified by the given <see cref="AppServiceConnectionInfo"/>.
        /// </summary>
        /// <param name="info">The information about the connection.</param>
        Task Connect(AppServiceConnectionInfo info);
        
        /// <summary>
        /// Sends an <see cref="AppServiceInput"/> message to a connected <see cref="IAppService"/> and returns the <see cref="AppServiceOutput"/> output.
        /// </summary>
        /// <param name="input">The message inputs and command name in an <see cref="AppServiceInput"/> object.</param>
        Task<AppServiceOutput> SendMessage(AppServiceInput input);

        /// <summary>
        /// Disconnects from the <see cref="IAppService"/> and disposes any connection data.
        /// </summary>
        void Disconnect();
    }

    public class AppServiceConnectionException : Exception
    {
        public AppServiceConnectionException() { }
        public AppServiceConnectionException(string message) : base(message) { }
        public AppServiceConnectionException(string message, Exception inner) : base(message, inner) { }
    }

    public static class AppServiceConnectionProviderExtensions
    {
        /// <summary>
        /// Connects to an app service as specified by the given <see cref="AppServiceConnectionInfo"/>.
        /// </summary>
        /// <param name="info">The information about the connection.</param>
        public static async Task<IAppServiceConnectionProvider> Connect(this IEnumerable<IAppServiceConnectionProvider> providers, AppServiceConnectionInfo info)
        {
            var provider = providers.FirstOrDefault(p => p.CanConnect(info));
            if(provider != null)
            {
                await provider.Connect(info);
                return provider;
            }
            else
            {
                throw new AppServiceConnectionException("No IAppServiceConnectionProviders could connect using the given connection info.");
            }
        }
    }
}
