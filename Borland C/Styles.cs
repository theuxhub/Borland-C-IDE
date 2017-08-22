using System;
using System.Drawing;
using FastColoredTextBoxNS;

namespace Borland_C__
{
	public class Styles
	{
		public Styles() {
			
		}
		
		Style s;
		
		public Style getStyle(StylesType style) {
			if(style == StylesType.StringStyle) {
				s = new TextStyle(Brushes.LightGreen,null, FontStyle.Regular);
			} else if(style == StylesType.KeywordStyle) {
				s = new TextStyle(Brushes.Violet,null,FontStyle.Regular);
			} else if(style == StylesType.NumberStyle) {
				s = new TextStyle(Brushes.Tomato,null,FontStyle.Regular);
			} else if(style == StylesType.PreprocessingStyle) {
				s = new TextStyle(Brushes.Turquoise,null,FontStyle.Regular);
			} else if(style == StylesType.CommentStyle) {
				s = new TextStyle(Brushes.Gray,null,FontStyle.Regular);
			} else if(style == StylesType.AfterKeywordStyle){
				s = new TextStyle(Brushes.LightYellow,null,FontStyle.Regular);
			}
			
			return s;
		}
	}
}
