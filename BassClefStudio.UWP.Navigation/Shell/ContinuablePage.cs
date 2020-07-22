using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BassClefStudio.UWP.Navigation.Shell
{
    /// <summary>
    /// Represents a <see cref="Page"/> that can be navigated to on application launch and referenced by a <see cref="Uri"/>, with support for state information.
    /// </summary>
    public abstract class ContinuablePage : Page
    {
        /// <summary>
        /// The name of the page, as used in the uri scheme.
        /// </summary>
        public static string BaseLink { get; set; }

        /// <summary>
        /// Returns a collection of key/value pairs for the query string that will be sent in the uri for this page.
        /// </summary>
        public abstract IDictionary<string, string> GetContinueQueries();

        /// <summary>
        /// Initializes the given <see cref="ContinuablePage"/> asynchronously with the given query information.
        /// </summary>
        /// <param name="qs">The query string key/value pairs.</param>
        public abstract Task ContinueFromUri(IDictionary<string, string> qs);

        /// <summary>
        /// Returns a <see cref="Uri"/> linking to this <see cref="ContinuablePage"/> in its current state.
        /// </summary>
        public Uri GetContinueUri()
        {
            var query = new Microsoft.QueryStringDotNET.QueryString();
            foreach (var q in GetContinueQueries())
            {
                query.Add(q.Key, q.Value);
            }

            string qString = (query.ToString() != null ? $"?{query.ToString()}" : null);
            return new Uri($"{BaseLink}:{this.GetType().Name}{qString}");
        }

        /// <inheritdoc/>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is IDictionary<string, string>)
            {
                ContinueFromUri(e.Parameter as IDictionary<string, string>);
            }
        }
    }
}
