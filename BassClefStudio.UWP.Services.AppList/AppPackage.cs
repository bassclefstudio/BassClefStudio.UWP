using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Xaml.Media.Imaging;

namespace BassClefStudio.UWP.Services.AppList
{
    public class AppPackage : Observable
    {
        public Package ParentPackage { get; }
        public AppListEntry AppInfo { get; }

        public string Name { get; }
        public string Id { get; }

        private BitmapImage iconImage;
        public BitmapImage IconImage { get => iconImage; private set => Set(ref iconImage, value); }

        public AppPackage(Package package, AppListEntry appInfo)
        {
            ParentPackage = package;
            AppInfo = appInfo;

            ////Setup default values.
            Name = AppInfo.DisplayInfo.DisplayName;
            Id = ParentPackage.Id.FamilyName;
        }

        public async Task<bool> GetLogoAsync(double size = 256)
        {
            try
            {
                IconImage = new BitmapImage();
                await IconImage.SetSourceAsync(await AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(size, size)).OpenReadAsync());
                return true;
            }
            catch
            {
                IconImage = null;
                return false;
            }
        }

        public async Task StartAsync()
        {
            await Launcher.LaunchUriAsync(ParentPackage.GetAppInstallerInfo().Uri);
        }
    }
}
