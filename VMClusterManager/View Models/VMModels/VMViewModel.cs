using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows;

namespace VMClusterManager.ViewModels.VMModels
{
    public class VMViewModel : ViewModelBase
    {
        private bool isActiveVM = false;

        public bool IsActiveVM
        {
            get { return isActiveVM; }
            set 
            {
                bool isActivePrevious = isActiveVM;
                isActiveVM = value;
                if (isActivePrevious && (!isActiveVM))
                {
                    OnDeactivated();
                }
                else
                {
                    OnActivated();
                }
                OnPropertyChanged("IsActiveVM");  
            }
        }
        private int statusJobPercent;

        public int StatusJobPercent
        {
            get { return statusJobPercent; }
            private set { statusJobPercent = value; OnPropertyChanged("StatusJobPercent"); }
        }

        #region VM fields
        private string vmName;
        private string hostName;
        private TimeSpan upTime;
        private string statusString;
        private DateTime creationTime;
        private string healthState;
        private string memoryUsage;
        private UInt16 numberOfProcessors;
        private string processorLoad;
        #endregion //VM fields
        #region VM Properties
        public string ProcessorLoad
        {
            get { return processorLoad; }
            private set 
            {
                if (virtualMachine.Status != VMState.Disabled && virtualMachine.Status != VMState.Paused && virtualMachine.Status != VMState.Suspended)
                {
                    processorLoad = value + '%';
                }
                else
                {
                    processorLoad = "";
                }
                OnPropertyChanged("ProcessorLoad"); 
            }
        }

        public UInt16 NumberOfProcessors
        {
            get { return numberOfProcessors; }
            private set { numberOfProcessors = value; OnPropertyChanged("NumberOfProcessors"); }
        }

        public string MemoryUsage
        {
            get { return memoryUsage; }
            private set { memoryUsage = (value == "")?(value):value + " MB"; OnPropertyChanged("MemoryUsage"); }
        }

        public string HealthState
        {
            get { return healthState; }
            private set { healthState = value; OnPropertyChanged("HealthState"); }
        }

        public DateTime CreationTime
        {
            get { return creationTime; }
        }

        public string StatusString
        {
            get { return statusString; }
            private set { statusString = value; OnPropertyChanged("StatusString"); }
        }

        public TimeSpan UpTime
        {
            get { return upTime; }
            private set { upTime = value; OnPropertyChanged("UpTime"); }
        }
        private VM virtualMachine;

        public VM VirtualMachine
        {
            get { return virtualMachine; }
        }

        public string HostName
        {
            get { return hostName; }
            private set { hostName = value; OnPropertyChanged("HostName"); }
        }

        public string VMName
        {
            get { return vmName; }
            set { vmName = value; OnPropertyChanged("VMName"); }
        }
        #endregion //VM Properties
        private Thread BackgroundThread;
        public VMViewModel(VM vm)
            : base()
        {
            this.virtualMachine = vm;
            this.VMName = virtualMachine.Name;
            this.HostName = virtualMachine.Host.Name;
            this.StatusString = virtualMachine.GetStatusString();
            this.creationTime = virtualMachine.CreationTime;
            UInt64 memoryUsage = virtualMachine.MemoryUsage;
            this.MemoryUsage = (memoryUsage == 0) ? "" : memoryUsage.ToString();
            ThreadStart backgroundThreadStart = new ThreadStart(delegate()
            {
                while (true)
                {
                     UpTime = vm.UpTime;
                     ProcessorLoad = (Utility.GetVMProcessorLoad(this.VirtualMachine)).ToString();
                    Thread.Sleep(1000);
                }
            });
            if ((vm.Status != VMState.Unknown) &&
                        (vm.Status != VMState.Disabled) &&
                        (vm.Status != VMState.Paused) &&
                        (vm.Status != VMState.Suspended))
            {
                BackgroundThread = StartNewThread(backgroundThreadStart);
            }
            virtualMachine.VMStatusChanged +=
                (o, e) =>
                {
                    this.StatusString = virtualMachine.GetStatusString();
                    if ((vm.Status != VMState.Unknown) &&
                        (vm.Status != VMState.Disabled) &&
                        (vm.Status != VMState.Paused) &&
                        (vm.Status != VMState.Suspended))
                    {
                        if (BackgroundThread != null)
                        {
                            if (!BackgroundThread.IsAlive)
                            {
                                BackgroundThread = StartNewThread(backgroundThreadStart);
                            }
                        }
                        else
                        {
                            BackgroundThread = StartNewThread(backgroundThreadStart);
                        }
                    }
                    else
                    {
                        if (BackgroundThread != null)
                        {
                            BackgroundThread.Abort();
                            BackgroundThread.Join(10);
                        }
                    }
                };
            virtualMachine.MemoryUsageChanged +=
                (o, e) =>
                {
                    try
                    {
                        memoryUsage = virtualMachine.MemoryUsage;
                        this.MemoryUsage = (memoryUsage == 0) ? "" : memoryUsage.ToString();
                    }
                    catch (COMException ex)
                    {
                        MessageBox.Show(ex.Message, virtualMachine.Host.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                };
        }

        private Thread StartNewThread(ThreadStart ts)
        {
            Thread BackgroundThread = new Thread(ts);
            BackgroundThread.IsBackground = true;
            BackgroundThread.Priority = ThreadPriority.Lowest;
            BackgroundThread.Start();
            return BackgroundThread;
        }

        public event EventHandler<EventArgs> Activated;
        public event EventHandler<EventArgs> Deactivated;

        private void OnActivated()
        {
            if (Activated != null)
                Activated(this, new EventArgs());
        }

        private void OnDeactivated()
        {
            if (Deactivated != null)
                Deactivated(this, null);
        }

        ~VMViewModel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (BackgroundThread != null)
            {
                BackgroundThread.Abort();
            }
        }
    }
}
