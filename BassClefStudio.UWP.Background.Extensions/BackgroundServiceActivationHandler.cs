using BassClefStudio.NET.Core;
using BassClefStudio.UWP.Background.Tasks;
using BassClefStudio.UWP.Lifecycle;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;

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
        public bool BackgroundActivated(Application app, BackgroundActivatedEventArgs args)
        {
            if(args.TaskInstance.TriggerDetails == null)
            {
                var task = BackgroundServices.GetBackgroundHandler(args.TaskInstance);
                
                if(task != null)
                {
                    var toRun = new SynchronousTask(() => RunBackgroundTask(task, args.TaskInstance, args.TaskInstance.GetDeferral()));
                    toRun.RunTask();
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

        private async Task RunBackgroundTask(IBackgroundService backgroundService, IBackgroundTaskInstance taskInstance, BackgroundTaskDeferral deferral)
        {
            try
            {
                await backgroundService.RunAsync(taskInstance);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}
