using System;
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

	public class ButtonBarPagerTabStripViewController
	{
		public ButtonBarPagerTabStripViewController()
		{
		}
	}
}
