using SharePointFileTransfer_Library;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace SharePointFileTransfer
{
    /// <summary>
    /// Interaction logic for ChangeCredentialPage.xaml
    /// </summary>
    public partial class ChangeCredentialPage : Page
    {
        public ChangeCredentialPage()
        {
            InitializeComponent();
            setTextBoxes();
        }
        public void setTextBoxes()
        {
            UsernameTB.Text = ConfigurationManager.AppSettings["user"];
        }

        private void UpdateCredential_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTB.Text;
            string psw = PasswortTB.Password;

            if (username == null || username.Length < 4)
                MessageBox.Show("Username muss mindestens 4 Zeichen haben!");
            else if (psw == null || psw.Length < 4)
                MessageBox.Show("Passwort muss mindestens über 4 Zeichen verfügen!");
            else
            {
                try
                {
                    SharePointFileTransfer_Library.SharePointFileTransfer.changeCredentials(username, psw);
                    MessageBox.Show("Credentials succesfully changed.");
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error occured during changing the Credentials; ErrorMsg: " + ex.Message);
                }
            }
        }
    }
}
