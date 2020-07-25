using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.UWP.Background.AppServices
{
    /// <summary>
    /// Represents a service that can handle app service requests with a specified command name.
    /// </summary>
    public abstract class CommandAppServiceHandler : IAppServiceHandler
    {
        /// <summary>
        /// The name of the command that activates this <see cref="AppServiceHandler"/>.
        /// </summary>
        public string CommandName { get; }

        /// <inheritdoc/>
        public bool CanExecute(AppServiceInput input)
        {
            return input.CommandName == CommandName;
        }

        /// <summary>
        /// A function that returns an asynchronous <see cref="Task{TResult}"/> that takes an <see cref="Dictionary{TKey, TValue}"/> and returns an <see cref="object"/>. To send and recieve <see cref="AppServiceOutput"/> and <see cref="AppServiceOutput"/> objects, use <see cref="Execute"/>.
        /// </summary>
        public abstract Task<object> GetOutputInternal(Dictionary<string, object> inputs);

        /// <inheritdoc/>
        public async Task<AppServiceOutput> Execute(AppServiceInput input)
        {
            try
            {
                var r = await GetOutputInternal(input.InputParameters);
                return new AppServiceOutput(true, VersionNumber, r);
            }
            catch(Exception ex)
            {
                return new AppServiceOutput(false, VersionNumber, null, ex.ToString());
            }
        }

        /// <summary>
        /// A constant <see cref="int"/> value that will be sent in app service requests as this apps <see cref="AppServiceHandler"/> version number.
        /// </summary>
        public const int VersionNumber = 1;
    }
}
