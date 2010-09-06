using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Windows.Controls;
using Microsoft.Practices.Composite.Wpf.Commands;

namespace VMClusterManager.ViewModels.ResourceViewModels
{
    public abstract class ResourceViewModel : ViewModelBase
    {
        protected List<VM> VMList;

        private string name;

        private Control view;

        public Control View
        {
            get { return view; }
            set { view = value; OnPropertyChanged("View"); }
        }

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        private DelegateCommand<object> applyCommand;

        private DelegateCommand<object> okCommand;

        private DelegateCommand<object> cancelCommand;

        public DelegateCommand<object> CancelCommand
        {
            get { return cancelCommand; }
            set { cancelCommand = value; OnPropertyChanged("CancelCommand"); }
        }

        public DelegateCommand<object> OKCommand
        {
            get { return okCommand; }
            set { okCommand = value; OnPropertyChanged("OKCommand"); }
        }

        public DelegateCommand<object> ApplyCommand
        {
            get { return applyCommand; }
            set { applyCommand = value; OnPropertyChanged("ApplyCommand"); }
        }

        /// <summary>
        /// Occurs when OK or Cancel button is clicked in order to close Properties View
        /// </summary>
        public event EventHandler<EventArgs> ViewCloseRequested;

        protected void OnViewCloseRequested()
        {
            if (ViewCloseRequested != null)
            {
                ViewCloseRequested(this, new EventArgs());
            }
        }


        public ResourceViewModel(string ResourceName, IEnumerable<VM> ActiveVMList)
            : base()
        {
            //Instance = resourceInstance;
            Name = ResourceName;
            this.VMList = new List<VM>(ActiveVMList);
        }
    }
}
