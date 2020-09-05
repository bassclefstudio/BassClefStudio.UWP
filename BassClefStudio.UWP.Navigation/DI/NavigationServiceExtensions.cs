using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.UWP.Navigation.DI
{
    public static class NavigationServiceExtensions
    {
        public static void AddViewModels(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            builder.RegisterModule(new ViewModelModule(assemblies));
        }
    }
}
