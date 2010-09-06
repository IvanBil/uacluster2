using System;
using System.Collections.ObjectModel;
using System.Management;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace VMClusterManager
{
    /// <summary>
    /// A physical computer that contains virtual machines 
    /// </summary>
    public class VMHost : INotifyPropertyChanged
    {
        
        private string name;
        private  ObservableCollection<VM> vmCollection;
        private VMHostGroup group;
        private VMHostStatus status;
        private string userName;
        private string password;
        private System.Security.SecureString securePassword;
        private ConnectionOptions hostConnectionOptions;

        public ConnectionOptions HostConnectionOptions
        {
            get { return hostConnectionOptions; }
            set { hostConnectionOptions = value; }
        }

        public System.Security.SecureString SecurePassword
        {
            get { return securePassword; }
            set { securePassword = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        public VMHostStatus Status
        {
            get { return status; }
            set 
            {
                switch (status)
                {
                    case VMHostStatus.OK: StatusString = "OK"; break;
                    case VMHostStatus.ERROR: StatusString = "ERROR!"; break;
                    case VMHostStatus.REFRESHING: StatusString = "REFRESHING..."; break;
                }
                OnPropertyChanged("Status");
                status = value; 
            }
        }

        private string statusString;

        public string StatusString
        {
            get { return statusString; }
            set 
            { 
                statusString = value;
                OnPropertyChanged("StatusString");
            }
        }

        public VMHostGroup Group
        {
            get { return group; }
            set { group = value; }
        }

        private UInt32 processorsCount;
        private UInt32 coresCount;
        private UInt64 totalMemory;

        public UInt64 TotalMemory
        {
            get { return totalMemory; }
            set { totalMemory = value; }
        }

        public UInt32 CoresCount
        {
            get { return coresCount; }
            set { coresCount = value; }
        }

        public UInt32 ProcessorsCount
        {
            get { return processorsCount; }
            set { processorsCount = value; }
        }
        /// <summary>
        /// Collection of VM under this host.
        /// </summary>
        public ObservableCollection<VM> VMCollection
        {
            get 
            {
                return vmCollection; 
            }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public VMHost()
            :this("localhost")
        {
        }
        

        public VMHost(string hostname)
        {
            name = hostname;
            this.HostConnectionOptions = new ConnectionOptions();
//#if DEBUG
//            this.HostConnectionOptions.Username = "Administrator";
//            this.HostConnectionOptions.Password = "P@ssw0rd";
//#endif
            HostConnectionOptions.Authentication = AuthenticationLevel.Call;
            vmCollection = new ObservableCollection<VM> ();
            
        }

        public VMHost(string hostname, string username, string password)
            : this(hostname, new ConnectionOptions(null, username, password,
                null, ImpersonationLevel.Impersonate, AuthenticationLevel.Call, true,
                new ManagementNamedValueCollection(), new TimeSpan(0, 1, 0)))
        {
        }

        public VMHost(string hostname, ConnectionOptions options)
        {
            name = hostname;
            this.HostConnectionOptions = options;
            vmCollection = null;
        }
        /// <summary>
        /// Reinitializes VM collection of this host
        /// </summary>
        public void Connect()
        {
            Status = VMHostStatus.REFRESHING;
            try
            {
                TotalMemory = GetTotalMemory();
                ProcessorsCount = GetProcessorsCount();
                CoresCount = GetCoreCount();
            }
            catch (COMException ex)
            {
                throw new RPCCallException(this, ex);
            }
            vmCollection = new ObservableCollection<VM>(Utility.GetHostVMCollection(this));
            OnPropertyChanged("VMCollection");
            Status = VMHostStatus.OK;
        }

        public UInt32 GetProcessorsCount()
        {
            UInt32 result = 0;
            try
            {
                result = (UInt32)GetPropertyValue("Win32_ComputerSystem", "NumberOfProcessors");
            }
            catch (Exception) { }
            return result;
        }

        public UInt32 GetCoreCount()
        {
            UInt32 result = 0;
            try
            {
                result = (UInt32)GetPropertyValue("Win32_Processor", "NumberOfCores");
            }
            catch (Exception) { }
            return result;
 
        }

        public UInt16 GetProcessorLoad()
        {
            UInt16 result = 0;
            try
            {
                result = (UInt16)GetPropertyValue("Win32_Processor", "LoadPercentage");
            }
            catch (Exception) { }
            return result;
            
        }

        public UInt64 GetTotalMemory()
        {
            UInt64 totalmem = 0;
            try
            {
                totalmem = (UInt64)GetPropertyValue("Win32_ComputerSystem", "TotalPhysicalMemory");
            }
            catch (Exception) { }
            return totalmem / 1024 / 1024;
        }

        public UInt64 GetFreeMemory()
        {
            UInt64 freeMem = 0;
            try
            {
                freeMem = (UInt64)GetPropertyValue("Win32_OperatingSystem", "FreePhysicalMemory");
            }
            catch (Exception) { }
            return freeMem / 1024;
        }

        private object GetPropertyValue(string WMIClassName, string PropertyName)
        {
            object returnValue=null;
            ManagementScope scope = new ManagementScope("\\\\" + this.Name + "\\root\\CIMV2", this.HostConnectionOptions);
            scope.Connect();
            ObjectQuery query = new ObjectQuery( string.Format("SELECT * FROM {0}",WMIClassName));
            EnumerationOptions EnumOpts = new EnumerationOptions();
            //Ensure to return complete collection
            EnumOpts.ReturnImmediately = false;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query, EnumOpts);
            ManagementObjectCollection collObj = searcher.Get();
            foreach (ManagementObject queryObj in collObj)
            {
                returnValue = queryObj[PropertyName];
                break;
            }

            return returnValue;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }


        #endregion
    }
    
}
