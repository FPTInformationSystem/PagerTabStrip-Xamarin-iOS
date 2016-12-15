using System;
namespace FPT.Framework.iOS.UI.PagerTabStrip
{

	public enum PagerTabStripBehaviourType
	{
		Common, Progressive
	}

	public class PagerTabStripBehaviour
	{

		public PagerTabStripBehaviourType PagerTabStripBehaviourType { get; set;}

		public bool SkipIntermediateViewControllers { get; set;}

		public bool ProgressiveIndicator
		{
			get
			{
				return PagerTabStripBehaviourType == PagerTabStripBehaviourType.Common ? false : true;
			}
		}

		public bool ElasticIndicatorLimit { get; set;}

		protected PagerTabStripBehaviour()
		{
		}

		public static PagerTabStripBehaviour Common(bool skipIntermediateViewControllers)
		{
			return new PagerTabStripBehaviour
			{
				PagerTabStripBehaviourType = PagerTabStripBehaviourType.Common,
				SkipIntermediateViewControllers = skipIntermediateViewControllers,
				ElasticIndicatorLimit = false
			};
		}

		public static PagerTabStripBehaviour Progressive(bool skipIntermediateViewControllers, bool elasticIndicatorLimit)
		{
			return new PagerTabStripBehaviour
			{
				PagerTabStripBehaviourType = PagerTabStripBehaviourType.Progressive,
				SkipIntermediateViewControllers = skipIntermediateViewControllers,
				ElasticIndicatorLimit = elasticIndicatorLimit
			};
		}
	}
}
