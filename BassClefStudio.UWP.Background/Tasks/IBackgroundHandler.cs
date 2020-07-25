using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace BassClefStudio.UWP.Background.Tasks
{
    public interface IBackgroundHandler
    {
        string Name { get; }

        Task Execute(IBackgroundTaskInstance task);
    }
}
