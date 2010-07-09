using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace VMClusterManager
{
    public interface IVMModel
    {
        VM ActiveVM { get; set; }
        ObservableCollection<VM> ActiveVMList { get; set; }
        VMGroup RootVMGroup { get; }
        VMGroup ActiveVMGroup { get; set; }
        object SelectedTreeItem { get; set; }
        object SelectedSnapshotItem { get; set; }
        object SelectedVMItem { get; set; }
        VMHost ActiveVMHost { get; set; }
        List<VMHost> ActiveVMHostList { get; set; }
        List<VMHost> VMHostList { get; }
        VMHostGroup RootVMHostGroup { get; }
        VMHostGroup ActiveVMHostGroup { get; set; }
        event EventHandler<SelectedItemChangedEventArgs> SelectedTreeItemChanged;
        event EventHandler<ActiveVMHostListChangedEventArgs> ActiveVMHostListChanged;
        event EventHandler<ActiveVMHostGroupChangedEventArgs> ActiveVMHostGroupChanged;
        event EventHandler<EventArgs> SelectedSnapshotItemChanged;
        event EventHandler<VMStateChangedEventArgs> VMStateChanged;
        event EventHandler<NotifyCollectionChangedEventArgs> ActiveVMListChanged;
        event EventHandler<ActiveVMGroupChangedEventArgs> ActiveVMGroupChanged;
        event EventHandler<VMHostTreeChangedEventArgs> VMHostTreeChanged;
        event EventHandler<SelectedItemChangedEventArgs> SelectedVMListItemChanged;
    }

    public class VMStateChangedEventArgs:EventArgs
    {

    }

    public class SelectedItemChangedEventArgs : EventArgs
    {
        object item;

        public object Item
        {
            get { return item; }
        }

        public SelectedItemChangedEventArgs(object _item)
        {
            this.item = _item;
        }
    }

    public class VMListChangedEventArgs 
    {
       
    }

    public class ActiveVMGroupChangedEventArgs : EventArgs
    {
    }
    public class VMHostStateChangedEventArgs : EventArgs
    {

    }


    public class VMHostListChangedEventArgs : EventArgs
    {
        private VMHost involvedHost;

        public VMHost InvolvedHost
        {
            get { return involvedHost; }
        }

        private bool isHostAdded;

        public bool IsHostAdded
        {
            get { return isHostAdded; }
            set { isHostAdded = value; }
        }

        public VMHostListChangedEventArgs(VMHost host, bool hostAdded)
        {
            this.involvedHost = host;
            this.isHostAdded = hostAdded;
        }

        public VMHostListChangedEventArgs(VMHost host)
            : this(host, true)
        {
        }
    }

    public class ActiveVMHostGroupChangedEventArgs : EventArgs
    {
    }

    public class ChildHostGroupsChangedEventArgs : EventArgs
    {
        private VMHostGroup involvedGroup;

        public VMHostGroup InvolvedGroup
        {
            get { return involvedGroup; }
        }

        private bool isGroupAdded;

        public bool IsGroupAdded
        {
            get { return isGroupAdded; }
            set { isGroupAdded = value; }
        }

        public ChildHostGroupsChangedEventArgs(VMHostGroup group, bool added)
        {
            this.involvedGroup = group;
            this.isGroupAdded = added;
        }
    }
    public class ActiveVMHostListChangedEventArgs : EventArgs
    {
    }

    public class VMHostTreeChangedEventArgs : EventArgs
    {

    }
}
