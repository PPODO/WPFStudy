using Chat.Net.NetManager;
using Chat.Net.Protocol;
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

namespace Chat.Content.ChatRoomPage
{
    public partial class ChatRoomPage : Page
    {
        #region Variable 

        Talk.Talk messages;

        private UInt32 session_id;

        #endregion

        #region Property

        public Talk.Talk Messages
        {
            get => messages;
        }

        public UInt32 SessionID
        {
            get => session_id;
            set => session_id = value;
        }

        #endregion

        #region PacketAction
        private void MSG_NOTICE_JOIN_SESSION(byte[] buffer)
        {
            var message = Net.Protocol.NOTICE_JOIN_SESSION.GetMessage(buffer);

            if (message._joined_user_nickname.Length > 0)
                JoinProcess("[WELCOME] " + message._joined_user_nickname + " JOIN!");
        }
        private void MSG_NOTICE_CHAT_MESSAGE(byte[] buffer)
        {
            var message = Net.Protocol.NOTICE_CHAT_MESSAGE.GetMessage(buffer);

            if (message._joined_user_nickname.Length > 0 && message._chat_message.Length > 0)
                Messages.Add(new Talk.Message(message._joined_user_nickname, message._chat_message));
        }

        #endregion

        public ChatRoomPage()
        {
            InitializeComponent();

            messages = new Talk.Talk();

            DataContext = this;

            NetManager.AddHandler(Protocol.MSG.MSG_NOTICE_JOIN_SESSION, MSG_NOTICE_JOIN_SESSION);
            NetManager.AddHandler(Protocol.MSG.MSG_NOTICE_CHAT_MESSAGE, MSG_NOTICE_CHAT_MESSAGE);
        }

        public void JoinProcess(string joinedUserNickname)
        {
            Messages.Add(new Talk.Message("SERVER", joinedUserNickname));
        }

        private void ChatTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NetManager.RequestModule.REQUEST_CHAT_MESSAGE(session_id, ChatTextBox.Text);
                ChatTextBox.Text = "";
            }
        }
    }
}
