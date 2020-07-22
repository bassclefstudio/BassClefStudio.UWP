using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace BassClefStudio.UWP.Core.Helpers
{
    public static class ResourceExtensions
    {
        /// <summary>
        /// Retrieves the given localized <see cref="string"/> value from the provided resource path. 
        /// </summary>
        /// <param name="resourceKey">The path to the resource, including the name of the .resw file and then the name of the key, separated by the '/' character.</param>
        public static string GetLocalized(this string resourceKey)
        {
            var path = resourceKey.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (path.Count == 1)
            {
                return new ResourceLoader("Resources").GetString(string.Join("/", path));
            }
            else
            {
                var file = path[0];
                path.RemoveAt(0);
                return new ResourceLoader(file).GetString(string.Join("/", path));
            }
        }
    }
}
