// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Linq;
using Foundation;
using UIKit;
using FPT.Framework.iOS.UI.PagerTabStrip;

namespace Demo
{
	public partial class ReloadExampleViewController : UIViewController
	{

		#region PROPERTIES

		private UILabel mTitleLabel = null;
		[Outlet]
		UIKit.UILabel titleLabel
		{
			get
			{
				if (mTitleLabel == null)
				{
					mTitleLabel = new UILabel();
				}
				return mTitleLabel;
			}
			set
			{
				mTitleLabel = value;
			}
		}

		private UILabel mBigLabel = null;
		public UILabel BigLabel
		{
			get
			{
				if (mBigLabel == null)
				{
					mBigLabel = new UILabel();
					mBigLabel.BackgroundColor = UIColor.Clear;
					mBigLabel.TextColor = UIColor.White;
					mBigLabel.Font = UIFont.BoldSystemFontOfSize(20f);
					mBigLabel.AdjustsFontSizeToFitWidth = true;
				}
				return mBigLabel;
			}
		}

		#endregion

		#region CONSTRUCTORS

		public ReloadExampleViewController(IntPtr handle) : base(handle)
		{
		}

		#endregion

		#region FUNCTIONS

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			if (NavigationController != null)
			{
				NavigationItem.TitleView = BigLabel;
				BigLabel.SizeToFit();
			}

			var pagerViewController = ChildViewControllers.Where(_ => _ is PagerTabStripViewController).FirstOrDefault();
			if (pagerViewController != null)
			{
				updateTitle(pagerViewController as PagerTabStripViewController);
			}
		}

		partial void closeTapped(Foundation.NSObject sender)
		{
			DismissViewController(true, null);
		}

		partial void reloadTapped(Foundation.NSObject sender)
		{
			foreach (var childViewController in ChildViewControllers)
			{
				var child = childViewController as PagerTabStripViewController;
				if (child == null) continue;

				child.ReloadPagerTabStripView();
				updateTitle(child);
				break;
			}
		}

		void updateTitle(PagerTabStripViewController pagerTabStripViewController)
		{
			titleLabel.Text = string.Format("Progressive = {0}  ElasticLimit = {1}",
			                                pagerTabStripViewController.PagerBehaviour.ProgressiveIndicator.ToString(),
			                                pagerTabStripViewController.PagerBehaviour.ElasticIndicatorLimit.ToString());

			if (NavigationItem.TitleView != null && NavigationItem.TitleView is UILabel)
			{
				((UILabel)NavigationItem.TitleView).Text = titleLabel.Text;
				NavigationItem.TitleView.SizeToFit();
			}
		}

		public override UIStatusBarStyle PreferredStatusBarStyle()
		{
			return UIStatusBarStyle.LightContent;
		}

		#endregion
	}
}
