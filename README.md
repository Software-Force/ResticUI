# ResticUI

ResticUI is a Windows-based graphical user interface for [restic](https://restic.net/), a fast, secure, and efficient backup program. It simplifies restic operations like snapshots management, backups, and restores for Windows 10 and 11 users.

## Features

- **Snapshot Management**: List all snapshots with short IDs, view detailed file listings, and remove snapshots with integrated space pruning (`--prune`).
- **Flexible Backup**: 
  - Select specific files and folders for manual backup.
  - Run custom backup scripts (supports `.bat`, `.cmd`, and `.ps1`).
  - Generate and save backup commands as Windows batch scripts for later use.
- **Easy Restore**: Browse snapshots and restore them to a selected target folder using a native folder picker.
- **Secure Configuration**: Set and persist `RESTIC_REPOSITORY` and `RESTIC_PASSWORD_FILE` environment variables.
- **Integrated Logging**: Real-time output and error logging for all restic operations.

## Configuration

Before running restic commands, configure your repository settings in **File > Settings**:

1.  **Restic Repository**: The path or URL to your restic repository.
2.  **Password File**: The local path to the file containing your repository password.
3.  **Backup Script**: (Optional) Path to a script file you wish to run for automated backups.

These settings are stored locally in `settings.json` and are passed as environment variables to the restic process.

## Usage

### Listing Snapshots
Go to **Restic > List Snapshots**. Select a snapshot to enable file listing, restoration, and removal options.

### Backing Up
1.  **Manual**: Go to **Restic > Backup...**, add your files/folders, and click **Run Backup**.
2.  **Script**: Configure a script in Settings and use **Restic > Run Backup Script**.

### Restoring
Select a snapshot from the list and go to **Restic > Restore Snapshot...**. Choose a target folder to begin the process.

### Removing Snapshots
Select a snapshot and go to **Restic > Remove Snapshot...**. You will be prompted to re-enter the snapshot ID (short or full) to confirm the deletion and pruning.

## Build Requirements

- .NET 8.0 or 10.0 SDK
- Windows 10 or 11
- [restic.exe](https://restic.net/) (Must be in your system PATH)

To build from source:
```bash
dotnet build
```
