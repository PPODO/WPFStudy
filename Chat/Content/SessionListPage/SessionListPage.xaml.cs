using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat.Content.SessionListPage
{
    public partial class SessionListPage : Page
    {
        #region Variable

        private SessionList.SessionList _sessionList;

        #endregion

        #region Property

        public SessionList.SessionList SessionInfos
        {
            get { return _sessionList; }
        }

        #endregion

        public SessionListPage()
        {
            InitializeComponent();

            DataContext = this;

            _sessionList = new SessionList.SessionList();
        }


    }
}
