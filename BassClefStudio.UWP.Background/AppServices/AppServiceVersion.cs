using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.UWP.Background.AppServices
{
    /// <summary>
    /// Represents static information about the version of an <see cref="IAppService"/> system, which is sent to and from <see cref="IAppService"/>s in messages.
    /// </summary>
    public static class AppServiceVersion
    {
        /// <summary>
        /// The version number of the <see cref="IAppService"/> API.
        /// </summary>
        public const int VersionNumber = 3;
    }
}
