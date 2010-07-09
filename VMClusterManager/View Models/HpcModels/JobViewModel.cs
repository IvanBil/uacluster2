using System;
using Microsoft.Hpc.Scheduler;
using System.Collections.ObjectModel;

namespace VMClusterManager.ViewModels.HpcModels
{
    public class JobViewModel : ViewModelBase
    {
        private ISchedulerJob Job;
        private string name;
        private string owner;
        private DateTime createTime;
        private DateTime startTime;
        private DateTime endTime;
        private int jobID;
        private string state;
        private ObservableCollection<ISchedulerTask> taskList;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }
        public int JobID
        {
            get { return jobID; }
            set { jobID = value; OnPropertyChanged("JobID"); }
        }
        public string State
        {
            get { return state; }
            set { state = value; OnPropertyChanged("State"); }
        }

        public string Owner
        {
            get { return owner; }
            set { owner = value; OnPropertyChanged("Owner"); }
        }

        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; OnPropertyChanged("EndTime"); }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; OnPropertyChanged("StartTime"); }
        }

        public DateTime CreateTime
        {
            get { return createTime; }
            set { createTime = value; OnPropertyChanged("CreateTime"); }
        }

        public JobViewModel(ISchedulerJob _job)
        {
            this.Job = _job;
            this.CreateTime = Job.CreateTime;
            this.Owner = Job.Owner;
            this.Name = Job.Name;
            this.State = Job.State.ToString();
            this.StartTime = Job.StartTime;
            this.EndTime = Job.EndTime;
            this.JobID = Job.Id;
        }
    }
}
