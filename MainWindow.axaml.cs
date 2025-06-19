using Avalonia.Controls;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace MDNotepad;

public partial class MainWindow : Window
{
    private ObservableCollection<string> markdownFiles = new();

    private string currentFolder = "";
    public MainWindow()
    {
        InitializeComponent();

        FilesListBox.ItemsSource = markdownFiles;

        SelectFolderButton.Click += SelectFolderButton_Click;

        FilesListBox.SelectionChanged += FilesListBox_SelectionChanged;
    }

    private async void FilesListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (FilesListBox.SelectedItem is string fileName && !string.IsNullOrEmpty(currentFolder))
        {
            var fullPath = Path.Combine(currentFolder, fileName);

            if (File.Exists(fullPath))
            {
                var content = await File.ReadAllTextAsync(fullPath);

                MarkDownTextBlock.Text = content;
            }
        }
    }

    private async void SelectFolderButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog();

        var folder = await dialog.ShowAsync(this);

        if (!string.IsNullOrEmpty(folder))
        {
            currentFolder = folder;
            LoadMarkdowFiles(folder);
        }
    }

    private void LoadMarkdowFiles(string folderPath)
    {
        markdownFiles.Clear();

        var files = Directory.GetFiles(folderPath, "*.md");

        foreach (var file in files)
        {
            // #Adding only the path (do load the file into the poor memory)
            markdownFiles.Add(Path.GetFileName(file));
        }
    }
}