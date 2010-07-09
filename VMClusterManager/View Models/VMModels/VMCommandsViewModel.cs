using System;
using System.Collections.Generic;
using VMClusterManager.Controls.VMViews;
using Microsoft.Practices.Composite.Wpf.Commands;
using System.Threading;
using System.Collections.ObjectModel;
using VMClusterManager.Controls.Dialogs;
using System.Windows;

namespace VMClusterManager.ViewModels.VMModels
{
    public class VMCommandsViewModel : ViewModelBase
    {
        
        VMModel model;
        public VMCommandsView View;
        ObservableCollection<VM> activeVMList;
        #region COMMANDS
        private DelegateCommand<ObservableCollection<VM>> vmListStart;
        private DelegateCommand<ObservableCollection<VM>> vmListStop;
        private DelegateCommand<ObservableCollection<VM>> vmListShutdown;
        private DelegateCommand<object> moveToVMGroup;
        private DelegateCommand<ObservableCollection<VM>> vmListSuspend;
        private DelegateCommand<ObservableCollection<VM>> vmListPause;
        private DelegateCommand<ObservableCollection<VM>> vmListReboot;
        private DelegateCommand<ObservableCollection<VM>> vmConnect;
        private DelegateCommand<ObservableCollection<VM>> vmListCreateSnapshot;
        private DelegateCommand<VMSnapshot> vmSnapshotApply;
        private DelegateCommand<VMSnapshot> vmSnapshotRemove;
        private DelegateCommand<VMSnapshot> vmSnapshotTreeRemove;


        public DelegateCommand<object> MoveToVMGroup
        {
            get { return moveToVMGroup; }
            set { moveToVMGroup = value; OnPropertyChanged("MoveToVMGroup"); }
        }

        public DelegateCommand<VMSnapshot> VMSnapshotTreeRemove
        {
            get { return vmSnapshotTreeRemove; }
            set { vmSnapshotTreeRemove = value; OnPropertyChanged("VMSnapshotTreeRemove"); }
        }

        public DelegateCommand<VMSnapshot> VMSnapshotRemove
        {
            get { return vmSnapshotRemove; }
            set { vmSnapshotRemove = value; OnPropertyChanged("VMSnapshotRemove"); }
        }

        public DelegateCommand<VMSnapshot> VMSnapshotApply
        {
            get { return vmSnapshotApply; }
            set { vmSnapshotApply = value; OnPropertyChanged("VMSnapshotApply"); }
        }
        //private DelegateCommand<ObservableCollection<VM>> 
        private List<Action> CanExecuteRaisers;  

        public DelegateCommand<ObservableCollection<VM>> VMListCreateSnapshot
        {
            get { return vmListCreateSnapshot; }
            set { vmListCreateSnapshot = value; OnPropertyChanged("VMListCreateSnapshot"); }
        }

        public DelegateCommand<ObservableCollection<VM>> VMConnect
        {
            get { return vmConnect; }
            set { vmConnect = value; OnPropertyChanged("VMConnect"); }
        }

        public DelegateCommand<ObservableCollection<VM>> VMListReboot
        {
            get { return vmListReboot; }
            set { vmListReboot = value; OnPropertyChanged("VMListReboot"); }
        }

        public DelegateCommand<ObservableCollection<VM>> VMListPause
        {
            get { return vmListPause; }
            set { vmListPause = value; OnPropertyChanged("VMListPause"); }
        }

        public DelegateCommand<ObservableCollection<VM>> VMListSuspend
        {
            get { return vmListSuspend; }
            set { vmListSuspend = value; OnPropertyChanged("VMListSuspend"); }
        }

        public DelegateCommand<ObservableCollection<VM>> VMListStop
        {
            get { return vmListStop; }
            set { vmListStop = value; OnPropertyChanged("VMListStop"); }
        }

        public DelegateCommand<ObservableCollection<VM>> VMListShutdown
        {
            get { return vmListShutdown; }
            set { vmListShutdown = value; OnPropertyChanged("VMListShutdown"); }
        }

        public DelegateCommand<ObservableCollection<VM>> VMListStart
        {
            get { return vmListStart; }
            set
            {
                vmListStart = value;
                OnPropertyChanged("VMListStart");
            }
        }
        #endregion //COMMANDS   
        private VMGroup activeVMGroup;

        public VMGroup ActiveVMGroup
        {
            get { return activeVMGroup; }
            set { activeVMGroup = value; OnPropertyChanged("ActiveVMGroup");}
        }

        private VM activeVM;

        public VM ActiveVM
        {
            get { return activeVM; }
            set 
            { 
                activeVM = value;
                OnPropertyChanged("ActiveVM");
            }
        }

        private object activeItem;

