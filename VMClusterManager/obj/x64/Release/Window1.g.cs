﻿#pragma checksum "..\..\..\Window1.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "81A843D6A435FD322BD1382724B58BCE"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VMClusterManager;


namespace VMClusterManager {
    
    
    /// <summary>
    /// Window1
    /// </summary>
    public partial class Window1 : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\..\Window1.xaml"
        internal VMClusterManager.Window1 wMain;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\Window1.xaml"
        private System.Windows.Controls.HeaderedContentControl detailsPlaceholder;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\Window1.xaml"
        private System.Windows.Controls.StackPanel navigationMenu;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\Window1.xaml"
        internal System.Windows.Controls.Button btnHosts;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\Window1.xaml"
        internal System.Windows.Controls.Button button1;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\..\Window1.xaml"
        private System.Windows.Controls.HeaderedContentControl navigationTree;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\Window1.xaml"
        private System.Windows.Controls.HeaderedContentControl actionMenu;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/VMClusterManager;component/window1.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Window1.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.wMain = ((VMClusterManager.Window1)(target));
            
            #line 1 "..\..\..\Window1.xaml"
            this.wMain.Loaded += new System.Windows.RoutedEventHandler(this.wMain_Loaded);
            
            #line default
            #line hidden
            
            #line 5 "..\..\..\Window1.xaml"
            this.wMain.Closed += new System.EventHandler(this.wMain_Closed);
            
            #line default
            #line hidden
            return;
            case 2:
            this.detailsPlaceholder = ((System.Windows.Controls.HeaderedContentControl)(target));
            return;
            case 3:
            this.navigationMenu = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 4:
            this.btnHosts = ((System.Windows.Controls.Button)(target));
            
            #line 50 "..\..\..\Window1.xaml"
            this.btnHosts.Click += new System.Windows.RoutedEventHandler(this.btnHosts_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.button1 = ((System.Windows.Controls.Button)(target));
            
            #line 51 "..\..\..\Window1.xaml"
            this.button1.Click += new System.Windows.RoutedEventHandler(this.btnVM_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.navigationTree = ((System.Windows.Controls.HeaderedContentControl)(target));
            return;
            case 7:
            this.actionMenu = ((System.Windows.Controls.HeaderedContentControl)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
