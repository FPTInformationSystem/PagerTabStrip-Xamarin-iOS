using System;
namespace FPT.Framework.iOS.UI.PagerTabStrip
{
	public class PagerTabStripError : Exception
	{
		public PagerTabStripError()
		{
		}
	}

	public class ViewControllerNotContainedInPagerTabStrip : PagerTabStripError
	{
		 
	}
}
