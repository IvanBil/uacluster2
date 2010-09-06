using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management;
using System.Windows;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Linq;

namespace VMClusterManager
{
    public class Utility
    {

        /// <summary>
        /// Common utility function to get a service oject
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static ManagementObject GetServiceObject(ManagementScope scope, string serviceName)
        {
            scope.Connect();
            ManagementPath wmiPath = new ManagementPath(serviceName);
            ManagementClass serviceClass = new ManagementClass(scope, wmiPath, null);
            EnumerationOptions EnumOpts = new EnumerationOptions();
            EnumOpts.ReturnImmediately = false;
            ManagementObjectCollection services = serviceClass.GetInstances(EnumOpts);

            ManagementObject serviceObject = null;

            foreach (ManagementObject service in services)
            {
                serviceObject = service;
            }
            return serviceObject;
        }

        public static ManagementObject GetHostSystemDevice(string deviceClassName, string deviceObjectElementName, ManagementScope scope)
        {
            string hostName = System.Environment.MachineName;
            ManagementObject systemDevice = GetSystemDevice(deviceClassName, deviceObjectElementName, hostName, scope);
            return systemDevice;
        }


        public static ManagementObject GetSystemDevice
        (
            string deviceClassName,
            string deviceObjectElementName,
            string vmGUID,
            ManagementScope scope)
        {
            ManagementObject systemDevice = null;
            ManagementObject computerSystem = Utility.GetTargetComputer(vmGUID, scope);

            ManagementObjectCollection systemDevices = computerSystem.GetRelated
            (
                deviceClassName,
                "Msvm_SystemDevice",
                null,
                null,
                "PartComponent",
                "GroupComponent",
                false,
                null
            );

            foreach (ManagementObject device in systemDevices)
            {
                if (device["ElementName"].ToString().ToLower() == deviceObjectElementName.ToLower())
                {
                    systemDevice = device;
                    break;
                }
            }

            return systemDevice;
        }



        public static bool JobCompleted(ManagementBaseObject outParams, ManagementScope scope,out UInt32 ErrorCode)
        {
            bool jobCompleted = false;

            //Retrieve msvc_StorageJob path. This is a full wmi path
            string JobPath = (string)outParams["Job"];
            ManagementObject Job = new ManagementObject(scope, new ManagementPath(JobPath), null);
            //Try to get storage job information
            try
            {
                Job.Get();
                jobCompleted = true;
                while ((UInt16)Job["JobState"] == JobState.Starting
                    || (UInt16)Job["JobState"] == JobState.Running)
                {
                    System.Threading.Thread.Sleep(1000);
                    Job.Get();
                }

                //Figure out if job failed
                UInt16 jobState = (UInt16)Job["JobState"];
                ErrorCode = 0;
                if (jobState != JobState.Completed)
                {
                    UInt16 jobErrorCode = (UInt16)Job["ErrorCode"];
                    string errorDescriprion = (string)Job["ErrorDescription"];

                    jobCompleted = false;
                    ErrorCode = jobErrorCode;
                    throw new Exception(errorDescriprion);
                }
            }
            catch (Exception)
            {
                ErrorCode = ReturnCode.Unknown;
            }
            return jobCompleted;
        }


        public static ManagementObject GetTargetComputer(string vmGUID, ManagementScope scope)
        {
            string query = string.Format("select * from Msvm_ComputerSystem Where Name = '{0}'", vmGUID);

            //if (scope.Options == null)
            //{
            //    ConnectionOptions conn = new ConnectionOptions();
            //    conn.Username = @"mscluster\Administrator";
            //    conn.Password = @"P@ssw0rd";
            //}
            EnumerationOptions EnumOpts = new EnumerationOptions();
            EnumOpts.ReturnImmediately = false;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, new ObjectQuery(query),EnumOpts);

            ManagementObjectCollection computers = searcher.Get();

            ManagementObject computer = null;

            foreach (ManagementObject instance in computers)
            {
                computer = instance;
                break;
            }
            return computer;
        }

