// Overstrike -- an open-source mod manager for PC ports of Insomniac Games' games.
// This program is free software, and can be redistributed and/or modified by you. It is provided 'as-is', without any warranty.
// For more details, terms and conditions, see GNU General Public License.
// A copy of the that license should come with this program (LICENSE.txt). If not, see <http://www.gnu.org/licenses/>.

using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System;
using System.Windows;

namespace ModdingTool;

public partial class ReplaceByNameWindow : Window {
    private readonly List<Structs.Asset> _assets;
    private readonly Dictionary<string, List<int>> _assetsByPath;
    private readonly Action<Structs.Asset, string> _replaceAssetCallback;
    public ObservableCollection<ReplaceByNameResult> Results { get; set; } = new();

    public ReplaceByNameWindow(List<Structs.Asset> assets, Dictionary<string, List<int>> assetsByPath, Action<Structs.Asset, string> replaceAssetCallback) {
        InitializeComponent();
        _assets = assets;
        _assetsByPath = assetsByPath;
        _replaceAssetCallback = replaceAssetCallback;
        SearchResults.ItemsSource = Results;
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e) {
        var dialog = new CommonOpenFileDialog {
            Title = "Select a folder...",
            Multiselect = false,
            RestoreDirectory = true,
            IsFolderPicker = true
        };

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
            FolderPathTextBox.Text = dialog.FileName;
            SearchAndPopulate(dialog.FileName);
        }
    }

    private void SearchAndPopulate(string folderPath) {
        Results.Clear();

        var files = Directory.GetFiles(folderPath);

        foreach (var file in files) {
            var fileName = Path.GetFileName(file);

            // Find all assets in the TOC with this file name
            var matchingAssets = _assets
    .Where(a => !string.IsNullOrEmpty(a.Name) && a.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase))
    .ToList();

            Results.Add(new ReplaceByNameResult {
                FileName = fileName,
                OriginalFilePath = file,
                MatchingFiles = matchingAssets,
                SelectedFile = matchingAssets.FirstOrDefault()
            });
        }

        ResultsCount.Text = $"{Results.Count} results:";
    }

    private void ReplaceButton_Click(object sender, RoutedEventArgs e) {
        int replacedCount = 0;
        foreach (var result in Results) {
            if (result.SelectedFile != null && !string.IsNullOrEmpty(result.OriginalFilePath)) {
                _replaceAssetCallback(result.SelectedFile, result.OriginalFilePath);
                replacedCount++;
            }
        }

        MessageBox.Show($"{replacedCount} assets replaced.", "Replace Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        Close();
    }

    public ReplaceByNameWindow(List<Structs.Asset> assets, Dictionary<string, List<int>> assetsByPath) {
        InitializeComponent();
        _assets = assets;
        _assetsByPath = assetsByPath;
        SearchResults.ItemsSource = Results;
    }
}

public class ReplaceByNameResult {
    public string FileName { get; set; } // The file name from the selected folder
    public string OriginalFilePath { get; set; } // The full path to the file on disk
    public List<Structs.Asset> MatchingFiles { get; set; } // Assets from TOC that match this file name
    public Structs.Asset SelectedFile { get; set; } // The selected asset to replace
    public string Span => SelectedFile?.Span.ToString() ?? "";
    public string Archive => SelectedFile?.Archive ?? "";
}
