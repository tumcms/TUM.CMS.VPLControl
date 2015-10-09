﻿#pragma checksum "..\..\ScriptingControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9DBDB0054F28C9DBD791D8785F0C2E86"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
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


namespace TUM.CMS.VPL.Scripting {
    
    
    /// <summary>
    /// ScriptingControl
    /// </summary>
    public partial class ScriptingControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\ScriptingControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel DockPanel;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\ScriptingControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox HighlightingComboBox;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\ScriptingControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ShowAssemblyManagerButton;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\ScriptingControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CompileButton;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\ScriptingControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ICSharpCode.AvalonEdit.TextEditor TextEditor;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\ScriptingControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextBlockError;
        
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
            System.Uri resourceLocater = new System.Uri("/TUM.CMS.VPL.Scripting;component/scriptingcontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ScriptingControl.xaml"
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
            this.DockPanel = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 2:
            
            #line 10 "..\..\ScriptingControl.xaml"
            ((System.Windows.Controls.ToolBar)(target)).Loaded += new System.Windows.RoutedEventHandler(this.FrameworkElement_OnLoaded);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 22 "..\..\ScriptingControl.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenFileClick);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 25 "..\..\ScriptingControl.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SaveFileClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.HighlightingComboBox = ((System.Windows.Controls.ComboBox)(target));
            
            #line 65 "..\..\ScriptingControl.xaml"
            this.HighlightingComboBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.HighlightingComboBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.ShowAssemblyManagerButton = ((System.Windows.Controls.Button)(target));
            
            #line 68 "..\..\ScriptingControl.xaml"
            this.ShowAssemblyManagerButton.Click += new System.Windows.RoutedEventHandler(this.ShowAssemblyManagerButton_OnClick);
            
            #line default
            #line hidden
            return;
            case 7:
            this.CompileButton = ((System.Windows.Controls.Button)(target));
            
            #line 70 "..\..\ScriptingControl.xaml"
            this.CompileButton.Click += new System.Windows.RoutedEventHandler(this.CompileButton_OnClick);
            
            #line default
            #line hidden
            return;
            case 8:
            this.TextEditor = ((ICSharpCode.AvalonEdit.TextEditor)(target));
            return;
            case 9:
            this.TextBlockError = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

