using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BassClefStudio.UWP.Background.AppServices.Core
{
    /// <summary>
    /// A basic <see cref="IAppService"/> that requests a scope or scopes using a given <see cref="IAppServiceAuthProvider"/>.
    /// </summary>
    public class AuthRequestAppService : CommandAppService
    {
        private IAppServiceAuthProvider AuthProvider { get; }
        
        /// <summary>
        /// Creates a new <see cref="AuthRequestAppService"/>.
        /// </summary>
        /// <param name="authProvider">The provider used for requesting authentication.</param>
        public AuthRequestAppService(IAppServiceAuthProvider authProvider) : base("auth", "Authentication Request", "Requests the app service provider to request a given string or string array 'scopes' for the application.")
        {
            AuthProvider = authProvider;
        }

        /// <inheritdoc/>
        protected override async Task<object> GetOutputInternal(AppServiceInput input)
        {
            if(input.ContainsKey("scopes"))
            {
                object scopes = input["scopes"];
                if (scopes is string s)
                {
                    await AuthProvider.RequestScopes(input.PackageFamilyName, new string[] { s });
                    return null;
                }
                else if (scopes is IEnumerable<string> ses)
                {
                    await AuthProvider.RequestScopes(input.PackageFamilyName, ses);
                    return null;
                }
                else
                {
                    throw new ArgumentException("The 'scopes' parameter is not convertible to a string or a collection of strings.", "scopes");
                }
            }
            else
            {
                throw new ArgumentException("Required input missing.", "scopes");
            }
        }
    }
}
