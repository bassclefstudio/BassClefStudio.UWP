using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;

namespace BassClefStudio.UWP.Lifecycle
{
    /// <summary>
    /// Represents a service that can initialize a particular service when the <see cref="Application"/> initializes.
    /// </summary>
    public interface IInitializationHandler : ILifecycleHandler
    {
        /// <summary>
        /// A method that is called when the <see cref="Application"/> initializes.
        /// </summary>
        /// <param name="app">The app's <see cref="Application"/> object.</param>
        /// <returns>A <see cref="bool"/> value indicating whether any action was performed successfully.</returns>
        bool Initialize(Application app);
    }

    /// <summary>
    /// Represents a service that can manage an <see cref="Application"/>'s back navigation.
    /// </summary>
    public interface IBackHandler : ILifecycleHandler
    {
        /// <summary>
        /// A method that is called whenever the <see cref="Application"/> receieves a back navigation request.
        /// </summary>
        /// <param name="app">The app's <see cref="Application"/> object.</param>
        /// <returns>A <see cref="bool"/> value indicating whether any action was performed successfully.</returns>
        bool BackRequested(Application app);
    }

    /// <summary>
    /// Represents a service that can manage an <see cref="Application"/>'s suspension from the foreground.
    /// </summary>
    public interface ISuspendingHandler : ILifecycleHandler
    {
        /// <summary>
        /// A method that is called whenever a foreground-activated <see cref="Application"/> is closing or returning to the background. Here, the <see cref="ILifecycleHandler"/> can dispose or broker resources before the <see cref="Application"/> has fully closed.
        /// </summary>
        /// <param name="app">The app's <see cref="Application"/> object.</param>
        /// <param name="args">A <see cref="SuspendingOperation"/> argument containing information about the suspending application.</param>
        /// <returns>A <see cref="bool"/> value indicating whether any action was performed successfully.</returns>
        bool OnSuspending(Application app, SuspendingOperation args);
    }

    /// <summary>
    /// Represents a service that can manage an <see cref="Application"/>'s foreground activation.
    /// </summary>
    public interface IForegroundActivationHandler : ILifecycleHandler
    {
        /// <summary>
        /// A method that is called whenever the <see cref="Application"/> is activated in the foreground - i.e., by a user launching the application, through a file or URI scheme, or the like.
        /// </summary>
        /// <param name="app">The app's <see cref="Application"/> object.</param>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> which describe the foreground activation.</param>
        ///<returns>A <see cref="bool"/> value indicating whether any action was performed successfully.</returns>
        bool ForegroundActivated(Application app, IActivatedEventArgs args);
    }

    /// <summary>
    /// Represents a service that can manage an <see cref="Application"/>'s window initialization and initial navigation.
    /// </summary>
    public interface INavigationHandler : ILifecycleHandler
    {
        /// <summary>
        /// A method that is called whenever a foreground <see cref="Application"/> is called to create the UI.
        /// </summary>
        /// <param name="app">The app's <see cref="Application"/> object.</param>
        /// <param name="pageType">The <see cref="Type"/> of the page to navigate to.</param>
        /// <param name="parameter">Any parameter that should be passed to the navigated page.</param>
        /// <param name="shellPageType">If specified, the <see cref="INavigationHandler"/> should use the page with the given <see cref="Type"/> as a shell (navigation) page.</param>
        /// <returns>A <see cref="bool"/> value indicating whether any action was performed successfully.</returns>
        bool ActivateWindow(Application app, Type pageType, object parameter = null, Type shellPageType = null);
    }

    /// <summary>
    /// Represents a service that can manage an <see cref="Application"/>'s background activation.
    /// </summary>
    public interface IBackgroundActivationHandler : ILifecycleHandler
    {
        /// <summary>
        /// An asynchronous <see cref="Task"/> that is called whenever the <see cref="Application"/> is activated in the background - i.e., there is no UI showing. Here, the <see cref="ILifecycleHandler"/> can act on background tasks and app service requests.
        /// </summary>
        /// <param name="app">The app's <see cref="Application"/> object.</param>
        /// <param name="args">The <see cref="BackgroundActivatedEventArgs"/> which describe the background activation.</param>
        /// <returns>A <see cref="bool"/> value indicating whether any action was performed successfully.</returns>
        bool BackgroundActivated(Application app, BackgroundActivatedEventArgs args);
    }

    /// <summary>
    /// Represents a service that can handle lifecycle events from an <see cref="Application"/>.
    /// </summary>
    public interface ILifecycleHandler
    {
        /// <summary>
        /// A <see cref="bool"/> value indicating whether the <see cref="ILifecycleHandler"/> is enabled.
        /// </summary>
        bool Enabled { get; }
    }
}
