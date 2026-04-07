using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ResticUI
{
    public class ResticService
    {
        private readonly AppSettings _settings;

        public ResticService(AppSettings settings)
        {
            _settings = settings;
        }

        public async Task<(string output, string error)> RunResticCommandAsync(string arguments)
        {
            return await RunProcessAsync("restic.exe", arguments);
        }

        public async Task<(string output, string error)> RunBackupScriptAsync()
        {
            if (string.IsNullOrEmpty(_settings.BackupScriptPath))
            {
                return (string.Empty, "Backup script path is not configured.");
            }

            string fileName = _settings.BackupScriptPath;
            string arguments = string.Empty;

            if (_settings.BackupScriptPath.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
            {
                fileName = "powershell.exe";
                arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{_settings.BackupScriptPath}\"";
            }

            return await RunProcessAsync(fileName, arguments);
        }

        private async Task<(string output, string error)> RunProcessAsync(string fileName, string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Set environment variables
            if (!string.IsNullOrEmpty(_settings.ResticRepository))
                startInfo.EnvironmentVariables["RESTIC_REPOSITORY"] = _settings.ResticRepository;
            if (!string.IsNullOrEmpty(_settings.ResticPasswordFile))
                startInfo.EnvironmentVariables["RESTIC_PASSWORD_FILE"] = _settings.ResticPasswordFile;

            try
            {
                using var process = new Process { StartInfo = startInfo };
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                return (output, error);
            }
            catch (Exception ex)
            {
                return (string.Empty, $"Failed to start process: {ex.Message}");
            }
        }
    }
}
