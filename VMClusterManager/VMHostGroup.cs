using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;
using System.Linq;


namespace VMClusterManager
{
    public class VMHostGroup : Group
    {
        public static object Storage;

        //private ObservableCollection<Object> children;

        public ObservableCollection<Object> Children
        {
            get
            {
                //ObservableCollection<Object> coll = new ObservableCollection<object>(ChildGroups.Cast<object>());
                ObservableCollection<Object> coll = new ObservableCollection<object>();
                lock (ChildGroups)
                {
                    foreach (VMHostGroup grp in ChildGroups)
                    {
                        coll.Add(grp);
                    }
                }
                lock (HostList)
                {
                    foreach (VMHost host in HostList)
                    {
                        coll.Add(host);
                    }
                }

                return coll;
            }
        }
        //private string name;

        //public string Name
        //{
        //    get { return name; }
        //    set 
        //    { 
        //        name = value;
        //        OnPropertyChanged("Name");
        //    }
        //}

        private bool isInEditMode;

        public bool IsInEditMode
        {
            get { return isInEditMode; }
            set { isInEditMode = value; OnPropertyChanged("IsInEditMode"); }
        }

        private bool isActive;

        public bool IsActive
        {
            get { return isActive; }
            set 
            { 
                isActive = value;
                VMModel.GetInstance().ActiveVMHostGroup = this;
                OnPropertyChanged("IsActive"); 
            }
        }

        private ObservableCollection<VMHost> hostList;
        public ObservableCollection<VMHost> HostList
        {
            get { return hostList; }
        }

        //private VMHostGroup parentGroup;

        //public VMHostGroup ParentGroup
        //{
        //    get { return parentGroup; }
        //}

        //private ObservableCollection<VMHostGroup> childGroups;
        //public ObservableCollection<VMHostGroup> ChildGroups
        //{
        //    get { return childGroups; }
        //}

        //private bool isRoot;

        //public bool IsRoot
        //{
        //    get { return isRoot; }
        //    set { isRoot = value; }
        //}

        public VMHostGroup(string name)
            : this(name, null)
        {
        }

        public VMHostGroup(string name, Group parent)
            : this(new XElement("Group", new XAttribute("Name", name), new XAttribute("GUID", Guid.NewGuid().ToString())), parent)
        {
            //this.name = name;
            //this.parentGroup = null;
            //this.childGroups = new ObservableCollection<VMHostGroup>();
            //this.IsRoot = true;
            //hostList = new ObservableCollection<VMHost>();
        }

        public VMHostGroup(XElement xgroup)
            : this(xgroup, null)
        {
            //this.IsRoot = true;
           
        }

        public VMHostGroup(XElement xgroup, Group parent)
            : base(xgroup, parent)
        {
            var childgroupQuery = from g in xgroup.Elements() where g.Name == "Group" select g;
            this.IsExpanded = true;
            foreach (XElement nextXChild in childgroupQuery)
            {
                this.ChildGroups.Add(new VMHostGroup(nextXChild, this));
            }
            hostList = new ObservableCollection<VMHost>();
            hostList.CollectionChanged +=
                (o, e) =>
                {
                    OnPropertyChanged("Children");
                };
            this.ChildGroups.CollectionChanged +=
                (o, e) =>
                {
                    OnPropertyChanged("Children");
                };
        }

        public void AddHost(VMHost host)
        {
            hostList.Add(host);
            host.Group = this;
            OnHostListChanged(host,true);
        }

        public void RemoveHost(VMHost host)
        {
            HostList.Remove(host);
            if (this.DataObject is XElement)
            {
                XElement tree = this.DataObject as XElement;
                var CheckVMHostPresenceQuery = from el in tree.Elements() where el.Attribute("Name").Value == host.Name select el;
                foreach (XElement XcurHost in CheckVMHostPresenceQuery)
                {
                    XcurHost.Remove();
                }
            }
            OnPropertyChanged("HostList");
            //OnPropertyChanged("Children");
        }

        public override Group CreateGroup(string name)
        {
            VMHostGroup newGroup = null;
            if (this.DataObject is XElement)
            {
                newGroup = new VMHostGroup(name);
                this.AddGroup(newGroup);
            }
            return newGroup;
        }


        protected XElement GetXElement(VMHostGroup curGroup)
        {
            XElement elem = new XElement("Group", new XAttribute("Name", curGroup.Name), new XAttribute("GUID", curGroup.GUID));
            //defend collection from modifying
            ObservableCollection<VMHost> _hostList;
            ObservableCollection<Group> _childGroups;
            lock (curGroup.HostList)
            {
                _hostList = new ObservableCollection<VMHost>(curGroup.HostList);
            }
            lock (curGroup.ChildGroups)
            {
                _childGroups = new ObservableCollection<Group>(curGroup.ChildGroups);
            }
            if (_hostList != null)
            {
                foreach (VMHost host in _hostList)
                {
                    elem.Add(new XElement("Host", new XAttribute("Name", host.Name)));
                }
            }
            if (_childGroups != null)
            {
                foreach (Group childGroup in _childGroups)
                {
                    elem.Add(GetXElement(childGroup as VMHostGroup));
                }
            }
            return elem;
        }
        /// <summary>
        /// Returns parent group for VM Host
        /// </summary>
        /// <param name="vm">VMHost a parent is searching for</param>
        /// <param name="root">Root VM group</param>
        /// <returns>Parent VMHostGroup if it exists of null otherwise</returns>
        public static VMHostGroup FindParentFor(VMHost host, VMHostGroup root)
        {
            return FindParentFor(host.Name, root);
            //VMHostGroup parent = null;
            //if (root.HostList.Contains(host))
            //{
            //    parent = root;
            //}
            //else
            //{
            //    foreach (VMHostGroup group in root.ChildGroups)
            //    {
            //        parent = FindParentFor(host, group);
            //        if (parent != null)
            //        {
            //            //parent = group;
            //            break;
            //        }
            //    }
            //}
            //return parent;
        }

        public static VMHostGroup FindParentFor(string hostName, VMHostGroup root)
        {
            VMHostGroup parent = null;
            if (root.HostList.Any((h)=>h.Name==hostName))
            {
                parent = root;
            }
            else
            {
                foreach (VMHostGroup group in root.ChildGroups)
                {
                    parent = FindParentFor(hostName, group);
                    if (parent != null)
                    {
                        //parent = group;
                        break;
                    }
                }
            }
            return parent;
        }

        public void SaveToXML(string FilePath)
        {
            //find root element
            VMHostGroup root = this;
            while (root.ParentGroup != null)
            {
                root = root.ParentGroup as VMHostGroup;
            }
            StreamWriter SW = File.CreateText(FilePath);
            SW.Write(root.GetXElement(root).ToString());
            SW.Close();
        }

        public override void Save()
        {
            if (Storage != null)
            {
                if (Storage is string)
                {
                    SaveToXML(Storage as string);
                }
            }
        }

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
