using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MaterialSkin;
using System.Diagnostics;
using FastColoredTextBoxNS;

namespace Borland_C__
{
	public partial class MainForm : MaterialSkin.Controls.MaterialForm
	{	
		Styles s = new Styles();
		
		public String Filepath = "";
		public String FileName = "";
		public bool IsFileChanged = false;
	 	StreamReader strReader;
	 	String str;
	 	
	 	Style stringStyle;
	 	Style numberStyle;
	 	Style commentStyle;
	 	Style keywordStyle;
	 	Style afterKeywordStyle;
	 	Style preprocessingStyle;
	 	
		public MainForm()
		{
			InitializeComponent();
			//Set the mainMenuStrip Render to Change its Background Color
			MainMenuStrip.Renderer = new MyRenderer(); 
			//Set if file has been changed
			IsFileChanged = false;
			fastColoredTextBox1.Focus();
			
			stringStyle = s.getStyle(StylesType.StringStyle);
			numberStyle = s.getStyle(StylesType.NumberStyle);
			commentStyle = s.getStyle(StylesType.CommentStyle);
			keywordStyle = s.getStyle(StylesType.KeywordStyle);
			afterKeywordStyle = s.getStyle(StylesType.AfterKeywordStyle);
			preprocessingStyle = s.getStyle(StylesType.PreprocessingStyle);
		}
		
		void FastColoredTextBox1TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
		{
			HTMLSyntaxHighlight((sender as FastColoredTextBox).VisibleRange);
			//Set if file has been changed
			IsFileChanged = true;
		}
		
