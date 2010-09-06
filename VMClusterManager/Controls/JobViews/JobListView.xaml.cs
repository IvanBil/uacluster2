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
using VMClusterManager.ViewModels.HpcModels;

namespace VMClusterManager.Controls.JobViews
{
    /// <summary>
    /// Interaction logic for JobListView.xaml
    /// </summary>
    public partial class JobListView : UserControl, IView
    {
        public JobListView()
        {
            InitializeComponent();
        }

        #region IView Members

        public void SetViewModel(object ViewModel)
        {
            this.DataContext = ViewModel;
        }

        #endregion

        private void dgTaskList_UnloadingRow(object sender, Microsoft.Windows.Controls.DataGridRowEventArgs e)
        {
            if (e.Row.DataContext != null)
            {
                (e.Row.DataContext as TaskViewModel).Dispose();
            }
        }
    }
}
