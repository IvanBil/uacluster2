using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;

namespace VMClusterManager.ViewModels.VMHostModels
{
    public class VMHostViewModel : ViewModelBase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        private UInt64 totalMemory;

        public UInt64 TotalMemory
        {
            get { return totalMemory; }
            set { totalMemory = value; OnPropertyChanged("TotalMemory"); }
        }

        private UInt64 freeMemory;

        public UInt64 FreeMemory
        {
            get { return freeMemory; }
            set { freeMemory = value; OnPropertyChanged("FreeMemory"); }
        }

        private UInt32 processorsCount;

        public UInt32 ProcessorsCount
        {
            get { return processorsCount; }
            set { processorsCount = value; OnPropertyChanged("ProcessorsCount"); }
        }

        private UInt32 coresCount;

        public UInt32 CoresCount
        {
            get { return coresCount; }
            set { coresCount = value; OnPropertyChanged("CoresCount"); }
        }

        private string cpuUsage;

        public string CpuUsage
        {
            get { return cpuUsage; }
            set { cpuUsage = value; OnPropertyChanged("CpuUsage"); }
        }

        private VMHost Host;

        private Timer backThread;

        private bool isActive;

        public bool IsActive
        {
            get { return isActive; }
            set 
            { 
                isActive = value; 
                OnPropertyChanged("IsActive");
                if (IsActive)
                {
                    VMModel.GetInstance().ActiveVMHostList.Add(this.Host);
                }
                else
                {
                    if (VMModel.GetInstance().ActiveVMHostList.Contains(this.Host))
                    {
                        VMModel.GetInstance().ActiveVMHostList.Remove(this.Host);
                    }
                }
            }
        }

        public VMHostViewModel(VMHost host)
            : base()
        {
            this.Host = host;
            this.Name = Host.Name;
            try
            {
                ProcessorsCount = Host.ProcessorsCount;
                CoresCount = Host.CoresCount;
                TotalMemory = Host.TotalMemory;
                backThread = new Timer(new TimerCallback(delegate(object o)
                    {
                        try
                        {
                            this.CpuUsage = Host.GetProcessorLoad().ToString() + "%";
                            this.FreeMemory = Host.GetFreeMemory();
                        }
                        finally
                        {
                        }
                    }
                    ),null, 0, 1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
