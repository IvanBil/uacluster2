using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMClusterManager.ViewModels.VMHostModels
{
    public class VMModel : IVMModel
    {

        #region IVMModel Members


        private object selectedItem;
        public object SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                OnSelectedItemChanged();
            }
        }

        private static VMModel instance;

        private VMModel()
        {
        }

        public static VMModel GetInstance()
        {
            if (instance == null)
                instance = new VMModel();
            return instance;
        }


        //private List<VM

        //public List<VMHost> VMHostList
        //{
        //    get { throw new NotImplementedException(); }
        //}



        //public event EventHandler<VMHostStateChangedEventArgs> VMHostStateChanged;

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected void OnSelectedItemChanged()
        {
            if (SelectedItemChanged != null)
                SelectedItemChanged(this, new SelectedItemChangedEventArgs());
        }

        public event EventHandler<VMHostListChangedEventArgs> ActiveVMHostListChanged;
        private void OnActiveVMHostListChanged()
        {
            if (ActiveVMHostListChanged != null)
            {
                ActiveVMHostListChanged(this, new VMHostListChangedEventArgs());
            }
        }




        #endregion

        #region IVMHostModel Members


        public List<VMHost> VMHostList
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
