using System;
using System.ComponentModel;

namespace TextBox.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        private Model.MainModel? mainModel = null;

        public MainViewModel()
        {
            mainModel = new Model.MainModel();
        }

        #region Property

        public Model.MainModel? Model
        {
            get { return mainModel; }
            set { mainModel = value; OnPropertyChanged("Model"); }
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler? handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }
}
