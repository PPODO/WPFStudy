using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace SingleChat.Content.ChatRoom.Talk
{
    public class Talk : ObservableCollection<Message>
    {
        public Talk()
        {
        }

        public void AddMessage(string sender, string content)
        {
            this.Add(new Message(sender, content));
        }
    }

    public class Message
    {
        public Message(string sender, string content)
        {
            Sender = sender;
            Content = content;
        }

        public string Sender { get; set; }
        public string Content { get; set; }
    }
}