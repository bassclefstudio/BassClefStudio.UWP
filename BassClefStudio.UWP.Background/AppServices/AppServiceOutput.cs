﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace BassClefStudio.UWP.Background.AppServices
{
    /// <summary>
    /// Represents the values returned from an app service managed by an <see cref="AppServiceHandler"/>.
    /// </summary>
    public class AppServiceOutput
    {
        /// <summary>
        /// A <see cref="bool"/> indicating whether the <see cref="AppServiceHandler"/>'s operation succeeded.
        /// </summary>
        public bool Sucess { get; }

        /// <summary>
        /// The version of the app service stack that created the <see cref="AppServiceOutput"/>.
        /// </summary>
        public int VersionNumber { get; }

        /// <summary>
        /// If <see cref="Sucess"/> is false, contains the error message as a <see cref="string"/>.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// If <see cref="Sucess"/> is true, contains the <see cref="object"/> sent from the <see cref="AppServiceHandler"/>.
        /// </summary>
        public object Output { get; }

        /// <summary>
        /// Creates a new <see cref="AppServiceOutput"/>.
        /// </summary>
        /// <param name="sucess">A <see cref="bool"/> indicating whether the <see cref="AppServiceHandler"/>'s operation succeeded.</param>
        /// <param name="output">If applicable, the error message as a <see cref="string"/>.</param>
        /// <param name="errorMessage">If applicable, the <see cref="object"/> sent from the <see cref="AppServiceHandler"/>.</param>
        public AppServiceOutput(bool sucess, int version, object output = null, string errorMessage = null)
        {
            Sucess = sucess;
            VersionNumber = version;
            ErrorMessage = errorMessage;
            Output = output;
        }

        /// <summary>
        /// Parses a returned <see cref="ValueSet"/> into a new <see cref="AppServiceOutput"/>.
        /// </summary>
        /// <param name="returnedValue">The value returned from the app service.</param>
        public AppServiceOutput(ValueSet returnedValue)
        {
            Sucess = returnedValue["Success"] as bool? ?? false;
            VersionNumber = returnedValue["Version"] as int? ?? 0;

            if (Sucess)
            {
                Output = returnedValue["Returns"];
            }
            else
            {
                ErrorMessage = returnedValue["Error"] as string;
            }
        }

        /// <summary>
        /// Creates a <see cref="ValueSet"/> which can be sent through an app service connection.
        /// </summary>
        public ValueSet CreateOutput()
        {
            var set = new ValueSet();
            set.Add("Success", Sucess);
            set.Add("Version", VersionNumber);

            if (Sucess)
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