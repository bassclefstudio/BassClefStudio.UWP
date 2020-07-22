using Autofac;
using BassClefStudio.UWP.Navigation.Shell;
using System;
using System.Linq;
using System.Reflection;

namespace BassClefStudio.UWP.Navigation.DI
{
    /// <summary>
    /// The registration module for view-models and associated types, used for DI in the <see cref="NavigationService"/>.
    /// </summary>
    public class ModuleRegistration : Autofac.Module
    {
        /// <summary>
        /// The <see cref="Assembly"/> containing the view-models.
        /// </summary>
        public static Assembly ViewModelAssembly { get; set; }

        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            //// Register all IViewModel implementations
            builder.RegisterTypes(
                ViewModelAssembly
                    .GetTypes()
                    .Where(t => typeof(IViewModel).IsAssignableFrom(t)).ToArray());

            //// Register current ContinuablePage instance
            builder.Register(p => NavigationService.Frame.Content as ContinuablePage).As<ContinuablePage>();
        }
    }
}
