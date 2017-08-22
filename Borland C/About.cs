using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin.Controls;
using System.Diagnostics;

namespace Borland_C__
{
	public partial class About : MaterialForm
	{
		public About()
		{
			InitializeComponent();
		}
		
		void MaterialRaisedButton2Click(object sender, EventArgs e)
		{
			Process.Start("http://www.facebook.com/decoderhub");
		}
		
		void MaterialRaisedButton1Click(object sender, EventArgs e)
		{
			Process.Start("http://www.facebook.com/decoderhub");			
		}
		
		void MaterialRaisedButton3Click(object sender, EventArgs e)
		{
			Process.Start("http://www.facebook.com/decoderhub");			
		}
	}
}
