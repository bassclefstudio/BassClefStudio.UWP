using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace BassClefStudio.UWP.ApplicationModel.Activation
{
    public class ActivationInfo
    {
        public Type ChildPageType { get; }
        public object Parameter { get; }
        public IActivatedEventArgs ActivatedEventArgs { get; }

        public ActivationInfo(IActivatedEventArgs activatedEventArgs, object parameter = null, Type pageType = null)
        {
            ActivatedEventArgs = activatedEventArgs;
            Parameter = parameter;
            ChildPageType = pageType;
        }
    }
}
