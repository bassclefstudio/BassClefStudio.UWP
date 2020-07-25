using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace BassClefStudio.UWP.Background.Tasks
{
    /// <summary>
    /// Represents a service that can manage the calling and registering of a background task.
    /// </summary>
    public interface IBackgroundService
    {
        /// <summary>
        /// The referencable name of the <see cref="IBackgroundService"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The attached <see cref="IBackgroundTaskRegistration"/> registered to the system.
        /// </summary>
        IBackgroundTaskRegistration Registration { get; }

        /// <summary>
        /// A <see cref="bool"/> value indicating whether the background task is registered and enabled.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// A <see cref="Task"/> that is called when the background task runs.
        /// </summary>
        /// <param name="task">The <see cref="IBackgroundTaskInstance"/> describing the background task that is running.</param>
        Task RunAsync(IBackgroundTaskInstance task);

        /// <summary>
        /// Asynchronously registers the associated background task, re-registering if <paramref name="activateIfDisabled"/> is set to true, and sets the associated <see cref="Registration"/>.
        /// </summary>
        /// <param name="reregisterTask">A <see cref="bool"/> indicating whether to unregister a currently registered attached background task.</param>
        Task<bool> RegisterTaskAsync(bool reregisterTask = false);

        /// <summary>
        /// Unregisters the associated background task, optionally canceling any currently running instances.
        /// </summary>
        /// <param name="cancelTask">A <see cref="bool"/> indicating whether a currently running task should be immediately canceled.</param>
        void UnRegisterTask(bool cancelTask = false);
    }

    public static class BackgroundHandlerExtensions
    {
        /// <summary>
        /// Gets the <see cref="IBackgroundService"/> from a collection of <see cref="IBackgroundService"/>s that handles the given <see cref="IBackgroundTaskInstance"/>.
        /// </summary>
        /// <param name="taskInstance">The <see cref="IBackgroundTaskInstance"/> to handle.</param>
        public static IBackgroundService GetBackgroundHandler(this IEnumerable<IBackgroundService> handlers, IBackgroundTaskInstance taskInstance)
            => handlers.GetBackgroundHandler(taskInstance.Task);

        /// <summary>
        /// Gets the <see cref="IBackgroundService"/> from a collection of <see cref="IBackgroundService"/>s that handles the given <see cref="IBackgroundTaskRegistration"/>.
        /// </summary>
        /// <param name="taskRegistration">The <see cref="IBackgroundTaskRegistration"/> to handle.</param>
        public static IBackgroundService GetBackgroundHandler(this IEnumerable<IBackgroundService> handlers, IBackgroundTaskRegistration taskRegistration)
        {
            return handlers.FirstOrDefault(h => h.Name == taskRegistration.Name);
        }
    }
}
