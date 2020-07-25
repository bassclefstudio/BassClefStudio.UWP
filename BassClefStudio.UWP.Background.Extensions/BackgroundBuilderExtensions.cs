using Autofac;
using BassClefStudio.UWP.Background.AppServices;
using BassClefStudio.UWP.Background.Tasks;
using System.Reflection;

namespace BassClefStudio.UWP.Background.Extensions
{
    public static class BackgroundBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="BackgroundServiceActivationHandler"/> and <see cref="BackgroundServiceInitializationHandler"/> to handle background tasks using the <see cref="IBackgroundService"/> interface.
        /// </summary>
        /// <param name="serviceAssemblies">A collection of <see cref="Assembly"/> objects where <see cref="IBackgroundService"/> implementations can be added to the DI container.</param>
        public static void AddBackgroundServices(this ContainerBuilder builder, params Assembly[] serviceAssemblies)
        {
            builder.RegisterType<BackgroundServiceActivationHandler>().AsImplementedInterfaces();
            builder.RegisterType<BackgroundServiceInitializationHandler>().AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(serviceAssemblies).AssignableTo<IBackgroundService>().SingleInstance().AsImplementedInterfaces();
        }

        /// <summary>
        /// Adds the <see cref="AppServiceActivationHandler"/> to handle app service requests using the <see cref="IAppService"/> interface.
        /// </summary>
        /// <param name="serviceAssemblies">A collection of <see cref="Assembly"/> objects where <see cref="IAppService"/> implementations can be added to the DI container.</param>
        public static void AddAppServices(this ContainerBuilder builder, params Assembly[] serviceAssemblies)
        {
            builder.RegisterType<AppServiceActivationHandler>().AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(serviceAssemblies).AssignableTo<IAppService>().SingleInstance().AsImplementedInterfaces();
        }
    }
}
