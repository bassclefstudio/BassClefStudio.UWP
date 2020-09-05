using Autofac;
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
    /// <summary>
    /// A wrapper class over the <see cref="Windows.UI.Xaml.Application"/>, this class provides a dependency-injection-driven way to create services, add navigation, and support application lifecyle in a UWP project, similarly to the way in which other .NET projects (such as ASP.NET Core) function.
    /// </summary>
    public abstract class Application : Windows.UI.Xaml.Application
    {
        #region Initialiation

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
            
            //// Create container.
            ContainerBuilder builder = new ContainerBuilder();
            BuildContainer(builder);
            LifecycleContainer = builder.Build();

            //// Get initialization handlers.
            var initHandlers = LifecycleContainer.Resolve<IEnumerable<IInitializationHandler>>();
            foreach (var handler in initHandlers.Where(h => h.Enabled))
            {
                handler.Initialize(this);
            }
        }

        /// <summary>
        /// Registers the <see cref="LifecycleContainer"/> with all of the needed <see cref="ILifecycleHandler"/>s for the <see cref="Application"/> scope.
        /// </summary>
        /// <param name="builder">The Autofac <see cref="ContainerBuilder"/> used to build the <see cref="IContainer"/>.</param>
        public abstract void BuildContainer(ContainerBuilder builder);

        private bool backHandled = false;
        /// <summary>
        /// Initializes the back navigation event, which the <see cref="IBackHandler"/> uses to handle the back navigation system button.
        /// </summary>
        public void InitializeBackNavigation()
        {
            if(!backHandled)
            {
                SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            }

            backHandled = true;
        }

        #endregion
        #region Events

        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            var backHandler = LifecycleContainer.Resolve<IBackHandler>();
            backHandler.BackRequested(this);
            e.Handled = true;
        }

        private void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var suspendHandlers = LifecycleContainer.Resolve<IEnumerable<ISuspendingHandler>>();
            foreach (var handler in suspendHandlers.Where(h => h.Enabled))
            {
                handler.OnSuspending(this);
            }
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            var foregroundHandlers = LifecycleContainer.Resolve<IEnumerable<IForegroundActivationHandler>>();
            foreach (var handler in foregroundHandlers.Where(h => h.Enabled))
            {
                handler.ForegroundActivated(this, args);
            }
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            var foregroundHandlers = LifecycleContainer.Resolve<IEnumerable<IForegroundActivationHandler>>();
            foreach (var handler in foregroundHandlers.Where(h => h.Enabled))
            {
                handler.ForegroundActivated(this, args);
            }
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            var backgroundHandlers = LifecycleContainer.Resolve<IEnumerable<IBackgroundActivationHandler>>();
            foreach (var handler in backgroundHandlers.Where(h => h.Enabled))
            {
                handler.BackgroundActivated(this, args);
            }
        }

        #endregion
        #region Methods

        

        #endregion
    }
}
