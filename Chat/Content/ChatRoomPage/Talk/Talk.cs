using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Content.ChatRoomPage.Talk
{
    public class Message
    {
        #region Variable

        public string Sender { get; set; }
        public string Msg { get; set; }

        #endregion

        public Message(string sender, string msg)
        {
            Sender = sender;
            Msg = msg;
        }
    }

    public class Talk : ObservableCollection<Message>
    {
        public Talk()
        {
        }

        public void AddTalk(Message message)
        {
            this.Add(message);
        }
    }
}
