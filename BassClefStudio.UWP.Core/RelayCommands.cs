using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BassClefStudio.UWP.Core
{
    /// <summary>
    /// Represents a service that can build a <see cref="RelayCommand"/> for the given <see cref="Action"/> or asynchronous <see cref="Task"/>s.
    /// </summary>
    public class RelayCommandBuilder
    {
        /// <summary>
        /// Creates a <see cref="RelayCommandBuilder"/> for an asynchronous <see cref="Task"/>.
        /// </summary>
        /// <param name="execute">The <see cref="Task"/> to execute.</param>
        public RelayCommandBuilder(Func<Task> execute)
        {
            Command = new RelayCommand(async () => await execute());
        }

        /// <summary>
        /// Creates a <see cref="RelayCommandBuilder"/> for an <see cref="Action"/>.
        /// </summary>
        /// <param name="execute">The <see cref="Action"/> to execute.</param>
        public RelayCommandBuilder(Action execute)
        {
            Command = new RelayCommand(execute);
        }

        /// <summary>
        /// Creates a <see cref="RelayCommandBuilder"/> for an asynchronous <see cref="Task"/> with a <paramref name="canExecute"/> conditional.
        /// </summary>
        /// <param name="execute">The <see cref="Task"/> to execute.</param>
        /// <param name="canExecute">A <see cref="Func{TResult}"/> that returns a <see cref="bool"/> indicating whether the <see cref="RelayCommand"/> is enabled.</param>
        public RelayCommandBuilder(Func<Task> execute, Func<bool> canExecute)
        {
            Command = new RelayCommand(async () => await execute(), canExecute);
        }

        /// <summary>
        /// Creates a <see cref="RelayCommandBuilder"/> for an <see cref="Action"/> with a <paramref name="canExecute"/> conditional.
        /// </summary>
        /// <param name="execute">The <see cref="Action"/> to execute.</param>
        /// <param name="canExecute">A <see cref="Func{TResult}"/> that returns a <see cref="bool"/> indicating whether the <see cref="RelayCommand"/> is enabled.</param>
        public RelayCommandBuilder(Action execute, Func<bool> canExecute)
        {
            Command = new RelayCommand(execute, canExecute);
        }

        /// <summary>
        /// Contains the created <see cref="RelayCommand"/>.
        /// </summary>
        public RelayCommand Command { get; }
    }

    /// <summary>
    /// Represents a service that can build a <see cref="RelayCommand{T}"/> for the given <see cref="Action{T}"/> or asynchronous <see cref="Task"/>s.
    /// </summary>
    public class RelayCommandBuilder<T>
    {
        /// <summary>
        /// Creates a <see cref="RelayCommandBuilder{T}"/> for an asynchronous <see cref="Task"/>.
        /// </summary>
        /// <param name="execute">The <see cref="Task"/> to execute.</param>
        public RelayCommandBuilder(Func<T,Task> execute)
        {
            Command = new RelayCommand<T>(async (t) => await execute(t));
        }

        /// <summary>
        /// Creates a <see cref="RelayCommandBuilder{T}"/> for an <see cref="Action{T}"/>.
        /// </summary>
        /// <param name="execute">The <see cref="Action{T}"/> to execute.</param>
        public RelayCommandBuilder(Action<T> execute)
        {
            Command = new RelayCommand<T>(execute);
        }

        /// <summary>
        /// Creates a <see cref="RelayCommandBuilder{T}"/> for an asynchronous <see cref="Task"/> with a <paramref name="canExecute"/> conditional.
        /// </summary>
        /// <param name="execute">The <see cref="Task"/> to execute.</param>
        /// <param name="canExecute">A <see cref="Func{T, TResult}"/> that returns a <see cref="bool"/> indicating whether the <see cref="RelayCommand{T}"/> is enabled.</param>
        public RelayCommandBuilder(Func<T, Task> execute, Func<T, bool> canExecute)
        {
            Command = new RelayCommand<T>(async (t) => await execute(t), (t) => canExecute(t));
        }

        /// <summary>
        /// Creates a <see cref="RelayCommandBuilder{T}"/> for an <see cref="Action{T}"/> with a <paramref name="canExecute"/> conditional.
        /// </summary>
        /// <param name="execute">The <see cref="Action{T}"/> to execute.</param>
        /// <param name="canExecute">A <see cref="Func{T, TResult}"/> that returns a <see cref="bool"/> indicating whether the <see cref="RelayCommand{T}"/> is enabled.</param>
        public RelayCommandBuilder(Action<T> execute, Func<T, bool> canExecute)
        {
            Command = new RelayCommand<T>(execute, canExecute);
        }

        /// <summary>
        /// Contains the created <see cref="RelayCommand{T}"/>.
        /// </summary>
        public RelayCommand<T> Command { get; }
    }

    /// <summary>
    /// Represents a basic command in the MVVM patern.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;

        private readonly Func<bool> _canExecute;

        /// <summary>
        /// An event that is fired when the <see cref="RelayCommand"/>'s ability to execute has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Creates a basic <see cref="RelayCommand"/> with no conditional.
        /// </summary>
        /// <param name="execute">The <see cref="Action"/> to execute.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a <see cref="RelayCommand"/> with a conditional <see cref="Func{TResult}"/>.
        /// </summary>
        /// <param name="execute">The <see cref="Action"/> to execute.</param>
        /// <param name="canExecute">A function that returns a <see cref="bool"/> indicating whether the <see cref="RelayCommand"/> can execute.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating whether the command can execute.
        /// </summary>
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        /// <summary>
        /// Executes the command and its related action.
        /// </summary>
        public void Execute(object parameter) => _execute();

        /// <summary>
        /// Fires the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Represents a basic command in the MVVM patern.
    /// </summary>
    /// <typeparam name="T">The type of the input parameter for the command.</typeparam>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;

        private readonly Func<T, bool> _canExecute;

        /// <summary>
        /// An event that is fired when the <see cref="RelayCommand"/>'s ability to execute has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Creates a basic <see cref="RelayCommand{T}"/> with no conditional.
        /// </summary>
        /// <param name="execute">The <see cref="Action{T}"/> to execute.</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a <see cref="RelayCommand{T}"/> with a conditional <see cref="Func{T, TResult}"/>.
        /// </summary>
        /// <param name="execute">The <see cref="Action{T}"/> to execute.</param>
        /// <param name="canExecute">A function that returns a <see cref="bool"/> indicating whether the <see cref="RelayCommand"/> can execute.</param>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Gets a <see cref="bool"/> indicating whether the command can execute with the given <typeparamref name="T"/> <paramref name="parameter"/>.
        /// </summary>
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);

        /// <summary>
        /// Executes the command and its related action.
        /// </summary>
        public void Execute(object parameter) => _execute((T)parameter);

        /// <summary>
        /// Fires the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
