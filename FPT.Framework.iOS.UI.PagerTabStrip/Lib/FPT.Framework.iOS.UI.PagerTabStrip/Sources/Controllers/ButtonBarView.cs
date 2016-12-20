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

		private UIView mSelectedBar = null;
		public UIView SelectedBar
		{
			get
			{
				if (mSelectedBar == null)
				{
					var bar = new UIView(new CGRect(0, this.Frame.Size.Height - this.SelectedBarHeight, 0, this.SelectedBarHeight));
					bar.Layer.ZPosition = 9999;
					mSelectedBar = bar;
				}
				return mSelectedBar;
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

		public nint SelectedIndex { get; set; } = 0;


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

		public void MoveTo(nint index, bool animated, SwipeDirection swipeDirection, PagerScroll pagerScroll)
		{
			SelectedIndex = index;
			UpdateSelectedBarPosition(animated, swipeDirection, pagerScroll);
		}

		public void Move(nint fromIndex, nint toIndex, nfloat progressPercentage, PagerScroll pagerScroll)
		{
			SelectedIndex = progressPercentage > 0.5f ? toIndex : fromIndex;

			var fromFrame = GetLayoutAttributesForItem(NSIndexPath.FromItemSection(fromIndex, 0)).Frame;
			var numberOfItems = DataSource.GetItemsCount(this, 0);

			CGRect toFrame;

			if (toIndex < 0 || toIndex > numberOfItems - 1)
			{
				if (toIndex < 0)
				{
					var cellAtts = GetLayoutAttributesForItem(NSIndexPath.FromItemSection(0, 0));
					toFrame = cellAtts.Frame;
					toFrame.Offset(-cellAtts.Frame.Width, 0);
				}
				else
				{
					var cellAtts = GetLayoutAttributesForItem(NSIndexPath.FromItemSection(numberOfItems - 1, 0));
					toFrame = cellAtts.Frame;
					toFrame.Offset(cellAtts.Frame.Width, 0);
				}
			}
			else
			{
				toFrame = GetLayoutAttributesForItem(NSIndexPath.FromItemSection(toIndex, 0)).Frame;
			}

			var targetFrame = fromFrame;
			targetFrame.Height = SelectedBar.Frame.Height;
			targetFrame.Width += (toFrame.Size.Width - fromFrame.Size.Width) * progressPercentage;
			targetFrame.X += (toFrame.X - fromFrame.X) * progressPercentage;

			SelectedBar.Frame = new CGRect(targetFrame.X, SelectedBar.Frame.Y, targetFrame.Width, SelectedBar.Frame.Height);

			nfloat targetContentOffset = 0.0f;
			if (ContentSize.Width > Frame.Width)
			{
				var toContentOffset = ContentOffsetForCell(toFrame, toIndex);
				var fromContentOffset = ContentOffsetForCell(fromFrame, fromIndex);

				targetContentOffset = fromContentOffset + ((toContentOffset - fromContentOffset) * progressPercentage);
			}

			var animated = Math.Abs(ContentOffset.X - targetContentOffset) > 30 || (fromIndex == toIndex);
			SetContentOffset(new CGPoint(targetContentOffset, 0), animated);
		}

		private void UpdateSelectedBarPosition(bool animated, SwipeDirection swipeDirection, PagerScroll pagerScroll)
		{
			var selectedBarFrame = SelectedBar.Frame;

			var selectedCellIndexPath = NSIndexPath.FromItemSection(SelectedIndex, 0);
			var attributes = GetLayoutAttributesForItem(selectedCellIndexPath);
			var selectedCellFrame = attributes.Frame;

			UpdateContentOffset(animated, pagerScroll, selectedCellFrame, (selectedCellIndexPath as NSIndexPath).Row);

			selectedBarFrame.Width = selectedCellFrame.Width;
			selectedBarFrame.X = selectedCellFrame.X;

			if (animated)
			{
				UIView.Animate(duration: 0.3, animation: () =>
				{
					this.SelectedBar.Frame = selectedBarFrame;
				});
			}
			else
			{
				SelectedBar.Frame = selectedBarFrame;
			}
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

		private nfloat ContentOffsetForCell(CGRect cellFrame, nint index)
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
