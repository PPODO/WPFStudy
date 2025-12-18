using Button.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Button.MainViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Window _window;

        private int _counter = 0;

        public event PropertyChangedEventHandler? PropertyChanged;

        public RelayCommand ClickCommand {  get; set; }

        public MainViewModel(Window window)
        {
            this._window = window;
            ClickCommand = new RelayCommand(OnClick);
        }

        public int Counter
        {
            get => _counter;
            set
            {
                _counter = value;
                OnPropertyChanged();
            }
        }

        private void OnClick()
        {
            ++Counter;
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
