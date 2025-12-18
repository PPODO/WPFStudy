using System;
using System.ComponentModel;

namespace TextBoxWithButton.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        #region Variable

        private Model.MainModel? model;

        private Command.Command buttonCommand;

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Property

        public Model.MainModel? Model
        {
            get { return model; }
            set
            {
                model = value;
                OnPropertyChanged("Model");
            }
        }

        public Command.Command ButtonCommand
        {
            get { return buttonCommand; }
        }

        #endregion

        public MainViewModel()
        {
            model = new Model.MainModel();
            buttonCommand = new Command.Command(ExecuteMethod, CanExecuteMethod);
        }

        private void ExecuteMethod(object? obj)
        {
            if (model != null) 
                model.OutputValue = model.InputValue * model.InputValue;
        }

        private bool CanExecuteMethod(object? obj)
        {

            return true;
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler? handler = PropertyChanged;

            if(handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
