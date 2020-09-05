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
        /// <summary>
        /// Adds all <see cref="IViewModel"/>s in a collection of <see cref="Assembly"/> objects to a <see cref="ContainerBuilder"/>, (usually one attached to the <see cref="NavigationService"/>).
        /// </summary>
        /// <param name="assemblies">A collection of <see cref="Assembly"/> objects where the view-models are defined.</param>
        public static void AddViewModels(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            builder.RegisterModule(new ViewModelModule(assemblies));
        }
    }
}
