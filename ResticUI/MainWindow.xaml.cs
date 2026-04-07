using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ResticUI
{
    public partial class MainWindow : Window
    {
        private AppSettings _settings;
        private ResticService _resticService;

        public MainWindow()
        {
            InitializeComponent();
            _settings = AppSettings.Load();
            _resticService = new ResticService(_settings);
            UpdateUI();
        }

        private void UpdateUI()
        {
            bool hasSelection = SnapshotsListView.SelectedItem != null;
            ListFilesMenuItem.IsEnabled = hasSelection;
            ListFilesDetailedMenuItem.IsEnabled = hasSelection;
            RestoreSnapshotMenuItem.IsEnabled = hasSelection;
            RemoveSnapshotMenuItem.IsEnabled = hasSelection;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(_settings);
            settingsWindow.Owner = this;
            if (settingsWindow.ShowDialog() == true)
            {
                _resticService = new ResticService(_settings);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void ListSnapshots_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Listing snapshots...";
            LogTextBox.Clear();
            
            var (output, error) = await _resticService.RunResticCommandAsync("snapshots --json");
            
            if (!string.IsNullOrEmpty(error))
            {
                LogTextBox.AppendText($"ERROR:\n{error}\n");
            }

            if (!string.IsNullOrEmpty(output))
            {
                try
                {
                    var snapshots = JsonConvert.DeserializeObject<List<Snapshot>>(output);
                    SnapshotsListView.ItemsSource = snapshots;
                    StatusTextBlock.Text = $"Found {snapshots?.Count ?? 0} snapshots.";
                }
                catch (Exception ex)
                {
                    LogTextBox.AppendText($"Failed to parse snapshots JSON: {ex.Message}\nRaw output:\n{output}\n");
                    StatusTextBlock.Text = "Error parsing snapshots.";
                }
            }
            else
            {
                StatusTextBlock.Text = "No snapshots found or error occurred.";
            }
        }

        private async void ListFiles_Click(object sender, RoutedEventArgs e)
        {
            await ListFilesAsync(false);
        }

        private async void ListFilesDetailed_Click(object sender, RoutedEventArgs e)
        {
            await ListFilesAsync(true);
        }

        private async Task ListFilesAsync(bool detailed)
        {
            if (SnapshotsListView.SelectedItem is Snapshot selectedSnapshot)
            {
                StatusTextBlock.Text = $"Listing files for snapshot {selectedSnapshot.id} {(detailed ? "(detailed)" : "")}...";
                LogTextBox.Clear();

                string args = detailed ? $"ls -l {selectedSnapshot.id}" : $"ls {selectedSnapshot.id}";
                var (output, error) = await _resticService.RunResticCommandAsync(args);

                if (!string.IsNullOrEmpty(error))
                {
                    LogTextBox.AppendText($"ERROR:\n{error}\n");
                }

                LogTextBox.AppendText(output);
                StatusTextBlock.Text = "File listing complete.";
            }
        }

        private async void Backup_Click(object sender, RoutedEventArgs e)
        {
            var backupWindow = new BackupWindow(_settings);
            backupWindow.Owner = this;
            if (backupWindow.ShowDialog() == true)
            {
                var paths = backupWindow.SelectedPaths;
                if (paths.Count > 0)
                {
                    StatusTextBlock.Text = "Running backup...";
                    LogTextBox.Clear();
                    LogTextBox.AppendText($"Starting backup of {paths.Count} items...\n");

                    string quotedPaths = string.Join(" ", paths.Select(p => $"\"{p}\""));
                    var (output, error) = await _resticService.RunResticCommandAsync($"backup {quotedPaths}");

                    if (!string.IsNullOrEmpty(error))
                    {
                        LogTextBox.AppendText($"ERROR:\n{error}\n");
                    }

                    LogTextBox.AppendText(output);
                    StatusTextBlock.Text = "Backup complete.";
                }
            }
        }

        private async void RestoreSnapshot_Click(object sender, RoutedEventArgs e)
        {
            if (SnapshotsListView.SelectedItem is Snapshot selectedSnapshot)
            {
                var dialog = new Microsoft.Win32.OpenFolderDialog
                {
                    Title = $"Select Target Folder to Restore Snapshot {selectedSnapshot.id}",
                };

                if (dialog.ShowDialog() == true)
                {
                    string targetPath = dialog.FolderName;
                    StatusTextBlock.Text = $"Restoring snapshot {selectedSnapshot.id} to {targetPath}...";
                    LogTextBox.Clear();
                    LogTextBox.AppendText($"Starting restore to: {targetPath}\n");

                    var (output, error) = await _resticService.RunResticCommandAsync($"restore {selectedSnapshot.id} --target \"{targetPath}\"");

                    if (!string.IsNullOrEmpty(error))
                    {
                        LogTextBox.AppendText($"ERROR:\n{error}\n");
                    }

                    LogTextBox.AppendText(output);
                    StatusTextBlock.Text = "Restore process finished.";
                }
            }
        }

        private async void RemoveSnapshot_Click(object sender, RoutedEventArgs e)
        {
            if (SnapshotsListView.SelectedItem is Snapshot selectedSnapshot)
            {
                var confirmWindow = new ConfirmationWindow(selectedSnapshot.id, selectedSnapshot.ShortId);
                confirmWindow.Owner = this;
                
                if (confirmWindow.ShowDialog() == true)
                {
                    StatusTextBlock.Text = $"Removing snapshot {selectedSnapshot.id} and pruning...";
                    LogTextBox.Clear();
                    LogTextBox.AppendText($"Starting removal of snapshot: {selectedSnapshot.id}\n");
                    LogTextBox.AppendText("This will also prune the repository to release space. This may take a while...\n");

                    var (output, error) = await _resticService.RunResticCommandAsync($"forget {selectedSnapshot.id} --prune");

                    if (!string.IsNullOrEmpty(error))
                    {
                        LogTextBox.AppendText($"ERROR:\n{error}\n");
                    }

                    LogTextBox.AppendText(output);
                    StatusTextBlock.Text = "Removal and pruning complete.";
                    
                    // Refresh the list
                    ListSnapshots_Click(sender, e);
                }
            }
        }

        private async void RunBackupScript_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Running backup script...";
            LogTextBox.Clear();
            
            var (output, error) = await _resticService.RunBackupScriptAsync();
            
            if (!string.IsNullOrEmpty(error))
            {
                LogTextBox.AppendText($"ERROR:\n{error}\n");
            }
            
            LogTextBox.AppendText(output);
            StatusTextBlock.Text = "Backup script finished.";
        }

        private void SnapshotsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUI();
        }
    }

    public class Snapshot
    {
        public string id { get; set; } = string.Empty;
        public DateTime time { get; set; }
        public List<string> paths { get; set; } = new List<string>();
        public string hostname { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public List<string> tags { get; set; } = new List<string>();

        public string ShortId => id?.Length >= 8 ? id.Substring(0, 8) : id ?? string.Empty;
        public string PathsDisplay => string.Join(", ", paths);
    }
}
