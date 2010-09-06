using System;
using System.Diagnostics;
using System.Management;
using System.Collections.Generic;

namespace VMClusterManager
{
    /// <summary>
    /// Defines a class for representing a virtual machine
    /// </summary>
    public class VM
    {

        private string name;
        private string dnsName = string.Empty;

        private string DnsName
        {
            get { return dnsName; }
            set { dnsName = value; }
        }

        private GuestOS guestOSParams = null;

        private GuestOS GuestOSParams
        {
            get { return guestOSParams; }
            set { guestOSParams = value; }
        }

        public string StatusString
        {
            get { return GetStatusString(); }
        }

        private string description;

        private VMHost host;
        private UInt16 status;
        /// <summary>
        /// A class that allows registration for WMI event of VM Status modification
        /// </summary>
        private VMModificationEventWatcher stateModificationWatcher;
        private VMSettingsModificationEventWatcher settingsModificationWatcher;
        private string guid;
        private TimeSpan upTime;
        private ManagementObject instance;
        private VMJob lastJob;

        public VMJob LastJob
        {
            get { return lastJob; }
            private set { lastJob = value; }
        }
        /// <summary>
        /// The most current version of Management Object for this VM
        /// </summary>
        public ManagementObject Instance
        {
            get 
            {
                instance.Get();
                return instance;
            }
            private set { instance = value; }
        }

