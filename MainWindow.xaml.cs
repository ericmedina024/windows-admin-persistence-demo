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

namespace windows_admin_persistence_demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(string title, string statusTextBlockText)
        {
            InitializeComponent();
            Title = title;
            statusTextBlock.Text = statusTextBlockText;
        }

        void HandleDeleteButton(object sender, RoutedEventArgs eventArgs)
        {
            if (App.IsAdministrator())
            {
                App.DeletePersistenceTask();
                statusTextBlock.Text = "Persistence task deleted";
            }
            else
            {
                statusTextBlock.Text = "Persistence task cant be deleted when not running as admin";
            }
        }
        
    }
}