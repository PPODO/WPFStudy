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

namespace SingleChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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