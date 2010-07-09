using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Hpc.Scheduler;
using Microsoft.Hpc.Scheduler.Properties;



namespace VMClusterManager.ViewModels.VMModels
{
    public class TaskListViewModel : ViewModelBase
    {
        IView view;
        private ISchedulerCollection taskCollection;

        public ISchedulerCollection TaskCollection
        {
            get { return taskCollection; }
            set { taskCollection = value; OnPropertyChanged("TaskCollection"); }
        }

        public TaskListViewModel(ISchedulerJob job)
            : base()
        {
            

        }
    }
}
