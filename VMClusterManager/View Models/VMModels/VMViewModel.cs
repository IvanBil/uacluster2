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
                    VMModel.GetInstance().ActiveVMList.Remove(this.VirtualMachine);
                }
                else
                {
                    if (!VMModel.GetInstance().ActiveVMList.Contains(this.VirtualMachine))
                    {
                        VMModel.GetInstance().ActiveVMList.Add(this.VirtualMachine);
                    }
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
        private Timer BackgroundThread;

        //private bool IsDisposed = false;
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
            BackgroundThread = new Timer((o) =>
                {
                    UpTime = vm.UpTime;
                    try
                    {
                        ProcessorLoad = (Utility.GetVMProcessorLoad(this.VirtualMachine)).ToString();
                    }
                    catch (Exception ex) { }
                }, null, Timeout.Infinite, 1000);
            //ThreadStart backgroundThreadStart = new ThreadStart(delegate()
            //{
            //    while (!this.IsDisposed)
            //    {
            //        UpTime = vm.UpTime;
            //        ProcessorLoad = (Utility.GetVMProcessorLoad(this.VirtualMachine)).ToString();
            //        Thread.Sleep(1000);
            //    }
                
            //});
            if ((vm.Status != VMState.Unknown) &&
                        (vm.Status != VMState.Disabled) &&
                        (vm.Status != VMState.Paused) &&
                        (vm.Status != VMState.Suspended))
            {
                StartTimer(BackgroundThread, 0, 1000);
                //BackgroundThread = StartNewThread(backgroundThreadStart);
            }
            virtualMachine.VMStatusChanged += new VMStatusChangedEventHandler(VMStatusChangedHandler);

            virtualMachine.MemoryUsageChanged += new EventHandler<EventArgs>(VMMemoryChangedHandler);
               
        }

        private void VMStatusChangedHandler(object sender, EventArgs args)
        {
            this.StatusString = virtualMachine.GetStatusString();
            if ((virtualMachine.Status != VMState.Unknown) &&
                (virtualMachine.Status != VMState.Disabled) &&
                (virtualMachine.Status != VMState.Paused) &&
                (virtualMachine.Status != VMState.Suspended))
            {
                StartTimer(BackgroundThread, 0, 1000);
            }
            else
            {
                StopTimer(BackgroundThread);
            }
        }

        private void VMMemoryChangedHandler(object sender, EventArgs args)
        {
            try
            {
                UInt64 memoryUsage = virtualMachine.MemoryUsage;
                this.MemoryUsage = (memoryUsage == 0) ? "" : memoryUsage.ToString();
            }
            catch (COMException ex)
            {
                MessageBox.Show(ex.Message, virtualMachine.Host.Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Thread StartNewThread(ThreadStart ts)
        {
            Thread BackgroundThread = new Thread(ts);
            BackgroundThread.IsBackground = false;
            BackgroundThread.Priority = ThreadPriority.Lowest;
            BackgroundThread.Start();
            return BackgroundThread;
        }

        private void StartTimer(Timer t, int dueTime, int period)
        {
            t.Change(dueTime, period);
        }

        private void StopTimer(Timer t)
        {
            t.Change(Timeout.Infinite, Timeout.Infinite);
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
            Dispose(false);
        }

       

        protected override void Dispose(bool disposing)
        {
            //if (BackgroundThread != null)
            //{
            //    BackgroundThread.Abort();
            //}
            //IsDisposed = true;
            if (!IsDisposed)
            {
                if (disposing)
                {
                    //base.Dispose(disposing);
                    VirtualMachine.VMStatusChanged -= new VMStatusChangedEventHandler(VMStatusChangedHandler);
                    VirtualMachine.MemoryUsageChanged -= new EventHandler<EventArgs>(VMMemoryChangedHandler);
                    
                    try
                    {
                        StopTimer(BackgroundThread);
                        BackgroundThread.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
            IsDisposed = true;
            
        }
    }
}
