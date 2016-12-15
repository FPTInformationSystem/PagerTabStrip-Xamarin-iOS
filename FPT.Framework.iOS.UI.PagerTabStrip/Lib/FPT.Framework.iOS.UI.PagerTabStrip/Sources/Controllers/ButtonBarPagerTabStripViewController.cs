using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

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

		private Func<IndicatorInfo, nfloat> Width { get; set;}

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
			public nfloat ButtonBarMinimumLineSpacing { get; set; }
			public nfloat ButtonBarRightContentInset { get; set;}

			public UIColor SelectedBarBackgroundColor { get; set; } = UIColor.Black;
			public nfloat SelectedBarHeight { get; set; } = 5f;

			public UIColor ButtonBarItemBackgroundColor { get; set;}
			public UIFont ButtonBarItemFont { get; set; } = UIFont.SystemFontOfSize(18f);
			public nfloat ButtonBarItemLeftRightMargin { get; set; } = 8f;
			public UIColor ButtonBarItemTitleColor { get; set;}
			public bool ButtonBarItemsShouldFillAvailiableWidth { get; set;}

			public nfloat ButtonBarHeight { get; set;}
		}

		public StyleDefinition Style { get; set; } = new StyleDefinition();
	}

	[Register("ButtonBarPagerTabStripViewController")]
	public class ButtonBarPagerTabStripViewController : PagerTabStripViewController, PagerTabStripDataSource, PagerTabStripIsProgressiveDelegate, IUICollectionViewDelegate, IUICollectionViewDataSource
	{

		#region PROPERTIES

		public ButtonBarPagerTabStripSettings Settings { get; set;}

		public ButtonBarItemSpec ButtonBarItemSpec { get; set;}

		public Action<ButtonBarViewCell, ButtonBarViewCell, bool> ChangeCurrentIndex { get; set;}

		public Action<ButtonBarViewCell, ButtonBarViewCell, nfloat, bool, bool> ChangeCurrentIndexProgressive { get; set;}

		[Outlet]
		ButtonBarView amountButton { get; set; }

		#endregion

		#region CONSTRUCTORS

		public ButtonBarPagerTabStripViewController()
		{
		}

		#endregion

		public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			throw new NotImplementedException();
		}

		public nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			throw new NotImplementedException();
		}

		public override IList<UIViewController> GetViewControllers(PagerTabStripViewController pagerTabStripController)
		{
			throw new NotImplementedException();
		}

		public void UpdateIndicator(PagerTabStripViewController viewController, int fromIndex, int toIndex)
		{
			throw new NotImplementedException();
		}

		public void UpdateIndicator(PagerTabStripViewController viewController, int fromIndex, int toIndex, nfloat progressPercentage, bool indexWasChanged)
		{
			throw new NotImplementedException();
		}
	}
}
