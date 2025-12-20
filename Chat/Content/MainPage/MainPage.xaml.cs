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

namespace Chat.Content.MainPage
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            DataContext = this;

            InitializeComponent();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            var currentWindow = (MainWindow)Application.Current.MainWindow;
            
            if (currentWindow != null)
            {
                currentWindow.UserNickname = UserNicknameTextBox.Text;
                currentWindow.ChangePage<SessionListPage.SessionListPage>("SessionListPage");
            }
        }
    }
}