        public static ManagementObject GetVirtualSystemSettingData(ManagementObject vm)
        {
            ManagementObject vmSetting = null;
            EnumerationOptions EnumOpts = new EnumerationOptions();
            EnumOpts.ReturnImmediately = false;
            ManagementObjectCollection vmSettings = vm.GetRelated
            (
                "Msvm_VirtualSystemSettingData",
                "Msvm_SettingsDefineState",
                null,
                null,
                "SettingData",
                "ManagedElement",
                false,
                EnumOpts
            );

            if (vmSettings.Count != 1)
            {
                throw new Exception(String.Format("{0} instance of Msvm_VirtualSystemSettingData was found", vmSettings.Count));
            }

            foreach (ManagementObject instance in vmSettings)
            {
                vmSetting = instance;
                break;
            }

            return vmSetting;
        }
        /// <summary>
        /// Gets the  snapshot that is the parent to the virtual system current settings
        /// </summary>
        /// <param name="vm">Virtual system instance</param>
        /// <returns></returns>
        public static ManagementObject GetLastAppliedVirtualSystemSnapshot(ManagementObject vm)
        {
            EnumerationOptions EnumOpts = new EnumerationOptions();
            EnumOpts.ReturnImmediately = false;
            ManagementObjectCollection settings = vm.GetRelated(
                "Msvm_VirtualSystemsettingData",
                "Msvm_PreviousSettingData",
                null,
                null,
                "SettingData",
                "ManagedElement",
                false,
                EnumOpts);
            ManagementObject virtualSystemsetting = null;
            if (settings != null)
            {
                foreach (ManagementObject setting in settings)
                {
                    virtualSystemsetting = setting;
                }

            }
            return virtualSystemsetting;
        }


        enum ValueRole
        {
            Default = 0,
            Minimum = 1,
            Maximum = 2,
            Increment = 3
        }
        enum ValueRange
        {
            Default = 0,
            Minimum = 1,
            Maximum = 2,
            Increment = 3
        }


        //
        // Get RASD definitions
        //
        public static ManagementObject GetResourceAllocationsettingDataDefault
        (
            ManagementScope scope,
            UInt16 resourceType,
            string resourceSubType,
            string otherResourceType
            )
        {
            ManagementObject RASD = null;

            string query = String.Format("select * from Msvm_ResourcePool where ResourceType = '{0}' and ResourceSubType ='{1}' and OtherResourceType = '{2}'",
                             resourceType, resourceSubType, otherResourceType);

            if (resourceType == ResourceType.Other)
            {
                query = String.Format("select * from Msvm_ResourcePool where ResourceType = '{0}' and ResourceSubType = null and OtherResourceType = {1}",
                                             resourceType, otherResourceType);
            }
            else
            {
                query = String.Format("select * from Msvm_ResourcePool where ResourceType = '{0}' and ResourceSubType ='{1}' and OtherResourceType = null",
                                             resourceType, resourceSubType);
            }

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, new ObjectQuery(query));

            ManagementObjectCollection poolResources = searcher.Get();

            //Get pool resource allocation ability
            if (poolResources.Count == 1)
            {
                foreach (ManagementObject poolResource in poolResources)
                {
                    ManagementObjectCollection allocationCapabilities = poolResource.GetRelated("Msvm_AllocationCapabilities");
                    foreach (ManagementObject allocationCapability in allocationCapabilities)
                    {
                        ManagementObjectCollection settingDatas = allocationCapability.GetRelationships("Msvm_SettingsDefineCapabilities");
                        foreach (ManagementObject settingData in settingDatas)
                        {

                            if (Convert.ToInt16(settingData["ValueRole"]) == (UInt16)ValueRole.Default)
                            {
                                RASD = new ManagementObject(settingData["PartComponent"].ToString());
                                break;
                            }
                        }
                    }
                }
            }

