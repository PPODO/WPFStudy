using System;
using System.Windows.Input;

namespace TextBoxWithButton.Command
{
    class Command : ICommand
    {
        Action<object> executeMethod;
        Func<object, bool> canExecuteMethod;

        public Command(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return canExecuteMethod(parameter);
        }

        public void Execute(object? parameter)
        {
            executeMethod(parameter);
        }
    }
}
