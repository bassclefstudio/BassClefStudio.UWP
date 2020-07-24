using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;

namespace BassClefStudio.UWP.Lifecycle
{
    public abstract class Application : Windows.UI.Xaml.Application
    {
        public Application()
        {
            ////Register system events
            this.Suspending += OnSuspending;
            SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
        }

        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            throw new NotImplementedException();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            throw new NotImplementedException();
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
