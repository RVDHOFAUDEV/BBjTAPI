﻿#pragma checksum "..\..\..\pages\extras.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "404C219C1AF1B78D2E2241796DC00A6A35B487C7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using BBjTapiClient;
using BBjTapiClient.pages;
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
using System.Windows.Shell;


namespace BBjTapiClient.pages {
    
    
    /// <summary>
    /// extras
    /// </summary>
    public partial class extras : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 32 "..\..\..\pages\extras.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel rootContainer;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\pages\extras.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSimIncoming;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\pages\extras.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbxNumCaller;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\pages\extras.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnMakeCall;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\..\pages\extras.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnDropCall;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\pages\extras.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbxNumCall;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\pages\extras.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnPostActual;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\pages\extras.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnPostAll;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/BBjTAPIClient;component/pages/extras.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\pages\extras.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.rootContainer = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 2:
            this.btnSimIncoming = ((System.Windows.Controls.Button)(target));
            
            #line 44 "..\..\..\pages\extras.xaml"
            this.btnSimIncoming.Click += new System.Windows.RoutedEventHandler(this.btnSimIncoming_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.tbxNumCaller = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.btnMakeCall = ((System.Windows.Controls.Button)(target));
            
            #line 52 "..\..\..\pages\extras.xaml"
            this.btnMakeCall.Click += new System.Windows.RoutedEventHandler(this.btnMakeCall_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnDropCall = ((System.Windows.Controls.Button)(target));
            
            #line 53 "..\..\..\pages\extras.xaml"
            this.btnDropCall.Click += new System.Windows.RoutedEventHandler(this.btnDropCall_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.tbxNumCall = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.btnPostActual = ((System.Windows.Controls.Button)(target));
            
            #line 60 "..\..\..\pages\extras.xaml"
            this.btnPostActual.Click += new System.Windows.RoutedEventHandler(this.btnPostActual_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.btnPostAll = ((System.Windows.Controls.Button)(target));
            
            #line 61 "..\..\..\pages\extras.xaml"
            this.btnPostAll.Click += new System.Windows.RoutedEventHandler(this.btnPostAll_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

