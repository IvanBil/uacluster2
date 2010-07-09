using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.IO;

namespace VMClusterManager
{ 
    public class VMGroup : Group
    {

        public static object Storage;
        

        private ObservableCollection<Object> children;

        public ObservableCollection<Object> Children
        {
            get
            {
                ObservableCollection<Object> coll = new ObservableCollection<object>();
                foreach (VMGroup grp in ChildGroups)
                {
                    coll.Add(grp);
                }
                foreach (VM vm in VMList)
                {
                    coll.Add(vm);
                }

               
                return coll;
            }
        }

        private ObservableCollection<VM> vmList;
  
        public ObservableCollection<VM> VMList
        {
            get { return vmList; }
        }


        private bool isRoot;

        public bool IsRoot
        {
            get { return isRoot; }
            set { isRoot = value; }
        }

        public VMGroup(string name):
            this(name,Guid.NewGuid())
        {

        }

        public VMGroup(XElement xVMGroup)
            : this(xVMGroup, null)
        {
        }

        public VMGroup(XElement xVMGroup, VMGroup parent)
            : base(xVMGroup,parent)
        {
            var childgroupQuery = from g in xVMGroup.Elements() where g.Name == "Group" select g;
            foreach (XElement nextXChild in childgroupQuery )
            {
                this.ChildGroups.Add(new VMGroup(nextXChild, this));
            }
            vmList = new ObservableCollection<VM>();
            vmList.CollectionChanged +=
                (o, e) =>
                {
                    OnPropertyChanged("Children");
                    OnVMListChanged(e);
                };
            ChildGroups.CollectionChanged +=
                (o, e) =>
                {
                    OnPropertyChanged("Children");
                };
        }

        public VMGroup(string name, Guid _guid):this(name, _guid, null)
        {
        }

        public VMGroup(string name, Guid _guid, VMGroup parent):base(name,_guid,parent)
        {
            vmList = new ObservableCollection<VM>();
            vmList.CollectionChanged +=
                (o, e) =>
                {
                    OnPropertyChanged("Children");
                    OnVMListChanged(e);
                };
            ChildGroups.CollectionChanged +=
                (o, e) =>
                {
                    OnPropertyChanged("Children");
                };
        }
       
        public void AddVM(VM vm)
        {
            vmList.Add(vm);
            if (this.DataObject is XElement)
            {
                XElement tree = this.DataObject as XElement;
                var CheckVMPresenceQuery = from el in tree.Elements() where el.Attribute("GUID").Value == vm.GUID.ToString() select el;
                if (CheckVMPresenceQuery.Count() == 0)
                {
                    tree.Add(new XElement("VM", new XAttribute("GUID", vm.GUID)));
                }
            }
            OnPropertyChanged("VMList");
        }

        public void RemoveVM(VM vm)
        {
            VMList.Remove(vm);
            if (this.DataObject is XElement)
            {
                XElement tree = this.DataObject as XElement;
                var CheckVMPresenceQuery = from el in tree.Elements() where el.Attribute("GUID").Value == vm.GUID.ToString() select el;
                foreach (XElement XcurVM in CheckVMPresenceQuery)
                {
                    XcurVM.Remove();
                }
            }
            OnPropertyChanged("VMList");
        }

        public override void CreateGroup(string name)
        {
            if (this.DataObject is XElement)
            {
                this.AddGroup(new VMGroup(new XElement("Group", new XAttribute("Name", name), new XAttribute("GUID", Guid.NewGuid())), this));
            }
        }


        protected override XElement GetXElement(VMGroup curGroup)
        {
            XElement elem = new XElement("Group", new XAttribute("Name", curGroup.Name), new XAttribute("GUID", curGroup.GUID));
            //defend collection from modifying
            ObservableCollection<VM> _vmList;
            ObservableCollection<Group> _childGroups;
            lock (curGroup.VMList)
            {
                _vmList = new ObservableCollection<VM>(curGroup.VMList);
            }
            lock (curGroup.ChildGroups)
            {
                _childGroups = new ObservableCollection<Group>(curGroup.ChildGroups);
            }
            if (_vmList != null)
            {
                foreach (VM vm in _vmList)
                {
                    elem.Add(new XElement("VM", new XAttribute("GUID", vm.GUID)));
                }
            }
            if (_childGroups != null)
            {
                foreach (Group childGroup in _childGroups)
                {
                    elem.Add(GetXElement(childGroup as VMGroup));
                }
            }
            return elem;
        }
        /// <summary>
        /// Returns parent group for VM
        /// </summary>
        /// <param name="vm">VM a parent is searching for</param>
        /// <param name="root">Root VM group</param>
        /// <returns>Parent VMGroup if it exists of null otherwise</returns>
        public static VMGroup FindParentFor(VM vm, VMGroup root)
        {
            VMGroup parent = null;
            if (root.VMList.Contains(vm))
            {
                parent = root;
            }
            else
            {
                foreach (VMGroup group in root.ChildGroups)
                {
                    if (FindParentFor(vm, group) != null)
                    {
                        parent = group;
                        break;
                    }
                }
            }
            return parent;
        }

        public void SaveToXML(string FilePath)
        {
            //find root element
            Group root = this;
            while (root.ParentGroup != null)
            {
                root = root.ParentGroup;
            }
            if (root.DataObject is XElement)
            {
                StreamWriter SW = File.CreateText(FilePath);
                SW.Write(root.DataObject.ToString());
                SW.Close();
            }
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
     
        public event EventHandler<NotifyCollectionChangedEventArgs> VMListChanged;
        protected void OnVMListChanged(NotifyCollectionChangedEventArgs e)
        {
            if (VMListChanged != null)
                VMListChanged(this, e);
        }

    }
}
