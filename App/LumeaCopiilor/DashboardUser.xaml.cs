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
using System.Data;
using Microsoft.Data.SqlClient;

namespace LumeaCopiilor
{
    /// <summary>
    /// Interaction logic for DashboardUser.xaml
    /// </summary>
    /// 
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
    }

    public partial class DashboardUser : Window
    {
        private readonly int _userId;

        public DashboardUser(int userId = 0)
        {
            InitializeComponent();
            _userId = userId;
            LoadProducts();
        }

        private const string ConnectionString = "Server=.\\SQLEXPRESS;Database=Lumea_Copiilor;Integrated Security=True;TrustServerCertificate=True;";

        private void LoadProducts(string searchTerm = null)
        {
            List<Product> products = new List<Product>();

            string query = "SELECT ProductID, Name, Price FROM Product WHERE Quantity > 0";
            if (!string.IsNullOrWhiteSpace(searchTerm))
                query += " AND Name LIKE @search";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (!string.IsNullOrWhiteSpace(searchTerm))
                    cmd.Parameters.AddWithValue("@search", $"%{searchTerm}%");

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ProductID = Convert.ToInt32(reader["ProductID"]),
                            Name = reader["Name"].ToString(),
                            Price = Convert.ToDecimal(reader["Price"]).ToString("0.00") + " lei"
                        });
                    }
                }
            }

            ProductsItemsControl.ItemsSource = products;
        }

        private void ProductCard_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var product = border?.DataContext as Product;
            if (product == null) return;

            this.Opacity = 0.78;
            var popup = new productUserPopup(product.ProductID, _userId) { Owner = this };
            popup.ShowDialog();
            this.Opacity = 1;
        }

        private void MyAcc_Click(object sender, RoutedEventArgs e)
        {
            UserAccountPage userAccount = new UserAccountPage(_userId, this);
            userAccount.Show();
            this.Hide();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts(SearchInput.Text);
        }

        private void Button_Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
