﻿#pragma checksum "..\..\..\..\..\Views\Pages\IntegrityTestPage.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "FD6F7D88D29DF3BE212E188D39E604A980697108"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GUISandbox.Views.Pages;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Converters;
using Wpf.Ui.Markup;


namespace GUISandbox.Views.Pages {
    
    
    /// <summary>
    /// IntegrityTestPage
    /// </summary>
    public partial class IntegrityTestPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 28 "..\..\..\..\..\Views\Pages\IntegrityTestPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label InfoProvide;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\..\Views\Pages\IntegrityTestPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label InfoProvideDescription;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\..\Views\Pages\IntegrityTestPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label ViolationNote;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\..\Views\Pages\IntegrityTestPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wpf.Ui.Controls.Button ResultsButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/GUISandbox;V1.0.0.0;component/views/pages/integritytestpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Views\Pages\IntegrityTestPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.8.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 27 "..\..\..\..\..\Views\Pages\IntegrityTestPage.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.InfoProvide = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.InfoProvideDescription = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.ViolationNote = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.ResultsButton = ((Wpf.Ui.Controls.Button)(target));
            
            #line 31 "..\..\..\..\..\Views\Pages\IntegrityTestPage.xaml"
            this.ResultsButton.Click += new System.Windows.RoutedEventHandler(this.See_Results_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

