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
    /// Interaction logic for HostListInitializeDialog.xaml
    /// </summary>
    public partial class HostListInitializeDialog : Window
    {
        public HostListInitializeDialog(string Message, string Header)
        {
            InitializeComponent();
            this.Title = Header;
            this.MessageText.Text = Message;
        }

        public void Close()
        {
            this.Close();
        }

        
    }
}
