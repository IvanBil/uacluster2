using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using VMClusterManager.ViewModels.VMModels;
using System.Collections.ObjectModel;
using VMClusterManager.ViewModels.VMHostModels;

namespace VMClusterManager.Converters
{
    public class ModelToViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is VMGroup)
                return new VMGroupViewModel(value as VMGroup);
            if (value is VM)
                return new VMViewModel(value as VM);
            if (value is IEnumerable<VM>)
            {
                List<VM> vmList = new List<VM>(value as IEnumerable<VM>);
                List<VMViewModel> vmViewModelList = new List<VMViewModel>();
                foreach (VM vm in vmList)
                {
                    vmViewModelList.Add(new VMViewModel(vm));
                }
                return vmViewModelList;
            }
            if (value is IEnumerable<VMHost>)
            {
                List<VMHost> HostList = new List<VMHost> (value as IEnumerable<VMHost>);
                List<VMHostViewModel> vmHostViewModelList = new List<VMHostViewModel>();
                foreach (VMHost host in HostList)
                {
                    vmHostViewModelList.Add(new VMHostViewModel(host));
                }
                return vmHostViewModelList;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
