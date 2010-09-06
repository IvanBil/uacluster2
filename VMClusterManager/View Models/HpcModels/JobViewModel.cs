using System;
using Microsoft.Hpc.Scheduler;
using System.Collections.ObjectModel;
using Microsoft.Practices.Composite.Wpf.Commands;
using VMClusterManager.ViewModels.VMModels;
using System.Windows;
using System.Threading;

namespace VMClusterManager.ViewModels.HpcModels
{
    public class JobViewModel : ViewModelBase
    {
        private ISchedulerJob job;

        public ISchedulerJob Job
        {
            get 
            {
                job = HpcScheduler.OpenJob(job.Id);
                return job; 
            }
            set { job = value; }
        }

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
            get 
            {
                return startTime; 
            }
            set { startTime = value; OnPropertyChanged("StartTime"); }
        }

        public DateTime CreateTime
        {
            get { return createTime; }
            set { createTime = value; OnPropertyChanged("CreateTime"); }
        }

        private bool isActive;

        public bool IsActive
        {
            get { return isActive; }
            set 
            { 
                isActive = value;
                if (value)
                {
                    ParentViewModel.CancelJobListCommand.RegisterCommand(CancelJobCommand);
                    ParentViewModel.RequeueJobListCommand.RegisterCommand(RequeueJobCommand);
                    
                }
                else
                {
                    ParentViewModel.CancelJobListCommand.UnregisterCommand(CancelJobCommand);
                    ParentViewModel.RequeueJobListCommand.UnregisterCommand(RequeueJobCommand);
                }
                OnPropertyChanged("IsActive"); 
            }
        }

        private bool isSelected;
        /// <summary>
        /// Equals "true" when corresponding row in Job datagrid selected 
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; OnPropertyChanged("IsSelected"); }
        }

        private DelegateCommand<object> cancelJobCommand;

        public DelegateCommand<object> CancelJobCommand
        {
            get { return cancelJobCommand; }
            set { cancelJobCommand = value; OnPropertyChanged("CancelJobCommand"); }
        }

        private DelegateCommand<object> requeueJobCommand;

        public DelegateCommand<object> RequeueJobCommand
        {
            get { return requeueJobCommand; }
            set { requeueJobCommand = value; OnPropertyChanged("RequeueJobCommand"); }
        }

        private DelegateCommand<object> selectCommand;

        public DelegateCommand<object> SelectCommand
        {
            get { return selectCommand; }
            set { selectCommand = value; OnPropertyChanged("SelectCommand"); }
        }

        private DelegateCommand<object> unselectCommand;

        public DelegateCommand<object> UnselectCommand
        {
            get { return unselectCommand; }
            set { unselectCommand = value; OnPropertyChanged("UnselectCommand"); }
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
        private JobListViewModel ParentViewModel;

        //private static ManualResetEvent manualEvent = new ManualResetEvent(false);

        public Scheduler HpcScheduler;

        public JobViewModel(ISchedulerJob _job, JobListViewModel _ParentViewModel)
        {
            this.ParentViewModel = _ParentViewModel;
            this.HpcScheduler = ParentViewModel.HpcScheduler;
            this.Job = _job;
            this.CreateTime = Job.CreateTime;
            this.Owner = Job.Owner;
            this.Name = Job.Name;
            this.State = Job.State.ToString();
            
            this.StartTime = Job.StartTime;
            this.EndTime = Job.EndTime;
            this.JobID = Job.Id;
            this.CancelTaskListCommand = this.ParentViewModel.CancelTaskListCommand;
            this.RequeueTaskListCommand = this.ParentViewModel.RequeueTaskListCommand;
            CancelJobCommand = new DelegateCommand<object>(CancelJob, CanCancelJob);
            RequeueJobCommand = new DelegateCommand<object>(RequeueJob, CanRequeueJob);
            SelectCommand = new DelegateCommand<object>(Select, CanSelect);
            UnselectCommand = new DelegateCommand<object>(Unselect, CanUnselect);
            ParentViewModel.SelectAllCommand.RegisterCommand(SelectCommand);
            ParentViewModel.UnselectAllCommand.RegisterCommand(UnselectCommand);
            
            //this.Job = ParentViewModel.HpcScheduler.OpenJob(this.JobID);
            this.Job.OnJobState +=  new EventHandler<JobStateEventArg> (JobStateCallback);
                //(object o, JobStateEventArg arg) =>
                //{
                //    this.State = Job.State.ToString();
                //    CancelJobCommand.RaiseCanExecuteChanged();
                //    RequeueJobCommand.RaiseCanExecuteChanged();
                //}
            //);
        }

        private void JobStateCallback(object sender, IJobStateEventArg args)
        {
            this.State = args.NewState.ToString();
            CancelJobCommand.RaiseCanExecuteChanged();
            RequeueJobCommand.RaiseCanExecuteChanged();
        }

        private void Select(object param)
        {
            if (!this.IsActive)
                this.IsActive = true;
        }

        private bool CanSelect(object param)
        {
            return true;
        }

        private void Unselect(object param)
        {
            if (this.IsActive)
                this.IsActive = false;
        }

        private bool CanUnselect(object param)
        {
            return true;
        }

        private void CancelJob(object param)
        {
            Thread background = new Thread(new ThreadStart
                (
                () =>
                {
                    try
                    {
                        HpcScheduler.CancelJob(this.JobID, "Cancelled by cluster administrator.");
                    }
                    catch (Exception ex)
                    {
                        VMLog.GetInstance().AddMessage(new LogMessage(LogMessageTypes.Error, ex.Message, this.Name, "Cancel Job"));
                    }
                }));
            background.IsBackground = true;
            background.Priority = ThreadPriority.Lowest;
            background.Start();
        }

        private bool CanCancelJob(object param)
        {
            bool canCancel = false;
            if ((this.Job.State == Microsoft.Hpc.Scheduler.Properties.JobState.Configuring)||
                (this.Job.State == Microsoft.Hpc.Scheduler.Properties.JobState.Queued) ||
                (this.Job.State == Microsoft.Hpc.Scheduler.Properties.JobState.Running) ||
                (this.Job.State == Microsoft.Hpc.Scheduler.Properties.JobState.Submitted))
                    {
                        canCancel = true;
                    }
            return canCancel;
        }

        private void RequeueJob(object param)
        {
            Thread background = new Thread(new ThreadStart
                (
                () =>
                {
                        try
                        {                            
                            HpcScheduler.ConfigureJob(this.JobID);                            
                            HpcScheduler.SubmitJob(this.Job, string.Empty, string.Empty);
                        }
                        catch (Exception ex)
                        {
                            VMLog.GetInstance().AddMessage(new LogMessage(LogMessageTypes.Error, ex.Message, this.Name, "Requeue Job"));
                        }
                }));
            background.IsBackground = true;
            background.Priority = ThreadPriority.Lowest;
            background.Start();
        }

        private bool CanRequeueJob(object param)
        {
            return this.Job.State == Microsoft.Hpc.Scheduler.Properties.JobState.Canceled;
        }
    }
}
