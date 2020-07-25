using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace BassClefStudio.UWP.Background
{
    /// <summary>
    /// Represents a service that can manage the calling and registering of a given background task.
    /// </summary>
    public class BackgroundHandler
    {
        /// <summary>
        /// The referencable name of the <see cref="BackgroundHandler"/>.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// The <see cref="IBackgroundTrigger"/> that will trigger the background task.
        /// </summary>
        public IBackgroundTrigger Trigger { get; }

        /// <summary>
        /// The <see cref="System.Threading.Tasks.Task"/> that will be run when the background task is called.
        /// </summary>
        public Func<IBackgroundTaskInstance, Task> Task { get; }

        /// <summary>
        /// A <see cref="bool"/> value indicating whether the network stack needs to be active when the background task is active.
        /// </summary>
        public bool IsNetworkRequested { get; }

        /// <summary>
        /// A <see cref="bool"/> value that indicates whether background tasks should be reregistered every time the <see cref="RegisterTaskAsync(bool)"/> task is called.
        /// </summary>
        public bool IsDebugEnabled { get; }

        /// <summary>
        /// The attached <see cref="IBackgroundTaskRegistration"/> registered to the system.
        /// </summary>
        public IBackgroundTaskRegistration Registration { get; private set; }

        /// <summary>
        /// A <see cref="bool"/> value indicating whether the background task is registered and enabled.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// An event that fires when the background task is unregistering.
        /// </summary>
        public event EventHandler Unregistering;

        /// <summary>
        /// An event that fires when the background task is registering.
        /// </summary>
        public event EventHandler Registering;

        /// <summary>
        /// An event that fires when the <see cref="BackgroundHandler.IsActive"/> changes.
        /// </summary>
        public event EventHandler IsActiveChanged;

        /// <summary>
        /// A collection of all the registered <see cref="BackgroundHandler"/>s.
        /// </summary>
        public static List<BackgroundHandler> BackgroundHandlers { get; }
        static BackgroundHandler()
        {
            BackgroundHandlers = new List<BackgroundHandler>();
        }

        /// <summary>
        /// Creates a new <see cref="BackgroundHandler"/> from the given information and associated <see cref="System.Threading.Tasks.Task"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="BackgroundHandler"/>.</param>
        /// <param name="task">The <see cref="System.Threading.Tasks.Task"/> that will be run when the background task is called.</param>
        /// <param name="trigger">The <see cref="IBackgroundTrigger"/> that will trigger the background task.</param>
        /// <param name="isNetworkRequested">A <see cref="bool"/> value indicating whether the network stack needs to be active when the background task is active.</param>
        /// <param name="isDebugEnabled">A <see cref="bool"/> value that indicates whether background tasks should be reregistered every time the <see cref="RegisterTaskAsync(bool)"/> task is called.</param>
        public BackgroundHandler(string name, Func<IBackgroundTaskInstance, Task> task, IBackgroundTrigger trigger, bool isNetworkRequested = false, bool isDebugEnabled = false)
        {
            Name = name;
            Task = task;
            Trigger = trigger;
            IsNetworkRequested = isNetworkRequested;
            IsDebugEnabled = isDebugEnabled;

            GetIsActive();
        }

        /// <summary>
        /// Creates a new <see cref="BackgroundHandler"/> from the given information and associated <see cref="System.Threading.Tasks.Task"/>, triggering on regular time intervals as given by <paramref name="minutes"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="BackgroundHandler"/>.</param>
        /// <param name="task">The <see cref="System.Threading.Tasks.Task"/> that will be run when the background task is called.</param>
        /// <param name="minutes">The number of minutes to set the <see cref="TimeTrigger"/> that will trigger the background task.</param>
        /// <param name="isNetworkRequested">A <see cref="bool"/> value indicating whether the network stack needs to be active when the background task is active.</param>
        /// <param name="isDebugEnabled">A <see cref="bool"/> value that indicates whether background tasks should be reregistered every time the <see cref="RegisterTaskAsync(bool)"/> task is called.</param>
        public BackgroundHandler(string name, Func<IBackgroundTaskInstance, Task> task, uint minutes, bool isNetworkRequested = false, bool isDebugEnabled = false)
            : this(name, task, new TimeTrigger(minutes, false), isNetworkRequested, isDebugEnabled) { }

        /// <summary>
        /// Completes the registration of all <see cref="BackgroundHandlers"/> by unregistering undeclared tasks.
        /// </summary>
        public static void CompleteRegistrations()
        {
            var toRemove = BackgroundTaskRegistration.AllTasks.Where(t => !BackgroundHandlers.Any(h => h.Name == t.Value.Name));
            Debug.WriteLine($"Unregistering {toRemove.Count()} background tasks.");
            foreach (var r in toRemove)
            {
                r.Value.Unregister(false);
            }
        }

        private void SetActive(bool active)
        {
            IsActive = active;

            if(active && !BackgroundHandlers.Contains(this))
            {
                BackgroundHandlers.Add(this);
            }
            else if(!active && BackgroundHandlers.Contains(this))
            {
                BackgroundHandlers.Remove(this);
            }

            if (IsActive)
            {
                Debug.WriteLine("Background task started and registered.");
            }
            else
            {
                Debug.WriteLine("Background task unregistered or failed to start.");
            }

            IsActiveChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Gets a value indicating whether the background task is active and registeres, and sets <see cref="IsActive"/>.
        /// </summary>
        public void GetIsActive()
        {
            var existing = BackgroundTaskRegistration.AllTasks.FirstOrDefault(t => t.Value.Name == this.Name).Value;
            if (existing == null)
            {
                SetActive(false);
            }
            else if (IsDebugEnabled)
            {
                existing.Unregister(true);
                SetActive(false);
            }
            else
            {
                Registration = existing;
                SetActive(true);
            }
        }

        /// <summary>
        /// Asynchronously registers the associated background task, re-registering if <paramref name="activateIfDisabled"/> or <see cref="IsDebugEnabled"/> are set to true, and sets <see cref="Registration"/>.
        /// </summary>
        /// <param name="activateIfDisabled">A <see cref="bool"/> indicating whether to unregister a currently registered attached background task.</param>
        public async Task<bool> RegisterTaskAsync(bool activateIfDisabled = true)
        {
            var existing = BackgroundTaskRegistration.AllTasks.FirstOrDefault(t => t.Value.Name == this.Name).Value;
            if (existing == null)
            {
                if (activateIfDisabled)
                {
                    return await RegisterTaskInternal();
                }
                else
                {
                    return false;
                }
            }
            else if(IsDebugEnabled)
            {
                existing.Unregister(true);
                if (activateIfDisabled)
                {
                    return await RegisterTaskInternal();
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Registration = existing;
                SetActive(true);
                return true;
            }
        }

        private async Task<bool> RegisterTaskInternal()
        {
            var backgroundStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy
               || backgroundStatus == BackgroundAccessStatus.AlwaysAllowed)
            {
                Registering?.Invoke(this, new EventArgs());

                var builder = new BackgroundTaskBuilder();
                builder.IsNetworkRequested = IsNetworkRequested;
                builder.Name = Name;
                builder.SetTrigger(Trigger);
                Registration = builder.Register();
                SetActive(true);
                return true;
            }
            else
            {
                SetActive(false);
                return false;
            }
        }

        /// <summary>
        /// Unregisters the associated background task, optionally canceling any currently running instances.
        /// </summary>
        /// <param name="cancelTask">A <see cref="bool"/> indicating whether a currently running task should be immediately canceled.</param>
        public void UnRegisterTask(bool cancelTask = true)
        {
            if (IsActive)
            {
                Unregistering?.Invoke(this, new EventArgs());

                var associatedTask = BackgroundTaskRegistration.AllTasks
                    .First(t => t.Value.Name == this.Name);

                associatedTask.Value.Unregister(cancelTask);
                SetActive(false);
            }
        }
    }

    public static class BackgroundHandlerExtensions
    {
        /// <summary>
        /// Gets the <see cref="BackgroundHandler"/> from a collection of <see cref="BackgroundHandler"/>s with the given <see cref="BackgroundHandler.Name"/>.
        /// </summary>
        /// <param name="name">The name of the handler.</param>
        public static BackgroundHandler GetHandler(this IEnumerable<BackgroundHandler> handlers, string name)
        {
            return handlers.FirstOrDefault(h => h.Name == name);
        }
    }
}
