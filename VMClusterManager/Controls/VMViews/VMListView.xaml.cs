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
using VMClusterManager.ViewModels;

namespace VMClusterManager.Controls
{
    /// <summary>
    /// Interaction logic for VMListView.xaml
    /// </summary>
    public partial class VMListView : UserControl, IView
    {
        public VMListView()
        {
            InitializeComponent();
            this.Unloaded += new RoutedEventHandler(VMListView_Unloaded);
        }

        void VMListView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                if (this.DataContext is ViewModelBase)
                {
                    (this.DataContext as ViewModelBase).Dispose();
                }
            }
        }

        #region IView Members

        public void SetViewModel(object ViewModel)
        {
            this.DataContext = ViewModel;
        }

        #endregion
    }
}
