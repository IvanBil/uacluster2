using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMClusterManager.Controls;
using System.Windows.Controls;

namespace VMClusterManager.ViewModels
{
    public class NavigationViewModel : ViewModelBase
    {
        private VMModel model;
        private NavigationControl view;
        private Control treeCommandsView;
        public Control TreeCommandsView
        {
            get { return treeCommandsView; }
            set { treeCommandsView = value; OnPropertyChanged("TreeCommandsView"); }
        }
        public NavigationControl View
        {
            get { return view; }
            set { view = value; }
        }

        public NavigationViewModel(VMModel model)
            : base()
        {
            this.model = model;
            View = new NavigationControl();
            View.SetViewModel(this);

            //TreeCommandsView = View.
        }
    }
}
