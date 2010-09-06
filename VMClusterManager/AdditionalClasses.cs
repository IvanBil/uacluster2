using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
namespace VMClusterManager
{
    public static class JobState
    {
        public const UInt16 New = 2;
        public const UInt16 Starting = 3;
        public const UInt16 Running = 4;
        public const UInt16 Suspened = 5;
        public const UInt16 ShuttingDown = 6;
        public const UInt16 Completed = 7;
        public const UInt16 Terminated = 8;
        public const UInt16 Killed = 9;
        public const UInt16 Exception = 10;
        public const UInt16 Service = 11;
    }

    static class ResourceType
    {
        public const UInt16 Other = 1;
        public const UInt16 ComputerSystem = 2;
        public const UInt16 Processor = 3;
        public const UInt16 Memory = 4;
        public const UInt16 IDEController = 5;
        public const UInt16 ParallelSCSIHBA = 6;
        public const UInt16 FCHBA = 7;
        public const UInt16 iSCSIHBA = 8;
        public const UInt16 IBHCA = 9;
        public const UInt16 EthernetAdapter = 10;
        public const UInt16 OtherNetworkAdapter = 11;
        public const UInt16 IOSlot = 12;
        public const UInt16 IODevice = 13;
        public const UInt16 FloppyDrive = 14;
        public const UInt16 CDDrive = 15;
        public const UInt16 DVDdrive = 16;
        public const UInt16 Serialport = 17;
        public const UInt16 Parallelport = 18;
        public const UInt16 USBController = 19;
        public const UInt16 GraphicsController = 20;
        public const UInt16 StorageExtent = 21;
        public const UInt16 Disk = 22;
        public const UInt16 Tape = 23;
        public const UInt16 OtherStorageDevice = 24;
        public const UInt16 FirewireController = 25;
        public const UInt16 PartitionableUnit = 26;
        public const UInt16 BasePartitionableUnit = 27;
        public const UInt16 PowerSupply = 28;
        public const UInt16 CoolingDevice = 29;
        public const UInt16 DisketteController = 1;
    }

    static class ResourceSubType
    {
        public const string DisketteController = null;
        public const string DisketteDrive = "Microsoft Synthetic Diskette Drive";
        public const string ParallelSCSIHBA = "Microsoft Synthetic SCSI Controller";
        public const string IDEController = "Microsoft Emulated IDE Controller";
        public const string DiskSynthetic = "Microsoft Synthetic Disk Drive";
        public const string DiskPhysical = "Microsoft Physical Disk Drive";
        public const string DVDPhysical = "Microsoft Physical DVD Drive";
        public const string DVDSynthetic = "Microsoft Synthetic DVD Drive";
        public const string CDROMPhysical = "Microsoft Physical CD Drive";
        public const string CDROMSynthetic = "Microsoft Synthetic CD Drive";
        public const string EthernetSynthetic = "Microsoft Synthetic Ethernet Port";

        //logical drive
        public const string DVDLogical = "Microsoft Virtual CD/DVD Disk";
        public const string ISOImage = "Microsoft ISO Image";
        public const string VHD = "Microsoft Virtual Hard Disk";
        public const string DVD = "Microsoft Virtual DVD Disk";
        public const string VFD = "Microsoft Virtual Floppy Disk";
        public const string videoSynthetic = "Microsoft Synthetic Display Controller";
    }
    public static class VMRequestedState
    {
        /// <summary>
        /// Turns the VM on.
        /// </summary>
        public const int Enabled = 2;
        /// <summary>
        /// Turns the VM off.
        /// </summary>
        public const int Disabled = 3;
        /// <summary>
        /// A hard reset of the VM.
        /// </summary>
        public const int Reboot = 10;
        /// <summary>
        /// Pauses the VM.
        /// </summary>
        public const int Paused = 32768;
        /// <summary>
        /// Saves the state of the VM.
        /// </summary>
        public const int Suspended = 32769;
    }

