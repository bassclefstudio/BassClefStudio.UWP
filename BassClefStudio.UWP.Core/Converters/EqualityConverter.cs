using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BassClefStudio.UWP.Core.Converters
{
    /// <summary>
    /// A converter that returns a <see cref="Visibility"/> value of <see cref="Visibility.Visible"/> or <see cref="Visibility.Collapsed"/> depending on whether the given object is equal or not equal to a specified object.
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        /// <summary>
        /// The object to check the input value against.
        /// </summary>
        public object CheckFor { get; set; }

        /// <summary>
        /// A <see cref="bool"/> indicating the result of the equality operation that returns <see cref="Visibility.Visible"/>. 
        /// </summary>
        public bool IsEqual { get; set; } = true;

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(CheckFor == null)
            {
                if((value == null) == IsEqual)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            else
            {
                if (CheckFor.Equals(value) == IsEqual)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
