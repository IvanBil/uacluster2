using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace VMClusterManager
{
    public class VMHostGroup : INotifyPropertyChanged
    {
        private string name;

        public string Name
        {
            get { return name; }
            set 
            { 
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private List<VMHost> hostList;
        public List<VMHost> HostList
        {
            get { return hostList; }
        }

        private VMHostGroup parentGroup;

        public VMHostGroup ParentGroup
        {
            get { return parentGroup; }
        }

        private List<VMHostGroup> childGroups;
        public List<VMHostGroup> ChildGroups
        {
            get { return childGroups; }
        }

        private bool isRoot;

        public bool IsRoot
        {
            get { return isRoot; }
            set { isRoot = value; }
        }

        public VMHostGroup(string name)
        {
            this.name = name;
            this.parentGroup = null;
            this.childGroups = new List<VMHostGroup> ();
            this.IsRoot = true;
            hostList = new List<VMHost> ();
        }

        public void AddHost(VMHost host)
        {
            hostList.Add(host);
            host.Group = this;
            OnHostListChanged(host,true);
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

        public event EventHandler<VMHostListChangedEventArgs> HostListChanged;
        protected void OnHostListChanged(VMHost host, bool added)
        {
            if (HostListChanged != null)
                HostListChanged(this, new VMHostListChangedEventArgs(host,added));
            OnPropertyChanged("HostList");

        }

        public event EventHandler<ChildHostGroupsChangedEventArgs> ChildGroupsChanged;
        protected void OnChildGroupsChanged(VMHostGroup group, bool added)
        {
            if (ChildGroupsChanged != null)
                ChildGroupsChanged(this, new ChildHostGroupsChangedEventArgs(group,added));
            OnPropertyChanged("ChildGroups");
        }
    }
}
