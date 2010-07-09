using System.Collections.Generic;
using System.ComponentModel;
using VMClusterManager.Controls.VMHostViews;

namespace VMClusterManager.ViewModels.VMHostModels
{
    public class VMHostTreeViewModel : INotifyPropertyChanged
    {
        private IVMModel vmHostModel;
        public VMHostTreeView View;
        private List<VMHostGroup> vmHostGroups;

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set 
            { 
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        private object selectedItem;

        public object SelectedItem
        {
            get { return selectedItem; }
            set 
            { 
                selectedItem = value; 
                
            }
        }

        public List<VMHostGroup> VMHostGroups
        {
            get { return vmHostGroups; }
        }

        public VMHostTreeViewModel(IVMModel model)
        {
            this.vmHostModel = model;
            this.View = new VMHostTreeView();
            vmHostGroups = new List<VMHostGroup>(1);
            vmHostGroups.Add(model.RootVMHostGroup);
            model.VMHostTreeChanged +=
                (o, e) =>
                {
                    OnPropertyChanged("VMHostGroups");
                };
            
            View.SetViewModel(this);
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
    }
}
