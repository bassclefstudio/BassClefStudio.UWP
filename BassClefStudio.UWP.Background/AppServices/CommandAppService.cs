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
        protected abstract Task<object> GetOutputInternal(AppServiceInput input);

        /// <inheritdoc/>
        public virtual async Task<AppServiceOutput> ExecuteAsync(AppServiceInput input)
        {
            try
            {
                var r = await GetOutputInternal(input);
                return new AppServiceOutput(true, output: r);
            }
            catch(Exception ex)
            {
                return new AppServiceOutput(false, errorMessage: ex.ToString());
            }
        }
    }

    /// <summary>
    /// A <see cref="CommandAppService"/> which only allows requests which are authorized with the given <see cref="Scopes"/>.
    /// </summary>
    public abstract class AuthenticatedCommandAppService : CommandAppService
    {
        private IAppServiceAuthProvider AuthProvider { get; }

        /// <summary>
        /// A collection of required scopes an app must have to access this <see cref="IAppService"/>.
        /// </summary>
        public string[] Scopes { get; }

        /// <summary>
        /// Creates a new <see cref="AuthenticatedCommandAppService"/>.
        /// </summary>
        /// <param name="authProvider">The <see cref="IAppServiceAuthProvider"/> which will manage authorization for this app service.</param>
        /// <param name="scopes">A collection of required scopes an app must have to access this <see cref="IAppService"/>.</param>
        /// <param name="commandName">The name of the command that activates this <see cref="IAppService"/>.</param>
        /// <param name="displayName">See <see cref="IAppService.Name"/>.</param>
        /// <param name="description">See <see cref="IAppService.Description"/>.</param>
        public AuthenticatedCommandAppService(IAppServiceAuthProvider authProvider, string[] scopes, string commandName, string displayName, string description) : base(commandName, displayName, description)
        {
            AuthProvider = authProvider;
            Scopes = scopes;
        }

        /// <inheritdoc/>
        public override async Task<AppServiceOutput> ExecuteAsync(AppServiceInput input)
        {
            string[] authorization = AuthProvider.GetScopes(input.PackageFamilyName);
            var missing = Scopes.Where(s => !authorization.Contains(s));
            if (missing.Any())
            {
                return new AppServiceOutput(false, errorMessage: $"This app is not authorized to access this API because it is missing the following scopes: {string.Join(", ", missing)}.");
            }
            else
            {
                return await base.ExecuteAsync(input);
            }
        }
    }
}
