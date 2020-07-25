using Autofac;
using BassClefStudio.UWP.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BassClefStudio.UWP.Navigation.Extensions
{
    /// <summary>
    /// An <see cref="INavigationHandler"/> and <see cref="IBackHandler"/> using the <see cref="NavigationService"/> for the basis of navigation and window activation.
    /// </summary>
    public class NavigationServiceHandler : INavigationHandler, IBackHandler
    {
        /// <inheritdoc/>
        public bool Enabled { get; }

        public NavigationServiceHandler()
        {
            Enabled = true;
        }

        /// <inheritdoc/>
        public bool ActivateWindow(Lifecycle.Application app, Type pageType, object parameter, Type shellPageType = null)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            //// Do not repeat app initialization when the Window already has content,
            //// just ensure that the window is active
            if (rootFrame == null)
            {
                //// Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.Content = new Frame();
                NavigationService.Frame = rootFrame;

                //// Navigate to the page...
                if(shellPageType != null)
                {
                    NavigationService.Navigate(shellPageType);
                }

                NavigationService.Navigate(pageType, parameter);
            }

            //// Ensure the current window is active, and then setup back navigation.
            Window.Current.Activate();
            app.InitializeBackNavigation();

            return true;
        }

        /// <inheritdoc/>
        public bool BackRequested(Lifecycle.Application app)
        {
            if(NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public static class NavigationBuilderExtensions
    {
        public static void AddNavigationService(this ContainerBuilder builder)
        {
            builder.RegisterType<NavigationServiceHandler>().AsImplementedInterfaces();
        }
    }
}
