using System;
using UIKit;

namespace FPT.Framework.iOS.UI.PagerTabStrip
{
	public partial class IndicatorInfo
	{

		#region PROPERTIES

		public string Title { get; set;}

		public UIImage Image { get; set;}

		public UIImage HighlightedImage { get; set;}

		#endregion

		#region CONSTRUCTORS

		public IndicatorInfo(string title, UIImage image = null, UIImage highlightedImage = null)
		{
			Title = title;
			Image = image;
			HighlightedImage = highlightedImage;
		}
		#endregion
	}
}
