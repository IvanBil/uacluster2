using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Hpc.Scheduler;
using Microsoft.Hpc.Scheduler.Properties;
using VMClusterManager.ViewModels.HpcModels;
using System.Collections.ObjectModel;
using Microsoft.Practices.Composite.Wpf.Commands;



namespace VMClusterManager.ViewModels.HpcModels
{
    public class TaskListViewModel : ViewModelBase
    {
        IView view;
        //private ISchedulerCollection taskCollection;

        //public ISchedulerCollection TaskCollection
        //{
        //    get { return taskCollection; }
        //    set { taskCollection = value; OnPropertyChanged("TaskCollection"); }
        //}

        private CompositeCommand selectAllCommand;

        public CompositeCommand SelectAllCommand
        {
            get { return selectAllCommand; }
            set { selectAllCommand = value; OnPropertyChanged("SelectAllCommand"); }
        }

        private CompositeCommand unselectAllCommand;

        public CompositeCommand UnselectAllCommand
        {
            get { return unselectAllCommand; }
            set { unselectAllCommand = value; OnPropertyChanged("UnselectAllCommand"); }
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

        private ObservableCollection<TaskViewModel> taskList;

        public ObservableCollection<TaskViewModel> TaskList
        {
            get { return taskList; }
            set { taskList = value; }
        }

        public TaskListViewModel(ISchedulerJob job, IEnumerable<VM> activeVMList, Scheduler hpcSched, 
            CompositeCommand _CancelTaskListCommand, CompositeCommand _RequeueTaskListCommand)
            : base()
        {
            TaskList = new ObservableCollection<TaskViewModel>();
            SelectAllCommand = new CompositeCommand();
            UnselectAllCommand = new CompositeCommand();
            CancelTaskListCommand = _CancelTaskListCommand;
            this.RequeueTaskListCommand = _RequeueTaskListCommand;
            //IEnumerable<VM> activeVMList = VMModel.GetInstance().ActiveVMList;
            try
            {
                //scheduler.Connect(JobListViewModel.MainNodeName);
                job.Refresh();
                ISchedulerCollection tasks = job.GetTaskList(null, null, true);
                var query = from ISchedulerTask task
                                    in tasks
                            where activeVMList.Any(j => task.AllocatedNodes.Contains(j.GetDomainName()))
                            select task;
                foreach (ISchedulerTask task in query)
                {
                    TaskList.Add(new TaskViewModel(task, job, this));
                }
            }
            catch (Exception ex)
            {
            }
            //foreach (ISchedulerTask task in job.GetTaskList(null, null, true))
            //{
            //    TaskList.Add(new TaskViewModel(task, job));
            //}
        }
    }
}
