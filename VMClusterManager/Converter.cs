using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using VMClusterManager.ViewModels;

namespace VMClusterManager
{
    public class Converter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VM)
            {
                VMDetailsViewModel viewModel = new VMDetailsViewModel(value as VM);
                return viewModel.View;
            }
            else if (value is VMGroup)
            {
                VMListViewModel viewModel = new VMListViewModel(VMModel.GetInstance());
                return viewModel.View;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
