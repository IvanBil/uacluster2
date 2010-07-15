using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.IO;
using VMClusterManager.Controls.Dialogs;
using System.Windows.Threading;
using System.Management;

namespace VMClusterManager
{
    public class VMModel : IVMModel
    {


        Dispatcher UIDispatcher;
        #region IVMModel Members
        private VM activeVM;

        public VM ActiveVM
        {
            get
            {
                return activeVM;
            }
            set
            {
                activeVM = value;
            }
        }

        public VMGroup RootVMGroup
        {
            get { return rootVMGroup; }
        }

        public VMGroup ActiveVMGroup
        {
            get { return activeVMGroup; }
            set
            {
                activeVMGroup = value;
                OnActiveVMGroupChanged();
            }
        }

        #endregion

        #region VMs
        private ObservableCollection<VM> activeVMList;

        private object selectedTreeItem;

        public object SelectedTreeItem
        {
            get { return selectedTreeItem; }
            set 
            { 
                selectedTreeItem = value;
                OnSelectedTreeItemChanged(selectedTreeItem);
            }
        }

        private VMGroup rootVMGroup;
        private VMGroup activeVMGroup;
        public ObservableCollection<VM> ActiveVMList
        {
            get { return activeVMList; }
            set
            {
                activeVMList = value;
            }
        }

        private object selectedVMItem;

        public object SelectedVMItem
        {
            get { return selectedVMItem; }
            set 
            { 
                selectedVMItem = value;
                OnSelectedVMListItemChanged(selectedVMItem);
            }
        }

        private object selectedSnapshotItem;

        public object SelectedSnapshotItem
        {
            get { return selectedSnapshotItem; }
            set { selectedSnapshotItem = value; OnSelectedSnapshotItemChanged(selectedSnapshotItem); }
        }


    
        #endregion

        # region HOSTS
        private VMHost activeVMHost;

        public VMHost ActiveVMHost
        {
            get
            {
                return activeVMHost;
            }
            set
            {
                activeVMHost = value;
            }
        }
        public List<VMHost> VMHostList
        {
            get { throw new NotImplementedException(); }
        }


        private List<VMHost> activeVMHostList;
        public List<VMHost> ActiveVMHostList
        {
            get
            {
                return activeVMHostList;
            }
            set
            {
                activeVMHostList = value;
                OnActiveVMHostListChanged();
            }
        }



        private VMHostGroup rootVMHostGroup;
        public VMHostGroup RootVMHostGroup
        {
            get { return rootVMHostGroup; }
        }

        private VMHostGroup activeVMHostGroup;
        public VMHostGroup ActiveVMHostGroup
        {
            get
            {
                return activeVMHostGroup;
            }
            set
            {
                activeVMHostGroup = value;
                OnActiveVMHostGroupChanged();
            }
        }

        public event EventHandler<ActiveVMHostGroupChangedEventArgs> ActiveVMHostGroupChanged;
        protected void OnActiveVMHostGroupChanged()
        {
            if (ActiveVMHostGroupChanged != null)
                ActiveVMHostGroupChanged(this, new ActiveVMHostGroupChangedEventArgs());
        }

        public event EventHandler<ActiveVMHostListChangedEventArgs> ActiveVMHostListChanged;
        protected void OnActiveVMHostListChanged()
        {
            if (ActiveVMHostListChanged != null)
            {
                ActiveVMHostListChanged(this, new ActiveVMHostListChangedEventArgs());
            }
        }

        public event EventHandler<VMHostTreeChangedEventArgs> VMHostTreeChanged;
        protected void OnVMHostTreeChanged()
        {
            if (VMHostTreeChanged != null)
                VMHostTreeChanged(rootVMHostGroup, new VMHostTreeChangedEventArgs());
        }

        #endregion

        private static VMModel inst;

        public static VMModel GetInstance()
        {
            if (inst == null)
                inst = new VMModel();
            return inst;
        }

        XElement VMTreeStructure = null;

