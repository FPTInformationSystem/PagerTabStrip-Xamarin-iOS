using System;

using Foundation;
using UIKit;

namespace FPT.Framework.iOS.UI.PagerTabStrip
{
	public partial class ButtonBarViewCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString("ButtonBarViewCell");
		public static readonly UINib Nib;

		static ButtonBarViewCell()
		{
			Nib = UINib.FromName("ButtonBarViewCell", NSBundle.MainBundle);
		}

		protected ButtonBarViewCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
			TitleLabel.Frame = this.ContentView.Bounds;
			TitleLabel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			TitleLabel.TextAlignment = UITextAlignment.Center;
			TitleLabel.Font = UIFont.BoldSystemFontOfSize(14.0f);
		}

		public override void WillMoveToSuperview(UIView newsuper)
		{
			base.WillMoveToSuperview(newsuper);

			if (TitleLabel.Superview != null)
			{
				ContentView.AddSubview(TitleLabel);
			}
		}
	}
}
