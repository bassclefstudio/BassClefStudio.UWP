using Autofac;
using BassClefStudio.UWP.Core;
using System;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace BassClefStudio.UWP.Services.Views
{
    /// <summary>
    /// Represents a service which can show the state of global progress or loading in an application.
    /// </summary>
    public interface IStatusBarService
    {
        /// <summary>
        /// Sets the loading state of the <see cref="IStatusBarService"/> to a <see cref="double"/> value between 0 or 1, or null.
        /// </summary>
        /// <param name="value">A <see cref="double"/> from 0 to 1 indicating progress shown on the status bar, or null if the status is indeterminate.</param>
        void SetProgress(double? value = null);

        /// <summary>
        /// Asynchronously starts the <see cref="IStatusBarService"/> with a given message.
        /// </summary>
        /// <param name="message">The <see cref="string"/> message to show, or null for no message.</param>
        Task StartAsync(string message = null);

        /// <summary>
        /// Hides and stops the <see cref="IStatusBarService"/> asynchronously.
        /// </summary>
        Task StopAsync();
    }

    public class MobileStatusBarService : IStatusBarService
    {
        /// <summary>
        /// Returns a <see cref="bool"/> value indicating whether the built-in mobile <see cref="IStatusBar"/> is available on the current device.
        /// </summary>
        public static bool IsSupported()
        {
            return ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");
        }

        public StatusBar StatusBar { get; }
        private bool Showing = false;

        public MobileStatusBarService()
        {
            if (IsSupported())
            {
                StatusBar = StatusBar.GetForCurrentView();
            }
        }

        /// <inheritdoc/>
        public void SetProgress(double? value = null)
        {
            if (StatusBar != null && Showing)
            { 
                StatusBar.ProgressIndicator.ProgressValue = value;
            }
        }

        /// <inheritdoc/>
        public async Task StartAsync(string message = null)
        {
            if (StatusBar != null)
            {
                await StatusBar.ProgressIndicator.ShowAsync();
                StatusBar.ProgressIndicator.Text = message;
                Showing = true;
            }
        }

        /// <inheritdoc/>
        public async Task StopAsync()
        {
            if (StatusBar != null)
            {
                Showing = false;
                await StatusBar.ProgressIndicator.HideAsync();
            }
        }
    }

    public class TitleStatusBarService : IStatusBarService
    {
        private bool Showing = false;

        public TitleStatusBarService()
        {
        }

        private bool CanShow()
        {
            return TitleBarService.ProgressControl != null;
        }

        public void SetProgress(double? value = null)
        {
            if(CanShow() && Showing)
            {
                TitleBarService.ProgressControl.IsIndeterminate = !value.HasValue;
                if (value.HasValue)
                {
                    TitleBarService.ProgressControl.Value = value.Value;
                }
            }
        }

        public async Task StartAsync(string message = null)
        {
            if (CanShow())
            {
                await DispatcherService.RunOnUIThread(
                    () => {
                        TitleBarService.ProgressControl.Visibility = Visibility.Visible;
                    });
                Showing = true;
            }
        }

        public async Task StopAsync()
        {
            if (CanShow())
            {
                Showing = false;
                await DispatcherService.RunOnUIThread(
                        () =>
                        {
                            TitleBarService.ProgressControl.Visibility = Visibility.Collapsed;
                        });
            }
        }
    }

    public static class StatusBarExtensions
    {
        public static void AddStatusBarService(this ContainerBuilder builder)
        {
            if(MobileStatusBarService.IsSupported())
            {
                builder.RegisterType<MobileStatusBarService>()
                    .SingleInstance()
                    .AsImplementedInterfaces();
            }
            else
            {
                builder.RegisterType<TitleStatusBarService>()
                    .SingleInstance()
                    .AsImplementedInterfaces();
            }
        }
    }
}
