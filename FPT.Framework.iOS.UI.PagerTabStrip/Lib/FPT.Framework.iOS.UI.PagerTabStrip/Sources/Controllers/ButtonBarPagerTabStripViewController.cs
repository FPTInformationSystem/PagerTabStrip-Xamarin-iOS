using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using UIKit;
using System.Diagnostics;

namespace FPT.Framework.iOS.UI.PagerTabStrip
{

	public enum ButtonBarItemSpecType
	{
		NibFile, CellClass
	}

	public class ButtonBarItemSpec
	{

		#region PROPERTIES

		public ButtonBarItemSpecType ButtonBarItemSpecType { get; set;}

		public string NibName { get; set;}

		public NSBundle Bundle { get; set;}

		public Func<IndicatorInfo, nfloat> Width { get; set;}

		public Func<IndicatorInfo, nfloat> Weight
		{
			get
			{
				if (ButtonBarItemSpecType == ButtonBarItemSpecType.CellClass)
					return Width;
				else if (ButtonBarItemSpecType == ButtonBarItemSpecType.NibFile)
				{
					return Width;
				}

				return null;
			}
		}

		#endregion

		#region CONSTRUCTORS

		protected ButtonBarItemSpec()
		{
		}

		#endregion

		#region STATIC FUNCTIONS

		public static ButtonBarItemSpec NibFile(string nibName, NSBundle bundle, Func<IndicatorInfo, nfloat> width)
		{
			return new ButtonBarItemSpec
			{
				NibName = nibName,
				Bundle = bundle,
				Width = width
			};
		}

		public static ButtonBarItemSpec CellClass(Func<IndicatorInfo, nfloat> width)
		{
			return new ButtonBarItemSpec
			{
				Width = width
			};
		}

		#endregion
	}

	public class ButtonBarPagerTabStripSettings
	{
		public class StyleDefinition
		{
			public UIColor ButtonBarBackgroundColor { get; set;}
			public nfloat? ButtonBarMinimumLineSpacing { get; set; }

			public nfloat? ButtonBarLeftContentInset { get; set; }
			public nfloat? ButtonBarRightContentInset { get; set;}

			public UIColor SelectedBarBackgroundColor { get; set; } = UIColor.Black;
			public nfloat SelectedBarHeight { get; set; } = 5f;

			public UIColor ButtonBarItemBackgroundColor { get; set;}
			public UIFont ButtonBarItemFont { get; set; } = UIFont.SystemFontOfSize(18f);
			public nfloat? ButtonBarItemLeftRightMargin { get; set; } = 8f;
			public UIColor ButtonBarItemTitleColor { get; set;}
			public bool ButtonBarItemsShouldFillAvailiableWidth { get; set;}

			public nfloat? ButtonBarHeight { get; set;}
		}

		public StyleDefinition Style { get; set; } = new StyleDefinition();
	}

	public partial class ButtonBarPagerTabStripViewController : PagerTabStripViewController, PagerTabStripDataSource, PagerTabStripIsProgressiveDelegate, IUICollectionViewDataSource, IUICollectionViewDelegateFlowLayout
	{

		#region PROPERTIES

		public ButtonBarPagerTabStripSettings Settings { get; set; } = new ButtonBarPagerTabStripSettings();

		public ButtonBarItemSpec ButtonBarItemSpec { get; set;}

		public Action<ButtonBarViewCell, ButtonBarViewCell, bool> ChangeCurrentIndex { get; set;}

		public Action<ButtonBarViewCell, ButtonBarViewCell, nfloat, bool, bool> ChangeCurrentIndexProgressive { get; set;}

		[Outlet]
		public ButtonBarView ButtonBarView { get; set; }

		private IList<nfloat> mCachedCellWidths = null;
		public IList<nfloat> CachedCellWidths
		{
			get
			{
				if (mCachedCellWidths == null)
				{
					mCachedCellWidths = this.CalculateWidths();
				}
				return mCachedCellWidths;
			}
			set
			{
				mCachedCellWidths = value;
			}
		}

		#endregion

		#region CONSTRUCTORS

		public ButtonBarPagerTabStripViewController(string nibNameOrNil, NSBundle nibBundleOrNil) : base(nibNameOrNil, nibBundleOrNil)
		{
			Delegate = this;
			DataSource = this;
		}

		public ButtonBarPagerTabStripViewController(NSCoder coder) : base(coder)
		{
			Delegate = this;
			DataSource = this;
		}

		public ButtonBarPagerTabStripViewController(IntPtr handle) : base(handle)
		{
			Delegate = this;
			DataSource = this;
		}

		#endregion

