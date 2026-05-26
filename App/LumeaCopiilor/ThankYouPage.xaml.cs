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
using System.Windows.Shapes;

namespace LumeaCopiilor
{
    /// <summary>
    /// Interaction logic for ThankYouPage.xaml
    /// </summary>
    public partial class ThankYouPage : Window
    {
        public ThankYouPage()
        {
            InitializeComponent();
        }

        private void InchidereButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void InapoiButton_Click(object sender, RoutedEventArgs e)
        {
            DashboardUser dashboardUser = new DashboardUser();
            dashboardUser.Show();
            this.Close();
        }
    }
}
