using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.CodeCompletion;
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
    public partial class ScriptingControlSimple
    {
        private CompletionWindow completionWindow;
        private string currentFileName;
        public EventHandler LoadAdditionalAssembly;

        public ScriptingControlSimple()
        {
            // Load our custom highlighting definition
            /*
			IHighlightingDefinition customHighlighting;
            using (Stream s = typeof(ScriptingControl).Assembly.GetManifestResourceStream("CustomHighlighting.xshd"))
            {
				if (s == null)
					throw new InvalidOperationException("Could not find embedded resource");
				using (XmlReader reader = new XmlTextReader(s)) {
					customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
						HighlightingLoader.Load(reader, HighlightingManager.Instance);
				}
			}
			// and register it in the HighlightingManager
			HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new[] { ".cool" }, customHighlighting);
			*/

            InitializeComponent();
#if DOTNET4
			this.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Display);
			#endif

            TextEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            TextEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;

            var foldingUpdateTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(2)};
            foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
            foldingUpdateTimer.Start();
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
            completionWindow.Closed += delegate { completionWindow = null; };
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (LoadAdditionalAssembly != null)
                LoadAdditionalAssembly(this, new EventArgs());
        }

        #region Folding

        private FoldingManager foldingManager;
        private object foldingStrategy;

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

        #endregion
    }
}