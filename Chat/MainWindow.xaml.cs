using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chat
{
    public partial class MainWindow : Window
    {
        #region Variable

        private string userNickname = "";

        #endregion

        #region Property

        public string UserNickname
        {
            get { return userNickname; }
            set { userNickname = value; }
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel.MainViewModel(this);
        }


        public T? ChangePage<T>(string titleName) where T : Page, new()
        {
            if (titleName.Length == 0) return null;

            T newPage = new T();
            newPage.Title = titleName;

            this.Content = newPage;

            return newPage;
        }
    }
}