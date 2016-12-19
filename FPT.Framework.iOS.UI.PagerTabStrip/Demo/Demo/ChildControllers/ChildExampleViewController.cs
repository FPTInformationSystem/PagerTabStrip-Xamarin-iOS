using System;
using UIKit;
using FPT.Framework.iOS.UI.PagerTabStrip;
using Foundation;

namespace Demo
{
	public class ChildExampleViewController : UIViewController, IndicatorInfoProvider
	{

		#region PROPERTIES

		IndicatorInfo ItemInfo { get; set; } = new IndicatorInfo("View");

		#endregion

		#region CONSTRUCTORS

		public ChildExampleViewController(IndicatorInfo itemInfo) : base (null, null)
		{
			this.ItemInfo = itemInfo;
		}

		public ChildExampleViewController(NSCoder coder) : base(coder)
		{
		}

		public ChildExampleViewController(IntPtr handle) : base(handle)
		{
		}

		#endregion

		#region FUNCTIONS

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var label = new UILabel();
			label.TranslatesAutoresizingMaskIntoConstraints = false;
			label.Text = "XLPagerTabStrip";

			View.AddSubview(label);
			View.BackgroundColor = UIColor.White;

			View.AddConstraint(NSLayoutConstraint.Create(
				view1: label,
				attribute1: NSLayoutAttribute.CenterX,
				relation: NSLayoutRelation.Equal,
				view2: View,
				attribute2: NSLayoutAttribute.CenterX,
				multiplier: 1,
				constant: 0
			));

			View.AddConstraint(NSLayoutConstraint.Create(
				view1: label,
				attribute1: NSLayoutAttribute.CenterY,
				relation: NSLayoutRelation.Equal,
				view2: View,
				attribute2: NSLayoutAttribute.CenterY,
				multiplier: 1,
				constant: -50
			));
		}

		public IndicatorInfo GetIndicatorInfo(PagerTabStripViewController pagerTabStripController)
		{
			return ItemInfo;
		}

		#endregion
	}
}
