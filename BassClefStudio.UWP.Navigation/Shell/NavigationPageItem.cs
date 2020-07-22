using BassClefStudio.NET.Core;
using System;

namespace BassClefStudio.UWP.Navigation.Shell
{
    /// <summary>
    /// Represents a navigatable location in the app in a navigation view (see <seealso cref="NavigationViewModel"/>).
    /// </summary>
    public class NavigationPageItem : Observable
    {
        private string name;
        /// <summary>
        /// The name of the <see cref="NavigationPageItem"/>.
        /// </summary>
        public string Name { get => name; set => Set(ref name, value); }

        private char icon;
        /// <summary>
        /// The character icon representing the <see cref="NavigationPageItem"/>.
        /// </summary>
        public char Icon { get => icon; set => Set(ref icon, value); }

        private Type pageType;
        /// <summary>
        /// The <see cref="Type"/> of the page to navigate to using the <see cref="NavigationService"/>.
        /// </summary>
        public Type PageType { get => pageType; set => Set(ref pageType, value); }

        /// <summary>
        /// Creates a <see cref="NavigationPageItem"/> using the given information and page <see cref="Type"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="NavigationPageItem"/>.</param>
        /// <param name="icon">The character icon representing the <see cref="NavigationPageItem"/>.</param>
        /// <param name="pageType">The <see cref="Type"/> of the page to navigate to using the <see cref="NavigationService"/>.</param>
        public NavigationPageItem(string name, char icon, Type pageType)
        {
            Name = name;
            Icon = icon;
            PageType = pageType;
        }
    }
}
