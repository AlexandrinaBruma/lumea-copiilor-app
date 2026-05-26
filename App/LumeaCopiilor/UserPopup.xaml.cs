using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Data.SqlClient;

namespace LumeaCopiilor
{
    public class CityItem
    {
        public int CityID { get; set; }
        public string DisplayName { get; set; }

        public override string ToString() => DisplayName;
    }

    public partial class UserPopup : Window
    {
        private const string ConnectionString =
            "Server=.\\SQLEXPRESS;Database=Lumea_Copiilor;Integrated Security=True;TrustServerCertificate=True;";

        private readonly int _userId;
        private bool _isEditMode = false;
        private bool _comboBoxesLoaded = false;

        private string _rawGender;
        private string _rawRole;
        private int _rawCityId;

        public UserPopup(int userId)
        {
            InitializeComponent();
            _userId = userId;
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
                    u.Role,
                    u.City            AS CityID,
                    c.Name            AS CityName,
                    co.Name           AS CountryName
                FROM Utilizator u
                JOIN City    c  ON u.City    = c.CityID
                JOIN Country co ON c.Country = co.CountryID
                WHERE u.UtilizatorID = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", _userId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        UserIdTitle.Text = $"Utilizator #{_userId}";

                        // ── Display TextBlocks ──
                        UsernameValue.Text = reader["Username"].ToString();
                        ParolaValue.Text = reader["Passwd"].ToString();
                        NumeValue.Text = reader["Name"].ToString();
                        PrenumeValue.Text = reader["Surname"].ToString();

                        BirthdateValue.Text = reader["Birthdate"] == DBNull.Value
                            ? "Nedefinit"
                            : Convert.ToDateTime(reader["Birthdate"]).ToString("dd.MM.yyyy");

                        EmailValue.Text = reader["Email"] == DBNull.Value
                            ? "Nedefinit" : reader["Email"].ToString();

                        PhoneValue.Text = reader["Phone_number"] == DBNull.Value
                            ? "Nedefinit" : reader["Phone_number"].ToString();

                        _rawGender = reader["Gender"] == DBNull.Value
                            ? null : reader["Gender"].ToString();
                        GenderValue.Text = _rawGender == null ? "Nedefinit"
                                         : _rawGender == "M" ? "Masculin" : "Feminin";

                        RegistrationDateValue.Text = Convert.ToDateTime(reader["Registration_date"])
                            .ToString("dd.MM.yyyy HH:mm");

                        CityValue.Text = $"{reader["CityName"]}, {reader["CountryName"]}";
                        _rawCityId = Convert.ToInt32(reader["CityID"]);

                        _rawRole = reader["Role"] == DBNull.Value
                            ? null : reader["Role"].ToString();
                        RoleValue.Text = _rawRole == null ? "Nedefinit"
                                       : _rawRole == "A" ? "Administrator" : "Utilizator";

                        // ── Seed input controls ──
                        UsernameInput.Text = reader["Username"].ToString();
                        ParolaInput.Text = reader["Passwd"].ToString();
                        NumeInput.Text = reader["Name"].ToString();
                        PrenumeInput.Text = reader["Surname"].ToString();

                        EmailInput.Text = reader["Email"] == DBNull.Value
                            ? "" : reader["Email"].ToString();
                        PhoneInput.Text = reader["Phone_number"] == DBNull.Value
                            ? "" : reader["Phone_number"].ToString();

                        BirthdateInput.Value = reader["Birthdate"] == DBNull.Value
                            ? (DateTime?)null
                            : Convert.ToDateTime(reader["Birthdate"]);

                        CityInput.Tag = _rawCityId;
                    }
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
                            CityID = Convert.ToInt32(r["CityID"]),
                            DisplayName = $"{r["CityName"]}, {r["CountryName"]}"
                        });
            }

            CityInput.ItemsSource = cities;
            CityInput.SelectedValue = CityInput.Tag; 

            foreach (ComboBoxItem item in GenderInput.Items)
                if (item.Tag?.ToString() == _rawGender)
                { GenderInput.SelectedItem = item; break; }

            foreach (ComboBoxItem item in RoleInput.Items)
                if (item.Tag?.ToString() == _rawRole)
                { RoleInput.SelectedItem = item; break; }
        }

        private void SetEditMode(bool editMode)
        {
            _isEditMode = editMode;

            var labelColor = editMode
                ? (SolidColorBrush)FindResource("Paragraph")
                : (SolidColorBrush)FindResource("DarkColor");

            foreach (var lbl in new[] { LblUsername, LblParola, LblNume, LblPrenume,
                                        LblBirthdate, LblEmail, LblPhone,
                                        LblGender, LblRegDate, LblCity, LblRole })
                lbl.Foreground = labelColor;

            var textVis = editMode ? Visibility.Collapsed : Visibility.Visible;
            var inputVis = editMode ? Visibility.Visible : Visibility.Collapsed;

            // TextBlocks
            UsernameValue.Visibility = textVis;
            ParolaValue.Visibility = textVis;
            NumeValue.Visibility = textVis;
            PrenumeValue.Visibility = textVis;
            BirthdateValue.Visibility = textVis;
            EmailValue.Visibility = textVis;
            PhoneValue.Visibility = textVis;
            GenderValue.Visibility = textVis;
            RegistrationDateValue.Visibility = textVis;  
            CityValue.Visibility = textVis;
            RoleValue.Visibility = textVis;

            // Inputs
            UsernameInput.Visibility = inputVis;
            ParolaInput.Visibility = inputVis;
            NumeInput.Visibility = inputVis;
            PrenumeInput.Visibility = inputVis;
            BirthdateBorder.Visibility = inputVis;
            EmailInput.Visibility = inputVis;
            PhoneInput.Visibility = inputVis;
            GenderInput.Visibility = inputVis;
            CityInput.Visibility = inputVis;
            RoleInput.Visibility = inputVis;

            EditButton.Content = editMode ? "Anulare editare" : "Editare";
            DeleteCancelButton.Content = editMode ? "Anulare" : "Ștergere";
            BackSaveButton.Content = editMode ? "Salvare" : "Înapoi";

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

            if (CityInput.SelectedItem == null)
            {
                MessageBox.Show("Selectează un oraș.", "Validare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var city = (CityItem)CityInput.SelectedItem;
            var gender = (GenderInput.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            var role = (RoleInput.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE Utilizator SET
                    Username    = @Username,
                    Passwd      = @Passwd,
                    Name        = @Name,
                    Surname     = @Surname,
                    Birthdate   = @Birthdate,
                    Email       = @Email,
                    Phone_number = @Phone,
                    Gender      = @Gender,
                    City        = @CityID,
                    Role        = @Role
                WHERE UtilizatorID = @UserID", conn))
            {
                cmd.Parameters.AddWithValue("@Username", UsernameInput.Text.Trim());
                cmd.Parameters.AddWithValue("@Passwd", ParolaInput.Text.Trim());
                cmd.Parameters.AddWithValue("@Name", NumeInput.Text.Trim());
                cmd.Parameters.AddWithValue("@Surname", PrenumeInput.Text.Trim());
                cmd.Parameters.AddWithValue("@Birthdate",
                    (object?)BirthdateInput.Value ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email",
                    string.IsNullOrWhiteSpace(EmailInput.Text)
                        ? DBNull.Value : (object)EmailInput.Text.Trim());
                cmd.Parameters.AddWithValue("@Phone",
                    string.IsNullOrWhiteSpace(PhoneInput.Text)
                        ? DBNull.Value : (object)PhoneInput.Text.Trim());
                cmd.Parameters.AddWithValue("@Gender",
                    gender == null ? DBNull.Value : (object)gender);
                cmd.Parameters.AddWithValue("@CityID", city.CityID);
                cmd.Parameters.AddWithValue("@Role",
                    role == null ? DBNull.Value : (object)role);
                cmd.Parameters.AddWithValue("@UserID", _userId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            _comboBoxesLoaded = false;
            LoadUser();
            SetEditMode(false);
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

        private void DeleteCancelButton_Click(object sender, RoutedEventArgs e)
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
                var dlg = new DeleteConfirmationPopup($"Ești sigur că vrei să ștergi Utilizator #{_userId}?") { Owner = this };
                bool confirmed = dlg.ShowDialog() == true;
                this.Opacity = 1;

                if (confirmed)
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM Utilizator WHERE UtilizatorID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _userId);
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
                SaveUser();
            else
                this.Close();
        }
    }
}