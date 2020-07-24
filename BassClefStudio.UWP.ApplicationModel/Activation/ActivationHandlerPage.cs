using BassClefStudio.UWP.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BassClefStudio.UWP.ApplicationModel.Activation
{
    /// <summary>
    /// Represents a <see cref="Page"/> that can manage through navigation requests and thus act as a shell page for application launch.
    /// </summary>
    public abstract class ActivationHandlerPage : Page
    {
        /// <summary>
        /// The <see cref="Frame"/> in the page that will be used for future navigation.
        /// </summary>
        public abstract Frame NavigationFrame { get; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.Frame = NavigationFrame;
            if (e.Parameter is ActivationInfo info)
            {
                NavigationService.Navigate(info.ChildPageType, GetChildParameter(info));
            }
        }

        /// <summary>
        /// Gets the <see cref="object"/> to pass through to the child page when navigating.
        /// </summary>
        /// <param name="info">The given <see cref="ActivationInfo"/> from the <see cref="Application"/>.</param>
        public abstract object GetChildParameter(ActivationInfo info);
    }
}
