using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BassClefStudio.UWP.Navigation.DI;
using BassClefStudio.NET.Core;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml;
using BassClefStudio.UWP.Navigation.Shell;

namespace BassClefStudio.UWP.Navigation
{
    /// <summary>
    /// Represents a service that can navigate between <see cref="Page"/>s and <see cref="ContentDialog"/>s in an app while providing DI for view-models and associated properties as part of the MVVM model and managing a back-stack.
    /// </summary>
    public static class NavigationService
    {
        #region Pages

        /// <summary>
        /// An event fired when the <see cref="Navigate(Type, object, NavigationTransitionInfo)"/> method completes.
        /// </summary>
        public static event NavigatedEventHandler Navigated;

        /// <summary>
        /// An event fired when navigation is started.
        /// </summary>
        public static event NavigatingCancelEventHandler NavigationStarted;

        /// <summary>
        /// An event fired when navigation fails. <see cref="NavigationFailedEventArgs"/> contains information about the error.
        /// </summary>
        public static event NavigationFailedEventHandler NavigationFailed;

        /// <summary>
        /// The <see cref="Type"/> of the currently navigated <see cref="Page"/>.
        /// </summary>
        public static Type CurrentPageType { get; private set; }

        private static Frame _frame;
        private static object _lastParamUsed;

        /// <summary>
        /// The current <see cref="Windows.UI.Xaml.Controls.Frame"/> of the application.
        /// </summary>
        public static Frame Frame
        {
            get
            {
                if (_frame == null)
                {
                    _frame = Window.Current.Content as Frame;
                    RegisterFrameEvents();
                }

                return _frame;
            }

            set
            {
                UnregisterFrameEvents();
                _frame = value;
                RegisterFrameEvents();
            }
        }

        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether the <see cref="Frame"/> can go back.
        /// </summary>
        public static bool CanGoBack => Frame.CanGoBack;

        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether the <see cref="Frame"/> can go forward.
        /// </summary>
        public static bool CanGoForward => Frame.CanGoForward;

        /// <summary>
        /// Navigates to the previously visited <see cref="Page"/>.
        /// </summary>
        public static bool GoBack()
        {
            if (CanGoBack)
            {
                Frame.GoBack();
                AssignViewModel(Frame.Content as IViewWithViewModel);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Navigates forwards to the next <see cref="Page"/>.
        /// </summary>
        public static bool GoForward()
        {
            if (CanGoForward)
            {
                Frame.GoForward();
                AssignViewModel(Frame.Content as IViewWithViewModel);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Navigates to a given <see cref="Page"/> provided the <see cref="Type"/> of that page, and sets relevant DI information (including the <see cref="IViewModel"/>), and calls <see cref="IViewModel.Initialize"/> and <see cref="IViewWithViewModel.OnViewModelInitialized"/>.
        /// </summary>
        /// <param name="pageType">The type of the <see cref="Page"/> to navigate to.</param>
        /// <param name="parameter">A parameter to pass to the navigated <see cref="Page"/>.</param>
        /// <param name="infoOverride">The navigation transition information override</param>
        public static bool Navigate(Type pageType, object parameter = null, NavigationTransitionInfo infoOverride = null)
        {
            // Don't open the same page multiple times
            if (Frame.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(_lastParamUsed)))
            {
                var currentFrame = Frame;
                var navigationResult = currentFrame.Navigate(pageType, parameter, infoOverride);
                if (navigationResult)
                {
                    _lastParamUsed = parameter;
                    AssignViewModel(currentFrame.Content as IViewWithViewModel);

                    CurrentPageType = pageType;
                }

                return navigationResult;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Navigates to a given <see cref="Page"/> provided the <see cref="Type"/> of that page, and sets relevant DI information (including the <see cref="IViewModel"/>), and calls <see cref="IViewModel.Initialize"/> and <see cref="IViewWithViewModel.OnViewModelInitialized"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Page"/> to navigate to.</typeparam>
        /// <param name="parameter">A parameter to pass to the navigated <see cref="Page"/>.</param>
        /// <param name="infoOverride">The navigation transition information override</param>
        public static bool Navigate<T>(object parameter = null, NavigationTransitionInfo infoOverride = null)
            where T : Page
            => Navigate(typeof(T), parameter, infoOverride);

        private static void RegisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigating += Frame_NavigationStarted;
                _frame.Navigated += Frame_Navigated;
                _frame.NavigationFailed += Frame_NavigationFailed;

                CurrentPageType = _frame.Content?.GetType();
            }
        }

        private static void UnregisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigating -= Frame_NavigationStarted;
                _frame.Navigated -= Frame_Navigated;
                _frame.NavigationFailed -= Frame_NavigationFailed;
            }
        }

        private static void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e) => NavigationFailed?.Invoke(sender, e);
        private static void Frame_NavigationStarted(object sender, NavigatingCancelEventArgs e) => NavigationStarted?.Invoke(sender, e);
        private static void Frame_Navigated(object sender, NavigationEventArgs e) => Navigated?.Invoke(sender, e);

