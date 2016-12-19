using System;
using Foundation;
namespace FPT.Framework.iOS.UI.PagerTabStrip
{

	[Register("ButtonBarPagerTabStripViewController")]
	partial class ButtonBarPagerTabStripViewController
	{
		void ReleaseDesignerOutlets()
		{
			if (ButtonBarView != null)
			{
				ButtonBarView.Dispose();
				ButtonBarView = null;
			}
		}
	}
}
