using Autofac;
using BassClefStudio.UWP.Background.Tasks;
using BassClefStudio.UWP.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace BassClefStudio.UWP.Background.Extensions
{
    /// <summary>
    /// An <see cref="IBackgroundActivationHandler"/> using the <see cref="BackgroundService"/> for the core background task management logic.
    /// </summary>
    public class BackgroundActivationHandler : IBackgroundActivationHandler
    {
        /// <inheritdoc/>
        public bool Enabled { get; }

        public IEnumerable<IBackgroundService> BackgroundHandlers { get; }

        public BackgroundActivationHandler(IEnumerable<IBackgroundService> backgroundHandlers)
        {
            BackgroundHandlers = backgroundHandlers;
            Enabled = true;
        }

        /// <inheritdoc/>
        public async Task<bool> BackgroundActivated(Application app, BackgroundActivatedEventArgs args)
        {
            if(args.TaskInstance.TriggerDetails == null)
            {
                var task = BackgroundHandlers.GetBackgroundHandler(args.TaskInstance);
                
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

    public static class BackgroundBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="BackgroundActivationHandler"/> to handle background tasks using the <see cref="IBackgroundService"/> interface.
        /// </summary>
        /// <param name="handlerAssemblies">A collection of <see cref="Assembly"/> objects where <see cref="IBackgroundService"/> implementations can be added to the DI container.</param>
        public static void AddBackgroundHandler(this ContainerBuilder builder, params Assembly[] handlerAssemblies)
        {
            builder.RegisterType<BackgroundActivationHandler>().AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(handlerAssemblies).AssignableTo<IBackgroundService>().AsImplementedInterfaces();
        }
    }
}
