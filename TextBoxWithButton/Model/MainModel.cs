using System;
using System.Collections;
using System.ComponentModel;

namespace TextBoxWithButton.Model
{
    class MainModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public MainModel()
        {
            inputValue = 0;
            outputValue = 0;
        }

        #region Variable

        private int inputValue;
        private int outputValue;

        private IDictionary<string, List<string>> propertyErrors = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        #endregion

        #region Property

        public int InputValue
        {
            get { return inputValue; }
            set
            {
                inputValue = value;
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

        public bool HasErrors
        {
            get
            {
                return propertyErrors.Values.Any(r => r.Any());
            }
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
           PropertyChangedEventHandler? handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
                GetErrors(propertyName);
            }
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (propertyName == null)
                return null;

            if (propertyErrors.TryGetValue(propertyName, out var errors))
                return errors;

            return null;
        }
    }
}
