using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Configuration;
using SharePointFileTransfer_Library;

namespace SharePointFileTransfer
{
    /// <summary>
    /// Interaction logic for SettingPage.xaml
    /// </summary>
    public partial class SettingPage : Page
    {
        public SettingPage()
        {
            InitializeComponent();
            fullFilePathTB.Text = ConfigurationManager.AppSettings["filePath"];
        }
        
        private void OpenFileDialogTB_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    fullFilePathTB.Text = folderDialog.SelectedPath;
                }
            }

        }

        private void UpdateFilePath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConfigurationManager.AppSettings["filePath"] = fullFilePathTB.Text;
                if(FileWatcher.MyFileSystemWatcher != null)
                    FileWatcher.ChangePath(fullFilePathTB.Text);
                System.Windows.MessageBox.Show("Filepath changed.");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error occured; ErrorMsg: " + ex.Message);
            }
}
    }
}