    public static class VMState
    {
        /// <summary>
        /// The state of the VM could not be determined.
        /// </summary>
        public const UInt16 Unknown = 0;
        /// <summary>
        /// The VM is running.
        /// </summary>
        public const UInt16 Enabled = 2;
        /// <summary>
        /// The VM is turned off.
        /// </summary>
        public const UInt16 Disabled = 3;
        /// <summary>
        /// The VM is paused.
        /// </summary>
        public const UInt16 Paused = 32768;
        /// <summary>
        /// The VM is in a saved state.
        /// </summary>
        public const UInt16 Suspended = 32769;
        /// <summary>
        /// The VM is starting. This is a transitional state between 3 (Disabled) or 32769 (Suspended) and 2 (Enabled) 
        /// initiated by a call to the RequestStateChange method with a RequestedState parameter of 2 (Enabled).
        /// </summary>
        public const UInt16 Starting = 32770;
        /// <summary>
        /// Starting with Windows Server 2008 R2 this value is not supported. If the VM is performing a snapshot operation, 
        /// the element at index 1 of the OperationalStatus property array will contain 32768 (Creating Snapshot), 32769 (Applying Snapshot), 
        /// or 32770 (Deleting Snapshot). Windows Server 2008: This value is supported and indicates the VM is performing a snapshot operation.
        /// </summary>
        public const UInt16 Snapshotting = 32771;
        /// <summary>
        /// The VM is saving its state. This is a transitional state between 2 (Enabled) and 32769 (Suspended) 
        /// initiated by a call to the RequestStateChange method with a RequestedState parameter of 32769 (Suspended).
        /// </summary>
        public const UInt16 Saving = 32773;
        /// <summary>
        /// The VM is turning off. This is a transitional state between 2 (Enabled) and 3 (Disabled) initiated by a call to the 
        /// RequestStateChange method with a RequestedState parameter of 3 (Disabled) or a guest operating system initiated power off.
        /// </summary>
        public const UInt16 Stopping = 32774;
        /// <summary>
        /// The VM is pausing. This is a transitional state between 2 (Enabled) and 32768 (Paused) initiated by a call to the 
        /// RequestStateChange method with a RequestedState parameter of 32768 (Paused).
        /// </summary>
        public const UInt16 Pausing = 32776;
        /// <summary>
        /// The VM is resuming from a paused state. This is a transitional state between 32768 (Paused) and 2 (Enabled).
        /// </summary>
        public const UInt16 Resuming = 32777;
    }

    static class ReturnCode
    {
        public const UInt32 Completed = 0;
        public const UInt32 Started = 4096;
        public const UInt32 Failed = 32768;
        public const UInt32 AccessDenied = 32769;
        public const UInt32 NotSupported = 32770;
        public const UInt32 Unknown = 32771;
        public const UInt32 Timeout = 32772;
        public const UInt32 InvalidParameter = 32773;
        public const UInt32 SystemInUse = 32774;
        public const UInt32 InvalidState = 32775;
        public const UInt32 IncorrectDataType = 32776;
        public const UInt32 SystemNotAvailable = 32777;
        public const UInt32 OutofMemory = 32778;
    }

    public enum VMHostStatus
    {
        OK=0,
        REFRESHING=1,
        ERROR=2
    }

    public struct VMData
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private TimeSpan upTime;

        public TimeSpan UpTime
        {
            get { return upTime; }
            set { upTime = value; }
        }
        private string gUID;

        public string GUID
        {
            get { return gUID; }
            set { gUID = value; }
        }
        private UInt16 status;

        public UInt16 Status
        {
            get { return status; }
            set { status = value; }
        }
        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        private UInt16[] operationalStatus;

        public UInt16[] OperationalStatus
        {
            get { return operationalStatus; }
            set { operationalStatus = value; }
        }
        private string[] statusDescriptions;

        public string[] StatusDescriptions
        {
            get { return statusDescriptions; }
            set { statusDescriptions = value; }
        }
        private DateTime creationTime;
        private string guestOperatingSystem;
        private UInt16 healthState;
        private UInt64 memoryUsage;
        private UInt16 numberOfProcessors;
        private UInt16 processorLoad;
        private UInt16[] processorLoadHistory;
        private byte[] thumbnailImage;

        public byte[] ThumbnailImage
        {
            get { return thumbnailImage; }
        }

        public UInt16[] ProcessorLoadHistory
        {
            get { return processorLoadHistory; }
        }

        public UInt16 ProcessorLoad
        {
            get { return processorLoad; }
        }


        public UInt16 NumberOfProcessors
        {
            get { return numberOfProcessors; }
        }

