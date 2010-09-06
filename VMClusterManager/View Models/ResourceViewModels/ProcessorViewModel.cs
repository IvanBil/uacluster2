using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using VMClusterManager.Controls.ResourceViews;
using System.Windows;
using Microsoft.Practices.Composite.Wpf.Commands;

namespace VMClusterManager.ViewModels.ResourceViewModels
{
    public class ProcessorViewModel : ResourceViewModel
    {
        private UInt64 virtualQuantity;

        private uint maxProcessorCount;

        public uint MaxProcessorCount
        {
            get { return maxProcessorCount; }
            set { maxProcessorCount = value; OnPropertyChanged("MaxProcessorCount"); }
        }

        private uint[] virtualQuantityCollection;

        public uint[] VirtualQuantityCollection
        {
            get { return virtualQuantityCollection; }
            set { virtualQuantityCollection = value; OnPropertyChanged("VirtualQuantityCollection"); }
        }

        public UInt64 VirtualQuantity
        {
            get 
            {
                
                return virtualQuantity; 
            }
            set 
            { 
                virtualQuantity = value; OnPropertyChanged("VirtualQuantity"); 
            }
        }

        public ProcessorViewModel(IEnumerable<VM> ActiveVMList)
            : base("Processor", ActiveVMList)
        {
            //VirtualQuantity = (UInt64)procInstance["VirtualQuantity"];
            VirtualQuantity = GetProcessor(this.VMList, out maxProcessorCount);
            VirtualQuantityCollection = new uint [maxProcessorCount] ;
            for (uint i = 0; i < maxProcessorCount; i++)
            {
                VirtualQuantityCollection[i] = i + 1;
            }
            OKCommand = new DelegateCommand<object>(PerformOK, CanSetProcessor);
            ApplyCommand = new DelegateCommand<object>(SetProcessor, CanSetProcessor);
            CancelCommand = new DelegateCommand<object>(PerformCancel, CanCancel);
            View = new ProcessorView(this);
        }

        private void SetProcessor(object quantity)
        {
            VMModel.GetInstance().SetProcessorVMList(VMList, VirtualQuantity);
        }

        private bool CanSetProcessor(object quantity)
        {
            bool canSet = true;
            foreach (VM vm in this.VMList)
            {
                if (vm.Status != VMState.Disabled)
                {
                    canSet = false;
                    break;
                }
            }
            return canSet;
        }

        private void PerformOK(object quantity)
        {
            SetProcessor(VirtualQuantity);
            OnViewCloseRequested();
        }

        private void PerformCancel(object obj)
        {
            OnViewCloseRequested();
        }

        private bool CanCancel(object obj)
        {
            return true;
        }

        private UInt64 GetProcessor(IEnumerable<VM> VMList, out uint MaxCount)
        {
            UInt64 virtualQuantity = 0;
            MaxCount = uint.MaxValue;
            try
            {
                foreach (VM vm in this.VMList)
                {
                    UInt64 curQuantity = (UInt64)Utility.GetProcessor(vm)["VirtualQuantity"];
                    uint curMaxCount = vm.Host.CoresCount;
                    MaxCount = curMaxCount < MaxCount ? curMaxCount : MaxCount; 

                    if ((virtualQuantity > 0) && (virtualQuantity != curQuantity))
                    {
                        virtualQuantity = 0;
                        break;
                    }
                    virtualQuantity = curQuantity;
                }
            }
            catch (ManagementException ex)
            {
                MessageBox.Show(ex.Message, this.Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return virtualQuantity;
        }
    }
}
