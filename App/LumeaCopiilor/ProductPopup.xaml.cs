using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Data.SqlClient;

namespace LumeaCopiilor
{
    public class CountryItem
    {
        public int CountryID { get; set; }
        public string Name { get; set; }
    }

    public class ImporterItem
    {
        public int ImportatorID { get; set; }
        public string CompanyName { get; set; }
    }

    public class ShopItem
    {
        public int ShopID { get; set; }
        public string DisplayName { get; set; }
    }

    public partial class ProductPopup : Window
    {
        private const string ConnectionString =
            "Server=.\\SQLEXPRESS;Database=Lumea_Copiilor;Integrated Security=True;TrustServerCertificate=True;";

        private readonly int _productId;
        private bool _isEditMode = false;
        private bool _comboBoxesLoaded = false;

        public ProductPopup(int productId)
        {
            InitializeComponent();
            _productId = productId;
            LoadProduct();
        }

        // Load & display product
        private void LoadProduct()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    p.Name,
                    p.Min_age,
                    p.Max_age,
                    p.Fab_date,
                    p.Exp_date,
                    p.Price,
                    p.Quantity,
                    p.Origin_country,
                    p.Importator      AS ImportatorID,
                    p.Shop            AS ShopID,
                    co.Name           AS CountryName,
                    i.Company_name    AS ImporterName,
                    s.Street_address  AS ShopAddress,
                    c.Name            AS ShopCity
                FROM Product p
                JOIN Country    co ON p.Origin_country = co.CountryID
                JOIN Importator i  ON p.Importator     = i.ImportatorID
                JOIN Shop       s  ON p.Shop            = s.ShopID
                JOIN City       c  ON s.City            = c.CityID
                WHERE p.ProductID = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", _productId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ProductIdTitle.Text = $"Produs #{_productId}";

                        // ── TextBlock display values ──
                        NameValue.Text = reader["Name"].ToString();
                        MinAgeValue.Text = $"{Convert.ToInt32(reader["Min_age"])} ani";
                        MaxAgeValue.Text = reader["Max_age"] == DBNull.Value
                            ? "Fără limită"
                            : $"{Convert.ToInt32(reader["Max_age"])} ani";
                        ManufactureDateValue.Text = Convert.ToDateTime(reader["Fab_date"]).ToString("dd.MM.yyyy");
                        ExpiryDateValue.Text = reader["Exp_date"] == DBNull.Value
                            ? "Fără dată de expirare"
                            : Convert.ToDateTime(reader["Exp_date"]).ToString("dd.MM.yyyy");
                        PriceValue.Text = Convert.ToDecimal(reader["Price"]).ToString("0.00") + " lei";
                        CountryValue.Text = reader["CountryName"].ToString();
                        ImporterValue.Text = reader["ImporterName"].ToString();
                        StoresValue.Text = $"{reader["ShopAddress"]}, {reader["ShopCity"]}";
                        QuantityValue.Text = reader["Quantity"].ToString();

                        // ── Seed input controls ──
                        NameInput.Text = reader["Name"].ToString();
                        MinAgeInput.Value = Convert.ToInt32(reader["Min_age"]);
                        MaxAgeInput.Value = reader["Max_age"] == DBNull.Value
                            ? (int?)null : Convert.ToInt32(reader["Max_age"]);
                        FabDateInput.Value = Convert.ToDateTime(reader["Fab_date"]);
                        ExpDateInput.Value = reader["Exp_date"] == DBNull.Value
                            ? (DateTime?)null : Convert.ToDateTime(reader["Exp_date"]);
                        PriceInput.Value = Convert.ToDecimal(reader["Price"]);
                        QuantityInput.Value = Convert.ToInt32(reader["Quantity"]);

