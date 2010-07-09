using System;
using System.Collections.Generic;
using VMClusterManager.Controls;
using VMClusterManager.ViewModels.VMModels;
using System.Threading;
using System.Collections.ObjectModel;
using Microsoft.Practices.Composite.Wpf.Commands;

namespace VMClusterManager.ViewModels
{
    class VMListViewModel : ViewModelBase
    {
        IVMModel vmModel;
        private VMListView view;

        public VMListView View
        {
            get { return view; }
            set { view = value; }
        }

        private DelegateCommand<ObservableCollection<VM>> refreshVMListCommand;
        private DelegateCommand<ObservableCollection<VMViewModel>> selectAllCommand;
        private DelegateCommand<ObservableCollection<VMViewModel>> unselectAllCommand;

        public DelegateCommand<ObservableCollection<VMViewModel>> UnselectAllCommand
        {
            get { return unselectAllCommand; }
            set { unselectAllCommand = value; OnPropertyChanged("UnselectAllCommand"); }
        }

        public DelegateCommand<ObservableCollection<VMViewModel>> SelectAllCommand
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

        private ObservableCollection<VMViewModel> vmList;

        public ObservableCollection<VMViewModel> VMList
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


        private void FillVMList()
        {
            ObservableCollection<VM> GroupVMList = new ObservableCollection<VM>(vmModel.ActiveVMGroup.VMList);
            foreach (VMViewModel vm in VMList)
            {
                vm.Dispose();
            }
            this.VMList.Clear();
            foreach (VM vm in GroupVMList)
            {
                VMViewModel vmViewModel = new VMViewModel(vm);
                vmViewModel.Activated +=
                    (o, e) =>
                    {
                        vmModel.ActiveVMList.Add((o as VMViewModel).VirtualMachine);
                    };
                vmViewModel.Deactivated +=
                    (o, e) =>
                    {
                        vmModel.ActiveVMList.Remove((o as VMViewModel).VirtualMachine);
                    };
                this.VMList.Add(vmViewModel);
                //OnPropertyChanged("VMList");
            }
        }

        public VMListViewModel(IVMModel vmModel)
        {
            this.vmModel = vmModel;
            View = new VMListView();
            this.vmList = new ObservableCollection<VMViewModel>();
            vmModel.ActiveVMList.Clear();
            FillVMList();
            RefreshVMListCommand = new DelegateCommand<ObservableCollection<VM>>(RefreshVMList, CanRefreshVMList);
            SelectAllCommand = new DelegateCommand<ObservableCollection<VMViewModel>> (SelectAll, CanSelectAll);
            UnselectAllCommand = new DelegateCommand<ObservableCollection<VMViewModel>>(UnselectAll, CanUnselectAll);
            CanExecuteRaisers = new List<Action>{
                RefreshVMListCommand.RaiseCanExecuteChanged,
                SelectAllCommand.RaiseCanExecuteChanged,
                UnselectAllCommand.RaiseCanExecuteChanged
            };

            this.VMList.CollectionChanged +=
                (obj, exp) =>
                {
                    RefreshCommands();
                    
                };
            vmModel.ActiveVMList.CollectionChanged +=
                (obj, exp) =>
                {
                    UnselectAllCommand.RaiseCanExecuteChanged();
                };

            vmModel.ActiveVMGroup.VMList.CollectionChanged +=
                (obj, exp) =>
                {
                    FillVMList();
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
            FillVMList();
        }

        private bool CanRefreshVMList(ObservableCollection<VM> vmList)
        {
            return true;
        }

        private void SelectAll(ObservableCollection<VMViewModel> vmToSelect)
        {
            foreach (VMViewModel vm in vmToSelect)
            {
                if (!vm.IsActiveVM)
                {
                    vm.IsActiveVM = true;
                }
            }
        }

        private bool CanSelectAll(ObservableCollection<VMViewModel> vmToSelect)
        {
            bool canSelectAll = false;
            if (vmToSelect != null)
            {
                canSelectAll = (vmToSelect.Count > 0);
            }
            return canSelectAll;
        }

        private void UnselectAll(ObservableCollection<VMViewModel> vmToUnSelect)
        {
            foreach (VMViewModel vm in vmToUnSelect)
            {
                if (vm.IsActiveVM)
                {
                    vm.IsActiveVM = false;
                }
            }
        }

        private bool CanUnselectAll(ObservableCollection<VMViewModel> vmToUnSelect)
        {
            bool canUnselect =false;
            if (vmToUnSelect != null)
            {
                canUnselect = vmModel.ActiveVMList.Count > 0;
            }
            return canUnselect;
        }

        public override void Dispose()
        {
            foreach (VMViewModel vmView in VMList)
            {
                vmView.Dispose();
            }
        }
       
    }
}
