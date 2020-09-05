using Autofac;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace BassClefStudio.UWP.Services.Views
{
    /// <summary>
    /// Represents a service that can hide the default UWP title bar and setup custom UI for the title bar.
    /// </summary>
    public class TitleBarService
    {
        /// <summary>
        /// Represents the <see cref="FrameworkElement"/> that is being used as the custom title bar.
        /// </summary>
        public FrameworkElement TitleBar { get; internal set; }

        /// <summary>
        /// Represents a <see cref="ProgressBar"/> that can display application loading information in the title bar.
        /// </summary>
        public ProgressBar ProgressControl { get; internal set; }

        /// <summary>
        /// Hides the system title bar and optionally replaces it with a <see cref="FrameworkElement"/> title bar.
        /// </summary>
        /// <param name="titleBarElement">The custom title bar. If null, no title bar is present, and the close butttons will appear overlayed on content.</param>
        /// <param name="progressControl">A <see cref="ProgressBar"/> that can display application loading information in the title bar.</param>
        public void HideTitleBar(FrameworkElement titleBarElement = null, ProgressBar progressControl = null)
        {
            var bar = CoreApplication.GetCurrentView().TitleBar;
            bar.ExtendViewIntoTitleBar = true;

            TitleBar = titleBarElement;
            if (TitleBar != null)
            {
                Window.Current.SetTitleBar(TitleBar);
                bar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            }

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            ProgressControl = progressControl;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            TitleBar.Height = sender.Height;
        }
    }

    public static class TitleBarExtensions
    {
        /// <summary>
        /// Registers the <see cref="TitleBarService"/> as a singleton service.
        /// </summary>
        public static void AddTitleBarService(this ContainerBuilder builder)
        {
            builder.RegisterType<TitleBarService>()
                .SingleInstance();
        }
    }
}
