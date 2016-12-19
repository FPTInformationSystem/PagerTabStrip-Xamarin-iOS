using System;
using Foundation;
namespace FPT.Framework.iOS.UI.PagerTabStrip
{

	[Register("PagerTabStripViewController")]
	partial class PagerTabStripViewController
	{
		void ReleaseDesignerOutlets()
		{
			if (ContainerView != null)
			{
				ContainerView.Dispose();
				ContainerView = null;
			}
		}
	}
}
