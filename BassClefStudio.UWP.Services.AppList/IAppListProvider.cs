using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.UWP.Services.AppList
{
    /// <summary>
    /// Represents a service that can get and manage a list of installed applications on the system (in the form of their <see cref="AppPackage"/> entries).
    /// </summary>
    public interface IAppListProvider
    {
        /// <summary>
        /// An <see cref="ObservableCollection{T}"/> of <see cref="AppPackage"/>s for every application installed on the system.
        /// </summary>
        ObservableCollection<AppPackage> Applications { get; }

        /// <summary>
        /// Asynchronously updates the <see cref="Applications"/> collection.
        /// </summary>
        Task Update();
    }
}
