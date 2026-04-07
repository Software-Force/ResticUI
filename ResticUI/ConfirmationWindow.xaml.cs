using System.Windows;

namespace ResticUI
{
    public partial class ConfirmationWindow : Window
    {
        private readonly string _expectedFullId;
        private readonly string _expectedShortId;

        public ConfirmationWindow(string fullId, string shortId)
        {
            InitializeComponent();
            _expectedFullId = fullId;
            _expectedShortId = shortId;
            InstructionsTextBlock.Text = $"To confirm deletion of snapshot {shortId}, please re-enter the ID (short or full) below:";
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string input = InputTextBox.Text.Trim();
            if (input == _expectedFullId || input == _expectedShortId)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show($"Incorrect ID. Please enter '{_expectedShortId}' or the full ID exactly.", "Verification Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
