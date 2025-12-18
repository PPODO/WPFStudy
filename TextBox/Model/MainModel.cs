using System;
using System.ComponentModel;

namespace TextBox.Model
{
    class MainModel : INotifyPropertyChanged
    {
        public MainModel()
        {
            inputValue = 0;
            outputValue = 0;
        }

        #region Variable

        private int inputValue;
        private int outputValue;

        #endregion

        #region Property

        public int InputValue
        {
            get { return inputValue; }
            set
            {
                inputValue = value;
                OutputValue = (value * value);
                OnPropertyChanged("InputValue");
            }
        }

        public int OutputValue
        {
            get { return outputValue; }
            set
            {
                outputValue = value; 
                OnPropertyChanged("OutputValue");
            }
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
