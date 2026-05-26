using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Data.SqlClient;

namespace LumeaCopiilor
{
    /// <summary>
    /// Interaction logic for SearchPopup.xaml
    /// </summary>
    public partial class SearchPopup : Window
    {
        private const string ConnectionString =
            "Server=.\\SQLEXPRESS;Database=Lumea_Copiilor;Integrated Security=True;TrustServerCertificate=True;";

        public SearchPopup()
        {
            InitializeComponent();
        }


        private void Inapoi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Cauta_Click(object sender, RoutedEventArgs e)
        {
            RunSearch();
        }

        // Allow pressing Enter in the search box to trigger search
        private void SearchInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                RunSearch();
        }


        private void RunSearch()
        {
            string term = SearchInput.Text.Trim();

            // Clear previous results
            ResultsPanel.Children.Clear();

            if (string.IsNullOrWhiteSpace(term))
            {
                ResultsScrollViewer.Visibility = Visibility.Collapsed;
                return;
            }

            // Query DB for matching products
            var results = new List<(int Id, string Name, decimal Price)>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT ProductID, Name, Price FROM Product " +
                    "WHERE Name LIKE @search " +
                    "ORDER BY Name", conn))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + term + "%");
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add((
                                Convert.ToInt32(reader["ProductID"]),
                                reader["Name"].ToString(),
                                Convert.ToDecimal(reader["Price"])
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la căutare: {ex.Message}", "Eroare",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (results.Count == 0)
            {
                ResultsPanel.Children.Add(BuildNoResultLabel());
            }
            else
            {
                for (int i = 0; i < results.Count; i++)
                {
                    var (id, name, price) = results[i];
                    ResultsPanel.Children.Add(BuildResultItem(id, name, price));

                    if (i < results.Count - 1)
                        ResultsPanel.Children.Add(BuildSeparator());
                }
            }

            ResultsScrollViewer.Visibility = Visibility.Visible;
        }

        private TextBlock BuildNoResultLabel()
        {
            return new TextBlock
            {
                Text = "Nu au fost găsite produse.",
                Style = (Style)FindResource("P"),
                Padding = new Thickness(8, 10, 8, 10)
            };
        }

        private Border BuildResultItem(int productId, string name, decimal price)
        {
            var label = new TextBlock
            {
                Text = $"{name}  —  {price:0.00} lei",
                Style = (Style)FindResource("P"),
                TextWrapping = TextWrapping.Wrap
            };

            var container = new Border
            {
                Padding = new Thickness(8, 10, 8, 10),
                CornerRadius = new CornerRadius(6),
                Cursor = Cursors.Hand,
                Background = Brushes.Transparent,
                Child = label
            };

            container.MouseEnter += (s, _) =>
                ((Border)s).Background = (SolidColorBrush)FindResource("Card");
            container.MouseLeave += (s, _) =>
                ((Border)s).Background = Brushes.Transparent;

            container.MouseLeftButtonUp += (s, _) => OpenProductPopup(productId);

            return container;
        }

        private Border BuildSeparator()
        {
            return new Border
            {
                Height = 1,
                Margin = new Thickness(8, 0, 8, 0),
                Background = (SolidColorBrush)FindResource("Placeholder"),
                Opacity = 0.35
            };
        }


        private void OpenProductPopup(int productId)
        {
            this.Opacity = 0.78;

            ProductPopup popup = new ProductPopup(productId);
            popup.Owner = this;
            popup.ShowDialog();

            this.Opacity = 1;
        }
    }
}
