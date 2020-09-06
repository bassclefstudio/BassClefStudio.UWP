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
    public abstract class CommandAppService : IAppService
    {
        /// <summary>
        /// The name of the command that activates this <see cref="IAppService"/>.
        /// </summary>
        public string CommandName { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Description { get; }

        /// <summary>
        /// Creates a new <see cref="CommandAppService"/>.
        /// </summary>
        /// <param name="commandName">The name of the command that activates this <see cref="IAppService"/>.</param>
        /// <param name="displayName">See <see cref="Name"/>.</param>
        /// <param name="description">See <see cref="Description"/>.</param>
        public CommandAppService(string commandName, string displayName, string description)
        {
            CommandName = commandName;
            Name = displayName;
            Description = description;
        }

        /// <inheritdoc/>
        public bool CanExecute(AppServiceInput input)
        {
            return input.CommandName.Equals(CommandName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// A function that returns an asynchronous <see cref="Task{TResult}"/> that takes an <see cref="Dictionary{TKey, TValue}"/> and returns an <see cref="object"/>. To send and recieve <see cref="AppServiceOutput"/> and <see cref="AppServiceOutput"/> objects, use <see cref="ExecuteAsync"/>.
        /// </summary>
        public abstract Task<object> GetOutputInternal(Dictionary<string, object> inputs);

        /// <inheritdoc/>
        public async Task<AppServiceOutput> ExecuteAsync(AppServiceInput input)
        {
            try
            {
                var r = await GetOutputInternal(input.InputParameters);
                return new AppServiceOutput(true, output: r);
            }
            catch(Exception ex)
            {
                return new AppServiceOutput(false, errorMessage: ex.ToString());
            }
        }
    }
}
