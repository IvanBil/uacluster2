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

namespace VMClusterManager.Controls.JobViews
{
    /// <summary>
    /// Interaction logic for JobCommandsView.xaml
    /// </summary>
    public partial class JobCommandsView : UserControl, IView
    {
        public JobCommandsView()
        {
            InitializeComponent();
        }

        public JobCommandsView(object ViewModel): this()
        {
            this.DataContext = ViewModel;
        }

        #region IView Members

        public void SetViewModel(object ViewModel)
        {
            this.DataContext = ViewModel;
        }

        #endregion
    }
}
