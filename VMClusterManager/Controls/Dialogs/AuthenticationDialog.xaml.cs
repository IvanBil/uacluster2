using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VMClusterManager.Controls.Dialogs
{
    /// <summary>
    /// Interaction logic for AuthenticationDialog.xaml
    /// </summary>
    public partial class AuthenticationDialog : Window
    {
        public AuthenticationDialog(string Header)
        {
            InitializeComponent();
            this.Title = Header;
        }

        public string UserName;
        public System.Security.SecureString Password;

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            UserName = tbUserName.Text;
            Password = pwdPassword.SecurePassword;
        }

    }
}