		#region FUNCTIONS

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			ButtonBarItemSpec = ButtonBarItemSpec.NibFile(
				nibName: "ButtonCell",
				bundle: NSBundle.FromClass(new ObjCRuntime.Class(typeof(ButtonBarViewCell))),
				width: (childItemInfo) =>
				{
					var label = new UILabel();
					label.TranslatesAutoresizingMaskIntoConstraints = false;
					label.Font = this.Settings.Style.ButtonBarItemFont;
					label.Text = childItemInfo.Title;
					var labelSize = label.IntrinsicContentSize;
				return labelSize.Width + (this.Settings.Style.ButtonBarItemLeftRightMargin?? 8) * 2;
				});

			UICollectionViewFlowLayout flowLayout;
			if (ButtonBarView == null)
			{
				flowLayout = new UICollectionViewFlowLayout();
				flowLayout.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
				var buttonBarHeight = Settings.Style.ButtonBarHeight ?? 44f;
				var buttonBar = new ButtonBarView(new CGRect(0, 0, View.Frame.Width, buttonBarHeight), flowLayout);
				buttonBar.BackgroundColor = UIColor.Orange;
				buttonBar.SelectedBar.BackgroundColor = UIColor.Black;
				buttonBar.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
				var newContainerViewFrame = ContainerView.Frame;
				newContainerViewFrame.Y = buttonBarHeight;
				newContainerViewFrame.Height = ContainerView.Frame.Height - (buttonBarHeight - ContainerView.Frame.Y);
				ContainerView.Frame = newContainerViewFrame;
				ButtonBarView = buttonBar;
			}

			if (ButtonBarView.Superview == null)
			{
				View.AddSubview(ButtonBarView);
			}

			if (ButtonBarView.Delegate == null)
			{
				ButtonBarView.Delegate = this;
			}

			if (ButtonBarView.DataSource == null)
			{
				ButtonBarView.DataSource = this;
			}

			ButtonBarView.ScrollsToTop = false;
			flowLayout = ButtonBarView.CollectionViewLayout as UICollectionViewFlowLayout;
			flowLayout.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
			flowLayout.MinimumInteritemSpacing = 0;
			flowLayout.MinimumLineSpacing = Settings.Style.ButtonBarMinimumLineSpacing ?? flowLayout.MinimumLineSpacing;
			var sectionInset = flowLayout.SectionInset;
			flowLayout.SectionInset = new UIEdgeInsets(sectionInset.Top,
													   Settings.Style.ButtonBarLeftContentInset ?? sectionInset.Left,
													   sectionInset.Bottom,
													   Settings.Style.ButtonBarRightContentInset ?? sectionInset.Right);

			ButtonBarView.ShowsHorizontalScrollIndicator = false;
			ButtonBarView.BackgroundColor = Settings.Style.ButtonBarBackgroundColor ?? ButtonBarView.BackgroundColor;
			ButtonBarView.SelectedBar.BackgroundColor = Settings.Style.SelectedBarBackgroundColor;

			ButtonBarView.SelectedBarHeight = Settings.Style.SelectedBarHeight;

			switch (ButtonBarItemSpec.ButtonBarItemSpecType)
			{
				case ButtonBarItemSpecType.NibFile:
					{
						ButtonBarView.RegisterNibForCell(UINib.FromName(ButtonBarItemSpec.NibName, ButtonBarItemSpec.Bundle), "Cell");
					}
					break;
				case ButtonBarItemSpecType.CellClass:
					{
						ButtonBarView.RegisterClassForCell(typeof(ButtonBarViewCell), "Cell");
					}
					break;
			}
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			ButtonBarView.LayoutIfNeeded();
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			if (IsViewAppearing || IsViewRotating)
			{
				CachedCellWidths = CalculateWidths();
				ButtonBarView.CollectionViewLayout.InvalidateLayout();

				ButtonBarView.MoveTo(CurrentIndex, false, SwipeDirection.None, PagerScroll.ScrollOnlyIfOutOfScreen);
			}
		}

		public override void ReloadPagerTabStripView()
		{
			base.ReloadPagerTabStripView();

			if (IsViewLoaded)
			{
				ButtonBarView.ReloadData();
				CachedCellWidths = CalculateWidths();
				ButtonBarView.MoveTo(CurrentIndex, false, SwipeDirection.None, PagerScroll.Yes);
			}
		}

