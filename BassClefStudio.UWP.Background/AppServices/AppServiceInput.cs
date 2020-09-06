using System.Collections.Generic;
using Windows.Foundation.Collections;

namespace BassClefStudio.UWP.Background.AppServices
{
    /// <summary>
    /// Represents the input to an app service managed by an <see cref="IAppService"/>.
    /// </summary>
    public class AppServiceInput
    {
        /// <summary>
        /// The name of the command to execute.
        /// </summary>
        public string CommandName { get; }

        /// <summary>
        /// The version of the app service stack that created the <see cref="AppServiceInput"/>.
        /// </summary>
        public int VersionNumber { get; }

        /// <summary>
        /// A collection of <see cref="object"/> inputs, keyed by <see cref="string"/> names.
        /// </summary>
        public Dictionary<string, object> InputParameters { get; }

        /// <summary>
        /// Creates a new <see cref="AppServiceInput"/>.
        /// </summary>
        /// <param name="commandName">The name of the command to execute.</param>
        /// <param name="inputs">A collection of <see cref="object"/> inputs, keyed by <see cref="string"/> names.</param>
        public AppServiceInput(string commandName, Dictionary<string, object> inputs)
        {
            CommandName = commandName;
            InputParameters = inputs;
            VersionNumber = AppServiceVersion.VersionNumber;
        }

        /// <summary>
        /// Parses a returned <see cref="ValueSet"/> into a new <see cref="AppServiceInput"/>.
        /// </summary>
        /// <param name="returnedValue">The value returned from the app service.</param>
        public AppServiceInput(ValueSet returnedValue)
        {
            CommandName = returnedValue["Command"] as string;
            VersionNumber = returnedValue["Version"] as int? ?? 0;

            InputParameters = new Dictionary<string, object>();
            foreach (var value in returnedValue)
            {
                if(value.Key.StartsWith("Input_"))
                {
                    InputParameters.Add(
                        value.Key.Replace("Input_", string.Empty),
                        value.Value);
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="ValueSet"/> which can be sent through an app service connection.
        /// </summary>
        public ValueSet CreateInput()
        {
            var set = new ValueSet();
            set.Add("Command", CommandName);
            set.Add("Version", VersionNumber);
            foreach (var input in InputParameters)
            {
                set.Add($"Input_{input.Key}", input.Value);
            }

            return set;
        }
    }
}
