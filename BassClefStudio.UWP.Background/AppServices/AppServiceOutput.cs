using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace BassClefStudio.UWP.Background.AppServices
{
    /// <summary>
    /// Represents the values returned from an app service managed by an <see cref="IAppService"/>.
    /// </summary>
    public class AppServiceOutput
    {
        /// <summary>
        /// A <see cref="bool"/> indicating whether the <see cref="IAppService"/>'s operation succeeded.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// The version of the app service stack that created the <see cref="AppServiceOutput"/>.
        /// </summary>
        public int VersionNumber { get; }

        /// <summary>
        /// If <see cref="Success"/> is false, contains the error message as a <see cref="string"/>.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// If <see cref="Success"/> is true, contains the <see cref="object"/> sent from the <see cref="IAppService"/>.
        /// </summary>
        public object Output { get; }

        /// <summary>
        /// Creates a new <see cref="AppServiceOutput"/>.
        /// </summary>
        /// <param name="sucess">A <see cref="bool"/> indicating whether the <see cref="IAppService"/>'s operation succeeded.</param>
        /// <param name="output">If applicable, the error message as a <see cref="string"/>.</param>
        /// <param name="errorMessage">If applicable, the <see cref="object"/> sent from the <see cref="IAppService"/>.</param>
        public AppServiceOutput(bool sucess, object output = null, string errorMessage = null)
        {
            Success = sucess;
            VersionNumber = AppServiceVersion.VersionNumber;
            ErrorMessage = errorMessage;
            Output = output;
        }

        /// <summary>
        /// Parses a returned <see cref="ValueSet"/> into a new <see cref="AppServiceOutput"/>.
        /// </summary>
        /// <param name="returnedValue">The value returned from the app service.</param>
        public AppServiceOutput(ValueSet returnedValue)
        {
            if(returnedValue.TryGetValue("Success", out var s))
            {
                Success = s as bool? ?? false;
            }
            else
            {
                throw new ArgumentException("A value was missing from the returned content.", "Success");
            }

            if (returnedValue.TryGetValue("Version", out var v))
            {
                VersionNumber = v as int? ?? 0;
            }
            else
            {
                throw new ArgumentException("A value was missing from the returned content.", "Version");
            }

            if (Success)
            {
                if (returnedValue.TryGetValue("Returns", out var r))
                {
                    Output = r;
                }
                else
                {
                    throw new ArgumentException("A value was missing from the returned content.", "Returns");
                }
            }
            else
            {

                if (returnedValue.TryGetValue("Error", out var ex))
                {
                    ErrorMessage = ex as string;
                }
                else
                {
                    throw new ArgumentException("A value was missing from the returned content.", "Error");
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="ValueSet"/> which can be sent through an app service connection.
        /// </summary>
        public ValueSet CreateOutput()
        {
            var set = new ValueSet();
            set.Add("Success", Success);
            set.Add("Version", VersionNumber);

            if (Success)
            {
                set.Add("Returns", Output);
            }
            else
            {
                set.Add("Error", ErrorMessage);
            }

            return set;
        }
    }
}
