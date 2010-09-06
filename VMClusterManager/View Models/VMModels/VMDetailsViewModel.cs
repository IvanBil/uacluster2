using System;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using VMClusterManager.ViewModels.VMModels;

namespace VMClusterManager
{
    public class VMDetailsViewModel : VMViewModel
    {
        VMDetailsView view;
        VMModel vmModel;

        //private string vmName;
        private string vmstatusString;
        private BitmapImage thumbnailImage;
        private VMSnapshotViewModel [] vmSnapshots;

        public VMSnapshotViewModel [] VMSnapshots
        {
            get { return vmSnapshots; }
            private set { vmSnapshots = value; OnPropertyChanged("VMSnapshots"); }
        }

        private object selectedItem;

        public object SelectedSnapshotItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                vmModel.SelectedSnapshotItem = selectedItem;
                OnPropertyChanged("SelectedSnapshotItem");
            }
        }
        public BitmapImage ThumbnailImage
        {
            get { return thumbnailImage; }
            set
            {
                thumbnailImage = value; OnPropertyChanged("ThumbnailImage");
            }
        }

        public string VMStatusString
        {
            get { return vmstatusString; }
            set
            {
                vmstatusString = value;
                OnPropertyChanged("VMStatusString");
            }
        }

        private string vmDomainName;

        public string VMDomainName
        {
            get { return vmDomainName; }
            set { vmDomainName = value; OnPropertyChanged("VMDomainName"); }
        }

        private string csdVersion;

        public string CSDVersion
        {
            get { return csdVersion; }
            set { csdVersion = value; OnPropertyChanged("CSDVersion"); }
        }

        private string osName;

        public string OSName
        {
            get { return osName; }
            set { osName = value; OnPropertyChanged("OSName"); }
        }

        private string osVersion;

        public string OSVersion
        {
            get { return osVersion; }
            set { osVersion = value; OnPropertyChanged("OSVersion"); }
        }

        private string iPv4Address;

        public string IPv4Address
        {
            get { return iPv4Address; }
            set { iPv4Address = value; OnPropertyChanged("IPv4Address"); }
        }

        
        private Thread BackgroundThread;
        public VMDetailsViewModel(VM vm):base(vm)
        {
            view = new VMDetailsView();
            this.vmModel = VMModel.GetInstance();
            this.IsActiveVM = true;
            //lock (vmModel.ActiveVMList)
            //{
            //    vmModel.ActiveVMList.Clear();
            //    vmModel.ActiveVMList.Add(this.VirtualMachine);
            //}
            
            VMName = this.VirtualMachine.Name;
            vmstatusString = this.VirtualMachine.StatusString;
            VMDomainName = this.VirtualMachine.GetDomainName();
            GuestOS guestOS = this.VirtualMachine.GetGuestOS();
            if (guestOS == null)
                guestOS = new GuestOS (string.Empty, string.Empty,string.Empty, string.Empty);
            OSName = guestOS.OSName;
            OSVersion = guestOS.OSVersion;
            CSDVersion = guestOS.CSDVersion;
            IPv4Address = guestOS.IPv4Address;
            VMSnapshot snapshotTree = this.VirtualMachine.SnapshotTree;
            if (snapshotTree != null)
            {
                VMSnapshots = new VMSnapshotViewModel[] { new VMSnapshotViewModel(snapshotTree,this.VirtualMachine, vmModel) };
            }
            else
            {
                VMSnapshots = new  VMSnapshotViewModel []{};
            }

            foreach (VMSnapshotViewModel snapshotVM in VMSnapshots)
            {
                snapshotVM.Selected +=
                    (o, e) =>
                    {
                        this.SelectedSnapshotItem = snapshotVM.SnapshotInstance;
                    };
            }

           
            this.VirtualMachine.VMSnapshotsChanged +=
                (o, e) =>
                {
                    snapshotTree = this.VirtualMachine.SnapshotTree;
                    if (snapshotTree != null)
                    {
                        VMSnapshots = new VMSnapshotViewModel[] { new VMSnapshotViewModel(snapshotTree,this.VirtualMachine,vmModel) };
                    }
                };
            ThreadStart backgroundThreadStart = new ThreadStart(() =>
            {
                while (true)
                {

                    GCHandle pinnedArray = GCHandle.Alloc(this.VirtualMachine.MediumThumbnailImage, GCHandleType.Pinned);
                    IntPtr ptr = pinnedArray.AddrOfPinnedObject();
                    Bitmap BI = new Bitmap(160, 120, 320, System.Drawing.Imaging.PixelFormat.Format16bppRgb565, ptr);
                    MemoryStream st = new MemoryStream();
                    BI.Save(st, System.Drawing.Imaging.ImageFormat.Bmp);
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = new MemoryStream(st.ToArray());
                    bmp.EndInit();
                    bmp.Freeze();
                    ThumbnailImage = bmp;
                    Thread.Sleep(2000);
                    pinnedArray.Free();
                }
            });
            if ((this.VirtualMachine.Status != VMState.Unknown) && (this.VirtualMachine.Status != VMState.Disabled))
            {
                BackgroundThread = new Thread(backgroundThreadStart);
                BackgroundThread.IsBackground = true;
                BackgroundThread.Priority = ThreadPriority.Lowest;
                BackgroundThread.Start();
            }

            this.VirtualMachine.VMStatusChanged +=
               (o, e) =>
               {
                   this.VMName = this.VirtualMachine.Name;
                   this.VMStatusString = this.VirtualMachine.GetStatusString();
                   this.VMDomainName = this.VirtualMachine.GetDomainName();
                   guestOS = this.VirtualMachine.GetGuestOS();
                   if (guestOS == null)
                       guestOS = new GuestOS(string.Empty, string.Empty, string.Empty,string.Empty);
                   OSName = guestOS.OSName;
                   OSVersion = guestOS.OSVersion;
                   CSDVersion = guestOS.CSDVersion;
                   IPv4Address = guestOS.IPv4Address;
                   if ((this.VirtualMachine.Status != VMState.Unknown) && (this.VirtualMachine.Status != VMState.Disabled))
                   {
                       if (BackgroundThread != null)
                       {
                           if (!BackgroundThread.IsAlive)
                           {
                               BackgroundThread = new Thread(backgroundThreadStart);
                               BackgroundThread.IsBackground = true;
                               BackgroundThread.Priority = ThreadPriority.Lowest;
                               BackgroundThread.Start();
                           }
                       }
                       else
                       {
                           BackgroundThread = new Thread(backgroundThreadStart);
                           BackgroundThread.IsBackground = true;
                           BackgroundThread.Priority = ThreadPriority.Lowest;
                           BackgroundThread.Start();
                       }
                   }
                   else
                   {
                       if (BackgroundThread != null)
                       {
                           BackgroundThread.Abort();
                           BackgroundThread.Join(10);
                       }
                   }
               };
            view.SetViewModel(this);
        }

        //private string GetOSName(VM vm)
        //{
        //    string osName = string.Empty;
        //    GuestOS guestOS = vm.GetGuestOS();
        //    if (guestOS != null)
        //    {
        //        osName = string.Format("{0} version {1} {2}", guestOS.OSName, guestOS.OSVersion, guestOS.CSDVersion);
        //    }
        //    return osName;
        //}

        public VMDetailsView View
        {
            get { return view; }
        }

        public void Dispose()
        {
            base.Dispose();
            if (BackgroundThread != null)
            {
                this.BackgroundThread.Abort();
            }
        }

        ~VMDetailsViewModel()
        {
            Dispose();
        }

    }
}
