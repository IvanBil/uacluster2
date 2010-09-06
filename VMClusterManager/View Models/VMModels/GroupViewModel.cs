using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMClusterManager.ViewModels.VMModels
{
    public class GroupViewModel : ViewModelBase
    {
        private string name;
        private bool isActive;

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; OnPropertyChanged("IsActive"); }
        }

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        public GroupViewModel(Group grp)
        {
            Name = grp.Name;
            IsActive = false;
        }
    }
}
