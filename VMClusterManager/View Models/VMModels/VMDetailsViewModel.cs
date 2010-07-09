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
        IVMModel vmModel;

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

        
        private Thread BackgroundThread;
        public VMDetailsViewModel(VM vm):base(vm)
        {
            view = new VMDetailsView();
            this.vmModel = VMModel.GetInstance();
            this.IsActiveVM = true;
            lock (vmModel.ActiveVMList)
            {
                vmModel.ActiveVMList.Clear();
                vmModel.ActiveVMList.Add(this.VirtualMachine);
            }
            
            VMName = this.VirtualMachine.Name;
            vmstatusString = this.VirtualMachine.StatusString;
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

        public VMDetailsView View
        {
            get { return view; }
        }

        public override void Dispose()
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
