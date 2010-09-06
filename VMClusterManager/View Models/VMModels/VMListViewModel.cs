using System;
using System.Collections.Generic;
using VMClusterManager.Controls;
using VMClusterManager.ViewModels.VMModels;
using System.Threading;
using System.Collections.ObjectModel;
using Microsoft.Practices.Composite.Wpf.Commands;
using System.Windows;
using VMClusterManager.Controls.JobViews;
using VMClusterManager.ViewModels.HpcModels;

namespace VMClusterManager.ViewModels
{
    class VMListViewModel : ViewModelBase
    {
        VMModel vmModel;
        private VMListView view;

        public VMListView View
        {
            get { return view; }
            set { view = value; }
        }

        private string mainNodeName;

        public string MainNodeName
        {
            get { return mainNodeName; }
            set { mainNodeName = value; ChangeMainNodeNameCommand.RaiseCanExecuteChanged(); OnPropertyChanged("MainNodeName"); }
        }

        private JobCommandsView jobActionsView;

        public JobCommandsView JobActionsView
        {
            get { return jobActionsView; }
            set { jobActionsView = value; OnJobActionsViewChanged(); }
        }

        public event EventHandler<EventArgs> JobActionsViewChanged;

        private void OnJobActionsViewChanged()
        {
            if (JobActionsViewChanged != null)
                JobActionsViewChanged(this, new EventArgs());
        }

        private DelegateCommand<ObservableCollection<VM>> refreshVMListCommand;
        private DelegateCommand<IEnumerable<VMViewModel>> selectAllCommand;
        private DelegateCommand<IEnumerable<VMViewModel>> unselectAllCommand;
        private DelegateCommand<string> changeMainNodeNameCommand;

        public DelegateCommand<string> ChangeMainNodeNameCommand
        {
            get { return changeMainNodeNameCommand; }
            set { changeMainNodeNameCommand = value; OnPropertyChanged("ChangeMainNodeNameCommand"); }
        }

        public DelegateCommand<IEnumerable<VMViewModel>> UnselectAllCommand
        {
            get { return unselectAllCommand; }
            set { unselectAllCommand = value; OnPropertyChanged("UnselectAllCommand"); }
        }

        public DelegateCommand<IEnumerable<VMViewModel>> SelectAllCommand
        {
            get { return selectAllCommand; }
            set { selectAllCommand = value; OnPropertyChanged("SelectAllCommand"); }
        }

        public DelegateCommand<ObservableCollection<VM>> RefreshVMListCommand
        {
            get { return refreshVMListCommand; }
            set { refreshVMListCommand = value; OnPropertyChanged("RefreshVMListCommand"); }
        }
        

        private List<Action> CanExecuteRaisers;

        public ObservableCollection<LogMessage> Log
        {
            get
            {
                return VMLog.GetInstance();
            }
        }

        private ObservableCollection<VM> vmList;

        public ObservableCollection<VM> VMList
        {
            get { return vmList; }
            set 
            { 
                vmList = value;
                OnPropertyChanged("VMList");
            }
        }

        private ObservableCollection<VM> activeVMList;

        public ObservableCollection<VM> ActiveVMList
        {
            get { return activeVMList; }
            private set { activeVMList = value; }
        }

        private object vmDetails;
        private object vmProperties;
        private object hpcJobsView;

        public object HpcJobsView
        {
            get { return hpcJobsView; }
            set { hpcJobsView = value; OnPropertyChanged("HpcJobsView"); }
        }

        public object VMProperties
        {
            get { return vmProperties; }
            set { vmProperties = value; OnPropertyChanged("VMProperties"); }
        }

        public object VMDetails
        {
            get { return vmDetails; }
            set { vmDetails = value; OnPropertyChanged("VMDetails"); }
        }

        private bool detailsExpanded;
        private bool propertiesExpanded;
        private bool jobsExpanded;
        private bool logExpanded;

        public bool LogExpanded
        {
            get { return logExpanded; }
            set { logExpanded = value; OnPropertyChanged("LogExpanded"); }
        }

        public bool JobsExpanded
        {
            get { return jobsExpanded; }
            set 
            { 
                jobsExpanded = value;
                if (value)
                {
                    if (vmModel.ActiveVMList.Count > 0)
                    {
                        JobListViewModel jobs = new JobListViewModel(MainNodeName, vmModel.ActiveVMList);
                        HpcJobsView = jobs.View;
                        JobActionsView = jobs.ActionsView;
                    }

                    else
                    {
                        HpcJobsView = "Select one or more Virtual Machines to explore HPC jobs running on.";
                        
                    }
                    
                }
                else
                {
                    JobActionsView = null;
                }
                OnPropertyChanged("JobsExpanded");
            }
        }

        public bool PropertiesExpanded
        {
            get { return propertiesExpanded; }
            set 
            { 
                propertiesExpanded = value;
                if (value)
                {
                    if (vmModel.ActiveVMList.Count > 0)
                    {
                        VMPropertiesViewModel vmProp = new VMPropertiesViewModel(vmModel.ActiveVMList);
                        vmProp.ViewCloseRequested +=
                            (o, e) =>
                            {
                                PropertiesExpanded = false;
                            };
                        VMProperties = vmProp.View;
                    }
                    else
                    {
                        VMProperties = "No Virtual Machine selected.";
                    }
                }
                OnPropertyChanged("PropertiesExpanded"); 
            }
        }

