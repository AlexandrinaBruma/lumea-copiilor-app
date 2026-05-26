using Microsoft.Data.SqlClient;
using System;
using System.Windows;

namespace LumeaCopiilor
{
    public partial class AddUserAccount : Window
    {
        public AddUserAccount()
        {
            InitializeComponent();
        }

        private const string ConnectionString = "Server=.\\SQLEXPRESS;Database=Lumea_Copiilor;Integrated Security=True;TrustServerCertificate=True;";

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string username, password, name, surname, email, phoneNumber, city, gender, genderCode;
            string role = "U";
            DateTime birthdate, regDate;
            int cityId;

            username = UsernameTextBox.Text.Trim();
            password = PasswordTextBox.Text.Trim();
            name = NumeTextBox.Text.Trim();
            surname = PrenumeTextBox.Text.Trim();
            email = EmailTextBox.Text.Trim();
            phoneNumber = TelefonTextBox.Text.Trim();
            city = (OrasulComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            gender = (GenulComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";
            regDate = DateTime.Now;


            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Introduceți username-ul.", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Introduceți parola.", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Introduceți numele.", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(surname))
            {
                MessageBox.Show("Introduceți prenumele.", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                MessageBox.Show("Introduceți o adresă de email validă.", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(phoneNumber))
            {
                MessageBox.Show("Introduceți numărul de telefon.", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DataNasterii.Value == null)
            {
                MessageBox.Show("Introduceți data nașterii.", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            birthdate = DataNasterii.Value.Value;

            if (birthdate >= DateTime.Today)
            {
                MessageBox.Show("Data nașterii trebuie să fie în trecut.", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(gender) || gender == "Selectați genul")
            {
                MessageBox.Show("Selectați genul.", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(city) || city == "Selectați orașul")
            {
                MessageBox.Show("Selectați orașul.", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (gender == "Masculin")
                genderCode = "M";
            else if (gender == "Feminin")
                genderCode = "F";
            else
            {
                MessageBox.Show("Selectați un gen valid (Masculin sau Feminin).", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    const string cityQuery = "SELECT CityID FROM City WHERE Name = @cityName";
                    using (SqlCommand cityCommand = new SqlCommand(cityQuery, connection))
                    {
                        cityCommand.Parameters.AddWithValue("@cityName", city);
                        object result = cityCommand.ExecuteScalar();

                        if (result == null)
                        {
                            MessageBox.Show($"Orașul '{city}' nu a fost găsit în baza de date.", "Eroare",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        cityId = Convert.ToInt32(result);
                    }

                    const string insertQuery = @"
                        INSERT INTO Utilizator 
                            (Username, Passwd, Name, Surname, Email, Phone_number, Gender, Birthdate, Registration_date, City, Role) 
                        VALUES 
                            (@username, @password, @name, @surname, @email, @phoneNumber, @gender, @birthdate, @regDate, @cityId, @role)";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@surname", surname);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                        command.Parameters.AddWithValue("@gender", genderCode);
                        command.Parameters.AddWithValue("@birthdate", birthdate);
                        command.Parameters.AddWithValue("@regDate", regDate);
                        command.Parameters.AddWithValue("@cityId", cityId);
                        command.Parameters.AddWithValue("@role", role);

                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Contul a fost creat cu succes!", "Succes",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"A apărut o eroare: {ex.Message}", "Eroare",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Anulare_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