		public nfloat CalculateStretchedCellWidths(IList<nfloat> minimumCellWidths, nfloat suggestedStretchedCellWidth, int previousNumberOfLargeCells)
		{
			var numberOfLargeCells = 0;
			nfloat totalWidthOfLargeCells = 0;

			foreach (var minimumCellWidthValue in minimumCellWidths)
			{
				if (minimumCellWidthValue > suggestedStretchedCellWidth)
				{
					totalWidthOfLargeCells += minimumCellWidthValue;
					numberOfLargeCells += 1;
				}
			}

			if (numberOfLargeCells <= previousNumberOfLargeCells) return suggestedStretchedCellWidth;

			var flowlayout = ButtonBarView.CollectionViewLayout as UICollectionViewFlowLayout;
			var collectionViewAvailiableWidth = ButtonBarView.Frame.Width - flowlayout.SectionInset.Left - flowlayout.SectionInset.Right;
			var numberOfCells = minimumCellWidths.Count;
			var cellSpacingTotal = (numberOfCells - 1) * flowlayout.MinimumLineSpacing;

			var numberOfSmallCells = numberOfCells - numberOfLargeCells;
			var newSuggestedStretchedCellWidth = (collectionViewAvailiableWidth - totalWidthOfLargeCells - cellSpacingTotal) / numberOfSmallCells;

			return CalculateStretchedCellWidths(minimumCellWidths, newSuggestedStretchedCellWidth, numberOfLargeCells);
		}

		#endregion

		public void UpdateIndicator(PagerTabStripViewController viewController, nint fromIndex, nint toIndex)
		{
			if (!ShouldUpdateButtonBarView) return;
			ButtonBarView.MoveTo(toIndex, true, toIndex < fromIndex ? SwipeDirection.Right : SwipeDirection.Left, PagerScroll.Yes);

			if (ChangeCurrentIndex != null)
			{
				var oldCell = ButtonBarView.CellForItem(NSIndexPath.FromItemSection(CurrentIndex != fromIndex ? fromIndex : toIndex, 0)) as ButtonBarViewCell;
				var newCell = ButtonBarView.CellForItem(NSIndexPath.FromItemSection(CurrentIndex, 0)) as ButtonBarViewCell;
				ChangeCurrentIndex(oldCell, newCell, true);
			}
		}

		public void UpdateIndicator(PagerTabStripViewController viewController, nint fromIndex, nint toIndex, nfloat progressPercentage, bool indexWasChanged)
		{
			if (!ShouldUpdateButtonBarView) return;
			ButtonBarView.Move(fromIndex, toIndex, progressPercentage, PagerScroll.Yes);
			if (ChangeCurrentIndexProgressive != null)
			{
				var oldCell = ButtonBarView.CellForItem(NSIndexPath.FromItemSection(CurrentIndex != fromIndex ? fromIndex : toIndex, 0)) as ButtonBarViewCell;
				var newCell = ButtonBarView.CellForItem(NSIndexPath.FromItemSection(CurrentIndex, 0)) as ButtonBarViewCell;
				ChangeCurrentIndexProgressive(oldCell, newCell, progressPercentage, indexWasChanged, true);
			}
		}

		#region UICollectionViewDelegateFlowLayout

		[Export("collectionView:layout:sizeForItemAtIndexPath:")]
		public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		{
			var cellWidthValue = CachedCellWidths[indexPath.Row];
			if (cellWidthValue == null)
			{
				Debug.Fail(String.Format("cachedCellWidths for {0} must not be nil", indexPath.Row));
			}
			return new CGSize(cellWidthValue, collectionView.Frame.Height);
		}

		[Export("collectionView:didSelectItemAtIndexPath:")]
		public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			if (indexPath.Item == CurrentIndex) return;

			ButtonBarView.MoveTo(indexPath.Item, true, SwipeDirection.None, PagerScroll.Yes);
			ShouldUpdateButtonBarView = false;

			var oldCell = ButtonBarView.CellForItem(NSIndexPath.FromItemSection(CurrentIndex, 0)) as ButtonBarViewCell;
			var newCell = ButtonBarView.CellForItem(NSIndexPath.FromItemSection(indexPath.Item, 0)) as ButtonBarViewCell;
			if (PagerBehaviour.ProgressiveIndicator)
			{
				if (ChangeCurrentIndexProgressive != null)
				{
					ChangeCurrentIndexProgressive(oldCell, newCell, 1f, true, true);
				}
			}
			else
			{
				if (ChangeCurrentIndex != null)
				{
					ChangeCurrentIndex(oldCell, newCell, true);
				}
			}

