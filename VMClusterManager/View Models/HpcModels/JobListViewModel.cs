using System;
using Microsoft.Hpc.Scheduler;
using System.Windows;
using VMClusterManager.Controls.JobViews;
using VMClusterManager.ViewModels.HpcModels;
using System.Collections.ObjectModel;

namespace VMClusterManager.ViewModels.VMModels
{
    public class JobListViewModel : ViewModelBase
    {
        public IView View;

        ObservableCollection<JobViewModel> jobList;

        public ObservableCollection<JobViewModel> JobList
        {
            get { return jobList; }
            set { jobList = value; OnPropertyChanged("JobList"); }
        }

        public JobListViewModel(string mainNodeName)
            : base()
        {
            Scheduler scheduler = new Scheduler();
            try
            {
                scheduler.Connect(mainNodeName);
                jobList = new ObservableCollection<JobViewModel>();
                foreach (ISchedulerJob job in scheduler.GetJobList(null, null))
                {
                    JobList.Add(new JobViewModel(job));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Getting Job list error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            View = new JobListView();
            View.SetViewModel(this);
        }
    }
}