        public bool DetailsExpanded
        {
            get { return detailsExpanded; }
            set 
            {
                detailsExpanded = value;
                if (value)
                {
                    if (vmModel.ActiveVMList.Count == 1)
                    {
                        VMDetailsViewModel vmDet = new VMDetailsViewModel(vmModel.ActiveVMList[0]);
                        VMDetails = vmDet.View;
                    }
                    else
                    {
                        VMDetails = "Details are available only for single VM checked";
                    }
                }
                OnPropertyChanged("DetailsExpanded"); 
            }
        }

        

        public VMListViewModel(VMModel vmModel, ObservableCollection<VM> _VMList)
        {
            this.vmModel = vmModel;
            View = new VMListView();
            this.VMList = _VMList;
            vmModel.ActiveVMList.Clear();
            
            //FillVMList();
            RefreshVMListCommand = new DelegateCommand<ObservableCollection<VM>>(RefreshVMList, CanRefreshVMList);
            SelectAllCommand = new DelegateCommand<IEnumerable<VMViewModel>>(SelectAll, CanSelectAll);
            UnselectAllCommand = new DelegateCommand<IEnumerable<VMViewModel>>(UnselectAll, CanUnselectAll);
            ChangeMainNodeNameCommand = new DelegateCommand<string>(ChangeMainNode, CanChangeMainNode);
            CanExecuteRaisers = new List<Action>{
                RefreshVMListCommand.RaiseCanExecuteChanged,
                SelectAllCommand.RaiseCanExecuteChanged,
                UnselectAllCommand.RaiseCanExecuteChanged

            };
            this.MainNodeName = vmModel.Settings.MainNodeName;

            this.VMList.CollectionChanged +=
                (obj, exp) =>
                {
                    RefreshCommands();
                    OnPropertyChanged("VMList");
                    
                };
            vmModel.ActiveVMList.CollectionChanged +=
                (obj, exp) =>
                {

                    DetailsExpanded = false;
                    PropertiesExpanded = false;
                    JobsExpanded = false;
                    GC.Collect();
                    UnselectAllCommand.RaiseCanExecuteChanged();
                };
            VMLog.GetInstance().CollectionChanged +=
                (o, e) =>
                {
                    LogExpanded = true;
                };
            View.SetViewModel(this);
        }

        private Thread CommandRefreshThread = null;
        private void RefreshCommands()
        {
            if (CommandRefreshThread != null)
            {
                if (CommandRefreshThread.ThreadState == ThreadState.Running)
                {
                    return;
                }
            }
            CommandRefreshThread = new Thread(new ThreadStart(delegate()
            {
                foreach (Action action in CanExecuteRaisers)
                {
                    action();
                }
            }));
            CommandRefreshThread.IsBackground = true;
            CommandRefreshThread.Start();

        }

        private void RefreshVMList(ObservableCollection<VM> vmList)
        {
            vmModel.ActiveVMList.Clear();
            //FillVMList();
        }

        private bool CanRefreshVMList(ObservableCollection<VM> vmList)
        {
            return true;
        }

        private void SelectAll(IEnumerable<VMViewModel> vmToSelect)
        {
            foreach (VMViewModel vm in vmToSelect)
            {
                if (!vm.IsActiveVM)
                {
                    vm.IsActiveVM = true;
                }
            }
        }

        private bool CanSelectAll(IEnumerable<VMViewModel> vmToSelect)
        {
            //bool canSelectAll = false;
            //if (vmToSelect != null)
            //{
            //    canSelectAll = (vmToSelect. > 0);
            //}
            return true;
        }

        private void UnselectAll(IEnumerable<VMViewModel> vmToUnSelect)
        {
            foreach (VMViewModel vm in vmToUnSelect)
            {
                if (vm.IsActiveVM)
                {
                    vm.IsActiveVM = false;
                }
            }
        }

        private bool CanUnselectAll(IEnumerable<VMViewModel> vmToUnSelect)
        {
            bool canUnselect =false;
            if (vmToUnSelect != null)
            {
                canUnselect = vmModel.ActiveVMList.Count > 0;
            }
            return canUnselect;
        }

        public  void Dispose()
        {
            //foreach (VMViewModel vmView in VMList)
            //{
            //    vmView.Dispose();
            //}
        }

        private void ChangeMainNode(string newName)
        {
            vmModel.Settings.MainNodeName = this.MainNodeName;
            vmModel.Settings.SaveToFile();
            ChangeMainNodeNameCommand.RaiseCanExecuteChanged();
            if (vmModel.ActiveVMList.Count > 0)
            {
                JobListViewModel jobs = new JobListViewModel(MainNodeName, vmModel.ActiveVMList);
                HpcJobsView = jobs.View;
                JobActionsView = jobs.ActionsView;
            }
        }

        private bool CanChangeMainNode(string newName)
        {
            return vmModel.Settings.MainNodeName != this.MainNodeName;
        }
       
    }
}
