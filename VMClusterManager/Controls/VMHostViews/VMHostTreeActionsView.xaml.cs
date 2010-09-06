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
    /// Interaction logic for VMHostTreeActionsView.xaml
    /// </summary>
    public partial class VMHostTreeActionsView : UserControl
    {
        public VMHostTreeActionsView()
        {
            InitializeComponent();
        }

        public VMHostTreeActionsView(object ViewModel):this()
        {
            this.DataContext = ViewModel;
        }
    }
}
