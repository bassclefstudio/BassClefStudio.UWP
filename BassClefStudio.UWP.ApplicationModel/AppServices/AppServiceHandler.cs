using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.UWP.ApplicationModel.AppServices
{
    public abstract class AppServiceHandler
    {
        public abstract string[] Commands { get; }
        public abstract Task<AppServiceOutput> RunAppService(AppServiceInput inputs);
    }
}
