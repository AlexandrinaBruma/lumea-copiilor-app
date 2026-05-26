using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace LumeaCopiilor
{
    /// <summary>
    /// Interaction logic for UserAccountPage.xaml
    /// </summary>
    public partial class UserAccountPage : Window
    {
        private const string ConnectionString =
            "Server=.\\SQLEXPRESS;Database=Lumea_Copiilor;Integrated Security=True;TrustServerCertificate=True;";

        private readonly int _userId;
        private readonly DashboardUser _dashboardUser;

        private bool _isEditMode = false;
        private bool _comboBoxesLoaded = false;

        private string _rawGender;
        private int _rawCityId;

        public UserAccountPage(int userId, DashboardUser dashboardUser)
        {
            InitializeComponent();
            _userId = userId;
            _dashboardUser = dashboardUser;
            LoadUser();
        }

        
        private void LoadUser()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    u.Username,
                    u.Passwd,
                    u.Name,
                    u.Surname,
                    u.Birthdate,
                    u.Email,
                    u.Phone_number,
                    u.Gender,
                    u.Registration_date,
                    u.City          AS CityID,
                    c.Name          AS CityName,
                    co.Name         AS CountryName
                FROM Utilizator u
                JOIN City    c  ON u.City    = c.CityID
                JOIN Country co ON c.Country = co.CountryID
                WHERE u.UtilizatorID = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", _userId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return;

                    NumeValue.Text    = reader["Name"].ToString();
                    PrenumeValue.Text = reader["Surname"].ToString();

                    BirthdateValue.Text = reader["Birthdate"] == DBNull.Value
                        ? "Nedefinit"
                        : Convert.ToDateTime(reader["Birthdate"]).ToString("dd.MM.yyyy");

                    EmailValue.Text = reader["Email"] == DBNull.Value
                        ? "Nedefinit" : reader["Email"].ToString();

                    TelefonValue.Text = reader["Phone_number"] == DBNull.Value
                        ? "Nedefinit" : reader["Phone_number"].ToString();

                    _rawGender = reader["Gender"] == DBNull.Value
                        ? null : reader["Gender"].ToString();
                    GenderValue.Text = _rawGender == null   ? "Nedefinit"
                                     : _rawGender == "M"   ? "Masculin"
                                                           : "Feminin";

                    UsernameValue.Text = reader["Username"].ToString();

                    // Show password as dots for display; actual value goes to input
                    string rawPasswd = reader["Passwd"].ToString();
                    ParolaValue.Text = new string('•', rawPasswd.Length);

                    _rawCityId = Convert.ToInt32(reader["CityID"]);
                    CityValue.Text = $"{reader["CityName"]}, {reader["CountryName"]}";

                    RegistrationDateValue.Text = Convert.ToDateTime(reader["Registration_date"])
                        .ToString("dd.MM.yyyy HH:mm");

                    NumeInput.Text    = reader["Name"].ToString();
                    PrenumeInput.Text = reader["Surname"].ToString();

                    BirthdateInput.Value = reader["Birthdate"] == DBNull.Value
                        ? (DateTime?)null
                        : Convert.ToDateTime(reader["Birthdate"]);

                    EmailInput.Text = reader["Email"] == DBNull.Value
                        ? "" : reader["Email"].ToString();

                    TelefonInput.Text = reader["Phone_number"] == DBNull.Value
                        ? "" : reader["Phone_number"].ToString();

                    UsernameInput.Text = reader["Username"].ToString();
                    ParolaInput.Text   = rawPasswd;

                    CityInput.Tag = _rawCityId;
                }
            }
        }

        private void LoadComboBoxes()
        {
            if (_comboBoxesLoaded) return;
            _comboBoxesLoaded = true;

            var cities = new List<CityItem>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    SELECT c.CityID, c.Name AS CityName, co.Name AS CountryName
                    FROM City c
                    JOIN Country co ON c.Country = co.CountryID
                    ORDER BY co.Name, c.Name", conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        cities.Add(new CityItem
                        {
                            CityID      = Convert.ToInt32(r["CityID"]),
                            DisplayName = $"{r["CityName"]}, {r["CountryName"]}"
                        });
            }

            CityInput.ItemsSource  = cities;
            CityInput.SelectedValue = CityInput.Tag;  

            foreach (ComboBoxItem item in GenderInput.Items)
                if (item.Tag?.ToString() == _rawGender)
                { GenderInput.SelectedItem = item; break; }
        }

        private void SetEditMode(bool editMode)
        {
            _isEditMode = editMode;

            var textVis  = editMode ? Visibility.Collapsed : Visibility.Visible;
            var inputVis = editMode ? Visibility.Visible   : Visibility.Collapsed;

            NumeValue.Visibility             = textVis;
            PrenumeValue.Visibility          = textVis;
            BirthdateValue.Visibility        = textVis;
            EmailValue.Visibility            = textVis;
            TelefonValue.Visibility          = textVis;
            GenderValue.Visibility           = textVis;
            UsernameValue.Visibility         = textVis;
            ParolaValue.Visibility           = textVis;
            CityValue.Visibility             = textVis;

            NumeInput.Visibility             = inputVis;
            PrenumeInput.Visibility          = inputVis;
            BirthdateBorder.Visibility       = inputVis;
            EmailInput.Visibility            = inputVis;
            TelefonInput.Visibility          = inputVis;
            GenderInput.Visibility           = inputVis;
            UsernameInput.Visibility         = inputVis;
            ParolaInput.Visibility           = inputVis;
            CityInput.Visibility             = inputVis;

            EditButton.Content          = editMode ? "Anulare editare" : "Editare";
            StergereContButton.Content  = editMode ? "Anulare"         : "Stergere cont";
            InapoiButton.Content        = editMode ? "Salvare"         : "Inapoi";

            if (editMode)
                LoadComboBoxes();
        }

        private void SaveUser()
        {
            if (string.IsNullOrWhiteSpace(UsernameInput.Text))
            {
                MessageBox.Show("Username-ul nu poate fi gol.", "Validare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ParolaInput.Text))
            {
                MessageBox.Show("Parola nu poate fi goală.", "Validare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(NumeInput.Text))
            {
                MessageBox.Show("Numele nu poate fi gol.", "Validare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CityInput.SelectedItem == null)
            {
                MessageBox.Show("Selectează un oraș.", "Validare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var city   = (CityItem)CityInput.SelectedItem;
            var gender = (GenderInput.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE Utilizator SET
                        Username     = @Username,
                        Passwd       = @Passwd,
                        Name         = @Name,
                        Surname      = @Surname,
                        Birthdate    = @Birthdate,
                        Email        = @Email,
                        Phone_number = @Phone,
                        Gender       = @Gender,
                        City         = @CityID
                    WHERE UtilizatorID = @UserID", conn))
                {
                    cmd.Parameters.AddWithValue("@Username", UsernameInput.Text.Trim());
                    cmd.Parameters.AddWithValue("@Passwd",   ParolaInput.Text.Trim());
                    cmd.Parameters.AddWithValue("@Name",     NumeInput.Text.Trim());
                    cmd.Parameters.AddWithValue("@Surname",  PrenumeInput.Text.Trim());
                    cmd.Parameters.AddWithValue("@Birthdate",
                        (object?)BirthdateInput.Value ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email",
                        string.IsNullOrWhiteSpace(EmailInput.Text)
                            ? DBNull.Value : (object)EmailInput.Text.Trim());
                    cmd.Parameters.AddWithValue("@Phone",
                        string.IsNullOrWhiteSpace(TelefonInput.Text)
                            ? DBNull.Value : (object)TelefonInput.Text.Trim());
                    cmd.Parameters.AddWithValue("@Gender",
                        gender == null ? DBNull.Value : (object)gender);
                    cmd.Parameters.AddWithValue("@CityID",  city.CityID);
                    cmd.Parameters.AddWithValue("@UserID",  _userId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                _comboBoxesLoaded = false;
                LoadUser();
                SetEditMode(false);

                MessageBox.Show("Modificările au fost salvate cu succes!", "Succes",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"A apărut o eroare la salvare:\n{ex.Message}", "Eroare",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode)
            {
                _comboBoxesLoaded = false;
                LoadUser();
                SetEditMode(false);
            }
            else
            {
                SetEditMode(true);
            }
        }

        private void InapoiButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode)
            {
                SaveUser();
            }
            else
            {
                _dashboardUser.Show();
                this.Close();
            }
        }

        private void StergereContButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode)
            {
                _comboBoxesLoaded = false;
                LoadUser();
                SetEditMode(false);
            }
            else
            {
                this.Opacity = 0.78;
                var dlg = new DeleteConfirmationPopup("Sunteți sigur că doriți să ștergeți contul?\nAceastă acțiune este ireversibilă.") { Owner = this };
                bool confirmed = dlg.ShowDialog() == true;
                this.Opacity = 1;

                if (!confirmed) return;

                try
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM Utilizator WHERE UtilizatorID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _userId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Contul a fost șters cu succes.", "Cont șters",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    _dashboardUser.Close();
                    new MainWindow().Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"A apărut o eroare la ștergere:\n{ex.Message}", "Eroare",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
