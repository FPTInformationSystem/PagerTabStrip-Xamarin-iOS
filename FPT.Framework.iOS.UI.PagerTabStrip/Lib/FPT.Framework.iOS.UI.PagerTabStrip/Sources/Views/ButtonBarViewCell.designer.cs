// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace FPT.Framework.iOS.UI.PagerTabStrip
{
	[Register ("ButtonBarViewCell")]
	partial class ButtonBarViewCell
	{

		[Outlet]
		public UIKit.UIImageView ImageView { get; set; }

		[Outlet]
		public UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (ImageView != null)
			{
				ImageView.Dispose();
				ImageView = null;
			}
		}
	}
}
