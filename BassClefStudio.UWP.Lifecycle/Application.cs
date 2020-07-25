using Autofac;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.UI.Core;

namespace BassClefStudio.UWP.Lifecycle
{
    public abstract class Application : Windows.UI.Xaml.Application
    {
        /// <summary>
        /// The Autofac <see cref="IContainer"/> for resolving <see cref="ILifecycleHandler"/> objects in the <see cref="Application"/>.
        /// </summary>
        public IContainer LifecycleContainer { get; private set; }

        /// <summary>
        /// Gets the <see cref="Application"/> object for the current application.
        /// </summary>
        public static new Application Current => (Application)Windows.UI.Xaml.Application.Current;

        public Application()
        {
            ////Register system events
            this.Suspending += OnSuspending;
            SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            
            //// Create container.
            ContainerBuilder builder = new ContainerBuilder();
            BuildContainer(builder);
            LifecycleContainer = builder.Build();
        }

        /// <summary>
        /// Registers the <see cref="LifecycleContainer"/> with all of the needed <see cref="ILifecycleHandler"/>s for the <see cref="Application"/> scope.
        /// </summary>
        /// <param name="builder">The Autofac <see cref="ContainerBuilder"/> used to build the <see cref="IContainer"/>.</param>
        public abstract void BuildContainer(ContainerBuilder builder);

        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            var backHandlers = LifecycleContainer.Resolve<IEnumerable<IBackHandler>>();
            foreach (var handler in backHandlers.Where(h => h.Enabled))
            {
                handler.BackRequested();
            }
        }

        private void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var suspendHandlers = LifecycleContainer.Resolve<IEnumerable<ISuspendingHandler>>();
            foreach (var handler in suspendHandlers.Where(h => h.Enabled))
            {
                handler.OnSuspending();
            }
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var foregroundHandlers = LifecycleContainer.Resolve<IEnumerable<IForegroundActivationHandler>>();
            foreach (var handler in foregroundHandlers.Where(h => h.Enabled))
            {
                handler.ForegroundActivated(args);
            }
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            var foregroundHandlers = LifecycleContainer.Resolve<IEnumerable<IForegroundActivationHandler>>();
            foreach (var handler in foregroundHandlers.Where(h => h.Enabled))
            {
                handler.ForegroundActivated(args);
            }
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            var deferral = args.TaskInstance.GetDeferral();
            var toRun = new SynchronousTask(() => BackgroundActivated(args, deferral));
            toRun.RunTask();
        }

        private async Task BackgroundActivated(BackgroundActivatedEventArgs args, BackgroundTaskDeferral deferral)
        {
            var backgroundHandlers = LifecycleContainer.Resolve<IEnumerable<IBackgroundActivationHandler>>();
            foreach (var handler in backgroundHandlers.Where(h => h.Enabled))
            {
                await handler.BackgroundActivated(args);
            }

            deferral.Complete();
        }
    }
}
