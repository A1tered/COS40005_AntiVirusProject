﻿#pragma checksum "..\..\..\..\..\..\GUI\Views\Pages\ProtectionHistoryPage.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "6817E06CC990259E12DC0610C89969B92028B7DC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using SimpleAntivirus.GUI.Views.Pages;
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


namespace SimpleAntivirus.GUI.Views.Pages {
    
    
    /// <summary>
    /// ProtectionHistoryPage
    /// </summary>
    public partial class ProtectionHistoryPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\..\..\..\..\GUI\Views\Pages\ProtectionHistoryPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wpf.Ui.Controls.TextBlock ItemAmount;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\..\..\GUI\Views\Pages\ProtectionHistoryPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid ProtectionHistoryDataGrid;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\..\..\..\GUI\Views\Pages\ProtectionHistoryPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Wpf.Ui.Controls.Button DetailsButton;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.5.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/SimpleAntivirus;component/gui/views/pages/protectionhistorypage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\GUI\Views\Pages\ProtectionHistoryPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.5.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 17 "..\..\..\..\..\..\GUI\Views\Pages\ProtectionHistoryPage.xaml"
            ((SimpleAntivirus.GUI.Views.Pages.ProtectionHistoryPage)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Page_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 26 "..\..\..\..\..\..\GUI\Views\Pages\ProtectionHistoryPage.xaml"
            ((Wpf.Ui.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.DeleteButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ItemAmount = ((Wpf.Ui.Controls.TextBlock)(target));
            return;
            case 4:
            this.ProtectionHistoryDataGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 28 "..\..\..\..\..\..\GUI\Views\Pages\ProtectionHistoryPage.xaml"
            this.ProtectionHistoryDataGrid.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.DataShow_Selected);
            
            #line default
            #line hidden
            return;
            case 5:
            this.DetailsButton = ((Wpf.Ui.Controls.Button)(target));
            
            #line 36 "..\..\..\..\..\..\GUI\Views\Pages\ProtectionHistoryPage.xaml"
            this.DetailsButton.Click += new System.Windows.RoutedEventHandler(this.DetailsButton_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 38 "..\..\..\..\..\..\GUI\Views\Pages\ProtectionHistoryPage.xaml"
            ((Wpf.Ui.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Send_Alert);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