			MoveToViewController(indexPath.Item);
        }

		#endregion

		#region UICollectionViewDataSource

		public nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			return ViewControllers.Count;
		}

		public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell("Cell", indexPath) as ButtonBarViewCell;

			if (cell == null)
			{
				Debug.Fail("UICollectionViewCell should be or extend from ButtonBarViewCell");
			}
			var childController = ViewControllers[(int)indexPath.Item] as IndicatorInfoProvider;
			var indicatorInfo = childController.GetIndicatorInfo(this);

			cell.TitleLabel.Text = indicatorInfo.Title;
			cell.TitleLabel.Font = Settings.Style.ButtonBarItemFont;
			cell.TitleLabel.TextColor = Settings.Style.ButtonBarItemTitleColor ?? cell.TitleLabel.TextColor;

			cell.ContentView.BackgroundColor = Settings.Style.ButtonBarItemBackgroundColor ?? cell.ContentView.BackgroundColor;
			cell.BackgroundColor = Settings.Style.ButtonBarItemBackgroundColor ?? cell.BackgroundColor;

			if (indicatorInfo.Image != null)
			{
				cell.ImageView.Image = indicatorInfo.Image;
			}

			if (indicatorInfo.HighlightedImage != null)
			{
				cell.ImageView.HighlightedImage = indicatorInfo.HighlightedImage;
			}

			ConfigureCell(cell, indicatorInfo);

			if (PagerBehaviour.ProgressiveIndicator)
			{
				if (ChangeCurrentIndexProgressive != null)
				{
					ChangeCurrentIndexProgressive(CurrentIndex == indexPath.Item ? null : cell, CurrentIndex == indexPath.Item ? cell : null, 1, true, false);
				}
			}
			else
			{
				if (ChangeCurrentIndex != null)
				{
					ChangeCurrentIndex(CurrentIndex == indexPath.Item ? null : cell, CurrentIndex == indexPath.Item ? cell : null, false);
				}
			}

			return cell;
		}



		public override IList<UIViewController> GetViewControllers(PagerTabStripViewController pagerTabStripController)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region UICollectionViewDataSource

		#endregion

		#region UIScrollViewDelegate

		[Export("scrollViewDidEndScrollingAnimation:")]
		public void ScrollAnimationEnded(UIScrollView scrollView)
		{
			base.ScrollAnimationEnded(scrollView);

			if (scrollView == ContainerView)
			{
				ShouldUpdateButtonBarView = true;
			}
		}

		#endregion

		public virtual void ConfigureCell(ButtonBarViewCell cell, IndicatorInfo indicatorInfo)
		{
		}

		private IList<nfloat> CalculateWidths()
		{
			var flowLayout = ButtonBarView.CollectionViewLayout as UICollectionViewFlowLayout;
			var numberOfCells = ViewControllers.Count;

			var minimumCellWidths = new List<nfloat>();
			nfloat collectionViewContentWidth = 0;

			foreach (var viewController in ViewControllers)
			{
				var childController = viewController as IndicatorInfoProvider;
				var indicatorInfo = childController.GetIndicatorInfo(this);
				switch (ButtonBarItemSpec.ButtonBarItemSpecType)
				{
					case ButtonBarItemSpecType.CellClass:
						{
							var width = ButtonBarItemSpec.Width(indicatorInfo);
							minimumCellWidths.Add(width);
							collectionViewContentWidth += width;
						}
						break;
					case ButtonBarItemSpecType.NibFile:
						{
							var width = ButtonBarItemSpec.Width(indicatorInfo);
							minimumCellWidths.Add(width);
							collectionViewContentWidth += width;
						}
						break;
				}
			}

			var cellSpacingTotal = (numberOfCells - 1) * flowLayout.MinimumLineSpacing;
			collectionViewContentWidth += cellSpacingTotal;

			var collectionViewAvailableVisibleWidth = ButtonBarView.Frame.Width - flowLayout.SectionInset.Left - flowLayout.SectionInset.Right;

			if (!Settings.Style.ButtonBarItemsShouldFillAvailiableWidth || collectionViewAvailableVisibleWidth < collectionViewContentWidth)
			{
				return minimumCellWidths;
			}
			else
			{
				var stretchedCellWidthIfAllEqual = (collectionViewAvailableVisibleWidth - cellSpacingTotal) / numberOfCells;
				var generalMinimumCellWidth = CalculateStretchedCellWidths(minimumCellWidths, stretchedCellWidthIfAllEqual, 0);
				var stretchedCellWidths = new List<nfloat>();

				foreach (var minimumCellWidthValue in minimumCellWidths)
				{
					var cellWidth = (minimumCellWidthValue > generalMinimumCellWidth) ? minimumCellWidthValue : generalMinimumCellWidth;
					stretchedCellWidths.Add(cellWidth);
				}

				return stretchedCellWidths;
			}
		}

		private bool ShouldUpdateButtonBarView { get; set; } = true;
	}
}
