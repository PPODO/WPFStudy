using Chat.Net.NetManager;
using Chat.Net.Protocol;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        #region PacketAction

        private void MSG_NOTICE_SESSION(byte[] buffer)
        {
            var message = Net.Protocol.NOTICE_SESSION.GetMessage(buffer);

            _sessionList.AddNewSession(message._sessionID, message._sessionName, message._joinedUserCount, message._maxUserCount);
        }

        private void MSG_RESPONSE_JOIN_SESSION(byte[] buffer)
        {
            var message = Net.Protocol.RESPONSE_JOIN_SESSION.GetMessage(buffer);
            if (message._feedback != 0)
            {
                MessageBox.Show("JOIN SESSION FAILED!");
                return;
            }

            var currentWindow = (MainWindow)Application.Current.MainWindow;
            if (currentWindow != null)
            {
                var page = currentWindow.ChangePage<ChatRoomPage.ChatRoomPage>("ChatRoomPage");

                if (page != null)
                {
                    page.JoinProcess("[WELCOME] " + currentWindow.UserNickname + " JOIN!");
                    page.SessionID = (UInt32)message._session_id;
                }
            }
        }

        #endregion

        private SessionList.SessionInfo? GetSessionInfoFromListView(RoutedEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while (!(dep is System.Windows.Controls.ListViewItem))
            {
                try
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                catch
                {
                    return null;
                }
            }

            ListViewItem item = (ListViewItem)dep;

            return (SessionList.SessionInfo)item.Content;
        }

        public SessionListPage()
        {
            InitializeComponent();

            DataContext = this;

            _sessionList = new SessionList.SessionList();

            NetManager.AddHandler(Protocol.MSG.MSG_NOTICE_SESSION, MSG_NOTICE_SESSION);
            NetManager.AddHandler(Protocol.MSG.MSG_RESPONSE_JOIN_SESSION, MSG_RESPONSE_JOIN_SESSION);

            NetManager.RequestModule.REQUEST_SESSION_LIST();
        }

        private void Create_Chat_Room_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var childWindow = new NewSessionWindow.NewSessionWindow();

            childWindow.ShowDialog();
        }

        private void Chat_Room_Enter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var currentWindow = (MainWindow)Application.Current.MainWindow;
            if (currentWindow == null) return;

            var sessionInfo = GetSessionInfoFromListView(e);
            if (sessionInfo == null)
            {
                MessageBox.Show("Invalid Session!");
                return;
            }

            NetManager.RequestModule.REQUEST_JOIN_SESSION(sessionInfo.SessionID, currentWindow.UserNickname);
        }
    }
}
