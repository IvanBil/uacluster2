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
using VMClusterManager.ViewModels.VMModels;

namespace VMClusterManager.Controls
{
    /// <summary>
    /// Interaction logic for DetailViewBase.xaml
    /// </summary>
    public partial class DetailViewBase : UserControl
    {
        //private ViewModelBase viewModel;
        public DetailViewBase()
        {
            InitializeComponent();
            VMModel model = VMModel.GetInstance();
            this.Unloaded += new RoutedEventHandler(DetailViewBase_Unloaded);
            model.SelectedTreeItemChanged +=
                (o, e) =>
                {
                    if (model.SelectedTreeItem is VM)
                    {
                        VMDetailsViewModel viewModel = new VMDetailsViewModel(model.SelectedTreeItem as VM);
                        this.Content = viewModel.View;
                        GC.Collect();
                        //this.detailContent.Content = viewModel.View;
                    }
                    else if (model.SelectedTreeItem is VMGroup)
                    {
                        
                        VMListViewModel viewModel = new VMListViewModel(VMModel.GetInstance());
                        this.Content = viewModel.View;
                        GC.Collect();
                    }

                };
        }

        public DetailViewBase(JobListViewModel jlvm)
        {
            InitializeComponent();
            VMModel model = VMModel.GetInstance();
            this.Unloaded += new RoutedEventHandler(DetailViewBase_Unloaded);
            this.Content = jlvm.View as UserControl;
        }

        void DetailViewBase_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext as ViewModelBase != null)
            {
                (this.DataContext as ViewModelBase).Dispose();
            }
        }
    }
}
