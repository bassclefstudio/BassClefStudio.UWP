using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.UWP.Background.AppServices
{
    /// <summary>
    /// Represents a service which can handle an app service request from another application.
    /// </summary>
    public interface IAppService
    {
        /// <summary>
        /// A display name for this <see cref="IAppService"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A description of this <see cref="IAppService"/> and the actions it performs.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether the given <see cref="IAppService"/> can handle the given <see cref="AppServiceInput"/>.
        /// </summary>
        /// <param name="input">The <see cref="AppServiceInput"/> sent from another application.</param>
        bool CanExecute(AppServiceInput input);

        /// <summary>
        /// An asynchronous <see cref="Task"/> that takes an <see cref="AppServiceInput"/> and returns an <see cref="AppServiceOutput"/>.
        /// </summary>
        /// <param name="input">The <see cref="AppServiceInput"/> sent from another application.</param>
        Task<AppServiceOutput> ExecuteAsync(AppServiceInput input);
    }
}
