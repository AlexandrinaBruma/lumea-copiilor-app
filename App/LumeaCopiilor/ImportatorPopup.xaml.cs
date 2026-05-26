using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Data.SqlClient;

namespace LumeaCopiilor
{
    public partial class ImportatorPopup : Window
    {
        private const string ConnectionString =
            "Server=.\\SQLEXPRESS;Database=Lumea_Copiilor;Integrated Security=True;TrustServerCertificate=True;";

        private readonly int _importerId;
        private bool _isEditMode = false;
        private bool _comboBoxesLoaded = false;

        public ImportatorPopup(int importatorId)
        {
            InitializeComponent();
            _importerId = importatorId;
            LoadImportator();
        }

        // ──────────────────────────────────────────────
        // Încarcă & afișează importatorul (mod read-only)
        // ──────────────────────────────────────────────
        private void LoadImportator()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    i.Company_name,
                    i.Contact_person,
                    i.Phone_number,
                    i.Email,
                    i.Website,
                    i.Street_address,
                    i.City            AS CityID,
                    i.Fiscal_code,
                    i.Contract_start_date,
                    i.Contract_end_date,
                    c.Name            AS CityName,
                    co.Name           AS CountryName
                FROM Importator i
                JOIN City    c  ON i.City    = c.CityID
                JOIN Country co ON c.Country = co.CountryID
                WHERE i.ImportatorID = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", _importerId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ImportatorIdTitle.Text = $"Importator #{_importerId}";

                        // ── Valori afișate (TextBlock) ──
                        NameValue.Text = reader["Company_name"].ToString();
                        ContactPersonValue.Text = reader["Contact_person"].ToString();
                        PhoneValue.Text = reader["Phone_number"].ToString();
                        EmailValue.Text = reader["Email"].ToString();
                        WebsiteValue.Text = reader["Website"] == DBNull.Value
                                                    ? "Nu are website"
                                                    : reader["Website"].ToString();
                        CityValue.Text = $"{reader["CityName"]}, {reader["CountryName"]}";
                        AddressValue.Text = reader["Street_address"].ToString();
                        FiscalCodeValue.Text = reader["Fiscal_code"].ToString();
                        StartDateValue.Text = reader["Contract_start_date"] == DBNull.Value
                                                    ? "Nu avem contract"
                                                    : Convert.ToDateTime(reader["Contract_start_date"]).ToString("dd.MM.yyyy");
                        EndDateValue.Text = reader["Contract_end_date"] == DBNull.Value
                                                    ? "-"
                                                    : Convert.ToDateTime(reader["Contract_end_date"]).ToString("dd.MM.yyyy");

                        // ── Pregătire câmpuri de editare ──
                        NameInput.Text = reader["Company_name"].ToString();
                        ContactInput.Text = reader["Contact_person"].ToString();
                        PhoneInput.Text = reader["Phone_number"].ToString();
                        EmailInput.Text = reader["Email"].ToString();
                        WebsiteInput.Text = reader["Website"] == DBNull.Value
                                                ? string.Empty
                                                : reader["Website"].ToString();
                        AddressInput.Text = reader["Street_address"].ToString();
                        FiscalInput.Text = reader["Fiscal_code"].ToString();

                        StartDateInput.Value = reader["Contract_start_date"] == DBNull.Value
                                                ? (DateTime?)null
                                                : Convert.ToDateTime(reader["Contract_start_date"]);
                        EndDateInput.Value = reader["Contract_end_date"] == DBNull.Value
                                                ? (DateTime?)null
                                                : Convert.ToDateTime(reader["Contract_end_date"]);