        private VMModel()
        {
            try
            {
                UIDispatcher = Dispatcher.CurrentDispatcher;
                DataReceiver DR = new DataReceiver();
                rootVMHostGroup = new VMHostGroup("All hosts");
                VMGroup.Storage = "VMTree.xml";
                //retrieve VM Tree structure
                if (!File.Exists("VMTree.xml"))
                {
                    VMGroup temp = new VMGroup("All VM");
                    temp.SaveToXML(VMGroup.Storage.ToString());

                }
                try
                    {
                        VMTreeStructure = XElement.Load("VMTree.xml");
                        rootVMGroup = new VMGroup(VMTreeStructure);
                    }
                catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error loading VM group structure", MessageBoxButton.OK, MessageBoxImage.Error);
                        rootVMGroup = new VMGroup("All VM");
                    }
                    
               
                ////---------------------------
                VMHostGroupEventSubscriber(rootVMHostGroup);
                List<VMHost> hostList = DR.GetHostListFromFile("hostlist.txt");
                foreach (VMHost host in hostList)
                {
                    rootVMHostGroup.AddHost(host);
                }
                activeVMList = new ObservableCollection<VM>();
                ActiveVMList.CollectionChanged +=
                                       (o, e) =>
                                       {
                                           OnActiveVMListChanged(e);
                                       };
                activeVM = null;
                activeVMGroup = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "VMModel()", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Recursively walks around tree of host groups and subscribes for event
        /// </summary>
        /// <param name="group"></param>
        private void VMHostGroupEventSubscriber(VMHostGroup group)
        {
            if (group != null)
            {
                group.HostListChanged +=
                    (o, e) =>
                    {
                        if (e.IsHostAdded)
                        {
                            if (e.InvolvedHost != null)
                            {
                                Thread vmListReceiver = new Thread(new ParameterizedThreadStart(GetVMList));
                                vmListReceiver.IsBackground = true;
                                vmListReceiver.Start(e.InvolvedHost);
                            }
                        }
                        OnVMHostTreeChanged();

                    };
                group.ChildGroupsChanged +=
                    (o, e) =>
                    {
                        VMHostGroupEventSubscriber(e.InvolvedGroup);
                        OnVMHostTreeChanged();
                    };
                if (group.ChildGroups != null)
                {
                    foreach (VMHostGroup grp in group.ChildGroups)
                    {
                        VMHostGroupEventSubscriber(grp);
                    }
                }
            }
        }

        ConnectionOptions NewAuth(VMHost host)
        {
            AuthenticationDialog dlg = new AuthenticationDialog("Login to " + host.Name);
            dlg.ShowDialog();
            ConnectionOptions connOpts = null;
            if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
            {
                connOpts = new ConnectionOptions();
                connOpts.Authentication = AuthenticationLevel.Call;
                connOpts.SecurePassword = dlg.Password;
                connOpts.Username = dlg.UserName;
            }
            return connOpts;
        }

        private void GetVMList(object param)
        {
            try
            {

                if (param is VMHost)
                {
                    VMHost host = param as VMHost;
                    while (true)
                    {
                        try
                        {
                            host.Connect();
                            break;
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            ConnectionOptions opts = (ConnectionOptions)UIDispatcher.Invoke(new Func<VMHost, ConnectionOptions>(NewAuth), host);
                            if (opts != null)
                            {
                                host.HostConnectionOptions = opts;
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (RPCCallException ex)
                        {
                            MessageBox.Show(ex.Message, host.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                        }
                        catch (ManagementException ex)
                        {
                            MessageBox.Show(ex.Message, host.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                        }

                    }

                    ObservableCollection<VM> vmColl = host.VMCollection;
                    if (vmColl != null)
                    {
                        foreach (VM vm in vmColl)
                        {
                            VMGroup parentForVM = DataReceiver.FindParentForVM(vm, VMTreeStructure, RootVMGroup);
                            if (parentForVM != null)
                            {
                                parentForVM.AddVM(vm);
                            }
                            else
                            {
                                rootVMGroup.AddVM(vm);
                            }
                            OnVMHostTreeChanged();

                        }
                    }

                }
                lock (VMGroup.Storage)
                {
                    RootVMGroup.Save();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, (param as VMHost).Name);
            }
            
            
        }

        private void VMHostGroupEventSubscriber(ChildHostGroupsChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #region IVMModel Members


        public event EventHandler<VMStateChangedEventArgs> VMStateChanged;

        protected void OnVMStateChanged()
        {
            if (VMStateChanged != null)
                VMStateChanged(this, new VMStateChangedEventArgs());
        }

        /// <summary>
        /// Occurs when Active element of Tree View changes
        /// </summary>
        public event EventHandler<SelectedItemChangedEventArgs> SelectedTreeItemChanged;

        protected void OnSelectedTreeItemChanged(object item)
        {
            if (SelectedTreeItemChanged != null)
                SelectedTreeItemChanged(this, new SelectedItemChangedEventArgs(item));
        }


        public event EventHandler<NotifyCollectionChangedEventArgs> ActiveVMListChanged;

        protected void OnActiveVMListChanged(NotifyCollectionChangedEventArgs e)
        {
            if (ActiveVMListChanged != null)
                ActiveVMListChanged(this,e);
        }

    

        public event EventHandler<ActiveVMGroupChangedEventArgs> ActiveVMGroupChanged;

        protected void OnActiveVMGroupChanged()
        {
            if (ActiveVMGroupChanged != null)
                ActiveVMGroupChanged(this, new ActiveVMGroupChangedEventArgs());
        }



        public event EventHandler<SelectedItemChangedEventArgs> SelectedVMListItemChanged;

        protected void OnSelectedVMListItemChanged(object item)
        {
            if (SelectedVMListItemChanged != null)
            {
                SelectedVMListItemChanged(this, new SelectedItemChangedEventArgs(item));
            }
        }

        #endregion
        public event EventHandler<EventArgs> SelectedSnapshotItemChanged;
        private void OnSelectedSnapshotItemChanged(object selectedSnapshotItem)
        {
            if (SelectedSnapshotItemChanged != null)
                SelectedSnapshotItemChanged(this, new EventArgs());
        }

        #region API functions

        public void CreateVMGroup(VMGroup parent)
        {
            const string DefaultGroupName = "New Group";
            string newGroupName = DefaultGroupName;
            //if parent group already contains group named as DefaultGroupName then add to DefaultGroupName 1, 2,... 
            for (int i = 1; (from g in parent.ChildGroups where g.Name == newGroupName select g).Count() > 0; i++)
            {
                newGroupName = DefaultGroupName + " " + i.ToString();
            }
            parent.CreateGroup(newGroupName);
        }

        public void RemoveVMGroup(VMGroup group)
        {
            if (group != null)
            {
                if (group.Children.Count == 0)
                {
                    if (MessageBox.Show("Are you shure you want to remove this group?", group.Name,
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                    {
                        VMGroup parentGroup = group.ParentGroup as VMGroup;
                        group.Remove();
                        parentGroup.Save();
                    }
                }
                else
                {
                    MessageBox.Show("Group is not empty! You can delete only empty groups.", group.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private const int JobWaitTime = 100;

        delegate void AddLogMessageDelegate(LogMessage message);


        /// <summary>
        /// Provides an action on specific collection of VM
        /// </summary>
        /// <param name="vmList"></param>
        /// <param name="action"></param>
        private void VMListAction(ObservableCollection<VM> vmList, Func<VM, VMJob> action)
        {
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    Dispatcher UIDispatcher = Dispatcher.CurrentDispatcher;
                    Thread vmThread = new Thread(new ParameterizedThreadStart(delegate(object param)
                    {
                        try
                        {

                            VMJob ThisJob = action(param as VM);
                            while ((ThisJob.JobState == JobState.Starting) || (ThisJob.JobState == JobState.Running))
                            {
                                Thread.Sleep(JobWaitTime);
                            }

                            if (ThisJob.JobState != JobState.Completed)
                            {
                                if (vmList.Count == 1)
                                {
                                    MessageBox.Show(ThisJob.GetError(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                                else
                                {
                                    //this is done to allow this thread to change collection which is monitored by user interface main thread
                                    //otherwise an error occures
                                    UIDispatcher.Invoke(new AddLogMessageDelegate(
                                        delegate(LogMessage message)
                                        {
                                            VMLog.GetInstance().Add(message);
                                        }
                                        ), new LogMessage(LogMessageTypes.Error, ThisJob.GetError(), param as VM, ThisJob));
                                }
                            }
                        }
                        catch (InvalidOperationException ex)
                        {
                            UIDispatcher.Invoke(new AddLogMessageDelegate(
                                        delegate(LogMessage message)
                                        {
                                            VMLog.GetInstance().Add(message);
                                        }
                                        ), new LogMessage(LogMessageTypes.Warning, ex.Message, param as VM, "StateChange"));
                        }
                    }));
                    vmThread.IsBackground = true;
                    vmThread.Start(vm);
                }
            }
        }

        public void StartVMList(ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, delegate(VM vm)
            {
                return vm.Start();
            }
            );

        }

        public bool CanStartVMList(ObservableCollection<VM> vmList)
        {
            bool canStart = false;
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    canStart = (vm.Status == VMState.Disabled || vm.Status == VMState.Paused ||
                        vm.Status == VMState.Suspended);
                    if (canStart) return canStart;
                }
            }
            return canStart;
        }

        public void StopVMList(ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, delegate(VM vm)
            {
                return vm.Stop();
            }
            );

        }

        public void ShutdownVMList(ObservableCollection<VM> vmList)
        {
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    Dispatcher UIDispatcher = Dispatcher.CurrentDispatcher;
                    Thread vmThread = new Thread(new ParameterizedThreadStart(delegate(object param)
                    {
                        try
                        {

                            uint returnValue = vm.Shutdown();
                            if (returnValue != 0)
                            {
                                if (vmList.Count == 1)
                                {
                                    MessageBox.Show("Failed to shutdown selected VM" + Environment.NewLine + "Error code: " + returnValue.ToString()
                                        , "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                                else
                                {
                                    //this is done to allow this thread to change collection which is monitored by user interface main thread
                                    //otherwise an error occures
                                    UIDispatcher.Invoke(new AddLogMessageDelegate(
                                        delegate(LogMessage message)
                                        {
                                            VMLog.GetInstance().Add(message);
                                        }
                                        ), new LogMessage(LogMessageTypes.Error, "Failed to shutdown selected VM. Error code: " + returnValue.ToString(), param as VM, "Shutdown"));
                                }
                            }
                        }
                        catch (InvalidOperationException ex)
                        {
                            UIDispatcher.Invoke(new AddLogMessageDelegate(
                                        delegate(LogMessage message)
                                        {
                                            VMLog.GetInstance().Add(message);
                                        }
                                        ), new LogMessage(LogMessageTypes.Warning, ex.Message, param as VM, "Shutdown"));
                        }
                    }));
                    vmThread.IsBackground = true;
                    vmThread.Start(vm);
                }
            }
        }

        public bool CanShutdownVMList(ObservableCollection<VM> vmList)
        {
            return CanStopVMList(vmList);
        }

        public bool CanStopVMList(ObservableCollection<VM> vmList)
        {
            bool canStop = false;
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    canStop = !(vm.Status == VMState.Disabled || vm.Status == VMState.Stopping || vm.Status == VMState.Suspended);
                    if (canStop) return canStop;
                }
            }
            return canStop;
        }

        public void SuspendVMList(ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, delegate(VM vm)
            {
                return vm.Suspend();
            }
            );

        }

        public bool CanSuspendVMList(ObservableCollection<VM> vmList)
        {
            bool canStop = false;
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    canStop = (vm.Status == VMState.Enabled || vm.Status == VMState.Paused);
                    if (canStop) return canStop;
                }
            }
            return canStop;
        }

        public void PauseVMList(ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, delegate(VM vm)
            {
                return vm.Pause();
            }
            );

        }

        public bool CanPauseVMList(ObservableCollection<VM> vmList)
        {
            bool canStop = false;
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    canStop = (vm.Status == VMState.Enabled || vm.Status == VMState.Resuming);
                    if (canStop) return canStop;
                }
            }
            return canStop;
        }

        public void RebootVMList(ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, delegate(VM vm)
            {
                return vm.Reboot();
            }
            );

        }

