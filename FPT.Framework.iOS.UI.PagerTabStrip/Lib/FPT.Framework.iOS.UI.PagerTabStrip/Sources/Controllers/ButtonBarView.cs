using System;
using Foundation;
using UIKit;
using CoreGraphics;

namespace FPT.Framework.iOS.UI.PagerTabStrip
{

	public enum PagerScroll
	{
		No, Yes, ScrollOnlyIfOutOfScreen
	}

	public enum SelectedBarAlignment
	{
		Left, Center, Right, Progressive
	}

	[Register("ButtonBarView")]
	public class ButtonBarView : UICollectionView
	{

		#region PROPERTIES

		public UIView SelectedBar
		{
			get
			{
				var bar = new UIView(new CGRect(0, this.Frame.Size.Height - this.SelectedBarHeight, 0, this.SelectedBarHeight));
				bar.Layer.ZPosition = 9999;
				return bar;
			}
		}

		private nfloat mSelectedBarHeight = 4f;
		internal nfloat SelectedBarHeight
		{
			get
			{
				return mSelectedBarHeight;
			}
			set
			{
				mSelectedBarHeight = value;
				UpdateSlectedBarYPosition();
			}
		}

		public SelectedBarAlignment SelectedBarAlignment { get; set; } = SelectedBarAlignment.Center;

		public int SelectedIndex { get; set; } = 0;


		#endregion

		#region CONSTRUCTORS

		protected ButtonBarView(IntPtr handle) : base(handle)
		{
			AddSubview(SelectedBar);
		}

		public ButtonBarView(NSCoder coder) : base(coder)
		{
			AddSubview(SelectedBar);
		}

		public ButtonBarView(CGRect frame, UICollectionViewLayout layout) : base(frame, layout)
		{
			AddSubview(SelectedBar);
		}

		#endregion

		#region FUNCTIONS

		public void MoveTo(int index, bool animated, SwipeDirection swipeDirection, PagerScroll pagerScroll)
		{
			SelectedIndex = index;
			UpdateSelectedBarPosition(animated, swipeDirection, pagerScroll);
		}

		public void MoveTo(int fromIndex, bool toIndex, nfloat progressPercentage, PagerScroll pagerScroll)
		{
		}

		private void UpdateSelectedBarPosition(bool animated, SwipeDirection swipeDirection, PagerScroll pagerScroll)
		{
			
		}

		#endregion

		#region HELPERS

		private void UpdateContentOffset(bool animated, PagerScroll pagerScroll, CGRect toFrame, int toIndex)
		{
			if (pagerScroll != PagerScroll.No || (pagerScroll != PagerScroll.ScrollOnlyIfOutOfScreen && (toFrame.X < ContentOffset.X || toFrame.X >= (ContentOffset.X + Frame.Width - ContentInset.Left))))
			{
				var targetContentOffset = ContentSize.Width > Frame.Size.Width ? ContentOffsetForCell(toFrame, toIndex) : 0;
				SetContentOffset(new CGPoint(targetContentOffset, 0), animated);
			}
		}

		private nfloat ContentOffsetForCell(CGRect cellFrame, int index)
		{
			var sectionInset = (CollectionViewLayout as UICollectionViewFlowLayout).SectionInset;
			nfloat alignmentOffset = 0.0f;

			switch (SelectedBarAlignment)
			{
				case SelectedBarAlignment.Left:
					{
						alignmentOffset = sectionInset.Left;
					}
					break;
				case SelectedBarAlignment.Right:
					{
						alignmentOffset = Frame.Width - sectionInset.Right - cellFrame.Width;
					}
					break;
				case SelectedBarAlignment.Center:
					{
						alignmentOffset = (Frame.Width - cellFrame.Width) * 0.5f;
					}
					break;
				case SelectedBarAlignment.Progressive:
					{
						var cellHalfWidth = cellFrame.Width * 0.5f;
						var leftAlignmentOffset = sectionInset.Left + cellHalfWidth;
						var rightAlignmentOffset = Frame.Width - sectionInset.Right - cellHalfWidth;
						var numberOfItems = DataSource.GetItemsCount(this, 0);
						var progress = index / (numberOfItems - 1);
						alignmentOffset = leftAlignmentOffset + (rightAlignmentOffset - leftAlignmentOffset) * progress - cellHalfWidth;
					}
					break;
			}

			var contentOffset = cellFrame.X - alignmentOffset;
			contentOffset = NMath.Max(0, contentOffset);
			contentOffset = NMath.Min(ContentSize.Width - Frame.Size.Width, contentOffset);
			return contentOffset;
		}

		private void UpdateSlectedBarYPosition()
		{
			var selectedBarFrame = SelectedBar.Frame;
			selectedBarFrame.Y = Frame.Size.Height - SelectedBarHeight;
			selectedBarFrame.Height = SelectedBarHeight;
			SelectedBar.Frame = selectedBarFrame;
		}

		#endregion
	}
}
