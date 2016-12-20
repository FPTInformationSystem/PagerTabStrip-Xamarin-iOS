using System;
using System.Collections.Generic;
using System.Diagnostics;
using Foundation;
using UIKit;
using CoreGraphics;

namespace FPT.Framework.iOS.UI.PagerTabStrip
{

	public interface IndicatorInfoProvider
	{
		IndicatorInfo GetIndicatorInfo(PagerTabStripViewController pagerTabStripController);
	}

	public interface PagerTabStripDelegate
	{
		void UpdateIndicator(PagerTabStripViewController viewController, nint fromIndex, nint toIndex);
	}

	public interface PagerTabStripIsProgressiveDelegate : PagerTabStripDelegate
	{
		void UpdateIndicator(PagerTabStripViewController viewController, nint fromIndex, nint toIndex, nfloat progressPercentage, bool indexWasChanged);
	}

	public interface PagerTabStripDataSource
	{
		IList<UIViewController> GetViewControllers(PagerTabStripViewController pagerTabStripController);
	}

	public partial class PagerTabStripViewController : UIViewController, IUIScrollViewDelegate
	{

		#region PROPERTIES
		[Outlet]
		public UIScrollView ContainerView { get; set;}

		public PagerTabStripDelegate Delegate { get; set;}
		public PagerTabStripDataSource DataSource { get; set;}

		public PagerTabStripBehaviour PagerBehaviour { get; set; } = PagerTabStripBehaviour.Progressive(true, true);

		public IList<UIViewController> ViewControllers { get; private set; } = new List<UIViewController>();
		public nint CurrentIndex { get; set; } = 0;

		public nfloat PageWidth
		{
			get
			{
				return ContainerView.Bounds.Width;
			}
		}

		public nfloat ScrollPercentage
		{
			get
			{
				if (SwipeDirection != SwipeDirection.Right)
				{
					var module = ContainerView.ContentOffset.X % PageWidth;
					return Math.Abs(module) < nfloat.Epsilon ? 1.0f : module / PageWidth;
				}

				return 1 - (ContainerView.ContentOffset.X >= 0? ContainerView.ContentOffset.X : PageWidth + ContainerView.ContentOffset.X) % PageWidth / PageWidth;
			}
		}

		public SwipeDirection SwipeDirection
		{
			get
			{
				if (ContainerView.ContentOffset.X > LastContentOffset)
				{
					return SwipeDirection.Left;
				}
				else if (ContainerView.ContentOffset.X < LastContentOffset)
				{
					return SwipeDirection.Right;
				}
				return SwipeDirection.None;
			}
		}

		#endregion

		#region CONSTRUCTORS

		protected PagerTabStripViewController(string nibNameOrNil, NSBundle nibBundleOrNil) : base(nibNameOrNil, nibBundleOrNil) { }

		protected PagerTabStripViewController(NSCoder coder) : base(coder) { }

		protected PagerTabStripViewController(IntPtr handle) : base(handle)
		{
		}

		public PagerTabStripViewController()
		{
		}

		#endregion

		#region FUNCTIONS

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			if (ContainerView == null)
			{
				ContainerView = new UIScrollView(new CGRect(0,0,View.Bounds.Width, View.Bounds.Height));
				ContainerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			}

			if (ContainerView.Superview == null)
			{
				View.AddSubview(ContainerView);
			}

			ContainerView.Bounces = true;
			ContainerView.AlwaysBounceHorizontal = true;
			ContainerView.AlwaysBounceVertical = false;
			ContainerView.ScrollsToTop = false;
			ContainerView.Delegate = this;
			ContainerView.ShowsVerticalScrollIndicator = false;
			ContainerView.ShowsHorizontalScrollIndicator = false;
			ContainerView.PagingEnabled = true;
			ReloadViewControllers();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			IsViewAppearing = true;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			LastSize = ContainerView.Bounds.Size;
			UpdateIfNeeded();
			IsViewAppearing = false;
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			UpdateIfNeeded();
		}

		public void MoveToViewController(nint index, bool animated = true)
		{
			if (!IsViewLoaded || View.Window == null)
			{
				CurrentIndex = index;
				return;
			}
			if (animated && PagerBehaviour.SkipIntermediateViewControllers && Math.Abs(CurrentIndex - index) > 1)
			{
				var tmpViewControllers = ViewControllers;
				var currentChildVC = ViewControllers[(int)CurrentIndex];
				var fromIndex = CurrentIndex < index ? index - 1 : index + 1;
				var fromChildVC = ViewControllers[(int)fromIndex];
				tmpViewControllers[(int)CurrentIndex] = currentChildVC;
				PagerTabStripChildViewControllersForScrolling = tmpViewControllers;
				ContainerView.SetContentOffset(new CGPoint(PageOffsetForChild(fromIndex), 0), false);
				if (NavigationController != null)
				{
					(NavigationController.View ?? View).UserInteractionEnabled = !animated;
				}
				ContainerView.SetContentOffset(new CGPoint(PageOffsetForChild(index), 0), true);
			}
			else
			{
				if (NavigationController != null)
				{
					(NavigationController.View ?? View).UserInteractionEnabled = !animated;
				}
				ContainerView.SetContentOffset(new CGPoint(PageOffsetForChild(index), 0), animated);
			}
		}

