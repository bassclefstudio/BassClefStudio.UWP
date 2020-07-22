using System.Threading.Tasks;

namespace BassClefStudio.UWP.Navigation.DI
{
    /// <summary>
    /// Represents a view-model in the MVVM pattern.
    /// </summary>
    public interface IViewModel
    {
        /// <summary>
        /// An asynchronous <see cref="Task"/> that is started when the <see cref="IViewModel"/> is initialized.
        /// </summary>
        Task Initialize();
    }

    /// <summary>
    /// Represents a view in the MVVM pattern.
    /// </summary>
    public interface IViewWithViewModel
    {
        /// <summary>
        /// A method that is called when the associated view-model is initialized.
        /// </summary>
        void OnViewModelInitialized();
    }

    /// <summary>
    /// Represents a view in the MVVM pattern with an attached view-model of type <typeparamref name="T"/>.
    /// </summary>
    public interface IViewWithViewModel<T> : IViewWithViewModel
        where T : IViewModel
    {
        /// <summary>
        /// The view's attached view-model.
        /// </summary>
        T ViewModel { get; set; }
    }
}
