using System;
using System.Drawing;
using System.Windows.Forms;

namespace Borland_C__
{
	class MyRenderer : ToolStripProfessionalRenderer
	{
		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)      
        { 
			Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
            Color c = e.Item.Selected ? Color.FromArgb(26, 35, 39) : Color.FromArgb(38, 50, 56);
            using (SolidBrush brush = new SolidBrush(c)) 
                e.Graphics.FillRectangle(brush, rc); 
        } 
	}
}
