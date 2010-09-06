using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using VMClusterManager.Controls.VMHostViews;

namespace VMClusterManager.ViewModels.VMHostModels
{
    public class VMHostListViewModel : ViewModelBase
    {
        public VMHostListView View;
        private VMModel Model;
        private ObservableCollection<VMHost> hostList;

        public ObservableCollection<VMHost> HostList
        {
            get { return hostList; }
            set { hostList = value; OnPropertyChanged("HostList"); }
        }

        public VMHostListViewModel(VMModel model, ObservableCollection<VMHost> hostList)
        {
            Model = model;
            this.HostList = hostList;
            Model.ActiveVMHostList.Clear();
            this.HostList.CollectionChanged +=
                (o, e) =>
                {
                    OnPropertyChanged("HostList");
                };
            View = new VMHostListView(this);
        }
    }
}
