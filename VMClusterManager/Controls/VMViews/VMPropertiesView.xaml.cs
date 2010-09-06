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

namespace VMClusterManager.Controls.VMViews
{
    /// <summary>
    /// Interaction logic for VMPropertiesView.xaml
    /// </summary>
    public partial class VMPropertiesView : UserControl, IView
    {
        public VMPropertiesView()
        {
            InitializeComponent();
        }

        #region IView Members

        public void SetViewModel(object ViewModel)
        {
            this.DataContext = ViewModel;
        }

        #endregion
    }
}
