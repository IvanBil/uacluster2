using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Xml.Linq;

namespace VMClusterManager
{
    public class DataReceiver
    {
        public List<VMHost> GetHostListFromFile(string path)
        {
            StreamReader SR = File.OpenText(path);
            List<VMHost> hostList = new List<VMHost> ();
            string hostName = SR.ReadLine();
            //AutoResetEvent signal = new AutoResetEvent(false);
            //int counter = 0;
            while (hostName != null)
            {
                if (hostName[0] != '#')
                {
                    string hstName = hostName;
                    VMHost host = new VMHost((string)hstName);
                    hostList.Add(host);
                    //ThreadPool.QueueUserWorkItem(delegate(object o)
                    //{
                    //    Interlocked.Increment(ref counter);
                    //    VMHost host = new VMHost((string)hstName);
                    //    lock (hostList)
                    //    {
                    //        hostList.Add(host);
                    //    }
                        
                    //    if (Interlocked.Decrement(ref counter)==0)
                    //    {
                    //        signal.Set();
                    //    }
                    //}, hstName);
                }
                hostName = SR.ReadLine();
            }
            //signal.WaitOne();
            return hostList;
        }

        public void FillVMHostTree(object dataObject, VMHostGroup root)
        {
            if (dataObject is XElement)
            {
                XElement hostTree = dataObject as XElement;
                foreach (XElement xHost in hostTree.Elements("Host"))
                {
                    root.AddHost(new VMHost(xHost.Attribute("Name").Value));
                }
                if (root.ChildGroups != null)
                {
                    foreach (VMHostGroup childgroup in root.ChildGroups)
                    {
                        FillVMHostTree(childgroup.DataObject, childgroup);
                    }
                }
            }
        }

        public VMHostGroup FillVMHostGroupAsync (string path, VMHostGroup hostGroup)
        {
            StreamReader SR = File.OpenText(path);
            string hostName = SR.ReadLine();
            while (hostName != null)
            {
                if (hostName[0] != '#')
                {
                    Thread HostAdderThead = new Thread(new ParameterizedThreadStart (CreateVMHost));
                    HostAdderThead.Start(new object[] { hostName, hostGroup });
                }
                hostName = SR.ReadLine();
            }
            return hostGroup; 
        }

        private void CreateVMHost(object args)
        {
            VMHost host = new VMHost((args as object [])[0] as string);
            VMHostGroup group = (args as object[])[1] as VMHostGroup;
            if (host != null)
                lock(group)
                {
                    group.AddHost(host);
                }
        }


        

        public static VMGroup FindParentForVM(VM vm, object dataObject, VMGroup root)
        {
            VMGroup parent = null;
            if (dataObject is XElement)
            {
                XElement VMTree = dataObject as XElement;
                
                if ((vm != null) && (VMTree != null))
                {
                    IEnumerable<XElement> children = VMTree.Descendants();
                    var query = from el in VMTree.Descendants() where el.Attribute("GUID").Value == vm.GUID.ToString() select el;
                    foreach (XElement xVM in query)
                    {
                        parent = Group.GetElementByID(xVM.Parent.Attribute("GUID").Value, root) as VMGroup;
                        break;
                    }

                }
            }
            return parent;
        }
    }
}