        public bool CanRebootVMList(ObservableCollection<VM> vmList)
        {
            bool canStop = false;
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    canStop = !(vm.Status == VMState.Disabled || vm.Status == VMState.Suspended);
                    if (canStop) return canStop;
                }
            }
            return canStop;
        }


        public void CreateSnapshot(ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, delegate(VM vm)
            {
                return vm.CreateSnapshot();
            }
            );

        }

        public bool CanCreateSnapshot(ObservableCollection<VM> vmList)
        {
            bool canStop = false;
            if (vmList != null)
            {
                foreach (VM vm in vmList)
                {
                    canStop = (vm.Status == VMState.Disabled || vm.Status == VMState.Suspended || vm.Status == VMState.Enabled);
                    if (canStop) return canStop;
                }
            }
            return canStop;
        }

        public void ApplySnapshot(VMSnapshot snapshot, ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, (vm) =>
            {
                return vm.ApplySnapshot(snapshot);
            });
        }

        public bool CanApplySnapshot(VMSnapshot snapshot, ObservableCollection<VM> vmList)
        {
            bool canApply = false;
            if (vmList != null)
            {
                if ((vmList.Count == 1) && snapshot != null)
                {
                    if (vmList[0].Status == VMState.Disabled || vmList[0].Status == VMState.Suspended)
                    {
                        canApply = true;
                    }
                }
            }
            return canApply;
        }

        public void RemoveSnapshot(VMSnapshot snapshot, ObservableCollection<VM> vmList)
        {
            VMListAction(vmList, (vm) =>
            {
                return vm.RemoveSnapshot(snapshot);
            });
            
        }

        public bool CanRemoveSnapshot(VMSnapshot snapshot, ObservableCollection<VM> vmList)
        {
            bool canRemove = false;
            if (vmList != null)
            {
                if ((vmList.Count == 1) && snapshot != null)
                {
                    canRemove = true;
                }
            }
            return canRemove;
        }

        public void RemoveSnapshotTree(VMSnapshot snapshot, ObservableCollection<VM> vmList)
        {
            
                VMListAction(vmList, (vm) =>
                {
                    return vm.RemoveSnapshotTree(snapshot);
                });
        
        }

        public bool CanRemoveSnapshotTree(VMSnapshot snapshot, ObservableCollection<VM> vmList)
        {
            bool canRemove = false;
            if (vmList != null)
            {
                if ((vmList.Count == 1) && snapshot != null)
                {
                    canRemove = true;
                }
            }
            return canRemove;
        }



        public void MoveToGroup(object item, VMGroup group)
        {
            lock (item)
            {
                if (item is VMGroup)
                {
                    (item as VMGroup).MoveTo(group);
                }
                if (item is VM)
                {
                    VM vm = item as VM;
                    VMGroup parentGroup = VMGroup.FindParentFor(vm, this.RootVMGroup);
                    if (parentGroup != group)
                    {
                        lock (group)
                        {
                            group.AddVM(vm);
                        }
                        lock (parentGroup)
                        {
                            parentGroup.RemoveVM(vm);
                        }
                    }
                }
            }
        }

        public void MoveToGroup(ObservableCollection<VM> vmList, VMGroup group)
        {
            lock (vmList)
            {
                ObservableCollection<VM> VMToMove = new ObservableCollection<VM>(vmList);
                foreach (VM vm in VMToMove)
                {
                    MoveToGroup(vm, group);
                }
            }
        }

        public bool CanMoveToGroup(object item)
        {
            bool canMove = false;
            if (item != null)
            {
                if (item is VMGroup)
                {
                    if ((item as VMGroup).ParentGroup != null)
                    {
                        canMove = true;
                    }
                                    }
                if (item is VM)
                {
                    canMove = true;
                }
            }
            return canMove;
        }

        public bool CanMoveToGroup(ObservableCollection<VM> vmList)
        {
            bool canMove = false;
            if (vmList != null)
            {
                if (vmList.Count > 0)
                {
                    canMove = true;
                }
            }
            return canMove;
        }

        #endregion //API functions

    }

}
