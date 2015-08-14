using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using Microsoft.Win32;


namespace TUM.CMS.VPL.Scripting
{
    /// <summary>
    ///     Interaction logic for Window1.xaml
    /// </summary>
    public partial class ScriptingControl
    {
        public static readonly DependencyProperty scriptFileProperty =
            DependencyProperty.Register("CurrentFile", typeof (ScriptFile), typeof (ScriptingControl),
                new PropertyMetadata(new ScriptFile(), ScriptFilePropertyChanged));

        private CompletionWindow completionWindow;
        private string currentFileName;
        private FoldingManager foldingManager;
        private object foldingStrategy;
        public EventHandler ShowAssemblyManagerEventHandler;
        public EventHandler StartCompilingEventHandler;
        public EventHandler StartCSharpCompilingEventHandler;
        public EventHandler StartPythonCompilingEventHandler;

        public ScriptingControl()
        {
            InitializeComponent();
#if DOTNET4
			this.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);
			#endif

            // PropertyGridComboBox.SelectedIndex = 2;


            TextEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            TextEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;


            ShowAssemblyManagerEventHandler += ShowAssemblyManager;

            /*
            using (var s = typeof(ScriptingControl).Assembly.GetManifestResourceStream(AppDomain.CurrentDomain.BaseDirectory + "ICSharpCode.PythonBinding.Resources.Python.xshd"))
            {
				if (s == null)
					throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(AppDomain.CurrentDomain.BaseDirectory + "ICSharpCode.PythonBinding.Resources.Python.xshd"))
                {
					customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
						HighlightingLoader.Load(reader, HighlightingManager.Instance);
				}
			}
             * */

            if (HighlightingManager.Instance.GetDefinition("Python") == null)
            {
                IHighlightingDefinition customHighlighting;

                using (
                    XmlReader reader =
                        new XmlTextReader(AppDomain.CurrentDomain.BaseDirectory +
                                          "ICSharpCode.PythonBinding.Resources.Python.xshd"))
                {
                    //customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                    //HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }


                // and register it in the HighlightingManager
                //HighlightingManager.Instance.RegisterHighlighting("Python", new[] { ".py" }, customHighlighting);
            }

            var foldingUpdateTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(2)};
            foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
            foldingUpdateTimer.Start();
        }

        public ScriptFile CurrentFile
        {
            get { return (ScriptFile) GetValue(scriptFileProperty); }
            set { SetValue(scriptFileProperty, value); }
        }

        private static void ScriptFilePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var src = source as ScriptingControl;
            var file = e.NewValue as ScriptFile;

            if (file == null || src == null) return;

            if (src.TextEditor.Document == null)
                src.TextEditor.Document = new TextDocument();

            src.TextEditor.Document.Text = file.ScriptContent;
            src.TextEditor.TextChanged += src.TextEditor_TextChanged;
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            var editor = sender as TextEditor;
            if (editor != null) CurrentFile.ScriptContent = editor.Document.Text;
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog {CheckFileExists = true};
            if (!(dlg.ShowDialog() ?? false)) return;
            currentFileName = dlg.FileName;
            TextEditor.Load(currentFileName);
            TextEditor.SyntaxHighlighting =
                HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(currentFileName));
        }

        private void SaveFileClick(object sender, EventArgs e)
        {
            if (currentFileName == null)
            {
                var dlg = new SaveFileDialog {DefaultExt = ".txt"};
                if (dlg.ShowDialog() ?? false)
                    currentFileName = dlg.FileName;
                else
                    return;
            }
            TextEditor.Save(currentFileName);
        }

        private void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != ".") return;
            // open code completion after the user has pressed dot:
            completionWindow = new CompletionWindow(TextEditor.TextArea);
            // provide AvalonEdit with the data:
            var data = completionWindow.CompletionList.CompletionData;


            data.Add(new MyCompletionData("Item1"));
            data.Add(new MyCompletionData("Item2"));
            data.Add(new MyCompletionData("Item3"));
            data.Add(new MyCompletionData("Another item"));


            completionWindow.Show();
            completionWindow.Closed += delegate
            {
                completionWindow = null;
                completionWindow = null;
            };
        }

        private void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length <= 0 || completionWindow == null) return;
            if (!char.IsLetterOrDigit(e.Text[0]))
            {
                // Whenever a non-letter is typed while the completion window is open,
                // insert the currently selected element.
                completionWindow.CompletionList.RequestInsertion(e);
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        private void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TextEditor.SyntaxHighlighting == null)
                foldingStrategy = null;
            else
            {
                switch (TextEditor.SyntaxHighlighting.Name)
                {
                    case "XML":
                        foldingStrategy = new XmlFoldingStrategy();
                        TextEditor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
                        break;
                    case "C#":
                    case "C++":
                    case "PHP":
                    case "Java":
                        TextEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(TextEditor.Options);
                        foldingStrategy = new BraceFoldingStrategy();
                        break;
                    default:
                        TextEditor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
                        foldingStrategy = null;
                        break;
                }
            }
            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(TextEditor.TextArea);
                UpdateFoldings();
            }
            else
            {
                if (foldingManager == null) return;
                FoldingManager.Uninstall(foldingManager);
                foldingManager = null;
            }
        }

        private void UpdateFoldings()
        {
            var strategy = this.foldingStrategy as BraceFoldingStrategy;
            if (strategy != null)
                strategy.UpdateFoldings(foldingManager, TextEditor.Document);
            var foldingStrategy = this.foldingStrategy as XmlFoldingStrategy;
            if (foldingStrategy != null)
                foldingStrategy.UpdateFoldings(foldingManager, TextEditor.Document);
        }

        private void ShowAssemblyManagerButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ShowAssemblyManagerEventHandler != null)
                ShowAssemblyManagerEventHandler(this, new EventArgs());
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            var toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
                overflowGrid.Visibility = Visibility.Collapsed;

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
                mainPanelBorder.Margin = new Thickness(0);
        }

        private void CompileButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (StartCompilingEventHandler != null)
                StartCompilingEventHandler(this, new EventArgs());

            // In case we have a C# snippet
            if (HighlightingComboBox.SelectedItem.ToString() == "C#")
            {
                if (StartCSharpCompilingEventHandler != null)
                    StartCSharpCompilingEventHandler(this, new EventArgs());
            }

            // In case we have a Python snippet
            if (HighlightingComboBox.SelectedItem.ToString() == "Python")
            {
                if (StartPythonCompilingEventHandler != null)
                    StartPythonCompilingEventHandler(this, new EventArgs());
            }
        }

        private void ShowAssemblyManager(object sender, EventArgs eventArgs)
        {
            var dialog = new AssemblyDialog(CurrentFile);
            dialog.ShowDialog();
        }
    }
}