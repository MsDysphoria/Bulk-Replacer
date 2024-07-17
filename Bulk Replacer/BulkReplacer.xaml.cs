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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static System.Net.WebRequestMethods;

namespace Bulk_Replacer
{
    public partial class BulkReplacer : Window
    {
        private BitmapImage discordD;
        private BitmapImage discordH;
        private BitmapImage patreonD;
        private BitmapImage patreonH;
        private BitmapImage githubD;
        private BitmapImage githubH;
        private BitmapImage webD;
        private BitmapImage webH;
        private BitmapImage deviantArtD;
        private BitmapImage deviantArtH;
        private BitmapImage artstationD;
        private BitmapImage artstationH;

        private BitmapImage button2X;
        private BitmapImage button2D;
        private BitmapImage button2H;
        private BitmapImage button3D;
        private BitmapImage button3H;
        private BitmapImage button4D;
        private BitmapImage button4H;

        private CancellationTokenSource cancellationTokenSource;

        private int totalFilesProcessed = 0;
        private int totalFilesSkipped = 0;

        private int lastType;
        private string lastFind;
        private string lastReplace;
        private string[] lastFilesArray;
        private bool revertEnabled;
        public BulkReplacer()
        {
            InitializeComponent();

            button2X = new BitmapImage(new Uri("/Images/Button2X.png", UriKind.Relative));
            button2D = new BitmapImage(new Uri("/Images/Button2D.png", UriKind.Relative));
            button2H = new BitmapImage(new Uri("/Images/Button2H.png", UriKind.Relative));
            button3D = new BitmapImage(new Uri("/Images/Button3D.png", UriKind.Relative));
            button3H = new BitmapImage(new Uri("/Images/Button3H.png", UriKind.Relative));
            button4D = new BitmapImage(new Uri("/Images/Button4D.png", UriKind.Relative));
            button4H = new BitmapImage(new Uri("/Images/Button4H.png", UriKind.Relative));

            discordD = new BitmapImage(new Uri("/Images/Discord.png", UriKind.Relative));
            discordH = new BitmapImage(new Uri("/Images/Discord_H.png", UriKind.Relative));
            patreonD = new BitmapImage(new Uri("/Images/Patreon.png", UriKind.Relative));
            patreonH = new BitmapImage(new Uri("/Images/Patreon_H.png", UriKind.Relative));
            githubD = new BitmapImage(new Uri("/Images/Github.png", UriKind.Relative));
            githubH = new BitmapImage(new Uri("/Images/Github_H.png", UriKind.Relative));
            webD = new BitmapImage(new Uri("/Images/Website.png", UriKind.Relative));
            webH = new BitmapImage(new Uri("/Images/Website_H.png", UriKind.Relative));
            deviantArtD = new BitmapImage(new Uri("/Images/DeviantArt.png", UriKind.Relative));
            deviantArtH = new BitmapImage(new Uri("/Images/DeviantArt_H.png", UriKind.Relative));
            artstationD = new BitmapImage(new Uri("/Images/Artstation.png", UriKind.Relative));
            artstationH = new BitmapImage(new Uri("/Images/Artstation_H.png", UriKind.Relative));
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

        // Basic Buttons
        private void btnInfo_Click(object sender, RoutedEventArgs e)
        { ShowInformation(); }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        { System.Windows.Application.Current.MainWindow.WindowState = WindowState.Minimized; }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        { System.Windows.Application.Current.Shutdown(); }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        { if (e.LeftButton == MouseButtonState.Pressed) { DragMove(); } }

        // Button Highlight
        private void OpenLastLogsBtn_MouseEnter(object sender, MouseEventArgs e)
        { OpenLastLogsBtn.Source = button3H; }
        private void OpenLastLogsBtn_MouseLeave(object sender, MouseEventArgs e)
        { OpenLastLogsBtn.Source = button3D; }
        private void RevertLastChanges_MouseEnter(object sender, MouseEventArgs e)
        { RevertLastChanges.Source = button3H; }
        private void RevertLastChanges_MouseLeave(object sender, MouseEventArgs e)
        { RevertLastChanges.Source = button3D; }
        private void PickDirectory_MouseEnter(object sender, MouseEventArgs e)
        { PickDirectory.Source = button4H; }
        private void PickDirectory_MouseLeave(object sender, MouseEventArgs e)
        { PickDirectory.Source = button4D; }
        private void ReplaceBtn_MouseEnter(object sender, MouseEventArgs e)
        { RenameBtn.Source = button2H; }
        private void ReplaceBtn_MouseLeave(object sender, MouseEventArgs e)
        { RenameBtn.Source = button2D; }
        private void Return_MouseEnter(object sender, MouseEventArgs e)
        { Return.Source = button3H; }
        private void Return_MouseLeave(object sender, MouseEventArgs e)
        { Return.Source = button3D; }
        private void Discord_MouseEnter(object sender, MouseEventArgs e)
        { Discord.Source = discordH; }
        private void Discord_MouseLeave(object sender, MouseEventArgs e)
        { Discord.Source = discordD; }
        private void Patreon_MouseEnter(object sender, MouseEventArgs e)
        { Patreon.Source = patreonH; }
        private void Patreon_MouseLeave(object sender, MouseEventArgs e)
        { Patreon.Source = patreonD; }
        private void Github_MouseEnter(object sender, MouseEventArgs e)
        { Github.Source = githubH; }
        private void Github_MouseLeave(object sender, MouseEventArgs e)
        { Github.Source = githubD; }
        private void Website_MouseEnter(object sender, MouseEventArgs e)
        { Website.Source = webH; }
        private void Website_MouseLeave(object sender, MouseEventArgs e)
        { Website.Source = webD; }
        private void DeviantArt_MouseEnter(object sender, MouseEventArgs e)
        { DeviantArt.Source = deviantArtH; }
        private void DeviantArt_MouseLeave(object sender, MouseEventArgs e)
        { DeviantArt.Source = deviantArtD; }
        private void Artstation_MouseEnter(object sender, MouseEventArgs e)
        { Artstation.Source = artstationH; }
        private void Artstation_MouseLeave(object sender, MouseEventArgs e)
        { Artstation.Source = artstationD; }

        // Links
        private void Patreon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string patreonLink = "https://www.patreon.com/msdysphoria";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = patreonLink,
                UseShellExecute = true
            });
        }

        private void Website_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string patreonLink = "https://msdysphoria.shop/";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = patreonLink,
                UseShellExecute = true
            });
        }

        private void DeviantArt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string patreonLink = "https://www.deviantart.com/msdysphoria";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = patreonLink,
                UseShellExecute = true
            });
        }

        private void Artstation_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string patreonLink = "https://www.artstation.com/msdysphoria";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = patreonLink,
                UseShellExecute = true
            });
        }

        private void Discord_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string patreonLink = "https://discord.gg/uQDPFt6WKn";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = patreonLink,
                UseShellExecute = true
            });
        }

        private void Github_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string patreonLink = "https://github.com/MsDysphoria";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = patreonLink,
                UseShellExecute = true
            });
        }

        private void Return_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
            Storyboard fadeOutStoryboard = this.FindResource("FadeOut_Info") as Storyboard;
            fadeOutStoryboard?.Begin();
        }
        private void ShowInformation()
        {

            cancellationTokenSource = new CancellationTokenSource();
            _ = Typewriter(0, cancellationTokenSource.Token);

            Storyboard fadeInStoryboard = this.FindResource("FadeIn_Info") as Storyboard;
            fadeInStoryboard.Begin();
        }
        public async Task Typewriter(int message, CancellationToken cancellationToken)
        {
            Author.Text = "";
            string msg;
            if (message == 0) { msg = ""; }
            else if (message == 1) { msg = "Created by Ms. Dysphoria"; }
            else { msg = "Discord: msdysphoria"; }

            Random randomDelay = new Random();

            for (int i = 0; i < msg.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                Author.Text += msg[i].ToString();
                int delay = randomDelay.Next(35, 55);
                await Task.Delay(delay, cancellationToken);
            }

            if (message == 0)
            {
                await Task.Delay(2500, cancellationToken);
                await Typewriter(1, cancellationToken);
            }
            else if (message == 1)
            {
                Storyboard fadeInStoryboard = this.FindResource("GlowAuthor") as Storyboard;
                fadeInStoryboard.Begin();
                await Task.Delay(5000, cancellationToken);
                await Typewriter(2, cancellationToken);
            }
            else
            {

                await Task.Delay(5000, cancellationToken);
                await Typewriter(1, cancellationToken);
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

            if (findText == "Pick a keyword to find")
            {
                AddErrorMessage("Please specify the keyword to find.");
                return;
            }

            if (replaceText == "Pick a keyword to replace with")
            {
                AddErrorMessage("Please specify the keyword to replace with.");
                return;
            }

            if (folderPath == "Pick the target directory")
            {
                AddErrorMessage("Please select a directory.");
                return;
            }

            IEnumerable<string> files = searchSubfolders
                ? Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                : Directory.GetFiles(folderPath);

            totalFilesProcessed = 0;
            totalFilesSkipped = 0;
            List<string> lastFiles = new List<string>();

            using (StreamWriter logWriter = new StreamWriter(logFilePath, false))
            {
                logWriter.WriteLine($"Processing files in {folderPath} ({(searchSubfolders ? "including subfolders" : "without subfolders")})");
                logWriter.WriteLine($"Find text: {findText}");
                logWriter.WriteLine($"Replace text: {replaceText}");
                logWriter.WriteLine("---");

                foreach (string file in files)
                {
                    string fileName = System.IO.Path.GetFileName(file);
                    string fileExtension = System.IO.Path.GetExtension(file);
                    string fileContent = System.IO.File.ReadAllText(file);

                    if (selectedType == 0)
                    {
                        if (fileName.Contains(findText, isSensitivityChecked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                        {
                            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
                            string newFileName;
                            if (isSensitivityChecked)
                            {
                                newFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), fileNameWithoutExtension.Replace(findText, replaceText) + fileExtension);
                            }
                            else
                            {
                                newFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), fileNameWithoutExtension.Replace(findText, replaceText, StringComparison.OrdinalIgnoreCase) + fileExtension);
                            }
                            System.IO.File.Move(file, newFileName);
                            logWriter.WriteLine($"File renamed: {fileName} -> {System.IO.Path.GetFileName(newFileName)}");
                            totalFilesProcessed++;
                            lastFiles.Add(newFileName);
                        }
                        else
                        {
                            totalFilesSkipped++;
                        }
                    }
                    else if (selectedType == 1)
                    {
                        if (fileExtension.Contains(findText, isSensitivityChecked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                        {
                            string newFileExtension = fileExtension.Replace(findText, replaceText, isSensitivityChecked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);

                            if (string.IsNullOrWhiteSpace(newFileExtension))
                            {
                                logWriter.WriteLine($"Warning: Skipping file {fileName} because the new extension is empty.");
                                totalFilesSkipped++;
                                continue;
                            }

                            string newFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file) + newFileExtension);
                            System.IO.File.Move(file, newFileName);
                            logWriter.WriteLine($"File extension changed: {fileName} -> {System.IO.Path.GetFileName(newFileName)}");
                            totalFilesProcessed++;
                            lastFiles.Add(newFileName);
                        }
                        else
                        {
                            totalFilesSkipped++;
                        }
                    }
                    else if (selectedType == 2)
                    {
                        fileContent = System.IO.File.ReadAllText(file);
                        if (fileContent.Contains(findText, isSensitivityChecked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                        {
                            string newContent = fileContent.Replace(findText, replaceText, isSensitivityChecked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
                            System.IO.File.WriteAllText(file, newContent);
                            logWriter.WriteLine($"File content updated: {fileName}");
                            totalFilesProcessed++;
                        }
                        else
                        {
                            totalFilesSkipped++;
                        }
                    }
                }

                logWriter.WriteLine("---");
                logWriter.WriteLine($"Total files processed: {totalFilesProcessed}");
                logWriter.WriteLine($"Total files skipped: {totalFilesSkipped}");
            }

            lastFilesArray = lastFiles.ToArray();
            if (selectedType == 0)
            {
                lastType = 0;
            }
            else if (selectedType == 1)
            {
                lastType = 1;
            }
            else
            {
                lastType = 2;
            }
            lastFind = Find.Text;
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
            int selectedType = Type.SelectedIndex;

            if (selectedType == 2)
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
                string fileContent = System.IO.File.ReadAllText(file);

                if (selectedType == 0)
                {
                    string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
                    string newFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), fileNameWithoutExtension.Replace(lastReplace, lastFind, StringComparison.OrdinalIgnoreCase) + fileExtension);
                    System.IO.File.Move(file, newFileName);
                }
                else if (selectedType == 1)
                {
                    string newFileExtension = fileExtension.Replace(lastReplace, lastFind);
                    string newFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file) + newFileExtension);
                    System.IO.File.Move(file, newFileName);
                }
            }
            revertEnabled = false;
        }



    }
}
