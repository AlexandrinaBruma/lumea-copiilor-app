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
using Microsoft.Data.SqlClient;

namespace LumeaCopiilor
{
    /// <summary>
    /// Interaction logic for DashboardAdmin.xaml
    /// </summary>
    /// 

    public class ProductAdmin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string Category { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class Importer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }

    public partial class DashboardAdmin : Window
    {
        public DashboardAdmin()
        {
            InitializeComponent();

            LoadProducts();
            LoadUsers();
            LoadImporters();
        }

        private const string ConnectionString = "Server=.\\SQLEXPRESS;Database=Lumea_Copiilor;Integrated Security=True;TrustServerCertificate=True;";

        private void LoadProducts()
        {
            List<ProductAdmin> products = new List<ProductAdmin>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT p.ProductID, p.Name, p.Price, c.Name AS CategoryName " +
                "FROM Product p " +
                "JOIN Category c ON p.Category = c.CategoryID", conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new ProductAdmin
                        {
                            Id = Convert.ToInt32(reader["ProductID"]),
                            Name = reader["Name"].ToString(),
                            Price = Convert.ToDecimal(reader["Price"]).ToString("0.00") + " lei",
                            Category = reader["CategoryName"].ToString()
                        });
                    }
                }
            }

            ProductsGrid.ItemsSource = products;
        }

        private void LoadUsers()
        {
            List<User> users = new List<User>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT UtilizatorID, Username, Passwd FROM Utilizator", conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = Convert.ToInt32(reader["UtilizatorID"]),
                            Name = reader["Username"].ToString(),
                            Password = reader["Passwd"].ToString()
                        });
                    }
                }
            }

            UsersGrid.ItemsSource = users;
        }

        private void LoadImporters()
        {
            List<Importer> importers = new List<Importer>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT i.ImportatorID, i.Company_name, c.Name AS CityName, co.Name AS CountryName " +
                "FROM Importator i " +
                "JOIN City c ON i.City = c.CityID " +
                "JOIN Country co ON c.Country = co.CountryID", conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        importers.Add(new Importer
                        {
                            Id = Convert.ToInt32(reader["ImportatorID"]),
                            Name = reader["Company_name"].ToString(),
                            City = reader["CityName"].ToString(),
                            Country = reader["CountryName"].ToString()
                        });
                    }
                }
            }

            ImportersGrid.ItemsSource = importers;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ProductsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProductsGrid.SelectedItem == null) return;
            ProductAdmin selected = (ProductAdmin)ProductsGrid.SelectedItem;
            this.Opacity = 0.78;
            ProductPopup popup = new ProductPopup(selected.Id);
            popup.Owner = this;
            popup.ShowDialog();
            this.Opacity = 1;
            ProductsGrid.SelectedItem = null;
        }

        private void UsersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UsersGrid.SelectedItem == null) return;
            User selected = (User)UsersGrid.SelectedItem;
            this.Opacity = 0.78;
            UserPopup popup = new UserPopup(selected.Id);
            popup.Owner = this;
            popup.ShowDialog();
            this.Opacity = 1;
            UsersGrid.SelectedItem = null;
        }

        private void ImportersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ImportersGrid.SelectedItem == null) return;
            Importer selected = (Importer)ImportersGrid.SelectedItem;
            this.Opacity = 0.78;
            ImportatorPopup popup = new ImportatorPopup(selected.Id);
            popup.Owner = this;
            popup.ShowDialog();
            this.Opacity = 1;
            ImportersGrid.SelectedItem = null;
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SearchToy_Click(object sender, RoutedEventArgs e)
        {
            this.Opacity = 0.78;
            SearchPopup popup = new SearchPopup();
            popup.Owner = this;
            popup.ShowDialog();
            this.Opacity = 1;
        }

        private void AddNewUser_Click(object sender, RoutedEventArgs e)
        {
            NewUserWindow newUser = new NewUserWindow();
            newUser.Show();
            this.Hide();
        }

        private void AddNewProduct_Click(object sender, RoutedEventArgs e)
        {
            NewProductWindow newProduct = new NewProductWindow();
            newProduct.Show();
            this.Hide();
        }

        private void ViewStats_Click(object sender, RoutedEventArgs e)
        {
            StatisticiWindow stats = new StatisticiWindow();
            stats.Show();
            this.Close();
        }

    }
}
