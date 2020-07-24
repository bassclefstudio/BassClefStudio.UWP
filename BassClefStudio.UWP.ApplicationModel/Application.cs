using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BassClefStudio.NET.Core;
using BassClefStudio.UWP.ApplicationModel.Activation;
using BassClefStudio.UWP.ApplicationModel.AppServices;
using BassClefStudio.UWP.Background;
using BassClefStudio.UWP.Navigation;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BassClefStudio.UWP.ApplicationModel
{
    /// <summary>
    /// Represents a UWP <see cref="Windows.UI.Xaml.Application"/> with additional logic for connecting to the BassClefStudio.UWP.Navigation stack, handling application launch, and managing background tasks.
    /// </summary>
    public abstract class Application : Windows.UI.Xaml.Application
    {
        #region Constructor

        /// <summary>
        /// A <see cref="Func{T, TResult}"/> that returns the <see cref="Type"/> of the main page of the application given the <see cref="IActivatedEventArgs"/>.
        /// </summary>
        public Func<IActivatedEventArgs, Type> MainPageType { get; }

        /// <summary>
        /// A <see cref="Func{T, TResult}"/> that returns the <see cref="Type"/> of the shell (navigation) page of the application given the <see cref="IActivatedEventArgs"/>.
        /// </summary>
        public Func<IActivatedEventArgs, Type> ShellPageType { get; }

        /// <summary>
        /// A list of the <see cref="Assembly"/> objects to initialize the Navigation DI container with.
        /// </summary>
        public Assembly[] Assemblies { get; }

        /// <summary>
        /// Creates an <see cref="Application"/> with the given activation information and DI parameters.
        /// </summary>
        /// <param name="shellPageType">A <see cref="Func{T, TResult}"/> that returns the <see cref="Type"/> of the shell (navigation) page of the application given the <see cref="IActivatedEventArgs"/>.</param>
        /// <param name="viewModelAssembly">The <see cref="Assembly"/> that the view-models are contained in.</param>
        /// <param name="assemblies">A collection of <see cref="Assembly"/> objects to register with the DI container.</param>
        /// <param name="mainPageType">A <see cref="Func{T, TResult}"/> that returns the <see cref="Type"/> of the main page of the application given the <see cref="IActivatedEventArgs"/>.</param>
        public Application(Func<IActivatedEventArgs, Type> shellPageType, Assembly viewModelAssembly, Assembly[] assemblies, Func<IActivatedEventArgs, Type> mainPageType)
        {
            ShellPageType = shellPageType;
            Assemblies = assemblies;
            NavigationService.InitializeContainer(viewModelAssembly, Assemblies);
            MainPageType = mainPageType;
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Creates an <see cref="Application"/> with the given activation information and DI parameters.
        /// </summary>
        /// <param name="shellPageType">The <see cref="Type"/> of the shell (navigation) page of the application.</param>
        /// <param name="viewModelAssembly">The <see cref="Assembly"/> that the view-models are contained in.</param>
        /// <param name="assemblies">A collection of <see cref="Assembly"/> objects to register with the DI container.</param>
        /// <param name="mainPageType">A <see cref="Func{T, TResult}"/> that returns the <see cref="Type"/> of the main page of the application given the <see cref="IActivatedEventArgs"/>.</param>
        public Application(Type shellPageType, Assembly viewModelAssembly, Assembly[] assemblies, Func<IActivatedEventArgs, Type> mainPageType) : this(a => shellPageType, viewModelAssembly, assemblies, mainPageType)
        { }

        /// <summary>
        /// Creates an <see cref="Application"/> with the given activation information and DI parameters.
        /// </summary>
        /// <param name="shellPageType">The <see cref="Type"/> of the shell (navigation) page of the application.</param>
        /// <param name="assemblies">A collection of <see cref="Assembly"/> objects to register with the DI container.</param>
        /// <param name="mainPageType">The <see cref="Type"/> of the main page of the application.</param>
        public Application(Type shellPageType, Type mainPageType, Assembly[] assemblies) : this(a => shellPageType, shellPageType.GetTypeInfo().Assembly, assemblies, a => mainPageType)
        { }

        /// <summary>
        /// Creates an <see cref="Application"/> with the given activation information and DI parameters.
        /// </summary>
        /// <param name="shellPageType">The <see cref="Type"/> of the shell (navigation) page of the application.</param>
        /// <param name="mainPageType">The <see cref="Type"/> of the main page of the application.</param>
        public Application(Type shellPageType, Type mainPageType) : this(a => shellPageType, shellPageType.GetTypeInfo().Assembly, new Assembly[] { shellPageType.GetTypeInfo().Assembly }, a => mainPageType)
        { }

        /// <summary>
        /// Creates an <see cref="Application"/> with the given activation information and DI parameters.
        /// </summary>
        /// <param name="shellPageType">A <see cref="Func{T, TResult}"/> that returns the <see cref="Type"/> of the shell (navigation) page of the application given the <see cref="IActivatedEventArgs"/>.</param>
        /// <param name="mainPageType">The <see cref="Type"/> of the main page of the application.</param>
        public Application(Func<IActivatedEventArgs, Type> shellPageType, Type mainPageType) : this(shellPageType, mainPageType.GetTypeInfo().Assembly, new Assembly[] { mainPageType.GetTypeInfo().Assembly }, a => mainPageType)
        { }

        #endregion
        #region SystemEvents

        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = true;
            if(NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        #endregion
        #region Lifecycle

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            ////Do app close/save stuff here.
            PushAllConnections();

            deferral.Complete();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.TileId == "App")
            {
                ActivateWindow(args);
            }
            else
            {
                ActivateWindow(args, args.TileActivatedInfo);
            }
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.File)
            {
                FileActivatedEventArgs fileArgs = args as FileActivatedEventArgs;
                ActivateWindow(fileArgs, fileArgs.Files);
            }
            else if (args.Kind == ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs protocolArgs = args as ProtocolActivatedEventArgs;
                var query = Microsoft.QueryStringDotNET.QueryString.Parse(protocolArgs.Uri.Query).ToDictionary(q => q.Name, q => q.Value.ToString());
                var continuablePageType = NavigationService.GetContinuablePage(protocolArgs.Uri.AbsolutePath);
                ActivateWindow(protocolArgs, query, continuablePageType);
            }
            else if (args.Kind == ActivationKind.ToastNotification)
            {
                ToastNotificationActivatedEventArgs toastArgs = args as ToastNotificationActivatedEventArgs;
                ActivateWindow(toastArgs, toastArgs.Argument);
            }
            else
            {
                ActivateWindow(args);
            }
        }

        private void ActivateWindow(IActivatedEventArgs args, object parameter = null, Type overridePageType = null)
        {
            Debug.WriteLine($"Activating window...");

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.Content = new Frame();
                NavigationService.Frame = rootFrame;

                if (overridePageType != null)
                {
                    NavigationService.Navigate(ShellPageType(args), new ActivationInfo(args, parameter, overridePageType));
                }
                else
                {
                    NavigationService.Navigate(ShellPageType(args), new ActivationInfo(args, parameter, MainPageType(args)));
                }

                Window.Current.Content = rootFrame;
            }

            //// Ensure the current window is active
            Window.Current.Activate();

            ////Register system events
            SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;

            ////Start setup services tasks here
            _ = CreateBackgroundHandlers();
        }

        #endregion
        #region Background

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            Debug.WriteLine("Background action activated...");

            base.OnBackgroundActivated(args);
            IBackgroundTaskInstance taskInstance = args.TaskInstance;
            if (taskInstance.TriggerDetails is AppServiceTriggerDetails appService)
            {
                _appServiceDeferral = taskInstance.GetDeferral();
                taskInstance.Canceled += OnAppServicesCanceled;
                _appServiceConnection = appService.AppServiceConnection;
                _appServiceConnection.RequestReceived += OnAppServiceRequestReceived;
                _appServiceConnection.ServiceClosed += AppServiceConnection_ServiceClosed;
            }
            else 
            {
                BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
                var toRun = new SynchronousTask(() => StartBackgroundTask(taskInstance, deferral), BackgroundTaskFailed);
                toRun.RunTask();
            }
        }

        #region AppServices

        private AppServiceConnection _appServiceConnection;
        private BackgroundTaskDeferral _appServiceDeferral;

        /// <summary>
        /// Represents the collection of <see cref="AppServiceHandler"/>s that can manage background and foreground app service requests.
        /// </summary>
        public AppServiceHandler[] AppServiceHandlers { get; private set; }

        /// <summary>
        /// Initializes the <see cref="AppServiceHandlers"/> collection.
        /// </summary>
        /// <param name="handlers">The <see cref="Application"/>'s <see cref="AppServiceHandler"/>s.</param>
        public void SetupAppServices(params AppServiceHandler[] handlers)
        {
            AppServiceHandlers = handlers;
        }

        private void OnAppServiceRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            AppServiceDeferral messageDeferral = args.GetDeferral();
            var toRun = new SynchronousTask(() => ProcessAppServiceRequest(args));
            toRun.RunTask();
        }

        private async Task ProcessAppServiceRequest(AppServiceRequestReceivedEventArgs args)
        {
            Debug.WriteLine($"Recieved app service request.");

            try
            {
                AppServiceInput inputs = AppServiceInput.Parse(args.Request.Message);
                AppServiceOutput output = await AppServiceHandlers.FirstOrDefault(a => a.Commands.Contains(inputs.Command)).RunAppService(inputs);
                await args.Request.SendResponseAsync(output.ToValueSet());
            }
            catch (Exception ex)
            {
                try
                {
                    AppServiceOutput output = new AppServiceOutput(AppServiceStatus.Fail, ex);
                    await args.Request.SendResponseAsync(output.ToValueSet());
                }
                catch
                {
                    ////Add internal exception handling here.
                }
            }
        }

        private void OnAppServicesCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _appServiceDeferral.Complete();
        }

        private void AppServiceConnection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            _appServiceDeferral.Complete();
        }

        #endregion
        #region Communication

        //public IBrokeredService[] BackgroundServices { get; private set; }

        //public void SetupBackgroundConnections(params IBrokeredService[] services)
        //{
        //    BackgroundServices = services;
        //}

        //private void PushAllConnections()
        //{
        //    if (BackgroundServices != null)
        //    {
        //        foreach (var connection in BackgroundServices)
        //        {
        //            if (connection.ServiceStatus == BrokeredServiceStatus.InProcess)
        //            {
        //                var result = connection.PushToBroker();
        //                if (result)
        //                {
        //                    Debug.WriteLine($"Connection {connection.Name} successfully pushed to broker.");
        //                }
        //                else
        //                {
        //                    Debug.WriteLine($"Connection {connection.Name} failed to push to broker.");
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion
        #region BackgroundHandlers

        /// <summary>
        /// Represents the collection of <see cref="BackgroundHandler"/>s that can manage background task activation.
        /// </summary>
        public BackgroundHandler[] BackgroundHandlers { get; private set; }

        /// <summary>
        /// A <see cref="bool"/> value indicating whether all <see cref="BackgroundHandler"/>s have been registered.
        /// </summary>
        public bool BackgroundHandlersLoaded { get; private set; } = false;

        /// <summary>
        /// Initializes the <see cref="BackgroundHandlers"/> collection.
        /// </summary>
        /// <param name="handlers">The <see cref="Application"/>'s <see cref="BackgroundHandler"/>s.</param>
        public void SetupBackgroundTasks(params BackgroundHandler[] handlers)
        {
            BackgroundHandlers = handlers;
        }

        private async Task CreateBackgroundHandlers()
        {
            List<Task> tasks = new List<Task>();
            if (BackgroundHandlers != null)
            {
                foreach (var r in BackgroundHandlers)
                {
                    tasks.Add(r.RegisterTaskAsync());
                }
            }

            //if (BackgroundServices != null)
            //{
            //    foreach (var c in BackgroundServices.OfType<IBackgroundSocketConnection>())
            //    {
            //        tasks.Add(c.ConnectedBackgroundTask.RegisterTaskAsync(false));
            //    }
            //}

            foreach (var task in tasks)
            {
                await task;
            }

            BackgroundHandler.CompleteRegistrations();
            BackgroundHandlersLoaded = true;
        }

        private async Task StartBackgroundTask(IBackgroundTaskInstance taskInstance, BackgroundTaskDeferral deferral)
        {
            if(!BackgroundHandlersLoaded)
            {
                await CreateBackgroundHandlers();
            }

            var task = BackgroundHandler.BackgroundHandlers
                    .FirstOrDefault(h => h.Name == taskInstance.Task.Name);
            if (task != null)
            {
                Debug.WriteLine($"Starting associated background task (name {task.Name})...");
                await task.Task(taskInstance);
            }
            else
            {
                Debug.WriteLine($"Failed to find the associated background task (name {taskInstance.Task.Name}).");
                ////Failed to find the task. This is a problem.
            }

            deferral.Complete();
        }

        private void BackgroundTaskFailed(Exception ex)
        {
            Debug.WriteLine($"Background task failed (exception {ex})...");
            ////Background task execution failed. Maybe get some sort of information here?
            throw ex;
        }

        #endregion

        #endregion
    }
}