            return RASD;
        }


        public static ManagementObject GetResourceAllocationsettingData
        (
            ManagementObject vm,
            UInt16 resourceType,
            string resourceSubType,
            string otherResourceType
            )
        {
            //vm->vmsettings->RASD for IDE controller
            ManagementObject RASD = null;
            ManagementObjectCollection settingDatas = vm.GetRelated("Msvm_VirtualSystemsettingData");
            foreach (ManagementObject settingData in settingDatas)
            {
                //retrieve the rasd
                ManagementObjectCollection RASDs = settingData.GetRelated("Msvm_ResourceAllocationsettingData");
                foreach (ManagementObject rasdInstance in RASDs)
                {
                    if (Convert.ToUInt16(rasdInstance["ResourceType"]) == resourceType)
                    {
                        //found the matching type
                        if (resourceType == ResourceType.Other)
                        {
                            if (rasdInstance["OtherResourceType"].ToString() == otherResourceType)
                            {
                                RASD = rasdInstance;
                                break;
                            }
                        }
                        else
                        {
                            if (rasdInstance["ResourceSubType"].ToString() == resourceSubType)
                            {
                                RASD = rasdInstance;
                                break;
                            }
                        }
                    }
                }

            }
            return RASD;
        }

        public static List<ManagementObject> GetRASD(VM _vm)
        {
            //ManagementObject RASD = null;
            ManagementObject vm = _vm.Instance;
            ManagementObjectCollection settingDatas = vm.GetRelated("Msvm_VirtualSystemsettingData");
            List<ManagementObject> RASDList = new List<ManagementObject>();
            foreach (ManagementObject settingData in settingDatas)
            {
                //retrieve the rasd
                ManagementObjectCollection RASDs = settingData.GetRelated("Msvm_ResourceAllocationsettingData");
                foreach (ManagementObject rasdInstance in RASDs)
                {
                    RASDList.Add(rasdInstance);
                }

            }
            return RASDList;
        }


        public static VMJob RequestStateChange(VM vm, int state)
        {
            ManagementScope scope = new ManagementScope(@"\\" + vm.Host.Name + @"\root\virtualization", vm.Host.HostConnectionOptions);
            ManagementObject vmObj = vm.Instance;//Utility.GetTargetComputer(vm.GUID, scope);
            ManagementBaseObject inParams = vmObj.GetMethodParameters("RequestStateChange");
            inParams["RequestedState"] = state;
            ManagementBaseObject outParams = vmObj.InvokeMethod("RequestStateChange", inParams, null);
            UInt32 returnValue = (UInt32)outParams["ReturnValue"];
            string jobPath = (string)outParams["Job"];
            VMJob ThisJob = null;
            if (jobPath != null)
            {
                ThisJob = new VMJob(new ManagementObject(scope, new ManagementPath(jobPath), null));
            }
          
            inParams.Dispose();
            outParams.Dispose();
            vmObj.Dispose();
            if ((jobPath == null) && (returnValue != ReturnCode.Completed) && (returnValue != ReturnCode.Started))
            {
                throw new VMStateChangeException(returnValue);
            }
            //JobCompleted(outParams, scope, out ErrorCode);
            return ThisJob;
        }

        public static uint ShutdownVM(VM vm)
        {
            ManagementScope scope = new ManagementScope(@"\\" + vm.Host.Name + @"\root\virtualization", vm.Host.HostConnectionOptions);
            ManagementObject vmObj = vm.Instance;
            //Use the association to get the msvm_shutdowncomponent for the msvm_computersystem
            ManagementObjectCollection collection = vmObj.GetRelated("Msvm_ShutdownComponent");
            ManagementObjectCollection.ManagementObjectEnumerator enumerator = collection.GetEnumerator();
            enumerator.MoveNext();
            ManagementObject msvm_shutdowncomponent = (ManagementObject)enumerator.Current;
            ManagementBaseObject inParams = msvm_shutdowncomponent.GetMethodParameters("InitiateShutdown");
            inParams["Force"] = true;
            inParams["Reason"] = "Need to Shutdown";

            //Invoke the Method
            ManagementBaseObject outParams = msvm_shutdowncomponent.InvokeMethod("InitiateShutdown", inParams, null);
            uint returnValue = (uint)outParams["ReturnValue"];
            //VMJob ThisJob = new VMJob(new ManagementObject(scope, new ManagementPath((string)outParams["Job"]), null));
            return returnValue;
        }


        public static ObservableCollection<VM> GetHostVMCollection(VMHost host)
        {
            //Query all properties from Msvm_ComputerSystem class
            ObjectQuery queryObj = new ObjectQuery("SELECT * FROM Msvm_ComputerSystem");
            ConnectionOptions connOpts = GetHostConnectionOptions(host);
            //Making connection to virtualization namespace
            //ManagementScope manScope = new ManagementScope(@"\\" + host.Name + @"\root\virtualization", connOpts);
            ManagementScope manScope = new ManagementScope(@"\\" + host.Name + @"\root\virtualization", connOpts);
            // connect and set up our search
            EnumerationOptions EnumOpts = new EnumerationOptions ();
            //Ensure to return complete collection
            EnumOpts.ReturnImmediately = false;
            //EnumOpts.Timeout = TimeSpan.FromMilliseconds(100);
            ManagementObjectSearcher vmSearcher = new ManagementObjectSearcher(manScope, queryObj, EnumOpts);
            ObservableCollection<VM> vmCollection = new ObservableCollection<VM> ();
            //ManagementOperationObserver watcher = new ManagementOperationObserver();
            try
            {
                // get the collection of computer system objects
                ManagementObjectCollection ManObjCollection = vmSearcher.Get();
                if (ManObjCollection != null)
                {
                    foreach (ManagementObject vmobj in ManObjCollection)
                    {
                        // only copy details of the Virtual Machines
                        //MSDN:A textual description of the object. This property is inherited from CIM_ManagedElement 
                        //and it is set to "Microsoft Virtual Computer System" if the instance represents a VM or "Microsoft Hosting Computer System" 
                        //if the instance represents the host system.
                        if (vmobj["Caption"].ToString().Contains("Virtual Machine"))
                        {

                            // capture the machine name, status and type
                            //MSDN:Caption - A short textual description (one-line string) of the object. This property is inherited 
                            //from CIM_ManagedElement and it is set to "Virtual Machine" if the instance represents a VM 
                            //or "Hosting Computer System" if the instance represents the host system.
                            //VM vm = new VM(host, vmobj["Name"].ToString());
                            //ManagementObject[] OutPut = n;
                            VM vm = new VM(host, vmobj);
                            vmCollection.Add(vm);
                        }
                    }
                }
                //String strTargetVM = "*";
                // loop through the machines


            }
            catch (COMException ex)
            {
                //MessageBox.Show(ex.Message, ex.ErrorCode.ToString());
                throw new RPCCallException(host, ex);
            }
            
            return vmCollection;
        }

        public static UInt16 GetVMState(VM vm)
        {
            
            //ManagementScope scope = new ManagementScope(@"\\" + vm.Host.Name + @"\root\virtualization", GetHostConnectionOptions(vm.Host));
            //ManagementScope scope = new ManagementScope(@"root\virtualization", null);
            ManagementObject vmObj = vm.Instance;//Utility.GetTargetComputer(vm.GUID, scope);
            return (UInt16)vmObj["EnabledState"];
        }

        public static TimeSpan GetVMUptime(VM vm)
        {
            //ManagementScope scope = new ManagementScope(@"\\" + vm.Host.Name + @"\root\virtualization", GetHostConnectionOptions(vm.Host));
            //ManagementScope scope = new ManagementScope(@"root\virtualization", null);
            ManagementObject vmObj = vm.Instance;//Utility.GetTargetComputer(vm.GUID, scope);
            long ticks = Convert.ToInt64((UInt64)vmObj["OnTimeInMilliseconds"])*10000;//Convert.ToDateTime((UInt64)vmObj["OnTimeInMilliseconds"] / 100;
            TimeSpan ts = new TimeSpan(ticks);
            return new TimeSpan(ts.Days, ts.Hours, ts.Minutes, ts.Seconds);

        }

        public static ConnectionOptions GetHostConnectionOptions(VMHost host)
        {
            //ConnectionOptions opt = new ConnectionOptions();
            //opt.Password = host.Password;
            //opt.Username = host.UserName;
            return host.HostConnectionOptions;
        }

        public static VMJob CreateVirtualSystemSnapshot(VM vm)
        {
            ManagementScope scope = new ManagementScope(@"\\" + vm.Host.Name + @"\root\virtualization", vm.Host.HostConnectionOptions);
            
            ManagementObject virtualSystemService = Utility.GetServiceObject(scope, "Msvm_VirtualSystemManagementService");

            ManagementObject vmObj = Utility.GetTargetComputer(vm.GUID, scope);

            ManagementBaseObject inParams = virtualSystemService.GetMethodParameters("CreateVirtualSystemSnapshot");
            inParams["SourceSystem"] = vmObj.Path.Path;

            ManagementBaseObject outParams = virtualSystemService.InvokeMethod("CreateVirtualSystemSnapshot", inParams, null);
            //UInt32 returnValue = (UInt32)outParams["ReturnValue"];
            VMJob ThisJob = new VMJob(new ManagementObject(scope, new ManagementPath((string)outParams["Job"]), null));
            inParams.Dispose();
            outParams.Dispose();
            vmObj.Dispose();
            virtualSystemService.Dispose();
            return ThisJob;
        }

        public static VMJob ApplyVirtualSystemSnapshot(VM vm, VMSnapshot snapshot)
        {
            ManagementScope scope = new ManagementScope(@"\\" + vm.Host.Name + @"\root\virtualization", vm.Host.HostConnectionOptions);
            ManagementObject virtualSystemService = Utility.GetServiceObject(scope, "Msvm_VirtualSystemManagementService");
            ManagementBaseObject inParams = virtualSystemService.GetMethodParameters("ApplyVirtualSystemSnapshotEx");
            ManagementObject vmObj = Utility.GetTargetComputer(vm.GUID, scope);
            ManagementObject vmSnapshotObj = snapshot.SnapshotInstance;
            inParams["SnapshotSettingData"] = vmSnapshotObj.Path.Path;
            inParams["ComputerSystem"] = vmObj.Path.Path;
            ManagementBaseObject outParams = virtualSystemService.InvokeMethod("ApplyVirtualSystemSnapshotEx", inParams, null);
            VMJob ThisJob = new VMJob(new ManagementObject(scope, new ManagementPath((string)outParams["Job"]), null));
            inParams.Dispose();
            outParams.Dispose();
            vmSnapshotObj.Dispose();
            vmObj.Dispose();
            virtualSystemService.Dispose();
            return ThisJob;
        }

        public static VMJob RemoveVirtualSystemSnapshot(VM vm, VMSnapshot snapshot)
        {
            ManagementScope scope = new ManagementScope(@"\\" + vm.Host.Name + @"\root\virtualization", vm.Host.HostConnectionOptions);
            ManagementObject virtualSystemService = Utility.GetServiceObject(scope, "Msvm_VirtualSystemManagementService");
            ManagementBaseObject inParams = virtualSystemService.GetMethodParameters("RemoveVirtualSystemSnapshot");
            ManagementObject vmObj = Utility.GetTargetComputer(vm.GUID, scope);
            ManagementObject vmSnapshotObj = snapshot.SnapshotInstance;
            inParams["SnapshotSettingData"] = vmSnapshotObj.Path.Path;
            ManagementBaseObject outParams = virtualSystemService.InvokeMethod("RemoveVirtualSystemSnapshot", inParams, null);
            VMJob ThisJob = new VMJob(new ManagementObject(scope, new ManagementPath((string)outParams["Job"]), null));
            
            inParams.Dispose();
            outParams.Dispose();
            vmSnapshotObj.Dispose();
            vmObj.Dispose();
            virtualSystemService.Dispose();
            return ThisJob;
        }

        public static VMJob RemoveVirtualSystemSnapshotTree(VM vm, VMSnapshot snapshot)
        {
            ManagementScope scope = new ManagementScope(@"\\" + vm.Host.Name + @"\root\virtualization", vm.Host.HostConnectionOptions);
            ManagementObject virtualSystemService = Utility.GetServiceObject(scope, "Msvm_VirtualSystemManagementService");
            ManagementBaseObject inParams = virtualSystemService.GetMethodParameters("RemoveVirtualSystemSnapshotTree");
            ManagementObject vmObj = Utility.GetTargetComputer(vm.GUID, scope);
            ManagementObject vmSnapshotObj = snapshot.SnapshotInstance;
            inParams["SnapshotSettingData"] = vmSnapshotObj.Path.Path;
            ManagementBaseObject outParams = virtualSystemService.InvokeMethod("RemoveVirtualSystemSnapshotTree", inParams, null);
            VMJob ThisJob = new VMJob(new ManagementObject(scope, new ManagementPath((string)outParams["Job"]), null));
            inParams.Dispose();
            outParams.Dispose();
            vmSnapshotObj.Dispose();
            vmObj.Dispose();
            virtualSystemService.Dispose();
            return ThisJob;
        }

        public static ManagementBaseObject GetSummaryInformation(VM vm, UInt32[] requestedInformation)
        {
            ManagementScope scope = new ManagementScope(@"\\" + vm.Host.Name + @"\root\virtualization", vm.Host.HostConnectionOptions);
            ManagementObject vmObj = vm.Instance;//GetTargetComputer(vm.GUID, scope);
            ManagementObject[] virtualSystemSettings = new ManagementObject[] { Utility.GetVirtualSystemSettingData(vmObj) };
            ManagementObject virtualSystemService = Utility.GetServiceObject(scope, "Msvm_VirtualSystemManagementService"); 
            ManagementBaseObject inParams = virtualSystemService.GetMethodParameters("GetSummaryInformation");
            string[] settingPathes = new string[virtualSystemSettings.Length];
            for (int i = 0; i < settingPathes.Length; ++i)
            {
                settingPathes[i] = virtualSystemSettings[i].Path.Path;
            }

            inParams["SettingData"] = settingPathes;
            inParams["RequestedInformation"] = requestedInformation;

            ManagementBaseObject outParams = virtualSystemService.InvokeMethod("GetSummaryInformation", inParams, null);
            if ((UInt32)outParams["ReturnValue"] == ReturnCode.Completed)
            {
                ManagementBaseObject[] summaryInformationArray = (ManagementBaseObject[])outParams["SummaryInformation"];
                return summaryInformationArray[0];
            }
            else
            {
                MessageBox.Show("Failed to retrieve virtual system summary information","Error!",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            inParams.Dispose();
            outParams.Dispose();
            return null;

        }
        /// <summary>
        /// Converts string returned from management object as Date and Time of something into .NET DateTime
        /// </summary>
        /// <param name="creationTimeString"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime (string dmtfDate)
        {
            //return new DateTime (
            //    Convert.ToInt32(creationTimeString.Substring(0,4)),
            //    Convert.ToInt32(creationTimeString.Substring(4,2)),
            //    Convert.ToInt32(creationTimeString.Substring(6,2)),
            //    Convert.ToInt32(creationTimeString.Substring(8,2)),
            //    Convert.ToInt32(creationTimeString.Substring(10,2)),
            //    Convert.ToInt32(creationTimeString.Substring(12,2))
            //    );
            return ManagementDateTimeConverter.ToDateTime(dmtfDate);
        }

        public static ushort GetVMProcessorLoad(VM vm)
        {
            ManagementBaseObject procLoadObj = Utility.GetSummaryInformation(vm, new uint[] { VMRequestedInformation.ProcessorLoad });
            UInt16 load = 0;
            if (procLoadObj["ProcessorLoad"] != null)
                load = (UInt16)procLoadObj["ProcessorLoad"];
            return load;
        }

        public static VMSnapshot GetVMSnapshotTree(VM vm)
        {
            //Getting the Management object containing snapshots array of current VM
           // ManagementBaseObject snapshots = Utility.GetSummaryInformation(vm, new uint[] { VMRequestedInformation.Snapshots });
            ManagementObjectCollection VirtualSystemSettingsCollection = vm.Instance.GetRelated("Msvm_VirtualSystemSettingData");
            
            VMSnapshot SnapshotTree = null;
            try
            {
                //Getting snapshots array of current VM
                //ManagementBaseObject[] VirtualSystemSettingsCollection = (ManagementBaseObject[])snapshots["Snapshots"];
                if (VirtualSystemSettingsCollection != null)
                {
                    Dictionary<string, VMSnapshot> AvailableSnapshots = new Dictionary<string,VMSnapshot> ();
                    //Building snapshots tree
                    foreach (ManagementObject snapshotsObj in VirtualSystemSettingsCollection)
                    {
                        //Check if the setting is snapshot (SettingType = 5). If the setting represents current settings, then SettingType = 3
                        if ((UInt16)snapshotsObj["SettingType"] == 5)
                        {
                            //current VMSnapshots instance
                            VMSnapshot child;
                            //check if instance of this snapshot is not already exist
                            if (!AvailableSnapshots.ContainsKey((string)snapshotsObj["InstanceID"]))
                            {
                                child = new VMSnapshot(snapshotsObj);
                                //Adding new snapshot instance to the Dictionary
                                AvailableSnapshots.Add((string)child.SnapshotInstance["InstanceID"], child);
                            }
                            else
                            {
                                //this snapshot is already exist in the Dictionary
                                child = AvailableSnapshots[(string)snapshotsObj["InstanceID"]];
                            }
                            if (snapshotsObj["Parent"] == null)
                            {
                                //Current snapshot is root snapshot
                                SnapshotTree = child;
                            }
                            else
                            {
                                //Current Snapshot is not a root snapshot
                                ManagementObject parentObj = new ManagementObject((string)snapshotsObj["Parent"]);
                                if (!AvailableSnapshots.ContainsKey((string)parentObj["InstanceID"]))
                                {
                                    //Parent VMSnapshot snapshot instance is not already present. So we create it
                                    VMSnapshot parent = new VMSnapshot(parentObj);
                                    //.. and add it to the Dictionary
                                    AvailableSnapshots.Add((string)parentObj["InstanceID"], parent);
                                }
                                ///Adding current child to children array of parent snapshot if child is not already exist. This because
                                ///GetRelated() function returns some snapshots several times as snapshot, last applied snapshot
                                ///and previous settings
                                if (!AvailableSnapshots[(string)parentObj["InstanceID"]].ChildSnapshots.Contains(child))
                                AvailableSnapshots[(string)parentObj["InstanceID"]].AddChild(child);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GetVMSnapshotTree");
            }
            return SnapshotTree;
        }

        public static ManagementObject GetMemory(VM vm)
        {
            ManagementObject mem = null;
            ManagementObject vmObj = vm.Instance;
            ManagementObjectCollection settings = vmObj.GetRelated("Msvm_VirtualSystemsettingData");
            foreach (ManagementObject setting in settings)
            {
                ManagementObjectCollection memSettings = setting.GetRelated("Msvm_MemorySettingData");
                foreach (ManagementObject memsetting in memSettings)
                {
                    mem = memsetting;
                }
            }
            return mem;
        }

        /// <summary>
        /// Sets amount of RAM for virtual machine 
        /// </summary>
        /// <param name="vm">Target Virtual Machine</param>
        /// <param name="quantity">Amount of memory</param>
        /// <returns>VMJob object. If returns null VMResourcesUpdateException is thrown in case of error occures.</returns>
        public static VMJob SetMemory(VM vm, UInt64 quantity)
        {
            return SetVMResource(vm, GetMemory(vm), new Dictionary<string, object> { { "VirtualQuantity", quantity } });
        }

        public static VMJob SetVMResource(VM vm, ManagementObject resource, Dictionary<string, object> Params)
        {
            ManagementScope scope = new ManagementScope(@"\\" + vm.Host.Name + @"\root\virtualization", vm.Host.HostConnectionOptions);
            ManagementObject virtualSystemService = Utility.GetServiceObject(scope, "Msvm_VirtualSystemManagementService");
            ManagementBaseObject inParams = virtualSystemService.GetMethodParameters("ModifyVirtualSystemResources");
            ManagementObject vmObj = vm.Instance;
            //ManagementObject resource = GetMemory(vm);
            //memSettings["Limit"] = quantity;
            //memSettings["Reservation"] = quantity;
            foreach (string key in Params.Keys)
            {
                resource[key] = Params[key];
            }
            //resource["VirtualQuantity"] = quantity;
            string[] Resources = new string[1];
            Resources[0] = resource.GetText(TextFormat.CimDtd20);
            //ManagementBaseObject inParams = virtualSystemService.GetMethodParameters("ModifyVirtualSystemResources");
            inParams["ComputerSystem"] = vmObj.Path.Path;
            inParams["ResourcesettingData"] = Resources;
            VMJob ThisJob = null;
            ManagementBaseObject outParams = virtualSystemService.InvokeMethod("ModifyVirtualSystemResources", inParams, null);
            //MessageBox.Show(outParams.GetText(TextFormat.Mof));
            string jobPath = (string)outParams["Job"];
            UInt32 returnValue = (UInt32)outParams["ReturnValue"];
            if (jobPath != null)
            {
                ManagementObject jobObj = new ManagementObject(scope, new ManagementPath(jobPath), null);
                ThisJob = new VMJob(jobObj);
            }
            inParams.Dispose();
            //resource.Dispose();
            outParams.Dispose();
            vmObj.Dispose();

            virtualSystemService.Dispose();
            if ((jobPath == null) && (returnValue != ReturnCode.Completed) && (returnValue != ReturnCode.Started))
            {
                throw new VMResourcesUpdateException(returnValue);
            }
            return ThisJob;
        }

        public static ManagementObject GetProcessor(VM vm)
        {
            ManagementObject proc = null;
            ManagementObject vmObj = vm.Instance;
            ManagementObjectCollection settings = vmObj.GetRelated("Msvm_VirtualSystemsettingData");
            foreach (ManagementObject setting in settings)
            {
                ManagementObjectCollection memSettings = setting.GetRelated("Msvm_ProcessorSettingData");
                foreach (ManagementObject memsetting in memSettings)
                {
                    proc = memsetting;
                }
            }
            return proc;
        }

        public static VMJob SetProcessor(VM vm, UInt64 quantity)
        {
            return SetVMResource(vm, GetProcessor(vm), new Dictionary<string, object> { { "VirtualQuantity", quantity } });
        }

        public static IDictionary<string, string> GetKvpItems(VM vm)
        {
            ManagementObject vmObj = vm.Instance;
            ManagementObjectCollection exchCompColl = vmObj.GetRelated("Msvm_KvpExchangeComponent");
            //string dnsName = null;
            Dictionary<string, string> kvpDict = new Dictionary<string, string>();
            foreach (ManagementObject exchComp in exchCompColl)
            {
                string[] kvpstrings = (string[])exchComp["GuestIntrinsicExchangeItems"];
                foreach (string kvpstring in kvpstrings)
                {
                    //kvpstring contains XML representation of Msvm_KvpExchangeDataItem object.
                    XElement xkvp = XElement.Parse(kvpstring);
                    kvpDict.Add(xkvp.Elements("PROPERTY").Single(p => p.Attribute("NAME").Value == "Name").Element("VALUE").Value, xkvp.Elements("PROPERTY").Single(p => p.Attribute("NAME").Value == "Data").Element("VALUE").Value);
                    //test if Name property of Msvm_KvpExchangeDataItem object is FullyQualifiedDomainName
                    //if (xkvp.Elements("PROPERTY").Any(p => p.Attribute("NAME").Value == "Name" && p.Element("VALUE").Value == "FullyQualifiedDomainName"))
                    //{
                    //    //get Fully Qualified DomainName from Value of Data property
                    //    dnsName = xkvp.Elements("PROPERTY").Single(p => p.Attribute("NAME").Value == "Data").Element("VALUE").Value;
                    //    break;
                    //}

                    
                }
                break;
            }
            return kvpDict;
        }

        public static string GetFullyQualifiedDomainName(VM vm)
        {
            //ManagementObject vmObj = vm.Instance;
            //ManagementObjectCollection exchCompColl = vmObj.GetRelated("Msvm_KvpExchangeComponent");
            //string dnsName = null;
            //foreach (ManagementObject exchComp in exchCompColl)
            //{
            //    string[] kvpstrings = (string[])exchComp["GuestIntrinsicExchangeItems"];
            //    foreach (string kvpstring in kvpstrings)
            //    {
            //        //kvpstring contains XML representation of Msvm_KvpExchangeDataItem object.
            //        XElement xkvp = XElement.Parse(kvpstring);

            //        //test if Name property of Msvm_KvpExchangeDataItem object is FullyQualifiedDomainName
            //        if (xkvp.Elements("PROPERTY").Any(p => p.Attribute("NAME").Value == "Name" && p.Element("VALUE").Value == "FullyQualifiedDomainName"))
            //        {
            //            //get Fully Qualified DomainName from Value of Data property
            //            dnsName = xkvp.Elements("PROPERTY").Single(p => p.Attribute("NAME").Value == "Data").Element("VALUE").Value;
            //            break;
            //        }

            //        //var query = from val
            //        //                in
            //        //                (from prop in xkvp.Elements() where (prop.Attribute("Name").Value == "Name") select prop)
            //        //            select val.Element("Value").Value;
            //        //foreach (string NameValue in query)
            //        //{
            //        //    if (NameValue == "FullyQualifiedDomainName")
            //        //    {
            //        //    }
            //        //}
            //        //if (xkvp.Elements() == "FullyQualifiedDomainName")
            //        //{
            //        //    dnsName = (string)kvp["Data"];
            //        //    break;
            //        //}
            //    }
            //    break;
            //}
            string retVal = null;
            try
            {
                retVal = Utility.GetKvpItems(vm)["FullyQualifiedDomainName"];
            }
            catch (Exception ex) { };
            return retVal;
        }

        // #region HOST UTILITIES

        //public static UInt64 GetHostMemoryAmount

        //#endregion
    }

   


}
