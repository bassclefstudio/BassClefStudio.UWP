using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.UWP.Background.AppServices
{
    /// <summary>
    /// Represents a service that stores and retreives authorization data regarding this app's <see cref="IAppService"/>s.
    /// </summary>
    public interface IAppServiceAuthProvider
    {
        /// <summary>
        /// Returns a list of <see cref="string"/> scopes defining the actions an app can perform on this app's <see cref="IAppService"/>s.
        /// </summary>
        /// <param name="packageName">The app package's family name.</param>
        string[] GetScopes(string packageName);

        /// <summary>
        /// Requests for the given scopes to be added to an app's capabilities.
        /// </summary>
        /// <param name="packageName">The app package's family name.</param>
        /// <param name="scopes">The scopes to add.</param>
        Task RequestScopes(string packageName, IEnumerable<string> scopes);

        /// <summary>
        /// Removes the given scopes from an app's capabilities.
        /// </summary>
        /// <param name="packageName">The app package's family name.</param>
        /// <param name="scopes">The scopes to remove.</param>
        void RemoveScopes(string packageName, IEnumerable<string> scopes);
    }
}
