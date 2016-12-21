using System;
using Foundation;
using UIKit;
using CoreGraphics;

namespace FPT.Framework.iOS.UI.PagerTabStrip
{
	[Register("BarView")]
	public partial class BarView : UIView
	{

		#region PROPERTIES

		private UIView mSelectedBar = null;
		public UIView SelectedBar
		{
			get
			{
				if (mSelectedBar == null)
				{
					var bar = new UIView(new CGRect(0, 0, this.Frame.Width, this.Frame.Height));
					mSelectedBar = bar;
				}
				return mSelectedBar;
			}
		}

		private int mOptionsCount = 1;
		public int OptionsCount
		{
			get
			{
				return mOptionsCount;
			}
			set
			{
				if (value <= SelectedIndex)
				{
					SelectedIndex = mOptionsCount - 1;
				}
				mOptionsCount = value;
			}
		}

		public nint SelectedIndex { get; set; } = 0;

		#endregion

		#region CONSTRUCTORS

		protected BarView(IntPtr handle) : base(handle)
		{
			AddSubview(SelectedBar);
		}

		public BarView(NSCoder coder) : base(coder)
		{
			AddSubview(SelectedBar);
		}

		public BarView(CGRect frame) : base(frame)
		{
			AddSubview(SelectedBar);
		}

		#endregion

		#region HELPERS

		private void UpdateSelectedBarPosition(bool animation)
		{
			var frame = SelectedBar.Frame;
			frame.Width = this.Frame.Width / OptionsCount;
			frame.X = frame.Width * SelectedIndex;
			if (animation)
			{
				UIView.Animate(duration: 0.3, animation: () =>
				{
					this.SelectedBar.Frame = frame;
				});
			}
			else
			{
				SelectedBar.Frame = frame;
			}
		}

		public void MoveTo(nint index, bool animated)
		{
			SelectedIndex = index;
			UpdateSelectedBarPosition(animated);
		}

		public void Move(nint fromIndex, nint toIndex, nfloat progressPercentage)
		{
			SelectedIndex = (progressPercentage > 0.5f) ? toIndex : fromIndex;

			var newFrame = SelectedBar.Frame;
			newFrame.Width = Frame.Width / OptionsCount;
			var fromFrame = newFrame;
			fromFrame.X = newFrame.Width * fromIndex;
			var toFrame = newFrame;
			toFrame.X = toFrame.Width * toIndex;
			var targetFrame = fromFrame;
			targetFrame.X += (toFrame.X - targetFrame.X) * progressPercentage;
			SelectedBar.Frame = targetFrame;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			UpdateSelectedBarPosition(false);
		}

		#endregion
	}
}