        public object ActiveItem
        {
            get { return activeItem; }
            set { activeItem = value; OnPropertyChanged("ActiveItem"); }
           
        }

        public ObservableCollection<VM> ActiveVMList
        {
            get { return activeVMList; }
            set 
            { 
                activeVMList = value;
                OnPropertyChanged("ActiveVMList");
            }
        }

        private VMSnapshot activeVMSnapshot;

        public VMSnapshot ActiveVMSnapshot
        {
            get { return activeVMSnapshot; }
            set { activeVMSnapshot = value; OnPropertyChanged("ActiveVMSnapshot"); }
        }

        public VMCommandsViewModel(VMModel model)
        {
            this.model = model;
            this.ActiveVMList = model.ActiveVMList;
            VMListStart = new DelegateCommand<ObservableCollection<VM>>(StartVMList, CanStartVMList);
            VMListStop = new DelegateCommand<ObservableCollection<VM>>(StopVMList, CanStopVMList);
            VMListShutdown = new DelegateCommand<ObservableCollection<VM>>(ShutdownVMList, CanShutdownVMList);
            VMListSuspend = new DelegateCommand<ObservableCollection<VM>>(SuspendVMList, CanSuspendVMList);
            VMListPause = new DelegateCommand<ObservableCollection<VM>>(PauseVMList, CanPauseVMList);
            VMListReboot = new DelegateCommand<ObservableCollection<VM>>(RebootVMList, CanRebootVMList);
            VMConnect = new DelegateCommand<ObservableCollection<VM>>(ConnectToVM, CanConnectToVM);
            VMListCreateSnapshot = new DelegateCommand<ObservableCollection<VM>>(CreateSnapshot, CanCreateSnapshot);
            VMSnapshotApply = new DelegateCommand<VMSnapshot>(ApplySnapshot, CanApplySnapshot);
            VMSnapshotRemove = new DelegateCommand<VMSnapshot>(RemoveSnapshot, CanRemoveSnapshot);
            VMSnapshotTreeRemove = new DelegateCommand<VMSnapshot>(RemoveSnapshotTree, CanRemoveSnapshotTree);
            MoveToVMGroup = new DelegateCommand<object>(MoveToGroup, CanMoveToGroup);
            CanExecuteRaisers = new List<Action>{
                            vmListStart.RaiseCanExecuteChanged,
            vmListStop.RaiseCanExecuteChanged,
            vmListShutdown.RaiseCanExecuteChanged,
            vmListPause.RaiseCanExecuteChanged,
            vmListReboot.RaiseCanExecuteChanged,
            vmListSuspend.RaiseCanExecuteChanged,
            vmConnect.RaiseCanExecuteChanged,
            vmListCreateSnapshot.RaiseCanExecuteChanged,
            vmSnapshotApply.RaiseCanExecuteChanged,
            vmSnapshotRemove.RaiseCanExecuteChanged,
            vmSnapshotTreeRemove.RaiseCanExecuteChanged,
            moveToVMGroup.RaiseCanExecuteChanged,
            };
            model.SelectedTreeItemChanged +=
                (o, e) =>
                {
                    this.ActiveItem = model.SelectedTreeItem;
                };
            model.ActiveVMGroupChanged +=
                (o, e) =>
                {
                    this.ActiveVMGroup = model.ActiveVMGroup;
                    RefreshCommands();
                };
            model.ActiveVMListChanged +=
                (o, e) =>
                {
                    if (e.NewItems != null)
                    {
                        this.ActiveVMList = model.ActiveVMList;
                        foreach (VM vm in e.NewItems)
                        {
                            vm.VMStatusChanged += new VMStatusChangedEventHandler(vm_VMStatusChanged);
                        }
                        if (e.OldItems != null)
                        {
                            foreach (VM vm in e.OldItems)
                            {
                                vm.VMStatusChanged -= vm_VMStatusChanged;
                            }
                        }
                       
                    }
                    RefreshCommands();
                };
            
            model.SelectedSnapshotItemChanged +=
                (o, e) =>
                {
                    ActiveVMSnapshot = (VMSnapshot)model.SelectedSnapshotItem;
                    RefreshCommands();
                };
            View = new VMCommandsView();
            View.SetViewModel(this);
        }

