using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static System.Net.WebRequestMethods;

namespace Bulk_Replacer
{
    public partial class BulkReplacer : UserControl
    {

        private CancellationTokenSource cancellationTokenSource;

        private int totalFilesProcessed = 0;
        private int totalFilesSkipped = 0;

        private string lastFind;
        private string lastReplace;
        private string[] lastFilesArray;
        private bool revertEnabled;
        public BulkReplacer()
        {
            InitializeComponent();
        }

        private void TargetFolder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog();
            folderDialog.IsFolderPicker = true;
            folderDialog.Title = "Select the target folder for the process";

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ExportConsole.Text = folderDialog.FileName;
            }
        }
     
        private void ProcessFiles_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int selectedType = Type.SelectedIndex;
            string findText = Find.Text;
            string replaceText = Replace.Text;
            string folderPath = ExportConsole.Text;
            bool searchSubfolders = SubfolderCheckbox.IsChecked.GetValueOrDefault();
            bool isSensitivityChecked = SensitivityCheckBox.IsChecked.GetValueOrDefault();
            string logFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Latest.log");


            var replacer = new Replacer()
                .Type((Replacer.ReplaceType)selectedType)
                .Find(findText)
                .Replace(replaceText)
                .In(folderPath)
                .Options(searchSubfolders, isSensitivityChecked);
            
            using (StreamWriter logWriter = new StreamWriter(logFilePath, false))
            {
                replacer.OnLog((log) =>
                {
                    logWriter.WriteLine(log);
                });
                replacer.Execute();
            }

            totalFilesProcessed = replacer.FilesProcessedCount;
            totalFilesSkipped = replacer.FilesSkippedCount;
            
            lastFilesArray = replacer.ProcessedFilesList.ToArray();
            lastReplace = Replace.Text;
            revertEnabled = true;
            
            UpdateConsole(1);
        }

        private void UpdateConsole(int message)
        {
            ResultConsole.Document.Blocks.Clear();

            if (message == 1)
            {
                Paragraph p = new Paragraph();
                p.TextAlignment = TextAlignment.Center;
                Run r = new Run($"▰▰▰▰▰▰▰▰▰▰ Process completed ▰▰▰▰▰▰▰▰▰▰");
                r.Foreground = new SolidColorBrush(Colors.Green);

                p.Inlines.Add(r);
                ResultConsole.Document.Blocks.Add(p);

                Paragraph pt = new Paragraph();
                pt.TextAlignment = TextAlignment.Center;
                if (Type.SelectedIndex == 0)
                {
                    Run rt = new Run($"Renamed " + totalFilesProcessed + " files in total");
                    rt.Foreground = new SolidColorBrush(Colors.White);
                    pt.Inlines.Add(rt);
                }
                else if (Type.SelectedIndex == 1)
                {
                    Run rt = new Run($"Changed extension for " + totalFilesProcessed + " files in total");
                    rt.Foreground = new SolidColorBrush(Colors.White);
                    pt.Inlines.Add(rt);
                }
                else if (Type.SelectedIndex == 2)
                {
                    Run rt = new Run($"Modified content for " + totalFilesProcessed + " files in total");
                    rt.Foreground = new SolidColorBrush(Colors.White);
                    pt.Inlines.Add(rt);
                }
                ResultConsole.Document.Blocks.Add(pt);

                Paragraph ps = new Paragraph();
                ps.TextAlignment = TextAlignment.Center;
                Run rs = new Run($"Open the latest log to view the details");
                rs.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x79, 0xD3, 0xF1));
                ps.Inlines.Add(rs);
                ResultConsole.Document.Blocks.Add(ps);
            }

        }

            private void AddErrorMessage(string errorMessage)
            {
            ResultConsole.Document.Blocks.Clear();

            Paragraph p = new Paragraph();
            Run r = new Run($"");

            p.Inlines.Add(r);
            ResultConsole.Document.Blocks.Add(p);

            Paragraph paragraph = new Paragraph
                {
                    TextAlignment = TextAlignment.Center
                };

                Run run = new Run(errorMessage)
                {
                    Foreground = new SolidColorBrush(Colors.IndianRed)
                };

                paragraph.Inlines.Add(run);
                ResultConsole.Document.Blocks.Add(paragraph);
            }

        

        private void OpenLastLogsBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string rootDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                string logFilePath = System.IO.Path.Combine(rootDirectory, "Latest.log");

                Process.Start("notepad.exe", logFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening log file: {ex.Message}");
            }
        }


        private void RevertLastChanges_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var selectedType = (Replacer.ReplaceType)Type.SelectedIndex;
            if (Replacer.ReplaceType.Content == selectedType)
            {
                AddErrorMessage("This feature is not supported for content.");
                return;
            }
            
            if (!revertEnabled)
            {
                AddErrorMessage("There is no file to revert.");
                return;
            }

            foreach (string file in lastFilesArray)
            {
                string fileName = System.IO.Path.GetFileName(file);
                string fileExtension = System.IO.Path.GetExtension(file);
                string newFileName;
                if (Replacer.ReplaceType.FileName == selectedType)
                {
                    string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
                    newFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), fileNameWithoutExtension.Replace(lastReplace, lastFind, StringComparison.OrdinalIgnoreCase) + fileExtension);
                    System.IO.File.Move(file, newFileName);
                }
                else
                {
                    string newFileExtension = fileExtension.Replace(lastReplace, lastFind);
                    newFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file) + newFileExtension);
                }
                System.IO.File.Move(file, newFileName);
            }
            revertEnabled = false;
        }
    }
}
