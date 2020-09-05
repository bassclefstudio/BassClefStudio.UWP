using BassClefStudio.UWP.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace BassClefStudio.UWP.Navigation.Extensions
{
    /// <summary>
    /// An abstract <see cref="IForegroundActivationHandler"/> that can provide an <see cref="INavigationHandler"/> to start navigation based on received <see cref="IActivatedEventArgs"/>.
    /// </summary>
    public abstract class NavigationActivationHandler<T> : IForegroundActivationHandler where T : IActivatedEventArgs
    {
        /// <inheritdoc/>
        public bool Enabled { get; }

        /// <summary>
        /// A collection of <see cref="INavigationHandler"/>s available to navigation to the application window.
        /// </summary>
        public IEnumerable<INavigationHandler> NavigationHandlers { get; }

        /// <summary>
        /// Creates a new <see cref="NavigationActivationHandler{T}"/>.
        /// </summary>
        public NavigationActivationHandler(IEnumerable<INavigationHandler> handlers)
        {
            Enabled = true;
            NavigationHandlers = handlers;
        }

        /// <summary>
        /// Starts the navigation and activation of the application window through the <see cref="INavigationHandler"/> provided, given the received <see cref="IActivatedEventArgs"/> from foreground activation.
        /// </summary>
        /// <param name="app">The current <see cref="Application"/> instance.</param>
        /// <param name="args">A <typeparamref name="T"/> activation args, describing how the <see cref="Application"/> was activated.</param>
        /// <param name="handler">The <see cref="INavigationHandler"/> which can perform navigation.</param>
        public abstract bool StartNavigation(Application app, T args, INavigationHandler handler);

        /// <inheritdoc/>
        public bool ForegroundActivated(Application app, IActivatedEventArgs args)
        {
            if(args is T a)
            {
                var handler = NavigationHandlers.FirstOrDefault(h => h.Enabled);
                if(handler != null)
                {
                    return StartNavigation(app, a, handler);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
