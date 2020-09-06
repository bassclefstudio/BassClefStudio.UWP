using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BassClefStudio.UWP.Background.AppServices;

namespace BassClefStudio.UWP.Services.AppServices
{
    /// <summary>
    /// Represents information on how an <see cref="IAppServiceConnectionProvider"/> can connect to an <see cref="IAppService"/>.
    /// </summary>
    public abstract class AppServiceConnectionInfo
    {
        /// <summary>
        /// The name of the <see cref="IAppService"/> group to connect to.
        /// </summary>
        public string AppServiceName { get; }

        public AppServiceConnectionInfo(string appServiceName)
        {
            AppServiceName = appServiceName;
        }
    }
}
