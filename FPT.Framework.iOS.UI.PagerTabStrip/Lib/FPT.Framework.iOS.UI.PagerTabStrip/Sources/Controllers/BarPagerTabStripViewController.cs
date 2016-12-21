using System;
using UIKit;
using Foundation;
using CoreGraphics;

namespace FPT.Framework.iOS.UI.PagerTabStrip
{

	public class BarPagerTabStripSettings
	{
		public class StyleDefinition
		{
			public UIColor BarBackgroundColor { get; set; }
			public UIColor SelectedBarBackgroundColor { get; set; }
			public nfloat BarHeight { get; set; } = 5f;
		}

		public StyleDefinition Style { get; set; } = new StyleDefinition();
	}

	public partial class BarPagerTabStripViewController : PagerTabStripViewController, PagerTabStripDataSource, PagerTabStripIsProgressiveDelegate
	{

		#region PROPERTIES

		public BarPagerTabStripSettings Settings { get; set; } = new BarPagerTabStripSettings();

		[Outlet]
		public BarView BarView { get; set;}

		#endregion

		#region CONSTRUCTORS

		public BarPagerTabStripViewController(string nibNameOrNil, NSBundle nibBundleOrNil) : base(nibNameOrNil, nibBundleOrNil)
		{
			Delegate = this;
			DataSource = this;
		}

		public BarPagerTabStripViewController(NSCoder coder) : base(coder)
		{
			Delegate = this;
			DataSource = this;
		}

		public BarPagerTabStripViewController(IntPtr handle) : base(handle)
		{
			Delegate = this;
			DataSource = this;
		}

		#endregion

		#region FUNCTIONS

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			if (BarView == null)
			{
				BarView = new BarView(new CGRect(0,0,View.Frame.Width, View.Frame.Height));
				BarView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
				BarView.BackgroundColor = UIColor.Black;
				BarView.SelectedBar.BackgroundColor = UIColor.White;
			}

			BarView.BackgroundColor = Settings.Style.BarBackgroundColor ?? BarView.BackgroundColor;
			BarView.SelectedBar.BackgroundColor = Settings.Style.SelectedBarBackgroundColor ?? BarView.SelectedBar.BackgroundColor;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			if (BarView.Superview == null)
			{
				View.AddSubview(BarView);
			}
			BarView.OptionsCount = ViewControllers.Count;
			BarView.MoveTo(CurrentIndex, false);
		}

		public override void ReloadPagerTabStripView()
		{
			base.ReloadPagerTabStripView();
			BarView.OptionsCount = ViewControllers.Count;
			if (IsViewLoaded)
			{
				BarView.MoveTo(CurrentIndex, false);
			}
		}

		public void UpdateIndicator(PagerTabStripViewController viewController, nint fromIndex, nint toIndex, nfloat progressPercentage, bool indexWasChanged)
		{
			BarView.Move(fromIndex, toIndex, progressPercentage);
		}

		public void UpdateIndicator(PagerTabStripViewController viewController, nint fromIndex, nint toIndex)
		{
			BarView.MoveTo(toIndex, true);
		}

		#endregion
	}
}
