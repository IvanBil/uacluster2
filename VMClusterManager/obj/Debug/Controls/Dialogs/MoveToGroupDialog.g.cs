﻿#pragma checksum "..\..\..\..\Controls\Dialogs\MoveToGroupDialog.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C9DD415A614E1CD5FFD9F9FCC9845DAB"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3603
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


namespace VMClusterManager.Controls.Dialogs {
    
    
    /// <summary>
    /// MoveToGroupDialog
    /// </summary>
    public partial class MoveToGroupDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 18 "..\..\..\..\Controls\Dialogs\MoveToGroupDialog.xaml"
        internal System.Windows.Controls.TreeView treeElements;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\..\Controls\Dialogs\MoveToGroupDialog.xaml"
        internal System.Windows.Controls.Button btnOK;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\Controls\Dialogs\MoveToGroupDialog.xaml"
        internal System.Windows.Controls.Button btnCansel;
        
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
            System.Uri resourceLocater = new System.Uri("/VMClusterManager;component/controls/dialogs/movetogroupdialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Controls\Dialogs\MoveToGroupDialog.xaml"
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
            this.treeElements = ((System.Windows.Controls.TreeView)(target));
            
            #line 19 "..\..\..\..\Controls\Dialogs\MoveToGroupDialog.xaml"
            this.treeElements.SelectedItemChanged += new System.Windows.RoutedPropertyChangedEventHandler<object>(this.treeElements_SelectedItemChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnOK = ((System.Windows.Controls.Button)(target));
            
            #line 35 "..\..\..\..\Controls\Dialogs\MoveToGroupDialog.xaml"
            this.btnOK.Click += new System.Windows.RoutedEventHandler(this.btnOK_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnCansel = ((System.Windows.Controls.Button)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}