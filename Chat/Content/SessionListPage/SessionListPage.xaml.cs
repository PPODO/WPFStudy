using Chat.Net.NetManager;
using Chat.Net.Protocol;
using System.Windows.Controls;

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

        private void MSG_NOTICE_SESSION_LIST(byte[] buffer)
        {
            var message = Net.Protocol.NOTICE_SESSION_LIST.GetMessage(buffer);

            _sessionList.AddNewSession(message._sessionID, message._sessionName, message._joinedUserCount, message._maxUserCount);
        }

        #endregion

        public SessionListPage()
        {
            InitializeComponent();

            DataContext = this;

            _sessionList = new SessionList.SessionList();

            NetManager.AddHandler(Protocol.MSG.MSG_NOTICE_SESSION_LIST, MSG_NOTICE_SESSION_LIST);

            NetManager.RequestModule.REQUEST_SESSION_LIST();
        }
    }
}
