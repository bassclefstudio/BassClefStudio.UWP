using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace BassClefStudio.UWP.ApplicationModel.Activation
{
    /// <summary>
    /// Information about the <see cref="IActivatedEventArgs"/> as well as navigation through a shell page.
    /// </summary>
    public class ActivationInfo
    {
        /// <summary>
        /// The <see cref="Type"/> of the page to navigate to.
        /// </summary>
        public Type ChildPageType { get; }

        /// <summary>
        /// The parameter passed through navigation.
        /// </summary>
        public object Parameter { get; }

        /// <summary>
        /// The <see cref="IActivatedEventArgs"/> from the <see cref="Application"/>.
        /// </summary>
        public IActivatedEventArgs ActivatedEventArgs { get; }

        /// <summary>
        /// Creates a new <see cref="ActivationInfo"/>.
        /// </summary>
        /// <param name="activatedEventArgs">The <see cref="IActivatedEventArgs"/> from the <see cref="Application"/>.</param>
        /// <param name="parameter">The parameter passed through navigation.</param>
        /// <param name="pageType">The <see cref="Type"/> of the page to navigate to.</param>
        public ActivationInfo(IActivatedEventArgs activatedEventArgs, object parameter = null, Type pageType = null)
        {
            ActivatedEventArgs = activatedEventArgs;
            Parameter = parameter;
            ChildPageType = pageType;
        }
    }
}
