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

namespace SingleChat.Content.Login
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = (MainWindow)Application.Current.MainWindow;

            if (parentWindow != null)
            {
                var chatPage = parentWindow.ChangePage<ChatRoom.ChatRoom>("ChatRoom");
                if (chatPage != null)
                    chatPage.LocalUserName = UserNameTextBox.Text;
            }
        }
    }
}
