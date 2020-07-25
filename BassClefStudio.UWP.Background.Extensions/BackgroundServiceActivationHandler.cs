using BassClefStudio.UWP.Background.Tasks;
using BassClefStudio.UWP.Lifecycle;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace BassClefStudio.UWP.Background.Extensions
{
    /// <summary>
    /// An <see cref="IBackgroundActivationHandler"/> using the <see cref="BackgroundService"/> for the core background task management logic.
    /// </summary>
    public class BackgroundServiceActivationHandler : IBackgroundActivationHandler
    {
        /// <inheritdoc/>
        public bool Enabled { get; }

        public IEnumerable<IBackgroundService> BackgroundServices { get; }

        public BackgroundServiceActivationHandler(IEnumerable<IBackgroundService> backgroundService)
        {
            BackgroundServices = backgroundService;
            Enabled = true;
        }

        /// <inheritdoc/>
        public async Task<bool> BackgroundActivated(Application app, BackgroundActivatedEventArgs args)
        {
            if(args.TaskInstance.TriggerDetails == null)
            {
                var task = BackgroundServices.GetBackgroundHandler(args.TaskInstance);
                
                if(task != null)
                {
                    await task.RunAsync(args.TaskInstance);
                    return true;
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
