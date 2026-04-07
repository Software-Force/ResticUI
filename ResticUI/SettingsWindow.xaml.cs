using Microsoft.Win32;
using System.Windows;

namespace ResticUI
{
    public partial class SettingsWindow : Window
    {
        private readonly AppSettings _settings;

        public SettingsWindow(AppSettings settings)
        {
            InitializeComponent();
            _settings = settings;

            RepoTextBox.Text = _settings.ResticRepository;
            PasswordFileTextBox.Text = _settings.ResticPasswordFile;
            BackupScriptTextBox.Text = _settings.BackupScriptPath;
        }

        private void BrowseRepo_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select Restic Repository Folder",
                InitialDirectory = _settings.ResticRepository
            };

            if (dialog.ShowDialog() == true)
            {
                RepoTextBox.Text = dialog.FolderName;
            }
        }

        private void BrowsePasswordFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Restic Password File",
                Filter = "All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                PasswordFileTextBox.Text = dialog.FileName;
            }
        }

        private void BrowseBackupScript_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Backup Script File",
                Filter = "Script Files (*.bat;*.ps1;*.cmd)|*.bat;*.ps1;*.cmd|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                BackupScriptTextBox.Text = dialog.FileName;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _settings.ResticRepository = RepoTextBox.Text;
            _settings.ResticPasswordFile = PasswordFileTextBox.Text;
            _settings.BackupScriptPath = BackupScriptTextBox.Text;
            _settings.Save();
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