		public void MoveTo(UIViewController viewController, bool animated = true)
		{
			MoveToViewController(ViewControllers.IndexOf(viewController), animated);
		}

		#endregion

		#region PagerTabStripDataSource

		public virtual IList<UIViewController> GetViewControllers(PagerTabStripViewController pagerTabStripController)
		{
			Debug.Fail("Sub-class must implement the PagerTabStripDataSource viewControllers(for:) method");

			return null;
		}

		#endregion

		#region HELPERS

		public void UpdateIfNeeded()
		{
			if (IsViewLoaded && !LastSize.Equals(ContainerView.Bounds.Size))
			{
				UpdateContent();
			}
		}

		public bool CanMoveTo(int index)
		{
			return CurrentIndex != index && ViewControllers.Count > index;
		}

		public nfloat PageOffsetForChild(nint index)
		{
			return index * ContainerView.Bounds.Width;
		}

		public nfloat OffsetForChild(int index)
		{
			return (index * ContainerView.Bounds.Width) + ((ContainerView.Bounds.Width - View.Bounds.Width) * 0.5f);
		}

		public nfloat OffsetForChild(UIViewController viewController)
		{
			var index = ViewControllers.IndexOf(viewController);
			if (index > -1)
				throw new ViewControllerNotContainedInPagerTabStrip();

			return OffsetForChild(index);
		}

		public int PageFor(nfloat contentOffset)
		{
			var result = VirtualPageFor(contentOffset);
			return PageFor(result);
		}

		public int VirtualPageFor(nfloat contentOffset)
		{
			return ((int)((contentOffset + 1.5f*PageWidth) / PageWidth)) -1;
		}

		public int PageFor(int virtualPage)
		{
			if (virtualPage < 0)
			{
				return 0;
			}
			if (virtualPage > ViewControllers.Count - 1)
			{
				return ViewControllers.Count - 1;
			}
			return virtualPage;
		}

		public void UpdateContent()
		{
			if (LastSize.Width != ContainerView.Bounds.Size.Width)
			{
				LastSize = ContainerView.Bounds.Size;
				ContainerView.ContentOffset = new CGPoint(PageOffsetForChild(CurrentIndex), 0);
			}
			LastSize = ContainerView.Bounds.Size;

			var pagerViewControllers = PagerTabStripChildViewControllersForScrolling ?? ViewControllers;
			ContainerView.ContentSize = new CGSize(ContainerView.Bounds.Width * pagerViewControllers.Count, ContainerView.ContentSize.Height);

			for (var index = 0; index < pagerViewControllers.Count; index++)
			{
				var childController = pagerViewControllers[index];
				var pageOffsetForChild = PageOffsetForChild(index);
				if (NMath.Abs(ContainerView.ContentOffset.X - pageOffsetForChild) < ContainerView.Bounds.Width)
				{
					if (childController.ParentViewController != null)
					{
						childController.View.Frame = new CGRect(OffsetForChild(index), 0, View.Bounds.Width, ContainerView.Bounds.Height);
						childController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
					}
					else
					{
						AddChildViewController(childController);
						childController.BeginAppearanceTransition(true, false);
						childController.View.Frame = new CGRect(OffsetForChild(index), 0, View.Bounds.Width, ContainerView.Bounds.Height);
						childController.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
						ContainerView.AddSubview(childController.View);
						childController.DidMoveToParentViewController(this);
						childController.EndAppearanceTransition();
					}
				}
				else
				{
					if (childController.ParentViewController != null)
					{
						childController.WillMoveToParentViewController(null);
						childController.BeginAppearanceTransition(false, false);
						childController.View.RemoveFromSuperview();
						childController.RemoveFromParentViewController();
						childController.EndAppearanceTransition();
					}
				}
			}

			var oldCurrentIndex = CurrentIndex;
			var virtualPage = VirtualPageFor(ContainerView.ContentOffset.X);
			var newCurrentIndex = PageFor(virtualPage);
			CurrentIndex = newCurrentIndex;
			var changeCurrentIndex = newCurrentIndex != oldCurrentIndex;

			var progressiveDelegate = this as PagerTabStripIsProgressiveDelegate;
			if (progressiveDelegate != null && PagerBehaviour.ProgressiveIndicator)
			{
				var t = ProgressiveIndicatorData(virtualPage);
				progressiveDelegate.UpdateIndicator(
					viewController: this,
					fromIndex: t.Item1,
					toIndex: t.Item2,
					progressPercentage: t.Item3,
					indexWasChanged: changeCurrentIndex);
			}
			else
			{
				if (Delegate != null)
				{
					Delegate.UpdateIndicator(this, NMath.Min(oldCurrentIndex, pagerViewControllers.Count - 1), newCurrentIndex);
				}
			}
		}

