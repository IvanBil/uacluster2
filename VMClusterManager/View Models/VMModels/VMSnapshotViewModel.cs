using System;
using System.Collections.ObjectModel;

namespace VMClusterManager.ViewModels.VMModels
{
    public class VMSnapshotViewModel : ViewModelBase
    {
        private VMModel model;
        private VMSnapshot snapshotInstance;


        private string elementName;
        private bool isSelected;
        private bool isLastApplied;

        public VMSnapshot SnapshotInstance
        {
            get { return snapshotInstance; }
        }

        public bool IsLastApplied
        {
            get { return isLastApplied; }
            private set { isLastApplied = value; OnPropertyChanged("IsLastApplied"); }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set 
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
                if (isSelected == true)
                {
                    model.SelectedSnapshotItem = this.snapshotInstance;
                    OnSelected();
                }
            }
        }

        public string ElementName
        {
            get { return elementName; }
            private set { elementName = value; OnPropertyChanged("ElementName"); }
        }

        private ObservableCollection<VMSnapshotViewModel> children;

        public ObservableCollection<VMSnapshotViewModel> Children
        {
            get { return children; }
        } 

        public VMSnapshotViewModel(VMSnapshot _snapshot, VM vm, VMModel _model)
            : base()
        {
            this.model = _model;
            this.snapshotInstance = _snapshot;
            this.ElementName = _snapshot.ElementName;
            this.children = new ObservableCollection<VMSnapshotViewModel>();
            foreach (VMSnapshot child in this.snapshotInstance.ChildSnapshots)
            {
                this.AddChild(new VMSnapshotViewModel(child,vm,model));
            }
            isSelected = false;
            //check if this snapshot is last applied
            VMSnapshot lastAppliedSnapshot = new VMSnapshot(Utility.GetLastAppliedVirtualSystemSnapshot(vm.Instance));
            if (lastAppliedSnapshot.ElementName == this.snapshotInstance.ElementName)
            {
                this.IsLastApplied = true;
            }
            else
            {
                this.IsLastApplied = false;
            }
        }

        public void AddChild(VMSnapshotViewModel child)
        {
            this.Children.Add(child);
        }

        public event EventHandler<EventArgs> Selected;

        private void OnSelected()
        {
            if (Selected != null)
                Selected(this, new EventArgs());
        }
    }
}
