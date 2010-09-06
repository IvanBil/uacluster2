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
        private DelegateCommand<object> vmGroupCreate;
        private DelegateCommand<object> vmGroupRemove;
        private DelegateCommand<object> renameTreeNode;

        public DelegateCommand<object> VMGroupRemove
        {
            get { return vmGroupRemove; }
            set { vmGroupRemove = value; OnPropertyChanged("VMGroupRemove"); }
        }

        public DelegateCommand<object> VMGroupCreate
        {
            get { return vmGroupCreate; }
            set { vmGroupCreate = value; OnPropertyChanged("VMGroupCreate"); }
        }

        public DelegateCommand<object> RenameTreeNode
        {
            get { return renameTreeNode; }
            set { renameTreeNode = value; OnPropertyChanged("RenameTreeNode"); }
        }
        private VMGroup activeVMGroup;

        public VMGroup ActiveVMGroup
        {
            get { return activeVMGroup; }
            set
            {
                activeVMGroup = value;
                OnPropertyChanged("ActiveVMGroup");
                //this.vmModel.ActiveVMGroup = activeVMGroup;
            }
        }

        private object selectedItem;

        public object SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                //vmModel.SelectedTreeItem = selectedItem;
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

        public event EventHandler<EventArgs> RenameActiveTreeNodeRequested;
        private void OnRenameActiveTreeNodeRequested()
        {
            if (RenameActiveTreeNodeRequested != null)
            {
                RenameActiveTreeNodeRequested(this, new EventArgs());
            }
        }

        public VMTreeViewModel(VMModel vmModel)
        {
            this.vmModel = vmModel;
            this.View = new VMTreeView();
            vmGroups = new ObservableCollection<VMGroup>();
            vmGroups.Add(vmModel.RootVMGroup);
            VMGroupCreate = new DelegateCommand<object>(CreateVMGroup, CanCreateVMGroup);
            VMGroupRemove = new DelegateCommand<object>(RemoveVMGroup, CanRemoveVMGroup);
            RenameTreeNode = new DelegateCommand<object>(RenameNode, CanRenameNode);
            CanExecuteRaisers = new List<Action>{
                vmGroupCreate.RaiseCanExecuteChanged,
                vmGroupRemove.RaiseCanExecuteChanged,
                renameTreeNode.RaiseCanExecuteChanged
            };
            this.vmModel.ActiveVMGroupChanged +=
                (o, e) =>
                {
                    this.ActiveVMGroup = this.vmModel.ActiveVMGroup;
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


        private void CreateVMGroup(object selectedItem)
        {
            VMGroup parent = selectedItem as VMGroup;
            if (parent != null)
            {
                VMGroup newGroup = vmModel.CreateGroup(parent) as VMGroup;
                parent.Save();
                parent.IsExpanded = true;
                newGroup.IsActive = true;
                newGroup.IsInEditMode = true;
            }
        }

        private bool CanCreateVMGroup(object selectedItem)
        {
            Group parent = selectedItem as Group;
            bool canCreate = (parent == null) ? false : true;
            return canCreate;
        }
        private void RemoveVMGroup(object selectedItem)
        {
            VMGroup group = this.SelectedItem as VMGroup;
            vmModel.RemoveGroup(group);
            group.Save();
        }

        private bool CanRemoveVMGroup(object selectedItem)
        {
            bool canremove = false;
            Group group = this.SelectedItem as Group;
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
            //this.View.RenameActiveTreeNode();
            OnRenameActiveTreeNodeRequested();
            VMGroup grp = this.SelectedItem as VMGroup;
            grp.IsInEditMode = true;
        }

        private bool CanRenameNode(object Node)
        {
            return (this.SelectedItem as VMGroup != null);
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
