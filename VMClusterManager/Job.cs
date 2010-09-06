using System;
using System.Management;
using System.Windows;

namespace VMClusterManager
{

    /// <summary>
    /// Represents CIM_ConcreteJob WMI Class
    /// </summary>
    public class Job
    {
        private string caption;
        private string description;
        private DateTime elapsedTime;
        private DateTime installDate;
        private string jobStatus;
        private string name;
        private int priority;
        private DateTime startTime;
        private string status;
        private DateTime timeSubmitted;
        private DateTime untilTime;
        ManagementObject jobInstance;

        public ManagementObject JobInstance
        {
            get 
            {
                if (jobInstance != null) { jobInstance.Get(); }
                return jobInstance;     
            }
            private set { jobInstance = value; }
        }

        public DateTime UntilTime
        {
            get { return untilTime; }
            set { untilTime = value; }
        }

        public DateTime TimeSubmitted
        {
            get { return timeSubmitted; }
            set { timeSubmitted = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string JobStatus
        {
            get { return jobStatus; }
            set { jobStatus = value; }
        }

        public DateTime InstallDate
        {
            get { return installDate; }
            set { installDate = value; }
        }

        public DateTime ElapsedTime
        {
            get { return elapsedTime; }
            set { elapsedTime = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string Caption
        {
            get { return jobInstance["Caption"].ToString(); }
        }

        public Job(ManagementObject _jobInstance)
        {
            this.jobInstance = _jobInstance;
        }

        public override string ToString()
        {
            return Caption;
        }
    }

    /// <summary>
    /// Represents Msvm_ConcreteJob WMI Class
    /// </summary>
    public class VMJob : Job
    {
        private UInt16 jobState;

        public UInt16 JobState
        {
            get { return (UInt16)JobInstance["JobState"]; }
        }

        public UInt16 PercentComplete
        {
            get { return (UInt16)JobInstance["PercentComplete"]; }
        }

        public UInt16 ErrorCode
        {
            get { return (UInt16)JobInstance["ErrorCode"]; }
        }

        public VMJob(ManagementObject _jobInstance)
            : base(_jobInstance)
        {
        }

        public string GetError()
        {
            return JobInstance["ErrorDescription"].ToString();
        }
    }
}
