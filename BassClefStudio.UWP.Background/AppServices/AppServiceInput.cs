using System;
using System.Collections;
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
        /// The name of the app package that sent the request.
        /// </summary>
        public string PackageFamilyName { get; }

        /// <summary>
        /// A collection of <see cref="object"/> inputs, keyed by <see cref="string"/> names.
        /// </summary>
        public Dictionary<string, object> InputParameters { get; }

        /// <summary>
        /// Gets a value from <see cref="InputParameters"/> with the given key.
        /// </summary>
        /// <param name="key">The <see cref="string"/> name of the input.</param>
        public object this[string key] => InputParameters[key];

        /// <summary>
        /// Attempts to get a value from <see cref="InputParameters"/> with the given key.
        /// </summary>
        /// <param name="key">The <see cref="string"/> name of the input.</param>
        /// <param name="o">If the value is found, the given variable is set to the <see cref="object"/> value.</param>
        public bool TryGetValue(string key, out object o) => InputParameters.TryGetValue(key, out o);

        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether an input value is present.
        /// </summary>
        /// <param name="key">The <see cref="string"/> name of the input.</param>
        public bool ContainsKey(string key) => InputParameters.ContainsKey(key);

        /// <summary>
        /// Creates a new <see cref="AppServiceInput"/>.
        /// </summary>
        /// <param name="commandName">The name of the command to execute.</param>
        /// <param name="packageFamilyName">The name of the app package that sent the request.</param>
        /// <param name="inputs">A collection of <see cref="object"/> inputs, keyed by <see cref="string"/> names.</param>
        public AppServiceInput(string commandName, string packageFamilyName, Dictionary<string, object> inputs)
        {
            CommandName = commandName;
            PackageFamilyName = packageFamilyName;
            InputParameters = inputs;
            VersionNumber = AppServiceVersion.VersionNumber;
        }

        /// <summary>
        /// Parses a returned <see cref="ValueSet"/> into a new <see cref="AppServiceInput"/>.
        /// </summary>
        /// <param name="returnedValue">The value returned from the app service.</param>
        public AppServiceInput(ValueSet returnedValue)
        {
            if (returnedValue.TryGetValue("Command", out var c))
            {
                CommandName = c as string;
            }
            else
            {
                throw new ArgumentException("A value was missing from the returned content.", "Command");
            }

            if (returnedValue.TryGetValue("Version", out var v))
            {
                VersionNumber = v as int? ?? 0;
            }
            else
            {
                throw new ArgumentException("A value was missing from the returned content.", "Version");
            }

            if (returnedValue.TryGetValue("Package", out var p))
            {
                PackageFamilyName = p as string;
            }
            else
            {
                throw new ArgumentException("A value was missing from the returned content.", "Package");
            }

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
            set.Add("Package", PackageFamilyName);
            foreach (var input in InputParameters)
            {
                set.Add($"Input_{input.Key}", input.Value);
            }

            return set;
        }
    }
}
