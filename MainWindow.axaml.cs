using Avalonia.Controls;
using System;
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

    private string HtmlTemplatePath = Path.Combine(AppContext.BaseDirectory, "Assets", "template.html");
    private string EditorJSPath = Path.Combine(AppContext.BaseDirectory, "Assets", "editor.js");
    private async void FilesListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (FilesListBox.SelectedItem is string fileName && !string.IsNullOrEmpty(currentFolder))
        {
            var fullPath = Path.Combine(currentFolder, fileName);

            if (File.Exists(fullPath))
            {
                var markdown = await File.ReadAllTextAsync(fullPath);
                string markdownHtml = Markdig.Markdown.ToHtml(markdown);
                string title = Path.GetFileNameWithoutExtension(fullPath);

                var javaScriptEditor = await File.ReadAllTextAsync(EditorJSPath);
                var htmlTemplate = await File.ReadAllTextAsync(HtmlTemplatePath);

                string filledHtml = htmlTemplate
                    .Replace("{{title}}", title)
                    .Replace("{{body}}", markdownHtml)
                    .Replace("{{editor}}", javaScriptEditor);

                MarkdownWebView.HtmlContent = filledHtml;
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
            // #Adding only the path (do not load the file into the poor memory)
            markdownFiles.Add(Path.GetFileName(file));
        }
    }
}