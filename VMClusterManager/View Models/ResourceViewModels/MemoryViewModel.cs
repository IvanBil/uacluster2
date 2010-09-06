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
    public class MemoryViewModel : ResourceViewModel
    {
        //public MemoryView View;
        private UInt64 size;
        private UInt64 minSize;
        private UInt64 maxSize;

        public UInt64 MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; OnPropertyChanged("MaxSize"); }
        }

        public UInt64 MinSize
        {
            get { return minSize; }
            set { minSize = value; }
        }

        public UInt64 Size
        {
            get 
            {
                
                return size; 
            }
            set 
            {
                size = value; OnPropertyChanged("Size");
            }
        }

        

        public MemoryViewModel(IEnumerable<VM> ActiveVMList)
            : base("Memory", ActiveVMList)
        {
            //Instance = MemoryInstance;
            //Size = (UInt64)MemoryInstance["Reservation"];
            //UInt64 limit;
            Size = GetMem(ActiveVMList);
            //MaxSize = limit;
            ApplyCommand = new DelegateCommand<object>(SetMemory,CanSetMemory);
            OKCommand = new DelegateCommand<object> (PerformOK, CanSetMemory);
            CancelCommand = new DelegateCommand<object>(PerformCancel, CanCancel);
            ApplyCommand.RaiseCanExecuteChanged();
            View = new MemoryView();
            (View as IView).SetViewModel(this);
        }

        private void SetMemory(object quantity)
        {
            VMModel.GetInstance().SetMemoryVMList(VMList, Size);
        }

        private bool CanSetMemory(object quantity)
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
            SetMemory(Size);
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

        private UInt64 GetMem(IEnumerable<VM> VMList)
        {
            UInt64 size = 0;
            MaxSize = UInt64.MaxValue;
            try
            {
                foreach (VM vm in this.VMList)
                {
                    ManagementObject memObj = Utility.GetMemory(vm);
                    UInt64 curSize=(UInt64) memObj["VirtualQuantity"];
                    MaxSize = (MaxSize > vm.Host.TotalMemory) ? vm.Host.TotalMemory : MaxSize;
                    //MaxSize = (Limit > curLimit) ? curLimit : Limit;
                    if ((size > 0) && (size != curSize))
                    {
                        size = 0;
                        break;
                    }
                    size = curSize;
                }
            }
            catch (ManagementException ex)
            {
                MessageBox.Show(ex.Message, "Memory", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return size;
        }

        
    }
}
