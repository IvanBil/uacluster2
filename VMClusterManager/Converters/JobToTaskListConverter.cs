using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using VMClusterManager.ViewModels.VMModels;
using VMClusterManager.ViewModels.HpcModels;

namespace VMClusterManager.Converters
{
    public class JobToTaskListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TaskListViewModel tl = new TaskListViewModel((value as JobViewModel).Job, VMModel.GetInstance().ActiveVMList,
                (value as JobViewModel).HpcScheduler, (value as JobViewModel).CancelTaskListCommand, (value as JobViewModel).RequeueTaskListCommand);
            return tl;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
