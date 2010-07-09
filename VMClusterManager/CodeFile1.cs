using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Management;
using System.Windows;
namespace VMClusterManager
{
    public delegate void VMStatusChangedEventHandler(object sender, EventArgs e);

    public abstract class WMIEventWatcher
    {
        private ManagementEventWatcher ewatcher;
        public WMIEventWatcher()
        {

        }
    }
}