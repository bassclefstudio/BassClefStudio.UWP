using Autofac;
using BassClefStudio.UWP.Background.Tasks;
using System.Reflection;

namespace BassClefStudio.UWP.Background.Extensions
{
    public static class BackgroundBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="BackgroundServiceActivationHandler"/> to handle background tasks using the <see cref="IBackgroundService"/> interface.
        /// </summary>
        /// <param name="handlerAssemblies">A collection of <see cref="Assembly"/> objects where <see cref="IBackgroundService"/> implementations can be added to the DI container.</param>
        public static void AddBackgroundServices(this ContainerBuilder builder, params Assembly[] handlerAssemblies)
        {
            builder.RegisterType<BackgroundServiceActivationHandler>().AsImplementedInterfaces();
            builder.RegisterType<BackgroundServiceInitializationHandler>().AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(handlerAssemblies).AssignableTo<IBackgroundService>().SingleInstance().AsImplementedInterfaces();
        }
    }
}
