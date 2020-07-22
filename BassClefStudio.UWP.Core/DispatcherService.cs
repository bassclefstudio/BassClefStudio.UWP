using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace BassClefStudio.UWP.Core
{
    /// <summary>
    /// Provides a wrapper for the <see cref="CoreDispatcher"/> in running syncronous/asyncronous code on the UI thread in a UWP application.
    /// </summary>
    public static class DispatcherService
    {
        /// <summary>
        /// Executes an <see cref="Action"/> <paramref name="execute"/> on the UI thread.
        /// </summary> 
        public static async Task RunOnUIThread(Action execute)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => execute());
        }

        /// <summary>
        /// Executes an asyncronous <see cref="Task"/> <paramref name="execute"/> on the UI thread.
        /// </summary> 
        public static async Task RunOnUIThread(Func<Task> execute)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () => await execute());
        }

        /// <summary>
        /// Executes a <see cref="Func{TResult}"/> <paramref name="execute"/> on the UI thread and returns the output.
        /// </summary> 
        public static async Task<T> RunOnUIThread<T>(Func<T> execute)
        {
            T output = default(T);

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => output = execute());

            return output;
        }

        /// <summary>
        /// Executes an asyncronous <see cref="Task{T}"/> <paramref name="execute"/> on the UI thread and returns the output.
        /// </summary> 
        public static async Task<T> RunOnUIThread<T>(Func<Task<T>> execute)
        {
            T output = default(T);

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () => output = await execute());

            return output;
        }
    }
}