        void vm_VMStatusChanged(object sender, EventArgs e)
        {
            VM vm = sender as VM;
            if (vm.Status == VMState.Disabled ||
                vm.Status == VMState.Enabled ||
                vm.Status == VMState.Paused ||
                vm.Status == VMState.Suspended ||
                vm.Status == VMState.Unknown)
            {
                RefreshCommands();
            }
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


        #region COMMAND EXECUTERS AND CANEXECUTE 
        
        private void StartVMList(ObservableCollection<VM> vmList)
        {
            
            model.StartVMList(vmList);            
        }

        public bool CanStartVMList(ObservableCollection<VM> vmList)
        {
            
            return model.CanStartVMList(vmList);
        }

        public void StopVMList(ObservableCollection<VM> vmList)
        {
            
            model.StopVMList(vmList);
        }

        private void ShutdownVMList(ObservableCollection<VM> vmList)
        {
            model.ShutdownVMList(vmList);
            
        }

        private bool CanShutdownVMList(ObservableCollection<VM> vmList)
        {
            return CanStopVMList(vmList);
        }

        public bool CanStopVMList(ObservableCollection<VM> vmList)
        {
            return model.CanStopVMList(vmList);
            
        }

        public void SuspendVMList(ObservableCollection<VM> vmList)
        {
            
            model.SuspendVMList(vmList);
           
        }

        public bool CanSuspendVMList(ObservableCollection<VM> vmList)
        {
            
            return model.CanSuspendVMList(vmList);
        }

        public void PauseVMList(ObservableCollection<VM> vmList)
        {
            
            model.PauseVMList(vmList);
        }

        public bool CanPauseVMList(ObservableCollection<VM> vmList)
        {
            
            return model.CanPauseVMList(vmList);
        }

        public void RebootVMList(ObservableCollection<VM> vmList)
        {
            
            model.RebootVMList(vmList);
            
        }

        public bool CanRebootVMList(ObservableCollection<VM> vmList)
        {
           
            return model.CanRebootVMList(vmList);
        }

        public void ConnectToVM(ObservableCollection<VM> vmList)
        {
            try
            {
                if (vmList != null)
                {
                    foreach (VM vm in vmList)
                    {
                        Thread vmThread = new Thread(new ParameterizedThreadStart(delegate(object param)
                            {
                                (param as VM).ConnectTo();
                                RefreshCommands();
                            }));
                        vmThread.Start(vm);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, vmList[0].Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CanConnectToVM(ObservableCollection<VM> vmList)
        {
            if (vmList != null)
            {
                return vmList.Count == 1;
            }
            else return false;
        }

        public void CreateSnapshot(ObservableCollection<VM> vmList)
        {
           
            model.CreateSnapshot(vmList);
            
        }

        public bool CanCreateSnapshot(ObservableCollection<VM> vmList)
        {
            
            return model.CanCreateSnapshot(vmList);
        }

      
        public void ApplySnapshot(VMSnapshot snapshot)
        {
            if (MessageBox.Show("Are you shure you want to apply this snapshot? Current settings will be lost", snapshot.ElementName,
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                
                model.ApplySnapshot(snapshot, ActiveVMList);
            }
        }

        public bool CanApplySnapshot(VMSnapshot snapshot)
        {
            
            return model.CanApplySnapshot(snapshot, ActiveVMList);
        }

        public void RemoveSnapshot(VMSnapshot snapshot)
        {
            if (MessageBox.Show("Are you shure you want to remove this snapshot?",snapshot.ElementName,
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                
                model.RemoveSnapshot(snapshot, ActiveVMList);
            }
        }

        public bool CanRemoveSnapshot(VMSnapshot snapshot)
        {
            
            return model.CanRemoveSnapshot(snapshot, ActiveVMList);
        }

        private void RemoveSnapshotTree(VMSnapshot snapshot)
        {
            if (MessageBox.Show("Are you shure you want to remove this snapshot and all it's child snapshots?", snapshot.ElementName,
                MessageBoxButton.YesNo, MessageBoxImage.Warning,MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                
                model.RemoveSnapshotTree(snapshot, ActiveVMList);
            }
        }

        private bool CanRemoveSnapshotTree(VMSnapshot snapshot)
        {
            
            return model.CanRemoveSnapshotTree(snapshot, ActiveVMList);
        }

       

        private void MoveToGroup(object item)
        {
            ObservableCollection<object> tree = new ObservableCollection<object> ();
            tree.Add(VMModel.GetInstance().RootVMGroup);
            MoveToGroupDialog dlg = new MoveToGroupDialog(tree);
            dlg.Owner = Window1.GetWindow(View.Parent);
            dlg.ShowDialog();
            if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
            {
                //only VMGroup selected
                VMGroup group = dlg.CheckedItem as VMGroup;
                if ((item is VMGroup)&&(ActiveVMList.Count == 0))
                {
                    model.MoveToGroup(item, group);
                    group.Save();
                    
                }
                else
                {//Some VM in Group selected or single VM is selected on tree
                    model.MoveToGroup(ActiveVMList, group);
                    group.Save();
                    
                }
            }
        }

        private bool CanMoveToGroup(object item)
        {
            
            return model.CanMoveToGroup(item) || model.CanMoveToGroup(ActiveVMList);
        }

        #endregion //COMMANDS AND EXECUTE ABILITIES
       

    }
}
