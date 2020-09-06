using Autofac;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Management.Deployment;

namespace BassClefStudio.UWP.Services.AppList
{
    /// <summary>
    /// An <see cref="IAppListProvider"/> that uses the <see cref="PackageManager"/> API to create a list of installed <see cref="AppPackage"/>s.
    /// </summary>
    public class PackageManagerAppListProvider : IAppListProvider
    {
        /// <inheritdoc/>
        public ObservableCollection<AppPackage> Applications { get; }

        /// <summary>
        /// Creates a new <see cref="PackageManagerAppListProvider"/>.
        /// </summary>
        public PackageManagerAppListProvider()
        {
            Applications = new ObservableCollection<AppPackage>();
        }

        /// <inheritdoc/>
        public async Task Update()
        {
            IEnumerable<Package> packages = await Task.Run(() => new PackageManager().FindPackagesForUser(string.Empty));

            List<Tuple<Package, IAsyncOperation<IReadOnlyList<AppListEntry>>>> getAppTasks = new List<Tuple<Package, IAsyncOperation<IReadOnlyList<AppListEntry>>>>();
            foreach (var package in packages)
            {
                getAppTasks.Add(new Tuple<Package, IAsyncOperation<IReadOnlyList<AppListEntry>>>(package, package.GetAppListEntriesAsync()));
            }

            List<Tuple<Package, AppListEntry>> apps = new List<Tuple<Package, AppListEntry>>();
            foreach (var task in getAppTasks)
            {
                var appsInPackage = await task.Item2;
                if (appsInPackage != null && appsInPackage.Any())
                {
                    apps.Add(new Tuple<Package, AppListEntry>(task.Item1, appsInPackage.First()));
                }
            }

            Applications.Sync(apps, (a, p) => a.AppInfo.AppUserModelId == p.Item2.AppUserModelId, p => new AppPackage(p.Item1, p.Item2), true);
        }
    }

    public static class PackageManagerExtensions
    {
        /// <summary>
        /// Registers the <see cref="PackageManagerAppListProvider"/> as a singleton <see cref="IAppListProvider"/>.
        /// </summary>
        public static void AddPackageManagerService(this ContainerBuilder builder)
        {
            builder.RegisterType<PackageManagerAppListProvider>().SingleInstance().AsImplementedInterfaces();
        }
    }
}
