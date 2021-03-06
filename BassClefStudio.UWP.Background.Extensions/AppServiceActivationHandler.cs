﻿using BassClefStudio.NET.Core;
using BassClefStudio.UWP.Background.AppServices;
using BassClefStudio.UWP.Lifecycle;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace BassClefStudio.UWP.Background.Extensions
{
    /// <summary>
    /// An <see cref="IBackgroundActivationHandler"/> that can handle app service requests to the "IAppService" app service using the registered <see cref="IAppService"/>s.
    /// </summary>
    public class AppServiceActivationHandler : IBackgroundActivationHandler
    {
        /// <inheritdoc/>
        public bool Enabled { get; }

        /// <summary>
        /// The collection of attached <see cref="IAppService"/>s.
        /// </summary>
        public IEnumerable<IAppService> AppServices { get; }

        /// <summary>
        /// Creates a new <see cref="AppServiceActivationHandler"/>.
        /// </summary>
        /// <param name="appServices">The collection of attached <see cref="IAppService"/>s.</param>
        public AppServiceActivationHandler(IEnumerable<IAppService> appServices)
        {
            AppServices = appServices;
            Enabled = true;
        }

        private BackgroundTaskDeferral taskDeferral;
        private AppServiceDeferral serviceDeferral;

        /// <inheritdoc/>
        public bool BackgroundActivated(Application app, BackgroundActivatedEventArgs args)
        {
            if (args.TaskInstance.TriggerDetails is AppServiceTriggerDetails triggerDetails)
            {
                if (triggerDetails.Name == "IAppService")
                {
                    IBackgroundTaskInstance taskInstance = args.TaskInstance;

                    taskDeferral = args.TaskInstance.GetDeferral();
                    taskInstance.Canceled += OnAppServiceCanceled;
                    var appServiceConnection = triggerDetails.AppServiceConnection;
                    appServiceConnection.RequestReceived += OnRequestRecieved;
                    appServiceConnection.ServiceClosed += OnServiceClosed;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void OnServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            taskDeferral.Complete();
        }

        private void OnAppServiceCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            taskDeferral.Complete();
        }

        private void OnRequestRecieved(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            serviceDeferral = args.GetDeferral();
            var toRun = new SynchronousTask(() => ProcessRequest(sender, args));
            toRun.RunTask();
        }

        private async Task ProcessRequest(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            try
            {
                var input = new AppServiceInput(args.Request.Message);
                var service = AppServices.FirstOrDefault(s => s.CanExecute(input));
                if (service != null)
                {
                    var output = await service.ExecuteAsync(input);
                    ValueSet result = output.CreateOutput();
                    await args.Request.SendResponseAsync(result);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An exception {ex} was caught while attempting to respond to an app service request.");
            }
            finally
            {
                serviceDeferral.Complete();
            }
        }
    }
}
