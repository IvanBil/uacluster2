using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Composite.Wpf.Commands;
using System.Net;

namespace VMClusterManager.ViewModels.DialogModels
{
    public class AddHostDialogViewModel : ViewModelBase
    {
        private bool isSingleHost;
        private string hostName;
        private System.Net.IPAddress startIP;
        private IPAddress endIP;
        private bool okAllowed;
        private bool validationError;

        private List<string> hostList;

        public List<string> HostList
        {
            get { return hostList; }
            set { hostList = value; }
        }

        public bool ValidationError
        {
            get { return validationError; }
            set 
            { 
                validationError = value; 
                //OnPropertyChanged("ValidationError");
                OKAllowed = !value;
            }
        }

        private bool OKAllowed
        {
            get { return okAllowed; }
            set { okAllowed = value; OKCommand.RaiseCanExecuteChanged(); }
        }

        public IPAddress EndIP
        {
            get { return endIP; }
            set 
            { 
                endIP = value; 
                OnPropertyChanged("EndIP");

                if ((this.EndIP != null) && (this.StartIP != null))
                {
                    OKAllowed = true;
                }
                
            }
        }


        public System.Net.IPAddress StartIP
        {
            get { return startIP; }
            set 
            { 
                startIP = value; 
                OnPropertyChanged("StartIP");
                
                if ((this.EndIP != null) && (this.StartIP != null))
                {
                    OKAllowed = true;
                }
                //OKCommand.RaiseCanExecuteChanged();
            }
        }

        public string HostName
        {
            get { return hostName; }
            set 
            { 
                hostName = value; 
                OnPropertyChanged("HostName");
                if (hostName != "")
                {
                    OKAllowed = true;
                }
                else
                {
                    OKAllowed = false;
                }
                //OKCommand.RaiseCanExecuteChanged(); 
            }
        }
        public bool IsSingleHost
        {
            get { return isSingleHost; }
            set 
            { 
                isSingleHost = value; 
                OnPropertyChanged("IsSingleHost");
                //if (value)
                //{
                //    if (HostName != "")
                //    {
                //        OKAllowed = true;
                //    }
                //    else
                //    {
                //        OKAllowed = false;
                //    }
                //}
            }
        }
        private DelegateCommand<List<string>> okCommand;

        public DelegateCommand<List<string>> OKCommand
        {
            get { return okCommand; }
            set { okCommand = value; OnPropertyChanged("OKCommand"); }
        }

        public event EventHandler<EventArgs> OKPerformed;

        private void OnOKPerformed()
        {
            if (OKPerformed != null)
            {
                OKPerformed(this, new EventArgs());
            }
        }

        public AddHostDialogViewModel(VMModel model)
            : base()
        {
            OKCommand = new DelegateCommand<List<string>>(OK, CanOK);
            IsSingleHost = true;
            HostName = "";
            OKAllowed = false;
        }

        private void OK(List<string> hosts)
        {
            HostList = new List<string>();
            if (IsSingleHost)
            {
                HostList.Add(HostName);
            }
            else
            {
                UInt32 ipCounter = IPtoLong(StartIP);
                while (ipCounter <= IPtoLong(EndIP))
                {
                    string addrstr = UIntToIP(ipCounter).ToString();
                    HostList.Add(addrstr);
                    ipCounter++;


                }
            }
            OnOKPerformed();
            
        }

        private bool CanOK(List<string> hosts)
        {
            //bool retVal = false;
            //if (hosts != null)
            //{
            //    if (hosts.Count > 0)
            //    {
            //        retVal = true;
            //    }
            //}
            return OKAllowed;
        }

        private uint IPtoLong(IPAddress IP)
        {
            byte[] addrbytes = IP.GetAddressBytes();
            //byte[] addrlongbytes = new byte[8];
            //int j = 0;
            for (int i = 0; i <= addrbytes.Length / 2; i++)
            {
                byte b = addrbytes[i];
                addrbytes[i] = addrbytes[addrbytes.Length - 1 - i];
                addrbytes[addrbytes.Length - 1 - i] = b;
            }
            return BitConverter.ToUInt32(addrbytes,0);
        }

        private IPAddress UIntToIP(UInt32 ipInt)
        {
            byte[] addrbytes = BitConverter.GetBytes(ipInt);
            for (int i = 0; i <= addrbytes.Length / 2; i++)
            {
                byte b = addrbytes[i];
                addrbytes[i] = addrbytes[addrbytes.Length - 1 - i];
                addrbytes[addrbytes.Length - 1 - i] = b;
            }
            return new IPAddress(BitConverter.ToInt32(addrbytes,0));
        }
    }
}
