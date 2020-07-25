using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace BassClefStudio.UWP.Lifecycle
{
    /// <summary>
    /// Represents a service that can manage an <see cref="Application"/>'s back navigation.
    /// </summary>
    public interface IBackHandler : ILifecycleHandler
    {
        /// <summary>
        /// A method that is called whenever the <see cref="Application"/> receieves a back navigation request.
        /// </summary>
        void BackRequested();
    }

    /// <summary>
    /// Represents a service that can manage an <see cref="Application"/>'s foreground activation.
    /// </summary>
    public interface IForegroundActivationHandler : ILifecycleHandler
    {
        /// <summary>
        /// A method that is called whenever the <see cref="Application"/> is activated in the foreground - i.e., by a user launching the application, through a file or URI scheme, or the like.
        /// </summary>
        void ForegroundActivated(IActivatedEventArgs args);
    }

    /// <summary>
    /// Represents a service that can manage an <see cref="Application"/>'s background activation.
    /// </summary>
    public interface IBackgroundActivationHandler : ILifecycleHandler
    {
        /// <summary>
        /// An asynchronous <see cref="Task"/> that is called whenever the <see cref="Application"/> is activated in the background - i.e., there is no UI showing. Here, the <see cref="ILifecycleHandler"/> can act on background tasks and app service requests.
        /// </summary>
        Task BackgroundActivated(BackgroundActivatedEventArgs args);
    }

    /// <summary>
    /// Represents a service that can manage an <see cref="Application"/>'s suspension from the foreground.
    /// </summary>
    public interface ISuspendingHandler : ILifecycleHandler
    {
        /// <summary>
        /// A method that is called whenever a foreground-activated <see cref="Application"/> is closing or returning to the background. Here, the <see cref="ILifecycleHandler"/> can dispose or broker resources before the <see cref="Application"/> has fully closed.
        /// </summary>
        void OnSuspending();
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
