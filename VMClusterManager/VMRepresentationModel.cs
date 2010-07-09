using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace VMClusterManager
{
    class VMRepresentationModel : INotifyPropertyChanged
    {
        private List<VM> vmCollection; 
        
        public List<VM> VMCollection 
        {
            get {return this.vmCollection;}
            set
            {
                this.vmCollection = value;
                this.sendPropertyChanged("VMCollection");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void sendPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