        #endregion
        #region Dialogs

        private static ContentDialog CurrentDialog;

        private static List<ContentDialog> Dialogs = new List<ContentDialog>();

        /// <summary>
        /// Pushes a <see cref="ContentDialog"/> to the notification stack.
        /// </summary>
        /// <param name="dialog">The <see cref="ContentDialog"/> to show.</param>
        /// <param name="isImportant">A <see cref="bool"/> indicating whether this dialog should replace other dialogs. Defaults to false.</param>
        public static async Task ShowDialog(ContentDialog dialog, bool isImportant = false)
        {
            AssignViewModel(dialog as IViewWithViewModel);

            if (!Dialogs.Contains(dialog))
            {
                Dialogs.Add(dialog);
            }

            if (CurrentDialog == null)
            {
                dialog.Closed += DialogClosed;
                CurrentDialog = dialog;
                await dialog.ShowAsync();
            }
            else if (isImportant)
            {
                Dialogs.Add(CurrentDialog);
                CurrentDialog.Hide();
                DialogClosed(CurrentDialog, null);
            }
        }

        private static async void DialogClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            sender.Closed -= DialogClosed;
            Dialogs.Remove(sender);
            CurrentDialog = null;
            if (Dialogs.Any())
            {
                await ShowDialog(Dialogs.First());
            }
        }

        #endregion
        #region DI

        /// <summary>
        /// The DI container used for view-model injection.
        /// </summary>
        public static IContainer Container { get; set; }

        private static void AssignViewModel(IViewWithViewModel pageWithModel)
        {
            if (pageWithModel != null)
            {
                var type = pageWithModel.GetType().GetInterfaces().First(inf => inf.IsConstructedGenericType && inf.GetGenericTypeDefinition() == typeof(IViewWithViewModel<>));
                var viewModelType = type.GetGenericArguments().First();
                var model = Container.Resolve(viewModelType);
                type.GetProperty("ViewModel").SetValue(pageWithModel, model);

                var viewModel = (model as IViewModel);

                if (viewModel != null)
                {
                    pageWithModel.OnViewModelInitialized();

                    var initTask = new SynchronousTask(
                        viewModel.Initialize,
                        SynchronousTask.DefaultExceptionAction);

                    initTask.RunTask();
                }
            }
        }

        /// <summary>
        /// Initializes the <see cref="Container"/> to get view-models from the given assemblies.
        /// </summary>
        /// <param name="viewModelAssemblyType">The <see cref="Assembly"/> containing the view-models.</param>
        /// <param name="modules">Any additional <see cref="Autofac.Module"/>s to add to the view-model DI <see cref="IContainer"/>.</param>
        public static void InitializeContainer(Assembly viewModelAssembly, IEnumerable<Autofac.Module> modules)
        {
            var builder = new ContainerBuilder();

            // Use module defined in this assembly to register types.
            builder.RegisterModule(new ModuleRegistration(viewModelAssembly));

            foreach (var module in modules)
            {
                // Use additional modules to register additional capabilities to the DI container.
                builder.RegisterModule(module);
            }

            Container = builder.Build();
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the registered <see cref="ContinuablePage"/> with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="ContinuablePage"/>.</param>
        public static Type GetContinuablePage(string name)
        {
            var types = Container.Resolve<IEnumerable<ContinuablePage>>();
            return types.FirstOrDefault(t => t.Name == name).GetType();
        }

        #endregion
    }
}
