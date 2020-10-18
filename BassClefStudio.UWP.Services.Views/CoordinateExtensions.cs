using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace BassClefStudio.UWP.Services.Views
{
    /// <summary>
    /// Provides extension methods on the <see cref="Windows.Foundation.Point"/> struct that deal with relative co-ordinates.
    /// </summary>
    public static class CoordinateExtensions
    {
        /// <summary>
        /// Transforms a <see cref="Point"/> relative to the <see cref="UIElement"/> into a <see cref="Point"/> relative to the <see cref="Window"/>.
        /// </summary>
        /// <param name="element">The <see cref="UIElement"/> that <paramref name="point"/> is relative to.</param>
        /// <param name="point">The <see cref="Point"/> to transform.</param>
        public static Point GetWindowCoordinate(this UIElement element, Point point)
        {
            GeneralTransform transform = element.TransformToVisual(Window.Current.Content);
            return transform.TransformPoint(point);
        }

        /// <summary>
        /// Transforms a <see cref="Point"/> relative to the <see cref="Window"/> into a <see cref="Point"/> relative to the <see cref="UIElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="UIElement"/> that <paramref name="point"/> is relative to.</param>
        /// <param name="point">The <see cref="Point"/> to transform.</param>
        public static Point GetElementCoordinate(this UIElement element, Point point)
        {
            GeneralTransform transform = Window.Current.Content.TransformToVisual(element);
            return transform.TransformPoint(point);
        }
    }
}
