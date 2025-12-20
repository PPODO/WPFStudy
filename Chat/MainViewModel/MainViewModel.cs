using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Chat.MainViewModel
{
    class MainViewModel
    {
        #region

        private Window _window;

        #endregion

        public MainViewModel(Window window)
        {
            this._window = window;
        }
    }
}