		public virtual void ReloadPagerTabStripView()
		{
			if (!IsViewLoaded) return;
			foreach (var childController in ViewControllers)
			{
				if (childController.ParentViewController != null)
				{
					childController.View.RemoveFromSuperview();
					childController.WillMoveToParentViewController(null);
					childController.RemoveFromParentViewController();
				}
			}
			ReloadViewControllers();
			ContainerView.ContentSize = new CGSize(ContainerView.Bounds.Width * ViewControllers.Count, ContainerView.ContentSize.Height);
			if (CurrentIndex >= ViewControllers.Count)
			{
				CurrentIndex = ViewControllers.Count - 1;
			}
			ContainerView.ContentOffset = new CGPoint(PageOffsetForChild(CurrentIndex), 0);
			UpdateContent();
		}

		#endregion

		#region UIScrollDelegate

		[Export("scrollViewDidScroll:")]
		public void Scrolled(UIScrollView scrollView)
		{
			if (ContainerView == scrollView)
			{
				UpdateContent();
			}
		}

		[Export("scrollViewWillBeginDragging:")]
		public void DraggingStarted(UIScrollView scrollView)
		{
			if (ContainerView == scrollView)
			{
				LastPageNumber = PageFor(scrollView.ContentOffset.X);
				LastContentOffset = scrollView.ContentOffset.X;
			}
		}

		[Export("scrollViewDidEndScrollingAnimation" +
		        ":")]
		public virtual void ScrollAnimationEnded(UIScrollView scrollView)
		{
			if (ContainerView == scrollView)
			{
				PagerTabStripChildViewControllersForScrolling = null;
				if (NavigationController != null)
				{
					(NavigationController.View ?? View).UserInteractionEnabled = true;
				}
				UpdateContent();
			}
		}

		#endregion

		#region PRIVATE MEMBERS

		private Tuple<nint, nint, nfloat> ProgressiveIndicatorData(int virtualPage)
		{
			var count = ViewControllers.Count;
			var fromIndex = CurrentIndex;
			var toIndex = CurrentIndex;
			var Direction = SwipeDirection;

			if (Direction == SwipeDirection.Left)
			{
				if (virtualPage > count - 1)
				{
					fromIndex = count - 1;
					toIndex = count;
				}
				else
				{
					if (this.ScrollPercentage >= 0.5f)
					{
						fromIndex = NMath.Max(toIndex -1, 0);
					}
					else
					{
						toIndex = fromIndex + 1;
					}
				}
			}
			else if (Direction == SwipeDirection.Right)
			{
				if (virtualPage < 0)
				{
					fromIndex = 0;
					toIndex = -1;
				}
				else
				{
					if (this.ScrollPercentage > 0.5)
					{
						fromIndex = NMath.Min(toIndex + 1, count - 1);
					}
					else
					{
						toIndex = fromIndex - 1;
					}
				}
			}
			var scrollPercentage = PagerBehaviour.ElasticIndicatorLimit ?
												 this.ScrollPercentage : ((toIndex < 0 || toIndex >= count) ? 0.0f : this.ScrollPercentage);
			return new Tuple<nint, nint, nfloat>(fromIndex, toIndex, scrollPercentage);
		}

		private void ReloadViewControllers()
		{
			Debug.Assert(DataSource != null, "dataSource must not be nil");
			ViewControllers = DataSource.GetViewControllers(this);
			Debug.Assert(ViewControllers.Count > 0, "viewControllers(for:) should provide at least one child view controller");
			foreach (var viewController in ViewControllers)
			{
				Debug.Assert((viewController is IndicatorInfoProvider), "Every view controller provided by PagerTabStripDataSource's viewControllers(for:) method must conform to  InfoProvider");
			}
		}

		private IList<UIViewController> PagerTabStripChildViewControllersForScrolling { get; set;}
		private int LastPageNumber { get; set;} = 0;
		private nfloat LastContentOffset { get; set;} = 0f;
		private int PageBeforeRotate { get; set;} = 0;
		private CGSize LastSize { get; set; } = new CGSize(0, 0);
		internal bool IsViewRotating { get; set;} = false;
		internal bool IsViewAppearing {get; set;} = false;

	    #endregion
	}
}
