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
    /// <summary>
    /// Represents an app package and its package information, as retrieved by an <see cref="IAppListProvider"/>.
    /// </summary>
    public class AppPackage : Observable
    {
        /// <summary>
        /// The parent <see cref="Package"/> of this app.
        /// </summary>
        public Package ParentPackage { get; }

        /// <summary>
        /// System information about the app as an <see cref="AppListEntry"/>.
        /// </summary>
        public AppListEntry AppInfo { get; }

        /// <summary>
        /// The display name of the <see cref="AppPackage"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The package family ID for this <see cref="AppPackage"/>.
        /// </summary>
        public string Id { get; }

        private BitmapImage iconImage;
        /// <summary>
        /// The basic app icon for this <see cref="AppPackage"/>.
        /// </summary>
        public BitmapImage IconImage { get => iconImage; private set => Set(ref iconImage, value); }

        /// <summary>
        /// Creates a new <see cref="AppPackage"/> from a parents <see cref="Package"/> and an <see cref="AppListEntry"/>.
        /// </summary>
        /// <param name="package">The parent <see cref="Package"/> of this app and its dependencies.</param>
        /// <param name="appInfo">System information about this specific app.</param>
        public AppPackage(Package package, AppListEntry appInfo)
        {
            ParentPackage = package;
            AppInfo = appInfo;

            ////Setup default values.
            Name = AppInfo.DisplayInfo.DisplayName;
            Id = ParentPackage.Id.FamilyName;
        }

        /// <summary>
        /// Asynchronously sets the value of <see cref="IconImage"/> from the <see cref="AppPackage"/> information.
        /// </summary>
        /// <param name="size">The desired size of the <see cref="IconImage"/>, in pixels square.</param>
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

        /// <summary>
        /// Launches the given <see cref="AppPackage"/>.
        /// </summary>
        public async Task StartAsync()
        {
            await Launcher.LaunchUriAsync(ParentPackage.GetAppInstallerInfo().Uri);
        }
    }
}
