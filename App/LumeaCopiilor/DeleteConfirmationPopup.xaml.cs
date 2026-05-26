using System.Windows;

namespace LumeaCopiilor
{
    /// <summary>
    /// Interaction logic for DeleteConfirmationPopup.xaml
    /// </summary>
    public partial class DeleteConfirmationPopup : Window
    {
        /// <param name="message">Optional custom confirmation message shown in the popup.</param>
        public DeleteConfirmationPopup(string message = null)
        {
            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(message))
                MessageText.Text = message;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