                        // Store FK IDs as tags for ComboBox pre-selection
                        CountryInput.Tag = Convert.ToInt32(reader["Origin_country"]);
                        ImporterInput.Tag = Convert.ToInt32(reader["ImportatorID"]);
                        ShopInput.Tag = Convert.ToInt32(reader["ShopID"]);
                    }
                }
            }
        }

        // Populate FK ComboBoxes 
        private void LoadComboBoxes()
        {
            if (_comboBoxesLoaded) return;
            _comboBoxesLoaded = true;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                // Countries
                var countries = new List<CountryItem>();
                using (var cmd = new SqlCommand("SELECT CountryID, Name FROM Country ORDER BY Name", conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        countries.Add(new CountryItem
                        {
                            CountryID = Convert.ToInt32(r["CountryID"]),
                            Name = r["Name"].ToString()
                        });

                CountryInput.ItemsSource = countries;
                CountryInput.SelectedValue = CountryInput.Tag;

                // Importers
                var importers = new List<ImporterItem>();
                using (var cmd = new SqlCommand("SELECT ImportatorID, Company_name FROM Importator ORDER BY Company_name", conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        importers.Add(new ImporterItem
                        {
                            ImportatorID = Convert.ToInt32(r["ImportatorID"]),
                            CompanyName = r["Company_name"].ToString()
                        });

                ImporterInput.ItemsSource = importers;
                ImporterInput.SelectedValue = ImporterInput.Tag;

                // Shops
                var shops = new List<ShopItem>();
                using (var cmd = new SqlCommand(@"
                    SELECT s.ShopID, s.Street_address, ci.Name AS CityName
                    FROM Shop s
                    JOIN City ci ON s.City = ci.CityID
                    ORDER BY ci.Name, s.Street_address", conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        shops.Add(new ShopItem
                        {
                            ShopID = Convert.ToInt32(r["ShopID"]),
                            DisplayName = $"{r["Street_address"]}, {r["CityName"]}"
                        });

                ShopInput.ItemsSource = shops;
                ShopInput.SelectedValue = ShopInput.Tag;
            }
        }

        private void SetEditMode(bool editMode)
        {
            _isEditMode = editMode;

            var labelColor = editMode
                ? (SolidColorBrush)FindResource("Paragraph")
                : (SolidColorBrush)FindResource("DarkColor");

            foreach (var lbl in new[] { LblName, LblMinAge, LblMaxAge, LblFabDate,
                                        LblExpDate, LblPrice, LblCountry,
                                        LblImporter, LblShop, LblQuantity })
                lbl.Foreground = labelColor;

            var textVis = editMode ? Visibility.Collapsed : Visibility.Visible;
            var inputVis = editMode ? Visibility.Visible : Visibility.Collapsed;

            NameValue.Visibility = textVis;
            MinAgeValue.Visibility = textVis;
            MaxAgeValue.Visibility = textVis;
            ManufactureDateValue.Visibility = textVis;
            ExpiryDateValue.Visibility = textVis;
            PriceValue.Visibility = textVis;
            CountryValue.Visibility = textVis;
            ImporterValue.Visibility = textVis;
            StoresValue.Visibility = textVis;
            QuantityValue.Visibility = textVis;

            NameInput.Visibility = inputVis;
            MinAgeInput.Visibility = inputVis;
            MaxAgeInput.Visibility = inputVis;
            FabDateBorder.Visibility = inputVis;
            ExpDateBorder.Visibility = inputVis;
            PriceInput.Visibility = inputVis;
            CountryInput.Visibility = inputVis;
            ImporterInput.Visibility = inputVis;
            ShopInput.Visibility = inputVis;
            QuantityInput.Visibility = inputVis;

            EditButton.Content = editMode ? "Anulare editare" : "Editare";
            DeleteCancelButton.Content = editMode ? "Anulare" : "Ștergere";
            BackSaveButton.Content = editMode ? "Salvare" : "Înapoi";

            if (editMode)
                LoadComboBoxes();
        }
        
        // Save changes to DB
        private void SaveProduct()
        {
            if (string.IsNullOrWhiteSpace(NameInput.Text))
            {
                MessageBox.Show("Numele produsului nu poate fi gol.", "Validare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CountryInput.SelectedItem == null ||
                ImporterInput.SelectedItem == null ||
                ShopInput.SelectedItem == null)
            {
                MessageBox.Show("Selectează țara, importatorul și magazinul.", "Validare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var country = (CountryItem)CountryInput.SelectedItem;
            var importer = (ImporterItem)ImporterInput.SelectedItem;
            var shop = (ShopItem)ShopInput.SelectedItem;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE Product SET
                    Name           = @Name,
                    Min_age        = @MinAge,
                    Max_age        = @MaxAge,
                    Fab_date       = @FabDate,
                    Exp_date       = @ExpDate,
                    Price          = @Price,
                    Quantity       = @Quantity,
                    Origin_country = @CountryID,
                    Importator     = @ImportatorID,
                    Shop           = @ShopID
                WHERE ProductID = @ProductID", conn))
            {
                cmd.Parameters.AddWithValue("@Name", NameInput.Text.Trim());
                cmd.Parameters.AddWithValue("@MinAge", MinAgeInput.Value ?? 0);
                cmd.Parameters.AddWithValue("@MaxAge", (object?)MaxAgeInput.Value ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FabDate", FabDateInput.Value ?? DateTime.Today);
                cmd.Parameters.AddWithValue("@ExpDate", (object?)ExpDateInput.Value ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Price", PriceInput.Value ?? 0m);
                cmd.Parameters.AddWithValue("@Quantity", QuantityInput.Value ?? 0);
                cmd.Parameters.AddWithValue("@CountryID", country.CountryID);
                cmd.Parameters.AddWithValue("@ImportatorID", importer.ImportatorID);
                cmd.Parameters.AddWithValue("@ShopID", shop.ShopID);
                cmd.Parameters.AddWithValue("@ProductID", _productId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            _comboBoxesLoaded = false;
            LoadProduct();
            SetEditMode(false);
        }

        // Buttons
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode)
            {
                _comboBoxesLoaded = false;
                LoadProduct();
                SetEditMode(false);
            }
            else
            {
                SetEditMode(true);
            }
        }

        private void DeleteCancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode)
            {
                _comboBoxesLoaded = false;
                LoadProduct();
                SetEditMode(false);
            }
            else
            {
                this.Opacity = 0.78;
                var dlg = new DeleteConfirmationPopup($"Ești sigur că vrei să ștergi Produs #{_productId}?") { Owner = this };
                bool confirmed = dlg.ShowDialog() == true;
                this.Opacity = 1;

                if (confirmed)
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Product WHERE ProductID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _productId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    this.Close();
                }
            }
        }

        private void BackSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode)
                SaveProduct();
            else
                this.Close();
        }
    }
}