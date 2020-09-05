using Autofac;
using BassClefStudio.UWP.Navigation.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BassClefStudio.UWP.Navigation.DI
{
    /// <summary>
    /// The registration module for view-models and associated types, used for DI in the <see cref="NavigationService"/>.
    /// </summary>
    public class ViewModelModule : Autofac.Module
    {
        /// <summary>
        /// A collection of <see cref="Assembly"/> objects containing the view-models.
        /// </summary>
        public Assembly[] ViewModelAssemblies { get; }

        /// <summary>
        /// Creates a new <see cref="ViewModelModule"/> given a collection of <see cref="Assembly"/> objects containing <see cref="IViewModel"/>s.
        /// </summary>
        /// <param name="viewModelAssemblies">A collection of <see cref="Assembly"/> objects containing the view-models.</param>
        public ViewModelModule(Assembly[] viewModelAssemblies)
        {
            ViewModelAssemblies = viewModelAssemblies;
        }

        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            //// Register all IViewModel implementations
            builder.RegisterAssemblyTypes(ViewModelAssemblies)
                .AssignableTo<IViewModel>();

            //// Register current ContinuablePage instance
            builder.Register(p => NavigationService.Frame.Content as ContinuablePage).As<ContinuablePage>();
        }
    }
}
