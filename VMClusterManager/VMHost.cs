using System;
using System.Collections.ObjectModel;
using System.Management;
using System.ComponentModel;

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
#if DEBUG
            this.HostConnectionOptions.Username = "Administrator";
            this.HostConnectionOptions.Password = "P@ssw0rd";
#endif
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
            vmCollection = new ObservableCollection<VM>(Utility.GetHostVMCollection(this));
            OnPropertyChanged("VMCollection");
            Status = VMHostStatus.OK;
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
