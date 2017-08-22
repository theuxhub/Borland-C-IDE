using System;
using System.IO;
using System.Windows.Forms;

namespace Borland_C__
{
	internal sealed class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			StreamReader strReader;
	 		String str;
			if (args != null && args.Length > 0)  
            {  
				String files = args[0];
                MainForm mf = new MainForm();  
				strReader = new StreamReader(files);  
				str = strReader.ReadToEnd();  
				strReader.Close();
				mf.fastColoredTextBox1.Text = str;  
				mf.Filepath = files;
				mf.FileName = mf.Filepath.Substring(mf.Filepath.LastIndexOf("\\") + 1);	
				mf.tabPage1.Text = mf.FileName;			  
                Application.EnableVisualStyles();  
                Application.Run(mf);  
                mf.IsFileChanged = false;
            } else
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new MainForm());
			}
		}
		
	}
}