        public TimeSpan UpTime
        {
            get 
            {
                long ticks = Convert.ToInt64((UInt64)Instance["OnTimeInMilliseconds"]) * 10000;//Convert.ToDateTime((UInt64)vmObj["OnTimeInMilliseconds"] / 100;
                TimeSpan ts = new TimeSpan(ticks);
                upTime = new TimeSpan(ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
                return upTime; 
            }
            private set { upTime = value; }
        }

        public string GUID
        {
            get { return guid; }
        }

        public UInt16 Status
        {
            get { return status; }
            //set { state = value; }
        }
        #region Msvm_SummaryInformation members
        private DateTime creationTime;
        private string guestOperatingSystem;
        private UInt16 healthState;
        private UInt64 memoryUsage;
        private UInt16 numberOfProcessors;
        private UInt16[] operationalStatus;
        private UInt16 processorLoad;
        private UInt16[] processorLoadHistory;
        private VMSnapshot snapshotTree;
        private DateTime timeOfLastConfigurationChange;
        private DateTime timeOfLastStateChange;

        public DateTime TimeOfLastStateChange
        {
            get {return Utility.ConvertToDateTime(Instance["TimeOfLastStateChange"].ToString()); }
            private set { timeOfLastStateChange = value; }
        }

        public DateTime TimeOfLastConfigurationChange
        {
            get { return Utility.ConvertToDateTime(Instance["TimeOfLastConfigurationChange"].ToString()); }
            private set { timeOfLastConfigurationChange = value; }
        }

        public VMSnapshot SnapshotTree
        {
            get 
            {
                return snapshotTree; 
            }
            private set { snapshotTree = value; OnVMSnapshotsChanged(); }
        }
        
        //private byte[] smallthumbnailImage;
        /// <summary>
        /// Small Thumbnail Image (80x60)
        /// </summary>
        public byte[] SmallThumbnailImage
        {
            get 
            {
                ManagementBaseObject ThumbImage = Utility.GetSummaryInformation(this, new uint[] { VMRequestedInformation.SmallThumbnailImage });
                byte[] smallthumbnailImage = (byte[])ThumbImage["ThumbnailImage"];
                return smallthumbnailImage; 
            }
            //set { thumbnailImage = value; }
        }
        /// <summary>
        /// MediumThumbnailImage (160x120)
        /// </summary>
        public byte[] MediumThumbnailImage
        {
            get
            {
                ManagementBaseObject ThumbImage = Utility.GetSummaryInformation(this, new uint[] { VMRequestedInformation.MediumThumbnailImage });
                byte[] mediumthumbnailImage = (byte[])ThumbImage["ThumbnailImage"];
                return mediumthumbnailImage; 
            }
        }
        /// <summary>
        /// LargeThumbnailImage (320x240)
        /// </summary>
        public byte[] LargeThumbnailImage
        {
            get
            {
                ManagementBaseObject ThumbImage = Utility.GetSummaryInformation(this, new uint[] { VMRequestedInformation.MediumThumbnailImage });
                byte[] largethumbnailImage = (byte[])ThumbImage["ThumbnailImage"];
                return largethumbnailImage; 
            }
        }
        public UInt16[] ProcessorLoadHistory
        {
            get { return processorLoadHistory; }
            //set { processorLoadHistory = value; }
        }

        public UInt16 ProcessorLoad
        {
            get {return processorLoad; }
            private set { processorLoad = value;}
        }

        public UInt16[] OperationalStatus
        {
            get 
            { 
                return (UInt16 [])Instance["OperationalStatus"]; 
            }
            //set { operationalStatus = value; }
        }

        public UInt16 NumberOfProcessors
        {
            get 
            {
                numberOfProcessors = 0;
                try
                {
                    ManagementBaseObject procNumObj = Utility.GetSummaryInformation(this, new uint[] { VMRequestedInformation.NumberOfProcessors });
                    numberOfProcessors = (UInt16)procNumObj["NumberOfProcessors"];

                }
                finally
                {
                    
                }
                return numberOfProcessors;
            }
            //set { numberOfProcessors = value; }
        }

        public UInt64 MemoryUsage
        {
            get 
            {
                memoryUsage = 0;
                if (this.Status == VMState.Enabled )
                {
                    try
                    {
                        ManagementBaseObject memUsageObj = Utility.GetSummaryInformation(this, new uint[] { VMRequestedInformation.MemoryUsage });
                        memoryUsage = (UInt64)memUsageObj["MemoryUsage"];
                    }
                    finally
                    {
                    }
                }
                return memoryUsage; 
            }
            private set { memoryUsage = value; OnMemoryUsageChanged(); }
        }
        public UInt16 HealthState
        {
            get { return healthState; }
            //set { healthState = value; }
        }
        public string GuestOperatingSystem
        {
            get { return guestOperatingSystem; }
            //set { guestOperatingSystem = value; }
        }
        public DateTime CreationTime
        {
            get { return creationTime; }
            //set { creationTime = value; }
        }
        #endregion
        /// <summary>
        /// Host that contains this VM
        /// </summary>
        public VMHost Host
        {
            get { return host; }
            //set { host = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public VM(VMHost hst, ManagementObject manObj)
        {
            host = hst;
            this.instance = manObj;            
            this.Name = manObj["ElementName"].ToString();
            //this.DnsName = Utility.GetFullyQualifiedDomainName(this);
            this.guid = manObj["Name"].ToString();
            this.status = (UInt16)manObj["EnabledState"];
            //this.TimeOfLastConfigurationChange = Utility.ConvertToDateTime(manObj["TimeOfLastConfigurationChange"].ToString());
            this.TimeOfLastStateChange = Utility.ConvertToDateTime(manObj["TimeOfLastStateChange"].ToString());
            this.description = manObj["Description"].ToString();
            ManagementBaseObject summaryInfo = Utility.GetSummaryInformation(this,new uint[] {
                VMRequestedInformation.CreationTime,
            });
            this.creationTime = Utility.ConvertToDateTime(summaryInfo["CreationTime"].ToString());
            SnapshotTree = Utility.GetVMSnapshotTree(this);
            //Register to WMI event of Status changing
            stateModificationWatcher = new VMModificationEventWatcher(this, OnVMStateChange);
            settingsModificationWatcher = new VMSettingsModificationEventWatcher(this, OnVMSettingsChange);
        }

        public GuestOS GetGuestOS()
        {
            GuestOS os = null;
            if (this.Status == VMState.Enabled)
            {
                if (GuestOSParams == null)
                {
                    IDictionary<string, string> kvps = null;
                    try
                    {
                        kvps = Utility.GetKvpItems(this);
                    }
                    catch (Exception ex) { };
                    if (kvps != null)
                    {
                        string OSName = null;
                        string OSVer = null;
                        string csdver = null;
                        string ip4Addr = string.Empty;
                        if (kvps.ContainsKey("OSName"))
                        { OSName = kvps["OSName"]; }
                        if (kvps.ContainsKey("OSVersion"))
                            OSVer = kvps["OSVersion"];
                        if (kvps.ContainsKey("CSDVersion"))
                            csdver = kvps["CSDVersion"];
                        if (kvps.ContainsKey("NetworkAddressIPv4"))
                            ip4Addr = kvps["NetworkAddressIPv4"] ?? string.Empty;
                        if ((OSName != null) && (OSVer != null) && (csdver != null))
                        {
                            os = new GuestOS(OSName, OSVer, csdver, ip4Addr);
                        }
                    }
                }
            }
            return os;
        }

        public string GetDomainName()
        {
            //string fqdn = String.Empty; 
            string dn = string.Empty;
            //GetFullyQualifiedDomainName works only for running VM with running windows
            if (this.Status == VMState.Enabled)
            {
                if (DnsName == string.Empty)
                {
                    DnsName = Utility.GetFullyQualifiedDomainName(this) ?? String.Empty;
                }
                if (DnsName.Contains("."))
                {
                    dn = DnsName.Remove(DnsName.IndexOf("."));
                }
                else
                {
                    dn = DnsName;
                }
            }
            return dn;
        }
  
        #region Events
        private event VMStatusChangedEventHandler vmStatusChanged;
        /// <summary>
        /// Occurs when amount of memory occupied by the VM is changed
        /// </summary>
        public event EventHandler<EventArgs> MemoryUsageChanged;
        /// <summary>
        /// Occurs when some changes are made to this VM Snapshots structure
        /// </summary>
        public event EventHandler<EventArgs> VMSnapshotsChanged;
        /// <summary>
        /// Occurs when status of VM is changed
        /// </summary>
        public event VMStatusChangedEventHandler VMStatusChanged
        {
            add{vmStatusChanged += value;}
            remove{vmStatusChanged -= value;}
        }
        #endregion //Events
        private void OnVMSnapshotsChanged()
        {
            if (VMSnapshotsChanged != null)
                VMSnapshotsChanged(this, new EventArgs());
        }
        /// <summary>
        /// Event handler of __InstanceModificationEvent WMI event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVMStateChange(object sender, EventArgs e)
        {
            //Check if changes are status changes
            DateTime lastTime = TimeOfLastStateChange;
            if (lastTime > this.timeOfLastStateChange)
            {
                status = Utility.GetVMState(this);
                OnMemoryUsageChanged();
                this.timeOfLastStateChange = lastTime;
                if (vmStatusChanged != null)
                {
                    vmStatusChanged(this, new EventArgs());
                }
            }
            
        }

        private void OnVMSettingsChange(object sender, EventArgs args)
        {
            SnapshotTree = Utility.GetVMSnapshotTree(this);
        }

        private void OnMemoryUsageChanged()
        {
            if (MemoryUsageChanged != null)
                MemoryUsageChanged(this, new EventArgs());
        }

        public string GetStatusString()
        {
            switch (status)
            {
                case 0: return "Unknown";
                case 2: return "Running";
                case 3: return "Off";
                case 32768: return "Paused";
                case 32769: return "Saved";
                case 32770: return "Starting";
                case 32771: return "Snapshotting";
                case 32773: return "Saving";
                case 32774: return "Stopping";
                case 32776: return "Pausing";
                case 32777: return "Resuming";
                default: return "Unknown Status";
            }
        }
        #region VMActions
        public VMJob Start()
        {
            this.LastJob = Utility.RequestStateChange(this, VMRequestedState.Enabled);
            return this.LastJob; 
        }

        public VMJob Stop()
        {
            this.LastJob = Utility.RequestStateChange(this, VMRequestedState.Disabled);
            return this.LastJob;
        }

        public uint Shutdown()
        {
            return Utility.ShutdownVM(this);
        }

        public VMJob Suspend()
        {
            return Utility.RequestStateChange(this, VMRequestedState.Suspended);
        }

        public VMJob Pause()
        {
            return Utility.RequestStateChange(this, VMRequestedState.Paused);
        }

        public VMJob Reboot()
        {
            return Utility.RequestStateChange(this, VMRequestedState.Reboot);
        }

        public void ConnectTo()
        {
            Process vmConnection = new Process();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Hyper-V\vmconnect.exe";
            ProcessStartInfo vmConnectionInfo = new ProcessStartInfo(path, this.Host.Name + " -G " + this.GUID);
            Process.Start(vmConnectionInfo);

        }

        public VMJob CreateSnapshot()
        {
            return Utility.CreateVirtualSystemSnapshot(this);
        }

        public VMJob ApplySnapshot(VMSnapshot snapshot)
        {
            return Utility.ApplyVirtualSystemSnapshot(this, snapshot);
        }

        public VMJob RemoveSnapshot(VMSnapshot snapshot)
        {
            return Utility.RemoveVirtualSystemSnapshot(this, snapshot);
        }

        public VMJob RemoveSnapshotTree(VMSnapshot snapshot)
        {
            return Utility.RemoveVirtualSystemSnapshotTree(this, snapshot);
        }
        #endregion //VMActions

        #region VM resources
        
        #endregion

        public override string ToString()
        {
            return this.Name;
        }
    }
}
