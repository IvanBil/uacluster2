using System;
using System.Management;
using System.Windows;
using System.Threading;
namespace VMClusterManager
{
    public delegate void VMStatusChangedEventHandler(object sender, EventArgs e);
    public delegate void WMIEventHandler(object o, EventArgs e);
    public class WMIEventWatcher:IDisposable
    {
        public ManagementEventWatcher watcher;
        private Thread eventWatch;
        private ManagementScope scope;
        private WqlEventQuery query;
        public WMIEventWatcher(WqlEventQuery query,ManagementScope scope, WMIEventHandler handler)
        {
            watcher = new ManagementEventWatcher(scope,query);
            this.scope = scope;
            this.query = query;
            eventWatch = new Thread(new ParameterizedThreadStart(LookForEvent));
            eventWatch.IsBackground = true;
            eventWatch.Start(handler);
        }
 
        private void LookForEvent(object param)
        {
            try
            {
                using (ManagementEventWatcher watcher = new ManagementEventWatcher(this.scope, this.query))
                {
                    while (true)
                    {
                        ManagementBaseObject eventObj = watcher.WaitForNextEvent();
                        ManagementBaseObject sender = (ManagementBaseObject)eventObj["TargetInstance"];
                        (param as WMIEventHandler)(sender, new EventArgs());
                    }
                }
            }
            //catch (ThreadAbortException ex)
            //{

            //}
            catch (ManagementException ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
            }

        }

        ~WMIEventWatcher()
        {
            //this.Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            //watcher.Stop();
            //eventWatch.Abort();
            //eventWatch.Join(1000);
            //watcher.Dispose();
        }

        #endregion
    }
    /// <summary>
    /// Class than manages __InstanceModificationEvent WMI event of MsVM_ComputerSystem WMI class for specific VM
    /// </summary>
    public class VMModificationEventWatcher : WMIEventWatcher
    {
        public VMModificationEventWatcher(VM vm, WMIEventHandler handler):
            base(
            new WqlEventQuery(string.Format("Select * from __InstanceModificationEvent within 1 where targetinstance isa 'MsVM_ComputerSystem' and targetinstance.Name=\"{0}\"",
                   vm.GUID)),
            new ManagementScope(@"\\" + vm.Host.Name + @"\root\virtualization", Utility.GetHostConnectionOptions(vm.Host)),
            handler)

        {
        }
        ~VMModificationEventWatcher()
        {
        }
    }

    public class VMSettingsModificationEventWatcher : WMIEventWatcher
    {
        public VMSettingsModificationEventWatcher(VM vm, WMIEventHandler handler) :
            base(
            new WqlEventQuery(
                string.Format(
                "Select * from __InstanceOperationEvent within 1 where targetinstance isa 'Msvm_VirtualSystemSettingData' and targetinstance.SystemName=\"{0}\"",
                vm.GUID.ToString())),
            new ManagementScope(@"\\" + vm.Host.Name + @"\root\virtualization", vm.Host.HostConnectionOptions),
            handler)
        {
        }

        ~VMSettingsModificationEventWatcher()
        {
        }
    }
}