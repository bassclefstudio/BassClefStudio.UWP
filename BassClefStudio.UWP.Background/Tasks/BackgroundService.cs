using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace BassClefStudio.UWP.Background.Tasks
{
    /// <summary>
    /// Provides a core implementation of <see cref="IBackgroundService"/> that allows for the creation of named background tasks.
    /// </summary>
    public abstract class BackgroundService : IBackgroundService
    {
        #region Properties

        /// <summary>
        /// The referencable name of the <see cref="BackgroundService"/>.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// The <see cref="IBackgroundTrigger"/> that will trigger the background task.
        /// </summary>
        public IBackgroundTrigger Trigger { get; }

        /// <summary>
        /// A <see cref="bool"/> value indicating whether the network stack needs to be active when the background task is active.
        /// </summary>
        public bool IsNetworkRequested { get; }

        #endregion
        #region Interface

        /// <inheritdoc/>
        public IBackgroundTaskRegistration Registration { get; private set; }

        /// <inheritdoc/>
        public bool IsActive { get; private set; }

        /// <inheritdoc/>
        public abstract Task RunAsync(IBackgroundTaskInstance taskInstance);

        #endregion
        #region Initialize

        /// <summary>
        /// Creates a new <see cref="BackgroundService"/> from the given information.
        /// </summary>
        /// <param name="name">The name of the <see cref="BackgroundService"/>.</param>
        /// <param name="trigger">The <see cref="IBackgroundTrigger"/> that will trigger the background task.</param>
        /// <param name="isNetworkRequested">A <see cref="bool"/> value indicating whether the network stack needs to be active when the background task is active.</param>
        public BackgroundService(string name, IBackgroundTrigger trigger, bool isNetworkRequested = false)
        {
            Name = name;
            Trigger = trigger;
            IsNetworkRequested = isNetworkRequested;

            Initialize();
        }

        /// <summary>
        /// Creates a new <see cref="BackgroundService"/> from the given information, triggering on regular time intervals as given by <paramref name="minutes"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="BackgroundService"/>.</param>
        /// <param name="minutes">The number of minutes to set the <see cref="TimeTrigger"/> that will trigger the background task.</param>
        /// <param name="isNetworkRequested">A <see cref="bool"/> value indicating whether the network stack needs to be active when the background task is active.</param>
        public BackgroundService(string name, uint minutes, bool isNetworkRequested = false)
            : this(name, new TimeTrigger(minutes, false), isNetworkRequested) { }

        ///// <summary>
        ///// Completes the registration of all <see cref="BackgroundHandlers"/> by unregistering undeclared tasks.
        ///// </summary>
        //public static void CompleteRegistrations()
        //{
        //    var toRemove = BackgroundTaskRegistration.AllTasks.Where(t => !BackgroundHandlers.Any(h => h.Name == t.Value.Name));
        //    Debug.WriteLine($"Unregistering {toRemove.Count()} background tasks.");
        //    foreach (var r in toRemove)
        //    {
        //        r.Value.Unregister(false);
        //    }
        //}

        /// <summary>
        /// Sets <see cref="IsActive"/> to a <see cref="bool"/> value indicating whether the background task is active and registered, and sets the <see cref="IBackgroundTaskRegistration"/> <see cref="Registration"/> if registered.
        /// </summary>
        public void Initialize()
        {
            if(BackgroundTaskRegistration.AllTasks.Any(t => t.Value.Name == this.Name))
            {
                IsActive = false;
            }
            else
            {
                var existing = BackgroundTaskRegistration.AllTasks.FirstOrDefault(t => t.Value.Name == this.Name).Value;
                Registration = existing;
                IsActive = true;
            }
        }

        #endregion
        #region Registration

        /// <inheritdoc/>
        public async Task<bool> RegisterTaskAsync(bool reregisterTask = true)
        {
            Initialize();

            if(!IsActive || IsActive && reregisterTask)
            {
                await RegisterTaskInternal();
            }

            return IsActive;
        }

        private async Task RegisterTaskInternal()
        {
            var backgroundStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy
               || backgroundStatus == BackgroundAccessStatus.AlwaysAllowed)
            {
                var builder = new BackgroundTaskBuilder();
                builder.IsNetworkRequested = IsNetworkRequested;
                builder.Name = Name;
                builder.SetTrigger(Trigger);
                Registration = builder.Register();
                IsActive = true;
            }
            else
            {
                IsActive = false;
            }
        }

        /// <inheritdoc/>
        public void UnRegisterTask(bool cancelTask = false)
        {
            if (IsActive)
            {
                var associatedTask = BackgroundTaskRegistration.AllTasks
                    .First(t => t.Value.Name == this.Name);

                associatedTask.Value.Unregister(cancelTask);
                IsActive = false;
            }
        }

        #endregion
    }
}
