using BassClefStudio.UWP.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BassClefStudio.UWP.ApplicationModel.Activation
{
    public abstract class ActivationHandlerPage : Page
    {
        public abstract Frame NavigationFrame { get; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.Frame = NavigationFrame;
            if (e.Parameter is ActivationInfo info)
            {
                NavigationService.Navigate(info.ChildPageType, GetChildParameter(info));
            }
        }

        public abstract object GetChildParameter(ActivationInfo info);
    }
}
