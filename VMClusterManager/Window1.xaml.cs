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
using VMClusterManager.Controls;
using VMClusterManager.ViewModels.VMHostModels;
using VMClusterManager.ViewModels.VMModels;
using VMClusterManager.Controls.JobViews;
using System.Diagnostics;
using VMClusterManager.Windows;
using VMClusterManager.ViewModels;

namespace VMClusterManager
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, IView
    {
        public Window1()
        {
            InitializeComponent();
            this.Title = System.Windows.Forms.Application.ProductName + " v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.DataContext = new MainWindowViewModel();
           
        }


        private void wMain_Loaded(object sender, RoutedEventArgs e)
        {
            //btnHosts_Click(this, new RoutedEventArgs());
            //VMTreeViewModel treepm = new VMTreeViewModel(VMModel.GetInstance());
        }

        void vm_VMStatusChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            //MessageBox.Show("State changed", (sender as VM).Name);
        }

        private void btnStart_Loaded(object sender, RoutedEventArgs e)
        {
            //if (((sender as Control).Parent as Control).Parent
            (sender as Button).Content = "Start";
            
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void wMain_Closed(object sender, EventArgs e)
        {
            //foreach (VM vm in lstBox.Items)
            //    vm.Close();
           
        }

        private void btnVM_Click(object sender, RoutedEventArgs e)
        {
            //VMTreeViewModel treepm = new VMTreeViewModel(VMModel.GetInstance());

            ////Binding bind = new Binding("SelectedItem");
            ////Binding bind = new Binding();
            ////bind.Source = treepm.View.treeElements;
            //TreeCommands.Content = treepm.GroupCommandsView;
            //DetailViewBase vb = new DetailViewBase();
            ////vb.SetBinding(DetailViewBase.DataContextProperty, bind);

            //navigationTree.Content = treepm.View;
            //detailsPlaceholder.Children.Clear();
            //detailsPlaceholder.Children.Add(vb);
            //VMCommandsViewModel vmCommandsViewModel = new VMCommandsViewModel(VMModel.GetInstance());
            //actionMenu.Content = vmCommandsViewModel.View;

        }

        private void btnHosts_Click(object sender, RoutedEventArgs e)
        {
            //VMHostTreeViewModel treevm = new VMHostTreeViewModel(VMModel.GetInstance());
            //navigationTree.Content = treevm.View;
            //TreeCommandsView treeCommands
        }

        private void btnTasks_Click(object sender, RoutedEventArgs e)
        {
            //VMTreeViewModel treepm = new VMTreeViewModel(VMModel.GetInstance());
            //navigationTree.Content = treepm.View;
            //JobListViewModel JbLstvm = new JobListViewModel("localhost");
            //DetailViewBase vb = new DetailViewBase(JbLstvm);
            //detailsPlaceholder.Children.Clear();
            //detailsPlaceholder.Children.Add(vb);
            //JobCommandsView commView = new JobCommandsView();
            //commView.SetViewModel(JbLstvm);
            //actionMenu.Content = commView;
            //JobListViewModel jvm = new JobListViewModel("hnode");
            //detailsPlaceholder.Children.Clear();
            //detailsPlaceholder.Children.Add(jvm.View as UIElement);
        }

        private void AppExit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AppExit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void AppHelp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AppHelp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start("http://uacluster2.codeplex.com/documentation");
        }


        private void mnuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow wndAbout = new AboutWindow();
            wndAbout.Owner = this;
            wndAbout.ShowDialog();
        }

        public void SetViewModel(object ViewModel)
        {
            this.DataContext = ViewModel;
        }
    }
}
