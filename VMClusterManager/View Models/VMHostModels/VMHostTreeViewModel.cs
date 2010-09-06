using System.Collections.Generic;
using System.ComponentModel;
using VMClusterManager.Controls.VMHostViews;
using Microsoft.Practices.Composite.Wpf.Commands;
using VMClusterManager.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace VMClusterManager.ViewModels.VMHostModels
{
    public class VMHostTreeViewModel : ViewModelBase
    {
        private VMModel vmHostModel;
        public VMHostTreeView View;
        public VMHostTreeActionsView ActionsView;
        private List<VMHostGroup> vmHostGroups;

        private List<Action> CanExecuteRaisers;
        //private bool isSelected;

        //public bool IsSelected
        //{
        //    get { return isSelected; }
        //    set 
        //    { 
        //        isSelected = value;
        //        OnPropertyChanged("IsSelected");
        //    }
        //}

        private object selectedItem;

        public object SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
                RefreshCommands();
            }
        }

        public List<VMHostGroup> VMHostGroups
        {
            get { return vmHostGroups; }
        }

        //public static DependencyProperty RenameTreeNodeProperty = DependencyProperty.Register("RenameTreeNode", typeof(DelegateCommand<object>), typeof(VMHostTreeViewModel));

        private DelegateCommand<object> addHostCommand;
        private DelegateCommand<object> vmHostGroupCreate;
        private DelegateCommand<object> removeVMHostTreeNodeCommand;
        private DelegateCommand<object> renameTreeNode;
        private DelegateCommand<object> moveToGroupCommand;

        public DelegateCommand<object> MoveToGroupCommand
        {
            get { return moveToGroupCommand; }
            set { moveToGroupCommand = value; OnPropertyChanged("MoveToGroupCommand"); }
        }

        public DelegateCommand<object> RemoveVMHostTreeNodeCommand
        {
            get { return removeVMHostTreeNodeCommand; }
            set { removeVMHostTreeNodeCommand = value; OnPropertyChanged("RemoveVMHostTreeNodeCommand"); }
        }

        public DelegateCommand<object> VMHostGroupCreate
        {
            get { return vmHostGroupCreate; }
            set { vmHostGroupCreate = value; OnPropertyChanged("VMHostGroupCreate"); }
        }

        public DelegateCommand<object> RenameTreeNode
        {
            get { return renameTreeNode; }
            set { renameTreeNode = value; OnPropertyChanged("RenameTreeNode"); }
            //get
            //{
            //    return (DelegateCommand<object>)GetValue(RenameTreeNodeProperty);
            //}
            //set
            //{
            //    SetValue(RenameTreeNodeProperty, value);
            //}
        }

        public DelegateCommand<object> AddHostCommand
        {
            get { return addHostCommand; }
            set { addHostCommand = value; OnPropertyChanged("AddHostCommand"); }
        }

        public event EventHandler<EventArgs> RenameActiveTreeNodeRequested;
        private void OnRenameActiveTreeNodeRequested()
        {
            if (RenameActiveTreeNodeRequested != null)
            {
                RenameActiveTreeNodeRequested(this, new EventArgs());
            }
        }

        public VMHostTreeViewModel(VMModel model):base()
        {
            this.vmHostModel = model;
            this.View = new VMHostTreeView();
            vmHostGroups = new List<VMHostGroup>(1);
            vmHostGroups.Add(model.RootVMHostGroup);
            AddHostCommand = new DelegateCommand<object>(AddHost, CanAddHost);
            RemoveVMHostTreeNodeCommand = new DelegateCommand<object>(RemoveVMHostTreeNode, CanRemoveVMHostTreeNode);
            VMHostGroupCreate = new DelegateCommand<object>(CreateVMHostGroup, CanCreateVMHostGroup);
            RenameTreeNode = new DelegateCommand<object>(RenameNode, CanRenameNode);
            MoveToGroupCommand = new DelegateCommand<object>(MoveToGroup, CanMoveToGroup);
            CanExecuteRaisers = new List<Action> {
                AddHostCommand.RaiseCanExecuteChanged,
                RemoveVMHostTreeNodeCommand.RaiseCanExecuteChanged,
                VMHostGroupCreate.RaiseCanExecuteChanged,
                RenameTreeNode.RaiseCanExecuteChanged,
                MoveToGroupCommand.RaiseCanExecuteChanged
            };
            model.VMHostTreeChanged +=
                (o, e) =>
                {
                    OnPropertyChanged("VMHostGroups");
                };
            
            View.SetViewModel(this);
            ActionsView = new VMHostTreeActionsView(this);
        }

        private void MoveToGroup(object selectedItem)
        {
            ObservableCollection<object> tree = new ObservableCollection<object>();
            tree.Add(vmHostModel.RootVMHostGroup);
            MoveToGroupDialog dlg = new MoveToGroupDialog(tree);
            dlg.Owner = Window1.GetWindow(View.Parent);
            dlg.ShowDialog();
            if (dlg.DialogResult.HasValue && dlg.DialogResult.Value)
            {
                //only VMHostGroup selected
                VMHostGroup group = dlg.CheckedItem as VMHostGroup;
                if ((this.SelectedItem is VMHostGroup) && (vmHostModel.ActiveVMHostList.Count == 0))
                {
                    vmHostModel.MoveToGroup(this.SelectedItem as VMHostGroup, group);
                    group.Save();
                }
                else
                {//Some Host in Group selected or single Host is selected on tree

                    vmHostModel.MoveToGroup(vmHostModel.ActiveVMHostList, group);
                    vmHostModel.ActiveVMHostList.Clear();
                    group.Save();

                }
            }
        }

        private bool CanMoveToGroup(object selectedItem)
        {
            //bool canMove = false;
            //if (selectedItem
            return vmHostModel.CanMoveToGroup(this.SelectedItem) || vmHostModel.CanMoveToGroup(vmHostModel.ActiveVMHostList);
        }

        private void AddHost(object selectedItem)
        {
            AddHostDialog dlg = new AddHostDialog();
            if (dlg.ShowDialog().Value)
            {
                VMHostGroup hostParent = this.SelectedItem as VMHostGroup;
                foreach (string hostname in dlg.HostNames)
                {
                    //check if host is already present
                    if (VMHostGroup.FindParentFor(hostname, vmHostModel.RootVMHostGroup) == null)
                    {
                        hostParent.AddHost(new VMHost(hostname));
                    }
                }
                hostParent.Save();
            }
        }

        private bool CanAddHost(object selectedItem)
        {
            VMHostGroup parent = this.SelectedItem as VMHostGroup;
            return parent != null;
        }

        private void CreateVMHostGroup(object selectedItem)
        {
            Group parent = this.SelectedItem as Group;
            if (parent != null)
            {
                VMHostGroup newGroup = this.vmHostModel.CreateGroup(parent) as VMHostGroup;
                parent.Save();
                parent.IsExpanded = true;
                newGroup.IsActive = true;
                newGroup.IsInEditMode = true;
                //OnRenameActiveTreeNodeRequested();
            }
        }

        private bool CanCreateVMHostGroup(object selectedItem)
        {
            Group parent = this.SelectedItem as Group;
            bool canCreate = (parent == null) ? false : true;
            return canCreate;
        }
        private void RemoveVMHostTreeNode(object selectedItem)
        {
            try
            {
                if (this.SelectedItem is VMHostGroup)
                {
                    VMHostGroup group = this.SelectedItem as VMHostGroup;
                    vmHostModel.RemoveGroup(group);
                    group.Save();
                    return;
                }
                if (this.SelectedItem is VMHost)
                {
                    vmHostModel.RemoveHost(this.SelectedItem as VMHost);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Removing Host tree node.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanRemoveVMHostTreeNode(object selectedItem)
        {
            bool canremove = false;
            VMHostGroup group = this.SelectedItem as VMHostGroup;
            if (group != null)
            {
                if (group.ParentGroup != null)
                {
                    canremove = true;
                }
                return canremove;
            }
            VMHost host = this.SelectedItem as VMHost;
            if (host != null)
            {
                canremove = true;
            }
            return canremove;
        }

        private void RenameNode(object Node)
        {
            //this.View.RenameActiveTreeNode();
            OnRenameActiveTreeNodeRequested();
            VMHostGroup grp = this.SelectedItem as VMHostGroup;
            grp.IsInEditMode = true;
        }

        private bool CanRenameNode(object Node)
        {
            return (this.SelectedItem is Group);
        }

        private void RefreshCommands()
        {
            foreach (Action action in CanExecuteRaisers)
            {
                action();
            }

        }
    }
}
