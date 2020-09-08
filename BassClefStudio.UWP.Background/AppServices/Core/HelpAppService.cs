using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.UWP.Background.AppServices.Core
{
    /// <summary>
    /// A basic <see cref="IAppService"/> that returns a list of all <see cref="IAppService"/>s.
    /// </summary>
    public class HelpAppService : CommandAppService
    {
        /// <summary>
        /// A collection of all available <see cref="IAppService"/>s.
        /// </summary>
        public IEnumerable<IAppService> AppServices { get; set; }

        /// <summary>
        /// Creates a new <see cref="HelpAppService"/>.
        /// </summary>
        public HelpAppService() : base("help", "Help", "Returns a list of available commands and their descriptions.")
        { }

        /// <inheritdoc/>
        protected override async Task<object> GetOutputInternal(AppServiceInput inputs)
        {
            return AppServices.Select(s => $"{s.Name}{(s is CommandAppService c ? $" ({c.CommandName})" : "")}: {s.Description}").ToArray();
        }
    }
}
