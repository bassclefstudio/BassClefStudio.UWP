using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// Sends a request to the user and application that a given application wishes to access the given scopes.
        /// </summary>
        /// <param name="request">An <see cref="AppServiceAuthRequest"/> detailing the scopes to request.</param>
        Task RequestScopesAsync(AppServiceAuthRequest request);

        /// <summary>
        /// Adds the given scopes to an app's capabilities.
        /// </summary>
        /// <param name="request">An <see cref="AppServiceAuthRequest"/> detailing the scopes to add.</param>
        Task AddScopesAsync(AppServiceAuthRequest request);

        /// <summary>
        /// Gets a collection of <see cref="AppServiceAuthRequest"/>s that have been requested by other applications but not yet approved by the app/user.
        /// </summary>
        ReadOnlyObservableCollection<AppServiceAuthRequest> PendingRequests { get; }

        /// <summary>
        /// Removes the given scopes from an app's capabilities.
        /// </summary>
        /// <param name="request">An <see cref="AppServiceAuthRequest"/> detailing the scopes to remove.</param>
        void RemoveScopes(AppServiceAuthRequest request);
    }

    /// <summary>
    /// Represents a request for the <see cref="IAppServiceAuthProvider"/> to perform an action (add, remove, request) for the given application and the collection of <see cref="string"/> scopes.
    /// </summary>
    public class AppServiceAuthRequest
    {
        /// <summary>
        /// The app package's family name.
        /// </summary>
        public string PackageFamilyName { get; }

        /// <summary>
        /// The given <see cref="string"/> authorization scopes.
        /// </summary>
        public IEnumerable<string> Scopes { get; }

        /// <summary>
        /// Creates a new <see cref="AppServiceAuthRequest"/>
        /// </summary>
        /// <param name="packageName">The app package's family name.</param>
        /// <param name="scopes">The given <see cref="string"/> authorization scopes.</param>
        public AppServiceAuthRequest(string packageName, IEnumerable<string> scopes)
        {
            PackageFamilyName = packageName;
            Scopes = scopes;
        }
    }
}