		void HTMLSyntaxHighlight(Range range) {
			
			//Clear the styles for all Text			
			range.ClearStyle(commentStyle);
			range.ClearStyle(stringStyle);
			range.ClearStyle(afterKeywordStyle);
			range.ClearStyle(keywordStyle);
			range.ClearStyle(numberStyle);
			range.ClearStyle(preprocessingStyle);

			//Set the styles for number after getting a number
			range.SetStyle(numberStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");
			
			//Set the styles for class|struct|enum|interface after getting a the words 
			range.SetStyle(keywordStyle, @"\b(class|struct|enum|interface)\b");
			
			//Set the styles for the word that comes after class|struct|enum|interface
			range.SetStyle(afterKeywordStyle, @"(?<=\b(class|struct|enum|interface)\s+)\p{L}+");
			
			//Set the styles for Keywords 
			range.SetStyle(keywordStyle, @"\b(LRESULT|CString|String|WORD|DWORD|TCHAR|BYTE|BOOL|unsigned|signed|int|bool|char|short|const|long|float|double|string|wchar|wchar_t|__int8|__int16|__int32|__int64|struct|class|enum|interface|if|else|switch|case|break|defalut|return|true|false|for|do|while|cout|cin|void|Boolean)\b"); 
			
			
			//Set the styles for Following Commands command 
			range.SetStyle(preprocessingStyle, @"#\b(include|pragma|if|else|elif|ifndef|ifdef|endif|undef|define|line|error|catch|continue|goto|new|list|map|using|namespace|private|protected|public|const|delete)\b|__[^>]*__", RegexOptions.Singleline); 
			
			
			//Set the Styles for method i.e. the word befor ()
			range.SetStyle(preprocessingStyle,@"(\w+?)(?=\()");
			
			//Set the Styles for string 
			range.SetStyle(stringStyle, @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'"); 
			
			//Set the styles for Comments			
			range.SetStyle(commentStyle, @"//.*$", RegexOptions.Multiline);
			range.SetStyle(commentStyle, @"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline);
			               range.SetStyle(commentStyle, @"(/\*.*?\*/)|(.*\*/)", RegexOptions.Singleline |
			                          RegexOptions.RightToLeft);
			range.ClearFoldingMarkers(); 
			
		}
		
		void OpenToolStripMenuItemClick(object sender, EventArgs e)
		{
			//Check if file has been changed
			if(IsFileChanged) {
		 		DialogResult dg  = MessageBox.Show("The File :" + FileName + " has been Changed. Do You Want to save the File","File Changed",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Exclamation);
		 		if(dg == DialogResult.Yes) {
		 			Save();
		 		}else if(dg == DialogResult.No){
		 			Open();
		 			//Set if file has been changed
		 			IsFileChanged = false;
		 		}
		 	} else
			{
				Open();
				//Set if file has been changed
				IsFileChanged = false;
			}
		}
		
		void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
		{
			SaveAs();
			//Set if file has been changed
			IsFileChanged = false;
		}
		
		void SaveToolStripMenuItemClick(object sender, EventArgs e)
		{
			Save();
			//Set if file has been changed
			IsFileChanged = false;
		}
		
		void NewToolStripMenuItemClick(object sender, EventArgs e)
		{
			MainForm m = new MainForm();
			m.Show();
		}
		
		void CompileToolStripMenuItemClick(object sender, EventArgs e)
		{
			//Check if file hasbeen Saved or not
			if(FileName != "") {
				//Save the file if already have a path
				File.WriteAllText(Filepath,fastColoredTextBox1.Text);
				fastColoredTextBox1.Focus();
				ResultsTextBox.Text = "";
				CompileC();
				//Set if file has been changed
				IsFileChanged = false;
			}
			else {
				//Save the file after getting a path
				SaveFileDialog s = new SaveFileDialog();
				s.Filter = "C++ Files (*.cpp)|*.cpp;*.c;*.h|All files (*.*)|*.*";
				if(s.ShowDialog() == DialogResult.OK) {
					File.WriteAllText(s.FileName,fastColoredTextBox1.Text);
					Filepath = s.FileName;
					FileName = Filepath.Substring(Filepath.LastIndexOf("\\") + 1);
					tabPage1.Text = FileName;
					this.Refresh();
					//Set if file has been changed
					IsFileChanged = false;
					ResultsTextBox.Text = "";
					CompileC();
				}else {
					ResultsTextBox.Text = "You Must First Save The File Before Compiling.";
				}
			}
			fastColoredTextBox1.Focus();
		}
		
		public async void CompileC() {
			
			var proc = new Process {
			    StartInfo = new ProcessStartInfo {
			        FileName = "bcc32",
			        Arguments = '"' + FileName + '"',
			        WorkingDirectory = Path.GetDirectoryName(Filepath),
			        UseShellExecute = false,
			        RedirectStandardOutput = true,
			        RedirectStandardError = true,
			        CreateNoWindow = true
			    }
			};
			proc.Start();
			string res = proc.StandardOutput.ReadToEnd();
			ResultsTextBox.Text = res;
			if(!res.Contains("Error")) {
				ResultsTextBox.Text += "\nProgram Compiled Successfully.................!";
			}
			fastColoredTextBox1.Focus();
		}
		
		void AboutToolStripMenuItemClick(object sender, EventArgs e)
		{
			About a = new About();
			a.ShowDialog();
		}
		
		void CompileRunToolStripMenuItemClick(object sender, EventArgs e)
		{
			if(FileName != "") {
				File.WriteAllText(Filepath,fastColoredTextBox1.Text);
				fastColoredTextBox1.Focus();
				ResultsTextBox.Text = "";
				CompileC();
				//Set if file has been changed
				IsFileChanged = false;
				Process.Start(Filepath.Substring(0, Filepath.IndexOf(".")) + ".exe");
			}
			else {
				SaveFileDialog s = new SaveFileDialog();
				s.Filter = "C++ Files (*.cpp)|*.cpp;*.c;*.h|All files (*.*)|*.*";
				if(s.ShowDialog() == DialogResult.OK) {
					File.WriteAllText(s.FileName,fastColoredTextBox1.Text);
					Filepath = s.FileName;
					FileName = Filepath.Substring(Filepath.LastIndexOf("\\") + 1);
					tabPage1.Text = FileName;
					this.Refresh();
					//Set if file has been changed
					IsFileChanged = false;
					ResultsTextBox.Text = "";
					CompileC();
					Process.Start(Filepath.Substring(0, Filepath.IndexOf(".")) + ".exe");
				}else {
					ResultsTextBox.Text = "You Must First Save The File Before Compiling.";
				}
			}
			fastColoredTextBox1.Focus();
		}
		
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			//Checks if File hasbeen changed or not
			if(IsFileChanged) {
				DialogResult dg  = MessageBox.Show("The File :" + FileName + " Has Been Changed. Do You Want to Save this File","File Changed",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Exclamation);
		 		if(dg == DialogResult.Yes) {
		 			Save();
		 		}else if(dg == DialogResult.No){
					Application.Exit();
					this.Close();
		 		}
			} else {
				Application.Exit();
				this.Close();
			}
		}
		
		
		void Save() {
			if(FileName != "") {
				File.WriteAllText(Filepath,fastColoredTextBox1.Text);
				fastColoredTextBox1.Focus();
			}
			else {
				SaveAs();
				fastColoredTextBox1.Focus();
			}
		}
		
		void SaveAs() {
			SaveFileDialog s = new SaveFileDialog();
			s.Filter = "C++ Files (*.cpp)|*.cpp;*.c;*.h|All files (*.*)|*.*";
			if(s.ShowDialog() == DialogResult.OK) {
				File.WriteAllText(s.FileName,fastColoredTextBox1.Text);
				Filepath = s.FileName;
				FileName = Filepath.Substring(Filepath.LastIndexOf("\\") + 1);
				tabPage1.Text = FileName;
				this.Refresh();
				//Set the file changed false
				IsFileChanged = false;
				fastColoredTextBox1.Focus();
			}
		}
		
		void Open() {
			OpenFileDialog o = new OpenFileDialog();
 			o.Filter = "C++ Files (*.cpp)|*.cpp;*.c;*.h|All files (*.*)|*.*";
			if(o.ShowDialog() == DialogResult.OK)
			{
				strReader = new StreamReader(o.FileName);  
				str = strReader.ReadToEnd();  
				strReader.Close();
				fastColoredTextBox1.Text = str;
				Filepath = o.FileName;
				FileName = Filepath.Substring(Filepath.LastIndexOf("\\") + 1);
				tabPage1.Text = FileName;
				this.Refresh();
				HTMLSyntaxHighlight(fastColoredTextBox1.VisibleRange);
				//Set if file has been changed
				IsFileChanged = false;
				fastColoredTextBox1.Focus();
			}
		}
		
		
		void CutToolStripMenuItem1Click(object sender, EventArgs e)
		{
			fastColoredTextBox1.Cut();
		}
		
		void CopyToolStripMenuItem1Click(object sender, EventArgs e)
		{
			fastColoredTextBox1.Copy();			
		}
		
		void PasteToolStripMenuItem1Click(object sender, EventArgs e)
		{
			fastColoredTextBox1.Paste();
		}
		
		void CompileToolStripMenuItem1Click(object sender, EventArgs e)
		{
			if(FileName != "") {
				File.WriteAllText(Filepath,fastColoredTextBox1.Text);
				fastColoredTextBox1.Focus();
				ResultsTextBox.Text = "";
				CompileC();
			}
			else {
				SaveFileDialog s = new SaveFileDialog();
				s.Filter = "C++ Files (*.cpp)|*.cpp;*.c;*.h|All files (*.*)|*.*";
				if(s.ShowDialog() == DialogResult.OK) {
					File.WriteAllText(s.FileName,fastColoredTextBox1.Text);
					Filepath = s.FileName;
					FileName = Filepath.Substring(Filepath.LastIndexOf("\\") + 1);
					tabPage1.Text = FileName;
					this.Refresh();
					//Set if file has been changed
					IsFileChanged = false;
					ResultsTextBox.Text = "";
					CompileC();
				}else {
					ResultsTextBox.Text = "You Must First Save The File Before Compiling.";
				}
				fastColoredTextBox1.Focus();
			}
			fastColoredTextBox1.Focus();		
		}
		
		void CompileRunToolStripMenuItem1Click(object sender, EventArgs e)
		{
			if(FileName != "") {
				File.WriteAllText(Filepath,fastColoredTextBox1.Text);
				fastColoredTextBox1.Focus();
				ResultsTextBox.Text = "";
				CompileC();
				Process.Start(Filepath.Substring(0, Filepath.IndexOf(".")) + ".exe");
			}
			else {
				SaveFileDialog s = new SaveFileDialog();
				s.Filter = "C++ Files (*.cpp)|*.cpp;*.c;*.h|All files (*.*)|*.*";
				if(s.ShowDialog() == DialogResult.OK) {
					File.WriteAllText(s.FileName,fastColoredTextBox1.Text);
					Filepath = s.FileName;
					FileName = Filepath.Substring(Filepath.LastIndexOf("\\") + 1);
					tabPage1.Text = FileName;
					this.Refresh();
					IsFileChanged = false;
					ResultsTextBox.Text = "";
					CompileC();
					Process.Start(Filepath.Substring(0, Filepath.IndexOf(".")) + ".exe");
				}else {
					ResultsTextBox.Text = "You Must First Save The File Before Compiling.";
				}
				fastColoredTextBox1.Focus();
			}
			fastColoredTextBox1.Focus();			
		}
		
		
		void UndoToolStripMenuItemClick(object sender, EventArgs e)
		{
			fastColoredTextBox1.Undo();
		}
		
		void RedoToolStripMenuItemClick(object sender, EventArgs e)
		{
			fastColoredTextBox1.Redo();
		}
		
		void CutToolStripMenuItemClick(object sender, EventArgs e)
		{
			fastColoredTextBox1.Cut();
		}
		
		void CopyToolStripMenuItemClick(object sender, EventArgs e)
		{
			fastColoredTextBox1.Copy();
		}
		
		void PasteToolStripMenuItemClick(object sender, EventArgs e)
		{
			fastColoredTextBox1.Paste();
		}
		
		void SelectAllToolStripMenuItemClick(object sender, EventArgs e)
		{
			fastColoredTextBox1.SelectAll();
		}
		
		
		void ExportToHTMLToolStripMenuItemClick(object sender, EventArgs e)
		{	
			String html = "<head><style>div.code{background:rgb(38, 50, 56);padding:10px;}body{margin:0;}</style></head><body><div class=\"code\">" + fastColoredTextBox1.Html + "</div></body>";
			SaveFileDialog s = new SaveFileDialog();
			s.Filter = "HTML File (*.html)|*.html;*.htm|All files (*.*)|*.*";
			if(s.ShowDialog() == DialogResult.OK) {
				File.WriteAllText(s.FileName, html);
				fastColoredTextBox1.Focus();
			}			
		}
		
		void FastColoredTextBox1VisibleRangeChanged(object sender, EventArgs e)
		{
			HTMLSyntaxHighlight(fastColoredTextBox1.VisibleRange);
		}
		
		
		void MainFormFormClosed(object sender, FormClosedEventArgs e)
		{
			if(IsFileChanged) {
			DialogResult dg  = MessageBox.Show("The File :" + FileName + " Has Been Changed. Do You Want to Save this File","File Changed",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Exclamation);
		 		if(dg == DialogResult.Yes) {
		 			Save();
		 		}else if(dg == DialogResult.No){
					Application.Exit();
		 		}
			} else {
				Application.Exit();
			}
		}
		
		void AutoIndentToolStripMenuItemClick(object sender, EventArgs e)
		{
			fastColoredTextBox1.DoAutoIndent();			
		}
		
		void FastColoredTextBox1AutoIndentNeeded(object sender, AutoIndentEventArgs e)
		{
			if(e.LineText.Trim() == "{")
			{
				e.ShiftNextLines = e.TabLength;
				return;
			}
			if(e.LineText.Trim() == "}") {
				e.Shift = -e.TabLength;
				e.ShiftNextLines = -e.TabLength;
				return;
			}
		}
		
		void FastColoredTextBox1VisibleRangeChangedDelayed(object sender, EventArgs e)
		{
			HTMLSyntaxHighlight(fastColoredTextBox1.VisibleRange);
		}
	}
	
}
