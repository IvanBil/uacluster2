using System;
using Microsoft.Hpc.Scheduler;
using System.Windows;
using VMClusterManager.Controls.JobViews;
using VMClusterManager.ViewModels.HpcModels;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.Composite.Wpf.Commands;
using System.Collections.Generic;

namespace VMClusterManager.ViewModels.HpcModels
{
    public class JobListViewModel : ViewModelBase
    {
        public IView View;

        
        public JobCommandsView ActionsView;

        public static string MainNodeName;
        ObservableCollection<JobViewModel> jobList;

        public ObservableCollection<JobViewModel> JobList
        {
            get { return jobList; }
            set { jobList = value; OnPropertyChanged("JobList"); }
        }


        private CompositeCommand cancelJobListCommand;

        public CompositeCommand CancelJobListCommand
        {
            get { return cancelJobListCommand; }
            set { cancelJobListCommand = value; OnPropertyChanged("CancelJobListCommand"); }
        }
        private CompositeCommand requeueJobListCommand;

        public CompositeCommand RequeueJobListCommand
        {
            get { return requeueJobListCommand; }
            set { requeueJobListCommand = value; }
        }

        private CompositeCommand selectAllCommand;
        private CompositeCommand unselectAllCommand;

        public CompositeCommand UnselectAllCommand
        {
            get { ; return unselectAllCommand; }
            set { unselectAllCommand = value; OnPropertyChanged("UnselectAllCommand"); }
        }

        public CompositeCommand SelectAllCommand
        {
            get { return selectAllCommand; }
            set { selectAllCommand = value; OnPropertyChanged("SelectAllCommand"); }
        }

        private CompositeCommand cancelTaskListCommand;

        public CompositeCommand CancelTaskListCommand
        {
            get { return cancelTaskListCommand; }
            set { cancelTaskListCommand = value; OnPropertyChanged("CancelTaskListCommand"); }
        }

        private CompositeCommand requeueTaskListCommand;

        public CompositeCommand RequeueTaskListCommand
        {
            get { return requeueTaskListCommand; }
            set { requeueTaskListCommand = value; OnPropertyChanged("RequeueTaskListCommand"); }
        }

        public Scheduler HpcScheduler;

        public JobListViewModel(string mainNodeName, ObservableCollection<VM> _ActiveVMList)
            : base()
        {
            //Scheduler scheduler = new Scheduler();
            MainNodeName = mainNodeName;
            jobList = new ObservableCollection<JobViewModel>();
            CancelJobListCommand = new CompositeCommand();
            RequeueJobListCommand = new CompositeCommand();
            SelectAllCommand = new CompositeCommand();
            UnselectAllCommand = new CompositeCommand();
            CancelTaskListCommand = new CompositeCommand();
            RequeueTaskListCommand = new CompositeCommand();
            HpcScheduler = new Scheduler();
            try
            {
                HpcScheduler.Connect(mainNodeName);
                ObservableCollection<VM> activeVMList = new ObservableCollection<VM>(_ActiveVMList);
                //IFilterCollection filters = scheduler.CreateFilterCollection();
                //filters.Add(Microsoft.Hpc.Scheduler.Properties.FilterOperator.In, PropId.
                //SchedulerNode node = 
                var query = from ISchedulerJob job
                                in HpcScheduler.GetJobList(null, null)
                            where activeVMList.Any(j => job.AllocatedNodes.Contains(j.GetDomainName()))
                            select job;
                foreach (ISchedulerJob job in query)
                {
                    JobList.Add(new JobViewModel(job, this));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Getting Job list error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //CancelJobListCommand = new DelegateCommand<ObservableCollection<JobViewModel>>(CancelJobList, CanCancelJobList);
            View = new JobListView();
            View.SetViewModel(this);
            ActionsView = new JobCommandsView(this);
        }

        private void SelectAll(IEnumerable<JobViewModel> jobsToSelect)
        {
            foreach (JobViewModel job in jobsToSelect)
            {
                if (!job.IsActive)
                {
                    job.IsActive = true;
                }
            }
        }

        private bool CanSelectAll(IEnumerable<JobViewModel> jobsToSelect)
        {
            return true;
        }

        private void UnselectAll(IEnumerable<JobViewModel> jobsToUnSelect)
        {
            foreach (JobViewModel job in jobsToUnSelect)
            {
                if (job.IsActive)
                {
                    job.IsActive = false;
                }
            }
        }

        private bool CanUnselectAll(IEnumerable<JobViewModel> jobsToUnSelect)
        {
            return jobsToUnSelect.Any(j => { return j.IsActive == true; });
        }

        
    }
}
