using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMClusterManager
{
    public class VMGroup
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private List<VM> vmList;

        public List<VM> VMList
        {
            get { return vmList; }
        }

        private VMGroup parentGroup;

        public VMGroup ParentGroup
        {
            get { return parentGroup; }
            //set { parentGroup = value; }
        }

        private VMGroup childGroup;

        public VMGroup ChildGroup
        {
            get { return childGroup; }
            //set { childGroup = value; }
        }

        private bool isRoot;

        public bool IsRoot
        {
            get { return isRoot; }
            //set { isRoot = value; }
        }

    }
}
