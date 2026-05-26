using System;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace LumeaCopiilor
{
    /// <summary>
    /// Interaction logic for productUserPopup.xaml
    /// </summary>
    public partial class productUserPopup : Window
    {
        private readonly int _productId;
        private readonly int _userId;
        private decimal _price;
        private const string ConnectionString = "Server=.\\SQLEXPRESS;Database=Lumea_Copiilor;Integrated Security=True;TrustServerCertificate=True;";

        public productUserPopup(int productId, int userId)
        {
            InitializeComponent();
            _productId = productId;
            _userId = userId;
            LoadProduct();
        }

        private void LoadProduct()
        {
            const string query = @"
                SELECT
                    p.Name,
                    p.Min_age,
                    p.Max_age,
                    p.Fab_date,
                    p.Exp_date,
                    p.Price,
                    p.Quantity,
                    co.Name        AS CountryName,
                    i.Company_name AS ImporterName,
                    s.Street_address + ', ' + ci.Name AS ShopAddress
                FROM Product p
                LEFT JOIN Country    co ON p.Origin_country = co.CountryID
                LEFT JOIN Importator i  ON p.Importator     = i.ImportatorID
                LEFT JOIN Shop       s  ON p.Shop           = s.ShopID
                LEFT JOIN City       ci ON s.City           = ci.CityID
                WHERE p.ProductID = @id";

            try
            {
                using SqlConnection conn = new SqlConnection(ConnectionString);
                using SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", _productId);
                conn.Open();

                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    NameValue.Text = reader["Name"].ToString();

                    QuantityValue.Text = reader["Quantity"] == DBNull.Value
                        ? "-"
                        : reader["Quantity"].ToString();

                    MinAgeValue.Text = reader["Min_age"] == DBNull.Value
                        ? "-"
                        : reader["Min_age"] + " ani";

                    MaxAgeValue.Text = reader["Max_age"] == DBNull.Value
                        ? "-"
                        : reader["Max_age"] + " ani";

                    ManufactureDateValue.Text = reader["Fab_date"] == DBNull.Value
                        ? "-"
                        : Convert.ToDateTime(reader["Fab_date"]).ToString("dd.MM.yyyy");

                    ExpiryDateValue.Text = reader["Exp_date"] == DBNull.Value
                        ? "-"
                        : Convert.ToDateTime(reader["Exp_date"]).ToString("dd.MM.yyyy");

                    if (reader["Price"] != DBNull.Value)
                    {
                        _price = Convert.ToDecimal(reader["Price"]);
                        PriceValue.Text = _price.ToString("0.00") + " lei";
                    }
                    else
                    {
                        PriceValue.Text = "-";
                    }

                    CountryValue.Text = reader["CountryName"] == DBNull.Value
                        ? "-"
                        : reader["CountryName"].ToString();

                    ImporterValue.Text = reader["ImporterName"] == DBNull.Value
                        ? "-"
                        : reader["ImporterName"].ToString();

                    StoresValue.Text = reader["ShopAddress"] == DBNull.Value
                        ? "-"
                        : reader["ShopAddress"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea produsului:\n{ex.Message}",
                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var purchase = new PurchaseWindow(_productId, _price, _userId) { Owner = this.Owner };
            purchase.Show();
            this.Close();
        }
    }
}
