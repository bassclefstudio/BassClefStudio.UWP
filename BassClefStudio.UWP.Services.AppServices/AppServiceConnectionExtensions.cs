using Autofac;
using BassClefStudio.UWP.Background.AppServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.UWP.Services.AppServices
{
    public static class AppServiceConnectionExtensions
    {
        /// <summary>
        /// Registers the <see cref="BaseAppServiceConnectionProvider"/> as an <see cref="IAppServiceConnectionProvider"/> to provide a way to connect to basic <see cref="IAppService"/>s and send messages.
        /// </summary>
        public static void AddBaseAppServices(this ContainerBuilder builder)
        {
            builder.RegisterType<BaseAppServiceConnectionProvider>().AsImplementedInterfaces();
        }
    }
}
