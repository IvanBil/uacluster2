﻿#pragma checksum "..\..\..\Windows\AboutWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6D4955163B48F93EAB51288C709D3070"
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


namespace VMClusterManager.Windows {
    
    
    /// <summary>
    /// AboutWindow
    /// </summary>
    public partial class AboutWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 4 "..\..\..\Windows\AboutWindow.xaml"
        internal VMClusterManager.Windows.AboutWindow wndAbout;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\Windows\AboutWindow.xaml"
        internal System.Windows.Controls.RichTextBox rtbAbout;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\Windows\AboutWindow.xaml"
        internal System.Windows.Documents.Hyperlink OffSite;
        
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
            System.Uri resourceLocater = new System.Uri("/VMClusterManager;component/windows/aboutwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Windows\AboutWindow.xaml"
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
            this.wndAbout = ((VMClusterManager.Windows.AboutWindow)(target));
            return;
            case 2:
            this.rtbAbout = ((System.Windows.Controls.RichTextBox)(target));
            return;
            case 3:
            this.OffSite = ((System.Windows.Documents.Hyperlink)(target));
            
            #line 19 "..\..\..\Windows\AboutWindow.xaml"
            this.OffSite.Click += new System.Windows.RoutedEventHandler(this.OffSite_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 31 "..\..\..\Windows\AboutWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