        public UInt64 MemoryUsage
        {
            get { return memoryUsage; }
        }
        public UInt16 HealthState
        {
            get { return healthState; }
        }
        public string GuestOperatingSystem
        {
            get { return guestOperatingSystem; }
        }
        public DateTime CreationTime
        {
            get { return creationTime; }
        }
        public VMData(
            string _name, 
            TimeSpan _time, 
            string _guid, 
            UInt16 _status, 
            string _description, 
            UInt16[] _operationalStatus, 
            string[] _statusDescriptions,
            DateTime _creationTime,
            string _guestOperatingSystem,
            UInt16 _healthState,
            UInt64 _memoryUsage,
            UInt16 _numberOfProcessors,
            UInt16 _processorLoad,
            UInt16[] _processorLoadHistory,
            byte[] _thumbnailImage
            )
        {
            name = _name;
            upTime = _time;
            gUID = _guid;
            status = _status;
            description = _description;
            operationalStatus = _operationalStatus;
            statusDescriptions = _statusDescriptions;
            creationTime = _creationTime;
            guestOperatingSystem = _guestOperatingSystem;
            healthState = _healthState;
            memoryUsage = _memoryUsage;
            numberOfProcessors = _numberOfProcessors;
            processorLoad = _processorLoad;
            processorLoadHistory = _processorLoadHistory;
            thumbnailImage = _thumbnailImage;
        }
    }

    public static class VMRequestedInformation
    {
        public static uint Name = 0;
        public static uint ElementName = 1;
        public static uint CreationTime = 2;
        public static uint Notes = 3;
        public static uint NumberOfProcessors = 4;
        public static uint SmallThumbnailImage = 5;
        public static uint MediumThumbnailImage = 6;
        public static uint LargeThumbnailImage = 7;
        public static uint EnabledState = 100;
        public static uint ProcessorLoad = 101;
        public static uint ProcessorLoadHistory = 102;
        public static uint MemoryUsage = 103;
        public static uint HeartBeat = 104;
        public static uint UpTime = 105;
        public static uint GuestOperatingSystem = 106;
        public static uint Snapshots = 107;
        public static uint AsynchronousTasks = 108;
        public static uint HealthState = 109;
        public static uint OperationalStatus = 110;
        public static uint StatusDescriptions = 111;
    }

    public class KVP
    {
        public readonly string Key;
        public readonly object Value;
        public KVP(string _key, object _value)
        {
            Key = _key;
            Value = _value;
        }
    }

    public class LogMessage
    {
        private short messageType;

        public short MessageType
        {
            get { return messageType; }
        }
        private string message;

        public string Message
        {
            get { return message; }
        }

        private DateTime time;

        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }

        private string source;

        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        private string action;

        public string Action
        {
            get { return action; }
            set { action = value; }
        }

        public LogMessage(DateTime _time, short _messageType, string _message, string _source, string _action)
        {
            time = _time;
            messageType = _messageType;
            message = _message;
            source = _source;
            action = _action;
        }

        public LogMessage(short _messageType, string message, string _source, string _action)
            : this(DateTime.Now, _messageType, message, _source, _action)
        {
        }
    }

    public static class LogMessageTypes
    {
        public static short Error = 0;
        public static short Warning = 1;
    }

    public class VMLog : ObservableCollection<LogMessage>
    {
        private static VMLog Instance;
        private VMLog()
        {
        }
        public static VMLog GetInstance()
        {
            if (Instance == null)
            {
                Instance = new VMLog();
            }
            return Instance;
        }

        public void AddMessage(LogMessage message)
        {
            Dispatcher UIDispatcher = VMModel.GetInstance().UIDispatcher;
            UIDispatcher.Invoke(
                new Action<LogMessage> (
                 delegate(LogMessage m)
                {
                    VMLog.GetInstance().Add(m);
                }), 
                message);
        }

    }

    public class GuestOS
    {
        private string oSName;

        public string OSName
        {
            get { return oSName; }
            private set { oSName = value; }
        }

        private string oSVersion;

        public string OSVersion
        {
            get { return oSVersion; }
            private set { oSVersion = value; }
        }

        private string cSDVersion;

        public string CSDVersion
        {
            get { return cSDVersion; }
            private set { cSDVersion = value; }
        }

        private string ipv4Address;

        public string IPv4Address
        {
            get { return ipv4Address; }
            private set { ipv4Address = value; }
        }

        public GuestOS(string name, string osVer, string csdVer, string ipAddr)
        {
            this.OSName = name;
            this.OSVersion = osVer;
            this.CSDVersion = csdVer;
            this.IPv4Address = ipAddr;
        }
    }
}