using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BassClefStudio.UWP.Background.AppServices.Core
{
    /// <summary>
    /// Represents an <see cref="IAppServiceAuthProvider"/> that uses the local settings to store authorization information.
    /// </summary>
    public class SettingsAuthProvider : IAppServiceAuthProvider
    {
        private ApplicationDataContainer Settings { get; }

        /// <summary>
        /// Creates a new <see cref="SettingsAuthProvider"/>.
        /// </summary>
        public SettingsAuthProvider()
        {
            Settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            PendingRequestsList = new ObservableCollection<AppServiceAuthRequest>();
            PendingRequests = new ReadOnlyObservableCollection<AppServiceAuthRequest>(PendingRequestsList);
        }

        /// <inheritdoc/>
        public string[] GetScopes(string packageName)
        {
            var container = Settings.CreateContainer("Authorization", ApplicationDataCreateDisposition.Always);
            if(container.Values.ContainsKey(packageName))
            {
                return (container.Values[packageName] as string).Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                return new string[0];
            }
        }

        /// <inheritdoc/>
        public virtual async Task RequestScopesAsync(AppServiceAuthRequest request)
        {
            PendingRequestsList.Add(request);
        }

        /// <inheritdoc/>
        public ReadOnlyObservableCollection<AppServiceAuthRequest> PendingRequests { get; }
        private ObservableCollection<AppServiceAuthRequest> PendingRequestsList { get; }

        /// <inheritdoc/>
        public async Task AddScopesAsync(AppServiceAuthRequest request)
        {
            if (PendingRequestsList.Contains(request))
            {
                var container = Settings.CreateContainer("Authorization", ApplicationDataCreateDisposition.Always);
                if (container.Values.ContainsKey(request.PackageFamilyName))
                {
                    var values = (container.Values[request.PackageFamilyName] as string).Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    container.Values[request.PackageFamilyName] = string.Join(";", values.Concat(request.Scopes).Distinct());
                }
                else
                {
                    container.Values.Add(request.PackageFamilyName, string.Join(";", request.Scopes));
                }

                // Remove item from collection if now added.
                PendingRequestsList.Remove(request);
            }
            else
            {
                throw new ArgumentException("The given AppServiceAuthRequest had not been requested using the RequestScopesAsync method before attempting to be added.", "request");
            }
        }

        /// <inheritdoc/>
        public void RemoveScopes(AppServiceAuthRequest request)
        {
            var container = Settings.CreateContainer("Authorization", ApplicationDataCreateDisposition.Always);
            if (container.Values.ContainsKey(request.PackageFamilyName))
            {
                var values = (container.Values[request.PackageFamilyName] as string).Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                container.Values[request.PackageFamilyName] = string.Join(";", values.Except(request.Scopes));
            }
        }
    }
}
