using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMClusterManager.Controls.VMViews;
using System.Collections.ObjectModel;
using System.Management;
using System.Windows;
using VMClusterManager.ViewModels.ResourceViewModels;

namespace VMClusterManager.ViewModels.VMModels
{
    public class VMPropertiesViewModel : ViewModelBase
    {
        public VMPropertiesView View;

        private ObservableCollection<ResourceViewModel> rasd;

        public ObservableCollection<ResourceViewModel> RASD
        {
            get { return rasd; }
            set { rasd = value; OnPropertyChanged("RASD"); }
        }

        

        /// <summary>
        /// Occurs when OK or Cancel button is clicked in order to close Properties View
        /// </summary>
        public event EventHandler<EventArgs> ViewCloseRequested;

        protected void OnViewCloseRequested()
        {
            if (ViewCloseRequested != null)
            {
                ViewCloseRequested(this, new EventArgs());
            }
        }

        public VMPropertiesViewModel(ICollection<VM> _VMList)
        {

            RASD = new ObservableCollection<ResourceViewModel>();
            try
            {
                RASD.Add(new ProcessorViewModel(_VMList));
                //MemoryViewModel memVM = new MemoryViewModel(_VMList);
                RASD.Add(new MemoryViewModel(_VMList));
                foreach (ResourceViewModel resVM in RASD)
                {
                    resVM.ViewCloseRequested +=
                        (o, e) =>
                        {
                            this.OnViewCloseRequested();
                        };
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Properties", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            //try
            //{
               
            //    foreach (ManagementObject res in Utility.GetRASD(_VMList.First()))
            //    {
            //        RASD.Add(res.GetText(TextFormat.Mof));
            //    }
            //}
            //catch (ManagementException ex)
            //{
            //    MessageBox.Show(ex.Message, "Properties", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            View = new VMPropertiesView();
            View.SetViewModel(this);
        }

        private ManagementObject GetMemory(VM vm)
        {
            ManagementObject mem = null;
            try
            {
                ManagementObject vmObj = vm.Instance;
                ManagementObjectCollection settings = vmObj.GetRelated("Msvm_VirtualSystemsettingData");
                foreach (ManagementObject setting in settings)
                {
                    ManagementObjectCollection memSettings = setting.GetRelated("Msvm_MemorySettingData");
                    foreach (ManagementObject memsetting in memSettings)
                    {
                        mem = memsetting;
                    }
                }
                
            }
            catch (ManagementException ex)
            {

                MessageBox.Show(ex.Message, "Properties", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return mem;
        }

        private ManagementObject GetProcessor(VM vm)
        {
            ManagementObject proc = null;
            try
            {
                ManagementObject vmObj = vm.Instance;
                ManagementObjectCollection settings = vmObj.GetRelated("Msvm_VirtualSystemsettingData");
                foreach (ManagementObject setting in settings)
                {
                    ManagementObjectCollection memSettings = setting.GetRelated("Msvm_ProcessorSettingData");
                    foreach (ManagementObject memsetting in memSettings)
                    {
                        proc = memsetting;
                    }
                }

            }
            catch (ManagementException ex)
            {

                MessageBox.Show(ex.Message, "Properties", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return proc;
        }
    }
}
