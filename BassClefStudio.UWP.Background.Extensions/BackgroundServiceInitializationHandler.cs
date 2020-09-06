using BassClefStudio.UWP.Background.Tasks;
using BassClefStudio.UWP.Lifecycle;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Background;

namespace BassClefStudio.UWP.Background.Extensions
{
    /// <summary>
    /// An <see cref="IInitializationHandler"/> that initializes the background task model by removing unused background task registrations.
    /// </summary>
    public class BackgroundServiceInitializationHandler : IInitializationHandler
    {
        /// <inheritdoc/>
        public bool Enabled { get; }

        /// <summary>
        /// The collection of attached <see cref="IBackgroundService"/>s.
        /// </summary>
        public IEnumerable<IBackgroundService> BackgroundServices { get; }

        /// <summary>
        /// Creates anew <see cref="BackgroundServiceInitializationHandler"/>.
        /// </summary>
        /// <param name="backgroundServices">The collection of attached <see cref="IBackgroundService"/>s.</param>
        public BackgroundServiceInitializationHandler(IEnumerable<IBackgroundService> backgroundServices)
        {
            BackgroundServices = backgroundServices;
            Enabled = true;
        }

        /// <inheritdoc/>
        public bool Initialize(Application app)
        {
            var toRemove = BackgroundTaskRegistration.AllTasks.Where(t => BackgroundServices.GetBackgroundHandler(t.Value) == null);
            foreach (var r in toRemove)
            {
                r.Value.Unregister(false);
            }

            return true;
        }
    }
}
