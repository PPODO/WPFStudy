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

namespace SingleChat.Content.ChatRoom
{
    /// <summary>
    /// Interaction logic for ChatRoom.xaml
    /// </summary>
    public partial class ChatRoom : Page
    {
        #region Variable

        private Talk.Talk messages;

        private string localUserName;

        #endregion

        #region Property

        public Talk.Talk Messages
        {
            get { return messages; }
            set { messages = value; }
        }

        public string LocalUserName
        {
            get { return localUserName; }
            set { localUserName = value; }
        }

        #endregion

        public ChatRoom()
        {
            InitializeComponent();

            DataContext = this;

            LocalUserName = "";
            Messages = new Talk.Talk();
        }

        public void OnEnterKeyHit(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Messages.AddMessage(LocalUserName, ChatTextBox.Text);
        }
    }
}
