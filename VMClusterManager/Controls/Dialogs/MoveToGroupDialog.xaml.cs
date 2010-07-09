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
using System.Collections.ObjectModel;

namespace VMClusterManager.Controls.Dialogs
{
    /// <summary>
    /// Interaction logic for MoveToGroupDialog.xaml
    /// </summary>
    public partial class MoveToGroupDialog : Window
    {
        public Group CheckedItem;
        public MoveToGroupDialog(ObservableCollection<object> root)
        {
            InitializeComponent();
            this.DataContext = root;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        //public void Show(Window owner)
        //{
        //    this.Owner = owner;
        //    this.Sho
        //}

        private void treeElements_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CheckedItem = e.NewValue as Group;
        }
    }
}
