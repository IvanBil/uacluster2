using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Practices.Composite.Wpf.Commands;
using VMClusterManager.Controls;
using System.Threading;

namespace VMClusterManager
{
    public class VMTreeViewModel : INotifyPropertyChanged
    {
        private List<Action> CanExecuteRaisers;  

        private ObservableCollection<VMGroup> vmGroups;
        private DelegateCommand<VMGroup> vmGroupCreate;
        private DelegateCommand<VMGroup> vmGroupRemove;
        private DelegateCommand<object> renameTreeNode;

        public DelegateCommand<VMGroup> VMGroupRemove
        {
            get { return vmGroupRemove; }
            set { vmGroupRemove = value; OnPropertyChanged("VMGroupRemove"); }
        }

        public DelegateCommand<VMGroup> VMGroupCreate
        {
            get { return vmGroupCreate; }
            set { vmGroupCreate = value; OnPropertyChanged("VMGroupCreate"); }
        }

        public DelegateCommand<object> RenameTreeNode
        {
            get { return renameTreeNode; }
            set { renameTreeNode = value; }
        }
        private VMGroup activeVMGroup;

        public VMGroup ActiveVMGroup
        {
            get { return activeVMGroup; }
            set 
            { 
                activeVMGroup = value;
                OnPropertyChanged("ActiveVMGroup");
                this.vmModel.ActiveVMGroup = activeVMGroup;
            }
        }

        private object selectedItem;

        public object SelectedItem
        {
            get { return selectedItem; }
            set 
            { 
                selectedItem = value;
                vmModel.SelectedTreeItem = selectedItem;
                OnPropertyChanged("SelectedItem");
                RefreshCommands();
            }
        }

        private VMModel vmModel;

        public VMTreeView View;
        public TreeCommandsView GroupCommandsView;

        public ObservableCollection<VMGroup> VMGroups
        {
            get { return vmGroups; }
        }

        public VMTreeViewModel(VMModel vmModel)
        {
            this.vmModel = vmModel;
            this.View = new VMTreeView();
            vmGroups = new ObservableCollection<VMGroup>();
            vmGroups.Add(vmModel.RootVMGroup);
            VMGroupCreate = new DelegateCommand<VMGroup>(CreateVMGroup, CanCreateVMGroup);
            VMGroupRemove = new DelegateCommand<VMGroup>(RemoveVMGroup, CanRemoveVMGroup);
            RenameTreeNode = new DelegateCommand<object>(RenameNode, CanRenameNode);
            CanExecuteRaisers = new List<Action>{
                vmGroupCreate.RaiseCanExecuteChanged,
                vmGroupRemove.RaiseCanExecuteChanged,
                renameTreeNode.RaiseCanExecuteChanged
            };
            View.SetViewModel(this);
            this.GroupCommandsView = new TreeCommandsView();
            this.GroupCommandsView.SetViewModel(this);
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


        private void CreateVMGroup(VMGroup parent)
        {
            vmModel.CreateVMGroup(parent);
            parent.Save();
        }

        private bool CanCreateVMGroup(VMGroup parent)
        {
            bool canCreate = (parent == null) ? false : true;
            return canCreate;
        }
        private void RemoveVMGroup(VMGroup group)
        {
            vmModel.RemoveVMGroup(group);
            group.Save();
        }

        private bool CanRemoveVMGroup(VMGroup group)
        {
            bool canremove = false;
            if (group != null)
            {
                if (group.ParentGroup != null)
                {
                    canremove = true;
                }
            }
            return canremove;
        }

        private void RenameNode(object Node)
        {
            this.View.RenameActiveTreeNode();
        }

        private bool CanRenameNode(object Node)
        {
            return (Node != null);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
