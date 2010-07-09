using System;
using System.Management;
using System.Collections.ObjectModel;

namespace VMClusterManager
{
    /// <summary>
    /// Represents a snapshot of VM
    /// </summary>
    public class VMSnapshot
    {
        private ObservableCollection<VMSnapshot> childSnapshots;
        private string elementName;
        /// <summary>
        /// Corresponds to ElementName of WMI Class fo snapshot
        /// </summary>
        public string ElementName
        {
            get { return elementName; }
        }
        private ManagementObject snapshotInstance;

        public ManagementObject SnapshotInstance
        {
            get 
            {
                if (snapshotInstance != null)
                { snapshotInstance.Get(); }
                return snapshotInstance; 
            }
        }

        public ObservableCollection<VMSnapshot> ChildSnapshots
        {
            get { return childSnapshots; }
            set { childSnapshots = value; }
        }
        private DateTime creationTime;

        public DateTime CreationTime
        {
            get { return creationTime; }
            set { creationTime = value; }
        }

        public VMSnapshot(ManagementObject instance)
        {
            this.snapshotInstance = instance;
            this.elementName = (string)snapshotInstance["ElementName"];
            this.ChildSnapshots = new ObservableCollection<VMSnapshot>();
            creationTime = Utility.ConvertToDateTime(snapshotInstance["CreationTime"].ToString());
        }

        public void AddChild(VMSnapshot snapshot)
        {
            ///Insert child snapshot to sorted collection of children by creation time
            foreach (VMSnapshot child in this.ChildSnapshots)
            {
                if (child.CreationTime < snapshot.CreationTime)
                {//jump to the next child
                    continue;
                }
                if (child.CreationTime > snapshot.CreationTime)
                {//insert before child with larger CreationTime property value
                    this.childSnapshots.Insert(this.ChildSnapshots.IndexOf(child),snapshot);
                    return;
                }
            }
            //the snapshot has the largest CreationTime property value so append it to the end
            this.ChildSnapshots.Add(snapshot);
        }
    }
}
