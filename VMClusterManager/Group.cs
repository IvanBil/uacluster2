using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using VMClusterManager.ViewModels;

namespace VMClusterManager
{
    public class Group : ViewModelBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                if (DataObject is XElement)
                {
                    (this.DataObject as XElement).SetAttributeValue("Name", name);
                }
                //this.Save();
                OnPropertyChanged("Name");
            }
        }

        private bool isExpanded;

        public bool IsExpanded
        {
            get { return isExpanded; }
            set { isExpanded = value; OnPropertyChanged("IsExpanded"); }
        }

        private Guid guid;
        public Guid GUID
        {
            get { return guid; }
        }

        private Group parentGroup;

        public Group ParentGroup
        {
            get { return parentGroup; }
            private set { parentGroup = value; }
        }

        private object dataObject;
        /// <summary>
        /// Gets object that contains data and structure for specific group. 
        /// (e.g. XElement that contains group structure from this group). Now only XElement is supported.
        /// </summary>
        public object DataObject
        {
            get { return dataObject; }
            protected set { dataObject = value; }
        }

        private ObservableCollection<Group> childGroups;

        public ObservableCollection<Group> ChildGroups
        {
            get { return childGroups; }
            set { childGroups = value; }
        }

        public Group(string name, Guid _guid, Group parent) : base()
        {
            this.name = name;
            this.parentGroup = parent;
            this.guid = _guid;
            this.childGroups = new ObservableCollection<Group>();
        }

        public Group(XElement xGroup, Group parent):this(GetName(xGroup),GetGUID(xGroup),parent)
        {
            this.DataObject = xGroup;
        }

        protected static string GetName(XElement xGroup)
        {
            if (xGroup != null)
            {
                if ((xGroup.Name != "Group")||(xGroup.Attribute("Name") == null))
                {
                    throw new System.Xml.XmlException("XML data is corrupt!");
                }
            }
            return xGroup.Attribute("Name").Value;
        }

        protected static Guid GetGUID(XElement xGroup)
        {
            if (xGroup != null)
            {
                if ((xGroup.Name != "Group") || (xGroup.Attribute("GUID") == null))
                {
                    throw new System.Xml.XmlException("XML data is corrupt!");
                }
            }
            return (Guid)xGroup.Attribute("GUID");
        }

        public void MoveTo(Group parent)
        {
            if (this != parent)
            {
                this.Remove();
                parent.AddGroup(this);
            }
        }

        protected void AddGroup(Group _group)
        {
            _group.ParentGroup = this;
            if (!this.ChildGroups.Contains(_group))
            {
                this.ChildGroups.Add(_group);
                if (this.DataObject is XElement)
                {
                    XElement tree = this.DataObject as XElement;
                    var CheckGroupPresenceQuery = from el in tree.Elements("Group") where el.Attribute("GUID").Value == _group.GUID.ToString() select el;
                    if (CheckGroupPresenceQuery.Count() == 0)
                    {
                        //Add new group to this XML tree
                        tree.Add(_group.DataObject as XElement);
                    }

                }
            }
        }
        /// <summary>
        /// Removes this group from it's parent
        /// </summary>
        public void Remove()
        {
            if (this.ParentGroup != null)
            {
                this.ParentGroup.ChildGroups.Remove(this);
                if (this.ParentGroup.DataObject is XElement)
                {
                    //Make changes to XML tree structure
                    (this.DataObject as XElement).Remove();
                }
            }
        }

        public virtual Group CreateGroup(string name)
        {
            throw new NotImplementedException();

        }

        public virtual void Save()
        {
            
        }

        public static Group GetElementByID(string _GUID, Group root)
        {
            Group result = null;
            if (root.GUID.ToString() == _GUID)
            {
                result = root;
            }
            else
            {
                if (root.ChildGroups != null)
                {
                    foreach (Group child in root.ChildGroups)
                    {
                        result = GetElementByID(_GUID, child);
                        if (result != null)
                        {
                            break;
                        }
                    }
                }
            }
            return result;
        }

        protected XElement GetXElement(Group curGroup)
        {
            XElement elem = new XElement("Group", new XAttribute("Name", curGroup.Name), new XAttribute("GUID", curGroup.GUID));
            //defend collection from modifying
            ObservableCollection<Group> _childGroups;    
            lock (curGroup.ChildGroups)
            {
                _childGroups = new ObservableCollection<Group>(curGroup.ChildGroups);
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
       
        //#region INotifyPropertyChanged Members

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected void OnPropertyChanged(string property)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(property));
        //    }
        //}

        //#endregion
    }
}
