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

namespace VMClusterManager
{
    /// <summary>
    /// Interaction logic for VMDetailsView.xaml
    /// </summary>
    public partial class VMDetailsView : UserControl, IView
    {
        public VMDetailsView()
        {
            InitializeComponent();
            this.Unloaded += new RoutedEventHandler(VMDetailsView_Unloaded);
        }

        void VMDetailsView_Unloaded(object sender, RoutedEventArgs e)
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
