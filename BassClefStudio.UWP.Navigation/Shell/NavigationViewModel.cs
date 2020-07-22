using BassClefStudio.NET.Core;
using BassClefStudio.UWP.Navigation.DI;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Media.Animation;

namespace BassClefStudio.UWP.Navigation.Shell
{
    /// <summary>
    /// Represents an <see cref="IViewModel"/> in the MVVM pattern which manages page-to-page navigation.
    /// </summary>
    public abstract class NavigationViewModel : Observable, IViewModel
    {
        /// <summary>
        /// A collection of the available locations to navigate to.
        /// </summary>
        public ObservableCollection<NavigationPageItem> NavigationPages { get; }

        private bool enableTransitions;

        private NavigationPageItem settingsPage;
        /// <summary>
        /// Represents the <see cref="NavigationPageItem"/> for the settings page, which is displayed separately.
        /// </summary>
        public NavigationPageItem SettingsPage { get => settingsPage; set => Set(ref settingsPage, value); }

        private AdvancedCollectionView navigationPagesView;
        /// <summary>
        /// The <see cref="AdvancedCollectionView"/> encapsulating the <see cref="NavigationPages"/> collection, for use in data binding.
        /// </summary>
        public AdvancedCollectionView NavigationPagesView { get => navigationPagesView; set => Set(ref navigationPagesView, value); }

        private object selectedObject;
        /// <summary>
        /// The currently selected <see cref="NavigationPageItem"/> or page (in the case of an unknown page or the <see cref="SettingsObject"/>).
        /// </summary>
        public object SelectedObject { get => selectedObject; set { Set(ref selectedObject, value); } }

        private object settingsObject;
        /// <summary>
        /// The value of <see cref="SelectedObject"/> when the selected page is the settings page (when managed separately to the <see cref="NavigationPages"/>).
        /// </summary>
        public object SettingsObject { get => settingsObject; set { Set(ref settingsObject, value); } }

        private bool canGoBack;
        /// <summary>
        /// A <see cref="bool"/> value indicating whether the navigation stack can go back.
        /// </summary>
        public bool CanGoBack { get => canGoBack; set => Set(ref canGoBack, value); }

        private NavigationPageItem selectedPage;
        /// <summary>
        /// The <see cref="SelectedObject"/> as its associated <see cref="NavigationPageItem"/>.
        /// </summary>
        public NavigationPageItem SelectedPage { get => selectedPage; set => Set(ref selectedPage, value); }

        /// <summary>
        /// Creates a new <see cref="NavigationViewModel"/>.
        /// </summary>
        /// <param name="liveShaping">Indicates whether the <see cref="NavigationPagesView"/> should be initialized with live shaping (such as filtering and sorting). Defaults to false.</param>
        public NavigationViewModel(bool liveShaping = false)
        {
            NavigationPages = new ObservableCollection<NavigationPageItem>();
            NavigationPagesView = new AdvancedCollectionView(NavigationPages, liveShaping);

            enableTransitions = ApiInformation.IsEnumNamedValuePresent("Windows.UI.Xaml.Media.Animation.SlideNavigationTransitionEffect", "FromLeft");
            NavigationService.Navigated += Navigated;
        }

        private void Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e) => Navigated(e.SourcePageType);
        private void Navigated(Type pageType)
        {
            CanGoBack = NavigationService.CanGoBack;
            if (pageType == SettingsPage.PageType)
            {
                SelectedObject = SettingsObject;
            }
            else
            {
                SelectedObject = NavigationPages.FirstOrDefault(p => p.PageType == pageType);
            }
        }

        /// <inheritdoc/>
        public async Task Initialize()
        {
        }

        /// <summary>
        /// Initializes the <see cref="NavigationPages"/> this view-model can navigate between, including the default settings information for a given <paramref name="settingsPageType"/>.
        /// </summary>
        /// <param name="pages">A collection of <see cref="NavigationPageItem"/> objects for each page.</param>
        /// <param name="settingsPageType">The <see cref="Type"/> of the settings page, which creates a <see cref="NavigationPageItem"/> with the name 'settings' and the settings icon.</param>
        public void InitializePages(IEnumerable<NavigationPageItem> pages, Type settingsPageType)
            => InitializePages(pages, new NavigationPageItem("Settings", '\uE713', settingsPageType));

        /// <summary>
        /// Initializes the <see cref="NavigationPages"/> this view-model can navigate between, including the <see cref="SettingsPage"/>.
        /// </summary>
        /// <param name="pages">A collection of <see cref="NavigationPageItem"/> objects for each page.</param>
        /// <param name="settingsPage">The <see cref="NavigationPageItem"/> for the settings page.</param>
        public void InitializePages(IEnumerable<NavigationPageItem> pages, NavigationPageItem settingsPage)
        {
            NavigationPages.AddRange(pages);
            SettingsPage = settingsPage;

            if (NavigationService.CurrentPageType != null)
            {
                Navigated(NavigationService.CurrentPageType);
            }
        }

        /// <summary>
        /// Calls the <see cref="NavigationService"/> to go back in the back stack.
        /// </summary>
        public void GoBack()
        {
            NavigationService.GoBack();
        }

        /// <summary>
        /// Navigates to the <see cref="SelectedObject"/> (usually set by an attached NavigationView or the like).
        /// </summary>
        public void NavigateToSelectedPage()
        {
            if (SelectedObject is NavigationPageItem navPage)
            {
                var oldPage = SelectedPage;
                SelectedPage = navPage;

                if (enableTransitions)
                {
                    if (NavigationPages.IndexOf(SelectedPage) > NavigationPages.IndexOf(oldPage))
                    {
                        NavigationService.Navigate(SelectedPage.PageType, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                    }
                    else
                    {
                        NavigationService.Navigate(SelectedPage.PageType, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
                    }
                }
                else
                {
                    NavigationService.Navigate(SelectedPage.PageType);
                }
            }
            else
            {
                NavigationService.Navigate(SettingsPage.PageType, new DrillInNavigationTransitionInfo());
                SelectedPage = SettingsPage;
            }
        }
    }
}
