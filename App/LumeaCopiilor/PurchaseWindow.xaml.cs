using System;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace LumeaCopiilor
{
    public partial class PurchaseWindow : Window
    {
        private readonly int _productId;
        private readonly decimal _unitPrice;
        private readonly int _userId;

        private const string ConnectionString =
            "Server=.\\SQLEXPRESS;Database=Lumea_Copiilor;Integrated Security=True;TrustServerCertificate=True;";
               
        private class ShopItem
        {
            public int ShopId { get; set; }
            public string DisplayText { get; set; } = string.Empty;
            public override string ToString() => DisplayText;
        }

        private class PaymentItem
        {
            public int PaymentId { get; set; }
            public string TypeName { get; set; } = string.Empty;
            public override string ToString() => TypeName;
        }

        public PurchaseWindow(int productId, decimal unitPrice, int userId)
        {
            InitializeComponent();
            _productId = productId;
            _unitPrice = unitPrice;
            _userId    = userId;

            LoadProductName();
            LoadShops();
            LoadPaymentTypes();
            UpdateTotal(1);
        }

        private void LoadProductName()
        {
            const string query = "SELECT Name FROM Product WHERE ProductID = @id";
            try
            {
                using SqlConnection conn = new(ConnectionString);
                using SqlCommand cmd  = new(query, conn);
                cmd.Parameters.AddWithValue("@id", _productId);
                conn.Open();
                NumeProdusTextBox.Text = cmd.ExecuteScalar()?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea produsului:\n{ex.Message}",
                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadShops()
        {
            const string query = @"
                SELECT s.ShopID,
                       s.Street_address + ', ' + c.Name AS DisplayText
                FROM   Shop s
                LEFT JOIN City c ON s.City = c.CityID
                ORDER BY c.Name, s.Street_address";
            try
            {
                using SqlConnection conn = new(ConnectionString);
                using SqlCommand cmd  = new(query, conn);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    MagazinComboBox.Items.Add(new ShopItem
                    {
                        ShopId      = Convert.ToInt32(reader["ShopID"]),
                        DisplayText = reader["DisplayText"]?.ToString() ?? "(necunoscut)"
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea magazinelor:\n{ex.Message}",
                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPaymentTypes()
        {
            const string query = "SELECT PaymentID, Type FROM Payment ORDER BY PaymentID";
            try
            {
                using SqlConnection conn = new(ConnectionString);
                using SqlCommand cmd  = new(query, conn);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    TipPlataComboBox.Items.Add(new PaymentItem
                    {
                        PaymentId = Convert.ToInt32(reader["PaymentID"]),
                        TypeName  = reader["Type"]?.ToString() ?? string.Empty
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea tipurilor de plată:\n{ex.Message}",
                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTotal(int qty)
        {
            decimal total = _unitPrice * Math.Max(qty, 1);
            SumaTotalTextBlock.Text = $"Suma totală:  {total:0.00} lei";
        }

        private void OnCantitateTextChanged(object sender,
            System.Windows.Controls.TextChangedEventArgs e)
        {
            if (int.TryParse(CantitateTextBox.Text, out int qty) && qty > 0)
                UpdateTotal(qty);
            else
                UpdateTotal(1);
        }

        private void OnTipPlataSelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TipPlataComboBox.SelectedItem is not PaymentItem selected) return;

            // Show card number field only for "Card bancar" (PaymentID = 2)
            bool isCard = selected.PaymentId == 2;
            NumarCardContainer.Visibility = isCard ? Visibility.Visible : Visibility.Collapsed;
            NumarCardTextBox.IsEnabled    = isCard;

            if (!isCard)
                NumarCardTextBox.Clear();
        }

        private void OnAnulareClick(object sender, RoutedEventArgs e)
        {
            this.Owner?.Focus();
            this.Close();
        }

        private void OnComandaClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NumeProdusTextBox.Text))
            {
                MessageBox.Show("Selectați un produs!", "Eroare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MagazinComboBox.SelectedItem is not ShopItem)
            {
                MessageBox.Show("Selectați magazinul ridicării comenzii!", "Eroare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(CantitateTextBox.Text, out int cantitate) || cantitate <= 0)
            {
                MessageBox.Show("Cantitatea trebuie să fie un număr întreg pozitiv!", "Eroare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (TipPlataComboBox.SelectedItem is not PaymentItem selectedPayment)
            {
                MessageBox.Show("Selectați tipul de plată!", "Eroare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedPayment.PaymentId == 2)
            {
                if (string.IsNullOrWhiteSpace(NumarCardTextBox.Text))
                {
                    MessageBox.Show("Introduceți numărul cardului!", "Eroare",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!ValidareNumarCard(NumarCardTextBox.Text))
                {
                    MessageBox.Show("Numărul cardului nu este valid! (13–19 cifre)", "Eroare",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (SalveazaComanda(cantitate, selectedPayment.PaymentId))
            {
                var owner = this.Owner;
                this.Owner = null;
                owner?.Close();

                new ThankYouPage().Show();
                this.Close();
            }
        }

        private static bool ValidareNumarCard(string numarCard)
        {
            string cleaned = numarCard.Replace(" ", "").Replace("-", "");
            return cleaned.Length >= 13 && cleaned.Length <= 19
                   && long.TryParse(cleaned, out _);
        }

        private bool SalveazaComanda(int cantitate, int paymentTypeId)
        {
            var shopItem = (ShopItem)MagazinComboBox.SelectedItem;
            string? cardNumber = paymentTypeId == 2
                ? NumarCardTextBox.Text.Replace(" ", "").Replace("-", "")
                : null;

            try
            {
                using SqlConnection conn = new(ConnectionString);
                conn.Open();

                // Check available stock
                int currentQty;
                using (SqlCommand checkCmd = new(
                    "SELECT Quantity FROM Product WHERE ProductID = @id", conn))
                {
                    checkCmd.Parameters.AddWithValue("@id", _productId);
                    object? result = checkCmd.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                    {
                        MessageBox.Show("Produsul nu a fost găsit în baza de date!",
                            "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    currentQty = Convert.ToInt32(result);
                }

                if (cantitate > currentQty)
                {
                    MessageBox.Show(
                        $"Cantitate insuficientă!\nStoc disponibil: {currentQty} buc.",
                        "Stoc insuficient", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                using SqlTransaction tx = conn.BeginTransaction();
                try
                {
                    const string insertPurchase = @"
                        INSERT INTO Purchase
                            (Client, Shop, Product, Payment_type, Purchase_date, Quantity, Card_number)
                        VALUES
                            (@client, @shop, @product, @paymentType, GETDATE(), @quantity, @cardNumber)";

                    using (SqlCommand cmd = new(insertPurchase, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@client",      _userId);
                        cmd.Parameters.AddWithValue("@shop",        shopItem.ShopId);
                        cmd.Parameters.AddWithValue("@product",     _productId);
                        cmd.Parameters.AddWithValue("@paymentType", paymentTypeId);
                        cmd.Parameters.AddWithValue("@quantity",    cantitate);
                        cmd.Parameters.AddWithValue("@cardNumber",  (object?)cardNumber ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new(
                        "UPDATE Product SET Quantity = Quantity - @qty WHERE ProductID = @id",
                        conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@qty", cantitate);
                        cmd.Parameters.AddWithValue("@id",  _productId);
                        cmd.ExecuteNonQuery();
                    }

                    int newQty;
                    using (SqlCommand cmd = new(
                        "SELECT Quantity FROM Product WHERE ProductID = @id", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", _productId);
                        newQty = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (newQty == 0)
                    {
                        const string archive = @"
                            INSERT INTO OutOfStock
                                (Name, Min_age, Max_age, Fab_date, Exp_date, Price,
                                 Origin_country, Importator, Shop, Category, Archived_date)
                            SELECT Name, Min_age, Max_age, Fab_date, Exp_date, Price,
                                   Origin_country, Importator, Shop, Category, GETDATE()
                            FROM Product WHERE ProductID = @id";

                        using SqlCommand cmd = new(archive, conn, tx);
                        cmd.Parameters.AddWithValue("@id", _productId);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return true;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la salvarea comenzii:\n{ex.Message}",
                    "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
