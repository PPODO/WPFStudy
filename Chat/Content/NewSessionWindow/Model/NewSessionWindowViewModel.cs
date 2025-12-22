using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Content.NewSessionWindow.Model
{
    public class NewSessionWindowViewModel : INotifyPropertyChanged
    {
        #region Variable

        private int _maxUserCount = 1;

        #endregion

        #region Property

        public int MaxUserCount
        {
            get { return _maxUserCount; }
            set
            {
                _maxUserCount = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
