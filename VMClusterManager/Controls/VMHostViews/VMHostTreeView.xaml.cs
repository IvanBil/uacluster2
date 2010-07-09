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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VMClusterManager.Controls.VMHostViews
{
    /// <summary>
    /// Interaction logic for VMHostTreeView.xaml
    /// </summary>
    public partial class VMHostTreeView : UserControl, IView
    {
        public VMHostTreeView()
        {
            InitializeComponent();
        }

        #region IView Members

        public void SetViewModel(object ViewModel)
        {
            this.DataContext = ViewModel;
        }

        #endregion

        private void treeElements_LayoutUpdated(object sender, EventArgs e)
        {
            //(sender as TreeView).Height = (sender as TreeView).ActualHeight; 
        }
    }
}
