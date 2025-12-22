using Chat.Content.NewSessionWindow.Model;
using Chat.Content.SessionListPage.SessionList;
using Chat.Net.NetManager;
using Chat.Net.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Chat.Content.NewSessionWindow
{
    public partial class NewSessionWindow : Window
    {
        #region Variable

        private NewSessionWindowViewModel _viewModel;

        #endregion

        #region PacketAction
        private void MSG_RESPONSE_CREATE_SESSION(byte[] buffer)
        {
            var message = Net.Protocol.RESPONSE_CREATE_SESSION.GetMessage(buffer);

            if (message._feedback == 0)
            {
                var currentWindow = (MainWindow)Application.Current.MainWindow;

                if (currentWindow != null)
                {
                    NetManager.RequestModule.REQUEST_JOIN_SESSION(message._session_id, currentWindow.UserNickname);
                    this.Close();
                }
            }
        }
        #endregion

        #region CallbackEvent

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (SessionNameTextBox.Text.Length <= 0)
                MessageBox.Show("Invalid SessionName!");

            NetManager.RequestModule.REQUEST_CREATE_SESSION(SessionNameTextBox.Text, (ushort)_viewModel.MaxUserCount);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        public NewSessionWindow()
        {
            InitializeComponent();

            _viewModel = new NewSessionWindowViewModel();
            DataContext = _viewModel;

            NetManager.AddHandler(Protocol.MSG.MSG_RESPONSE_CREATE_SESSION, MSG_RESPONSE_CREATE_SESSION);
        }

        private void Increase_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
                ++_viewModel.MaxUserCount;
        }

        private void Decrease_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
                --_viewModel.MaxUserCount;
        }
    }
}
