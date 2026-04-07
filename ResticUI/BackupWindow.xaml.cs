using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace ResticUI
{
    public partial class BackupWindow : Window
    {
        private readonly AppSettings _settings;
        public ObservableCollection<string> SelectedPaths { get; } = new ObservableCollection<string>();

        public BackupWindow(AppSettings settings)
        {
            InitializeComponent();
            _settings = settings;
            PathsListBox.ItemsSource = SelectedPaths;
        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select Folder to Backup"
            };

            if (dialog.ShowDialog() == true)
            {
                if (!SelectedPaths.Contains(dialog.FolderName))
                {
                    SelectedPaths.Add(dialog.FolderName);
                }
            }
        }

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select File to Backup",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var file in dialog.FileNames)
                {
                    if (!SelectedPaths.Contains(file))
                    {
                        SelectedPaths.Add(file);
                    }
                }
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = PathsListBox.SelectedItems.Cast<string>().ToList();
            foreach (var item in selectedItems)
            {
                SelectedPaths.Remove(item);
            }
        }

        private void RunBackup_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPaths.Count == 0)
            {
                MessageBox.Show("Please select at least one file or folder to backup.", "No Paths Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void SaveScript_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPaths.Count == 0)
            {
                MessageBox.Show("Please select at least one file or folder to backup.", "No Paths Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Title = "Save Backup Script",
                Filter = "Batch Files (*.bat)|*.bat|Command Files (*.cmd)|*.cmd",
                FileName = "restic_backup.bat"
            };

            if (dialog.ShowDialog() == true)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("@echo off");
                sb.AppendLine($"SET RESTIC_REPOSITORY={_settings.ResticRepository}");
                sb.AppendLine($"SET RESTIC_PASSWORD_FILE={_settings.ResticPasswordFile}");
                
                string paths = string.Join(" ", SelectedPaths.Select(p => $"\"{p}\""));
                sb.AppendLine($"restic backup {paths}");
                sb.AppendLine("pause");

                File.WriteAllText(dialog.FileName, sb.ToString());
                MessageBox.Show($"Script saved to: {dialog.FileName}", "Script Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
