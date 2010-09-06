using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Hpc.Scheduler;
using Microsoft.Hpc.Scheduler.Properties;
using VMClusterManager.ViewModels.HpcModels;
using Microsoft.Practices.Composite.Wpf.Commands;
using System.Windows.Input;
using VMClusterManager.ViewModels.VMModels;
using System.Threading;

namespace VMClusterManager.ViewModels.HpcModels
{
    public class TaskViewModel : ViewModelBase
    {
        private ISchedulerTask instance;

        private ISchedulerTask Instance
        {
            get { instance.Refresh(); return instance; }
            set { instance = value; }
        }
        private string name;
        private int taskID;
        private TaskState state;
        private string commandLine;

        private bool isActive;

        public bool IsActive
        {
            get { return isActive; }
            set 
            { 
                isActive = value;
                if (value)
                {
                    this.CancelTaskListCommand.RegisterCommand(CancelCommand);
                    this.RequeueTaskListCommand.RegisterCommand(RequeueCommand);
                }
                else
                {
                    this.CancelTaskListCommand.UnregisterCommand(CancelCommand);
                    this.RequeueTaskListCommand.UnregisterCommand(RequeueCommand);
                }
                OnPropertyChanged("IsActive"); 
            }
        }

        public int TaskID
        {
            get { return taskID; }
            set { taskID = value; OnPropertyChanged("TaskID"); }
        }

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        public TaskState State
        {
            get { return state; }
            set { state = value; OnPropertyChanged("State"); }
        }


        public string CommandLine
        {
            get { return commandLine; }
            set { commandLine = value; OnPropertyChanged("CommandLine"); }
        }

        private string requestedResources;

        public string RequestedResources
        {
            get { return requestedResources; }
            set { requestedResources = value; OnPropertyChanged("RequestedResources"); }
        }

        private DateTime startTime;

        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; OnPropertyChanged("StartTime"); }
        }

        private DateTime endTime;

        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; OnPropertyChanged("EndTime"); }
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

        private DelegateCommand<object> cancelCommand;

        public DelegateCommand<object> CancelCommand
        {
            get { return cancelCommand; }
            set { cancelCommand = value; OnPropertyChanged("CancelCommand"); }
        }

        private DelegateCommand<object> requeueCommand;

        public DelegateCommand<object> RequeueCommand
        {
            get { return requeueCommand; }
            set { requeueCommand = value; OnPropertyChanged("RequeueCommand"); }
        }

        private CompositeCommand SelectAllCommand;
        private CompositeCommand UnselectAllCommand;
        private CompositeCommand CancelTaskListCommand;
        private CompositeCommand RequeueTaskListCommand;
        //public TaskViewModel ParentTask;

        private TaskListViewModel ParentViewModel;

        private ISchedulerJob parentJob;

        private ISchedulerJob ParentJob
        {
            get { parentJob.Refresh(); return parentJob; }
            set { parentJob = value; }
        }

        public TaskViewModel(ISchedulerTask _instance, ISchedulerJob parentJob, TaskListViewModel _ParentViewModel)
        {
            this.instance = _instance;
            this.ParentJob = parentJob;
            this.ParentViewModel = _ParentViewModel;
            this.SelectAllCommand = ParentViewModel.SelectAllCommand;
            this.UnselectAllCommand = ParentViewModel.UnselectAllCommand;
            this.CancelTaskListCommand = ParentViewModel.CancelTaskListCommand;
            this.RequeueTaskListCommand = ParentViewModel.RequeueTaskListCommand;
            this.CancelCommand = new DelegateCommand<object>(Cancel, CanCancel);
            this.RequeueCommand = new DelegateCommand<object>(Requeue, CanRequeue);
            Name = this.instance.Name;
            TaskID = this.instance.TaskId.JobTaskId;
            State = this.instance.State;
            this.ParentJob.OnTaskState += new EventHandler<TaskStateEventArg>(ParentJob_OnTaskState);
            CommandLine = this.instance.CommandLine;
            int minUnit = 1;
            int maxUnit = 1;
            //JobUnitType unitType = JobUnitType.Core;
            switch (parentJob.UnitType)
            {
                case JobUnitType.Core: minUnit = this.instance.MinimumNumberOfCores; maxUnit = this.instance.MaximumNumberOfCores; break;
                case JobUnitType.Node: minUnit = this.instance.MinimumNumberOfNodes; maxUnit = this.instance.MaximumNumberOfNodes; break;
                case JobUnitType.Socket: minUnit = this.instance.MinimumNumberOfSockets; maxUnit = this.instance.MaximumNumberOfSockets; break;
            }
            RequestedResources = string.Format("{0}-{1} {2}s", minUnit, maxUnit, parentJob.UnitType);

            StartTime = this.instance.StartTime;
            EndTime = this.instance.EndTime;

            SelectCommand = new DelegateCommand<object>(Select, CanSelect);
            UnselectCommand = new DelegateCommand<object>(Unselect, CanUnselect);
            SelectAllCommand.RegisterCommand(SelectCommand);
            UnselectAllCommand.RegisterCommand(UnselectCommand);
        }

        void ParentJob_OnTaskState(object sender, TaskStateEventArg e)
        {
            if (e.TaskId.JobTaskId == this.TaskID)
            {
                this.State = e.NewState;
                CancelCommand.RaiseCanExecuteChanged();
                RequeueCommand.RaiseCanExecuteChanged();
            }
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

        private void Cancel(object param)
        {
            Thread background = new Thread(new ThreadStart
                (
                () =>
                {
                    try
                    {
                        this.ParentJob.CancelTask(this.instance.TaskId);
                    }
                    catch (Exception ex)
                    {
                        VMLog.GetInstance().AddMessage(new LogMessage(LogMessageTypes.Error, ex.Message, this.Name, "Cancel Task"));
                    }
                }));
            background.IsBackground = true;
            background.Priority = ThreadPriority.Lowest;
            background.Start();
            
        }

        private bool CanCancel(object param)
        {
            return (this.State == TaskState.Running)||(this.State == TaskState.Submitted) || 
                (this.State == TaskState.Configuring) || (this.State == TaskState.Queued);
        }

        private void Requeue(object param)
        {
            Thread background = new Thread(new ThreadStart
                (
                () =>
                {
                    try
                    {
                        this.ParentJob.RequeueTask(this.instance.TaskId);
                    }
                    catch (Exception ex)
                    {
                        VMLog.GetInstance().AddMessage(new LogMessage(LogMessageTypes.Error, ex.Message, this.Name, "Requeue Task"));
                    }
                }));
            background.IsBackground = true;
            background.Priority = ThreadPriority.Lowest;
            background.Start();
        }

        private bool CanRequeue(object param)
        {
            return this.State == TaskState.Failed;
        }

        ~TaskViewModel()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            //if (BackgroundThread != null)
            //{
            //    BackgroundThread.Abort();
            //}
            //IsDisposed = true;
            if (!IsDisposed)
            {
                if (disposing)
                {
                    //base.Dispose(disposing);
                    this.ParentJob.OnTaskState += new EventHandler<TaskStateEventArg>(ParentJob_OnTaskState);

                    try
                    {
                        this.CancelTaskListCommand.UnregisterCommand(CancelCommand);
                        this.RequeueTaskListCommand.UnregisterCommand(RequeueCommand);
                    }
                    catch
                    {
                    }
                }
            }
            IsDisposed = true;

        }
    }
}
