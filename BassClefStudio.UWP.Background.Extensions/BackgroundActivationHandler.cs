using BassClefStudio.UWP.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace BassClefStudio.UWP.Background.Extensions
{
    public class BackgroundActivationHandler : IBackgroundActivationHandler
    {
        /// <inheritdoc/>
        public bool Enabled { get; }

        public BackgroundActivationHandler()
        {
            Enabled = true;
        }

        /// <inheritdoc/>
        public async Task<bool> BackgroundActivated(Application app, BackgroundActivatedEventArgs args)
        {
            if(args.TaskInstance.TriggerDetails == null)
            {
                var task = BackgroundHandler.BackgroundHandlers
                    .FirstOrDefault(h => h.Name == args.TaskInstance.Task.Name);
                
                if(task != null)
                {
                    await task.Task(args.TaskInstance);
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
