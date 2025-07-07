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
                
                var markdown = await File.ReadAllTextAsync(fullPath);

                var bodyHtml = Markdig.Markdown.ToHtml(markdown);

                var html = $@"
                    <!DOCTYPE html>
<html>
   <head>
      <meta charset=""UTF-8"">
      <style>
         body {{
         font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen,
         Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif;
         padding: 20px;
         color: white;
         background-color: #161616;
         }}
         .container {{
         max-width: 800px;
         margin-top: 5px;
        margin-left: auto;
        margin-right: auto;
        margin-bottom: 40px;       
         padding: 20px;   
         }}
         hr {{
         border: none;
         height: 1px;
         background-color: #555;
         margin: 20px 0;
         }}
      </style>
   </head>
   <body>
      <div class=""container"">
      <h1>{FilesListBox.SelectedItem.ToString()}</h1>
         {bodyHtml}
      </div>
   </body>
</html>
                    ";

                MarkdownWebView.HtmlContent = html;
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