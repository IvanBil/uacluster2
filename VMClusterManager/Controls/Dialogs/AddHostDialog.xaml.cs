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
using VMClusterManager.ViewModels.DialogModels;

namespace VMClusterManager.Controls.Dialogs
{
    /// <summary>
    /// Interaction logic for AddHostDialog.xaml
    /// </summary>
    public partial class AddHostDialog : Window
    {
        public List<string> HostNames;

        public AddHostDialog()
        {
            AddHostDialogViewModel viewModel = new AddHostDialogViewModel(VMModel.GetInstance());
            this.DataContext = viewModel;
            viewModel.OKPerformed += new EventHandler<EventArgs>(viewModel_OKPerformed);
            InitializeComponent();
            
        }

        void viewModel_OKPerformed(object sender, EventArgs e)
        {
            HostNames = new List<string>((this.DataContext as AddHostDialogViewModel).HostList);
            this.DialogResult = true;
        }

       

        private void TextBox_Error(object sender, ValidationErrorEventArgs e)
        {
            (this.DataContext as AddHostDialogViewModel).ValidationError = true;
            
        }

        private void IsSingleHost_Checked(object sender, RoutedEventArgs e)
        {
            (this.DataContext as AddHostDialogViewModel).IsSingleHost = true;
            try
            {
                tbHostName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
            catch (Exception ) { }
        }

        private void IsSingleHost_Unchecked(object sender, RoutedEventArgs e)
        {
            (this.DataContext as AddHostDialogViewModel).IsSingleHost = false;
            try
            {
                tbStartIP.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                tbEndIP.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
            catch (Exception) { }
        }


    }
}