                        // Reținem CityID ca Tag pentru pre-selectare în ComboBox
                        CityInput.Tag = Convert.ToInt32(reader["CityID"]);
                    }
                }
            }
        }

        // ──────────────────────────────────────────────
        // Populează ComboBox-ul de orașe (leneș, o singură dată per sesiune)
        // ──────────────────────────────────────────────
        private void LoadComboBoxes()
        {
            if (_comboBoxesLoaded) return;
            _comboBoxesLoaded = true;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                var cities = new List<CityItem>();
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

                CityInput.ItemsSource = cities;
                CityInput.SelectedValue = CityInput.Tag; // pre-selectează orașul curent
            }
        }

        // ──────────────────────────────────────────────
        // Comutare între modul vizualizare și editare
        // ──────────────────────────────────────────────
        private void SetEditMode(bool editMode)
        {
            _isEditMode = editMode;

            // Culoare etichete: mai estompată în editare (la fel ca ProductPopup)
            var labelColor = editMode
                ? (SolidColorBrush)FindResource("Paragraph")
                : (SolidColorBrush)FindResource("DarkColor");

            foreach (var lbl in new[] { LblName, LblContact, LblPhone, LblEmail,
                                        LblWebsite, LblCity, LblAddress,
                                        LblFiscal, LblStartDate, LblEndDate })
                lbl.Foreground = labelColor;

            var textVis = editMode ? Visibility.Collapsed : Visibility.Visible;
            var inputVis = editMode ? Visibility.Visible : Visibility.Collapsed;

            // TextBlock-uri (mod vizualizare)
            NameValue.Visibility = textVis;
            ContactPersonValue.Visibility = textVis;
            PhoneValue.Visibility = textVis;
            EmailValue.Visibility = textVis;
            WebsiteValue.Visibility = textVis;
            CityValue.Visibility = textVis;
            AddressValue.Visibility = textVis;
            FiscalCodeValue.Visibility = textVis;
            StartDateValue.Visibility = textVis;
            EndDateValue.Visibility = textVis;

            // Input-uri (mod editare)
            NameInput.Visibility = inputVis;
            ContactInput.Visibility = inputVis;
            PhoneInput.Visibility = inputVis;
            EmailInput.Visibility = inputVis;
            WebsiteInput.Visibility = inputVis;
            CityInput.Visibility = inputVis;
            AddressInput.Visibility = inputVis;
            FiscalInput.Visibility = inputVis;
            StartDateBorder.Visibility = inputVis;
            EndDateBorder.Visibility = inputVis;

            // Butoane
            EditButton.Content = editMode ? "Anulare editare" : "Editare";
            DeleteCancelButton.Content = editMode ? "Anulare" : "Ștergere";
            BackSaveButton.Content = editMode ? "Salvare" : "Înapoi";

            if (editMode)
                LoadComboBoxes();
        }

        // ──────────────────────────────────────────────
        // Salvează modificările în baza de date
        // ──────────────────────────────────────────────
        private void SaveImportator()
        {
            // Validări de bază
            if (string.IsNullOrWhiteSpace(NameInput.Text))
            {
                MessageBox.Show("Numele companiei nu poate fi gol.", "Validare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ContactInput.Text))
            {
                MessageBox.Show("Persoana de contact nu poate fi goală.", "Validare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(AddressInput.Text))
            {
                MessageBox.Show("Adresa nu poate fi goală.", "Validare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(FiscalInput.Text) || FiscalInput.Text.Trim().Length < 6)
            {
                MessageBox.Show("Codul fiscal trebuie să aibă cel puțin 6 caractere.", "Validare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CityInput.SelectedItem == null)
            {
                MessageBox.Show("Selectează un oraș.", "Validare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (StartDateInput.Value == null)
            {
                MessageBox.Show("Data de start a contractului este obligatorie.", "Validare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (EndDateInput.Value.HasValue && EndDateInput.Value.Value <= StartDateInput.Value.Value)
            {
                MessageBox.Show("Data de încheiere a contractului trebuie să fie după data de start.", "Validare",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var city = (CityItem)CityInput.SelectedItem;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE Importator SET
                    Company_name        = @CompanyName,
                    Contact_person      = @ContactPerson,
                    Phone_number        = @Phone,
                    Email               = @Email,
                    Website             = @Website,
                    Street_address      = @Address,
                    City                = @CityID,
                    Fiscal_code         = @FiscalCode,
                    Contract_start_date = @StartDate,
                    Contract_end_date   = @EndDate
                WHERE ImportatorID = @ImportatorID", conn))
            {
                cmd.Parameters.AddWithValue("@CompanyName", NameInput.Text.Trim());
                cmd.Parameters.AddWithValue("@ContactPerson", ContactInput.Text.Trim());
                cmd.Parameters.AddWithValue("@Phone", string.IsNullOrWhiteSpace(PhoneInput.Text)
                                                                ? (object)DBNull.Value
                                                                : PhoneInput.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(EmailInput.Text)
                                                                ? (object)DBNull.Value
                                                                : EmailInput.Text.Trim());
                cmd.Parameters.AddWithValue("@Website", string.IsNullOrWhiteSpace(WebsiteInput.Text)
                                                                ? (object)DBNull.Value
                                                                : WebsiteInput.Text.Trim());
                cmd.Parameters.AddWithValue("@Address", AddressInput.Text.Trim());
                cmd.Parameters.AddWithValue("@CityID", city.CityID);
                cmd.Parameters.AddWithValue("@FiscalCode", FiscalInput.Text.Trim());
                cmd.Parameters.AddWithValue("@StartDate", StartDateInput.Value.Value);
                cmd.Parameters.AddWithValue("@EndDate", EndDateInput.Value.HasValue
                                                                ? (object)EndDateInput.Value.Value
                                                                : DBNull.Value);
                cmd.Parameters.AddWithValue("@ImportatorID", _importerId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // Reîncarcă datele și revine la mod vizualizare
            _comboBoxesLoaded = false;
            LoadImportator();
            SetEditMode(false);
        }

        // ──────────────────────────────────────────────
        // Handler-e butoane
        // ──────────────────────────────────────────────

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode)
            {
                // "Anulare editare" — reîncarcă valorile originale și iese din edit
                _comboBoxesLoaded = false;
                LoadImportator();
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
                LoadImportator();
                SetEditMode(false);
            }
            else
            {
                this.Opacity = 0.78;
                var dlg = new DeleteConfirmationPopup($"Ești sigur că vrei să ștergi Importator #{_importerId}?") { Owner = this };
                bool confirmed = dlg.ShowDialog() == true;
                this.Opacity = 1;

                if (confirmed)
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    using (SqlCommand cmd = new SqlCommand(
                        "DELETE FROM Importator WHERE ImportatorID = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _importerId);
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
                SaveImportator();
            else
                this.Close();
        }
    }
}