﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BassClefStudio.UWP.Core
{
    /// <summary>
    /// Represents a <see cref="Page"/> with added PropertyChanged handlers (similar to Observable).
    /// </summary>
    public class ObservablePage : Page
    {
        /// <summary>
        /// An event which is fired whenever a related property in an inheriting type is set.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the value of a field to a specific value and calls the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <typeparam name="T">The type of the value being set.</typeparam>
        /// <param name="storage">The field to store the value.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="propertyName">(Filled automatically) the name of the property being set.</param>
        protected void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Represents a <see cref="ContentDialog"/> with added PropertyChanged handlers (similar to Observable).
    /// </summary>
    public class ObservableDialog : ContentDialog
    {
        /// <summary>
        /// An event which is fired whenever a related property in an inheriting type is set.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the value of a field to a specific value and calls the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <typeparam name="T">The type of the value being set.</typeparam>
        /// <param name="storage">The field to store the value.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="propertyName">(Filled automatically) the name of the property being set.</param>
        protected void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Represents a <see cref="UserControl"/> with added PropertyChanged handlers (similar to Observable).
    /// </summary>
    public class ObservableControl : UserControl
    {
        /// <summary>
        /// An event which is fired whenever a related property in an inheriting type is set.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the value of a field to a specific value and calls the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <typeparam name="T">The type of the value being set.</typeparam>
        /// <param name="storage">The field to store the value.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="propertyName">(Filled automatically) the name of the property being set.</param>
        protected void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
