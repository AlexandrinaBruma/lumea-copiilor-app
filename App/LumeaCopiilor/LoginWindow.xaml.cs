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
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// 


    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private const string ConnectionString = "Server=.\\SQLEXPRESS;Database=Lumea_Copiilor;Integrated Security=True;TrustServerCertificate=True;";
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String username;
            String password;

            username = UsernameInput.Text.Trim();
            password = PasswordInput.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Introduceți username-ul și parola.", "Atenție",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string role = null;
                int userId = 0;

                const string query = "SELECT UtilizatorID, Role FROM Utilizator WHERE Username = @username AND Passwd = @password";

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@username", SqlDbType.NVarChar).Value = username;
                        command.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userId = Convert.ToInt32(reader["UtilizatorID"]);
                                role = reader["Role"].ToString();
                            }
                        }
                    }
                }

                if (role == null)
                {
                    MessageBox.Show("Username sau parolă incorectă.", "Autentificare eșuată",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (role == "U")
                {
                    DashboardUser dashboard = new DashboardUser(userId);
                    dashboard.Show();
                    this.Close();
                }
                else if (role == "A")
                {
                    DashboardAdmin dashboard = new DashboardAdmin();
                    dashboard.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Rol necunoscut: {role}", "Eroare",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"A apărut o eroare: {ex.Message}", "Eroare",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AddUserAccount newUser = new AddUserAccount();
            newUser.Show();
            this.Close();
        }
    }
}
