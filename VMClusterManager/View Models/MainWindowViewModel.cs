using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Microsoft.Practices.Composite.Wpf.Commands;
using VMClusterManager.ViewModels.VMHostModels;
using VMClusterManager.Controls;
using VMClusterManager.ViewModels.VMModels;
using VMClusterManager.Controls.Dialogs;
using System.Windows;

namespace VMClusterManager.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private Control vmHostNavigationView;
        private Control vmNavigationView;

        public Control VMNavigationView
        {
            get { return vmNavigationView; }
            set { vmNavigationView = value; OnPropertyChanged("VMNavigationView"); }
        }
        private Control detailsView;
        private Control actionsView;
        private Control treeCommandsView;
        private Control vmHostTreeActionsView;

        public Control VMHostTreeActionsView
        {
            get { return vmHostTreeActionsView; }
            set { vmHostTreeActionsView = value; OnPropertyChanged("VMHostTreeActionsView"); }
        }

        public Control TreeCommandsView
        {
            get { return treeCommandsView; }
            set { treeCommandsView = value; OnPropertyChanged("TreeCommandsView"); }
        }

        private VMModel model;

        public Control ActionsView
        {
            get { return actionsView; }
            set { actionsView = value; OnPropertyChanged("ActionsView"); }
        }

        public Control DetailsView
        {
            get { return detailsView; }
            set { detailsView = value; OnPropertyChanged("DetailsView"); }
        }

        public Control VMHostNavigationView
        {
            get { return vmHostNavigationView; }
            set { vmHostNavigationView = value; OnPropertyChanged("VMHostNavigationView"); }
        }

        private object hostListView;

        public object HostListView
        {
            get { return hostListView; }
            set { hostListView = value; OnPropertyChanged("HostListView"); }
        }

        private Control jobAndTaskCommandsView;

        public Control JobAndTaskCommandsView
        {
            get { return jobAndTaskCommandsView; }
            set { jobAndTaskCommandsView = value; OnPropertyChanged("JobAndTaskCommandsView"); }
        }

        private DelegateCommand<Window1> closeCommand;

        public DelegateCommand<Window1> CloseCommand
        {
            get { return closeCommand; }
            set { closeCommand = value; OnPropertyChanged("CloseCommand"); }
        }

        private bool hostListExpanded;

        public bool HostListExpanded
        {
            get { return hostListExpanded; }
            set 
            { 
                hostListExpanded = value;
                if (hostListExpanded)
                {
                    if (model.ActiveVMHostGroup != null)
                    {
                        VMHostListViewModel hostListViewModel = new VMHostListViewModel(model, model.ActiveVMHostGroup.HostList);
                        HostListView = hostListViewModel.View;
                        GC.Collect();
                    }
                    else
                    {
                        HostListView = "Select host group to explore its child hosts.";
                    }
                }
                OnPropertyChanged("HostListExpanded"); 
            }
        }


        public MainWindowViewModel()
            : base()
        {
            
            model = VMModel.GetInstance();
            
            VMHostTreeViewModel hostTree = new VMHostTreeViewModel(model);
            
            //NavigationViewModel nav = new NavigationViewModel(model);
            VMHostNavigationView = hostTree.View;
            VMHostTreeActionsView = hostTree.ActionsView;
            VMTreeViewModel vmTree = new VMTreeViewModel(model);
            TreeCommandsView = vmTree.GroupCommandsView;
            VMNavigationView = vmTree.View;
            model.ActiveVMHostGroupChanged +=
                (o, e) =>
                {
                    if (HostListExpanded)
                    {
                        HostListExpanded = false;
                        HostListExpanded = true;
                    }
                };
            model.ActiveVMGroupChanged +=
                (o, e) =>
                {
                    VMListViewModel listViewModel = new VMListViewModel(model, model.ActiveVMGroup.VMList);
                    DetailsView = listViewModel.View;
                    //JobAndTaskCommandsView = listViewModel.JobActionsView;
                    listViewModel.JobActionsViewChanged +=
                        (obj, arg) =>
                        {
                            JobAndTaskCommandsView = listViewModel.JobActionsView;
                        };
                    GC.Collect();
                };
            
            VMCommandsViewModel actionsvm = new VMCommandsViewModel(model);
            ActionsView = actionsvm.View;
            //VMListViewModel vmlstvm = new VMListViewModel(VMModel.GetInstance());
            //DetailsView = vmlstvm.View; 

        }

        
    }
}
