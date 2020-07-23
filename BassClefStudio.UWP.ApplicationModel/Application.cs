using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BassClefStudio.NET.Core;
using BassClefStudio.UWP.ApplicationModel.Activation;
using BassClefStudio.UWP.ApplicationModel.AppServices;
using BassClefStudio.UWP.Background;
using BassClefStudio.UWP.Navigation;
using BassClefStudio.UWP.Sockets.Background;
using BassClefStudio.UWP.Sockets.Core;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BassClefStudio.UWP.ApplicationModel
{
    public abstract class Application : Windows.UI.Xaml.Application
    {
        #region Constructor

        public Func<IActivatedEventArgs, Type> MainPageType { get; }
        public Func<IActivatedEventArgs, Type> ShellPageType { get; }
        public Type[] AssemblyTypes { get; }


        public Application(Func<IActivatedEventArgs, Type> shellPageType, Type viewModelAssemblyType, Type[] assemblyTypes, Func<IActivatedEventArgs, Type> mainPageType)
        {
            ShellPageType = shellPageType;
            AssemblyTypes = assemblyTypes;
            NavigationService.InitializeContainer(viewModelAssemblyType, AssemblyTypes);
            MainPageType = mainPageType;
            this.Suspending += OnSuspending;
        }

        public Application(Type shellPageType, Type viewModelAssemblyType, Type[] assemblyTypes, Func<IActivatedEventArgs, Type> mainPageType) : this(a => shellPageType, viewModelAssemblyType, assemblyTypes, mainPageType)
        { }

        public Application(Type shellPageType, Type mainPageType, Type[] assemblyTypes) : this(a => shellPageType, shellPageType, assemblyTypes, a => mainPageType)
        { }

        public Application(Type shellPageType, Type mainPageType) : this(a => shellPageType, shellPageType, new Type[] { shellPageType }, a => mainPageType)
        { }

        public Application(Func<IActivatedEventArgs, Type> shellPageType, Type mainPageType) : this(shellPageType, mainPageType, new Type[] { mainPageType }, a => mainPageType)
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

        public AppServiceHandler[] AppServiceHandlers { get; private set; }

        public void SetupAppServices(params AppServiceHandler[] handlers)
        {
            AppServiceHandlers = handlers;
        }

        private async void OnAppServiceRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            Debug.WriteLine($"Recieved app service request");


            AppServiceDeferral messageDeferral = args.GetDeferral();

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
            finally
            {
                messageDeferral.Complete();
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

        public IBrokeredService[] BackgroundServices { get; private set; }

        public void SetupBackgroundConnections(params IBrokeredService[] services)
        {
            BackgroundServices = services;
        }

        private void PushAllConnections()
        {
            if (BackgroundServices != null)
            {
                foreach (var connection in BackgroundServices)
                {
                    if (connection.ServiceStatus == BrokeredServiceStatus.InProcess)
                    {
                        var result = connection.PushToBroker();
                        if (result)
                        {
                            Debug.WriteLine($"Connection {connection.Name} successfully pushed to broker.");
                        }
                        else
                        {
                            Debug.WriteLine($"Connection {connection.Name} failed to push to broker.");
                        }
                    }
                }
            }
        }

        #endregion
        #region BackgroundHandlers

        public BackgroundHandler[] BackgroundHandlers { get; private set; }
        public bool BackgroundHandlersLoaded { get; private set; } = false;

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

            if (BackgroundServices != null)
            {
                foreach (var c in BackgroundServices.OfType<IBackgroundSocketConnection>())
                {
                    tasks.Add(c.ConnectedBackgroundTask.RegisterTaskAsync(false));
                }
            }

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
