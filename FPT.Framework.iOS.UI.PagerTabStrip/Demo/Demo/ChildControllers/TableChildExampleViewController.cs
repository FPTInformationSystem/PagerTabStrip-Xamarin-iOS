using System;
using System.Diagnostics;
using UIKit;
using Foundation;
using FPT.Framework.iOS.UI.PagerTabStrip;

namespace Demo
{
	public class TableChildExampleViewController : UITableViewController, IndicatorInfoProvider
	{

		#region PROPERTIES

		string CellIdentifier { get { return "postCell"; } }
		bool BlackTheme { get; set; } = false;
		IndicatorInfo ItemInfo { get; set; } = new IndicatorInfo("View");

		#endregion

		#region CONSTRUCTORS

		public TableChildExampleViewController(UITableViewStyle style, IndicatorInfo itemInfo) : base(style)
		{
			this.ItemInfo = itemInfo;
		}

		public TableChildExampleViewController(NSCoder coder) : base(coder)
		{
			Debug.Fail("init(coder:) has not been implemented");	
		}

		public TableChildExampleViewController(IntPtr handle) : base(handle)
		{
		}

		#endregion

		#region FUNCTIONS

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			TableView.RegisterNibForCellReuse(UINib.FromName("PostCell", NSBundle.MainBundle), CellIdentifier);
			TableView.EstimatedRowHeight = 60f;
			TableView.RowHeight = UITableView.AutomaticDimension;
			TableView.AllowsSelection = false;
			if (BlackTheme)
			{
				TableView.BackgroundColor = new UIColor(red: 15 / 255f, green: 16 / 255f, blue: 16 / 255f, alpha: 1f);
			}
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			TableView.ReloadData();
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell(CellIdentifier, indexPath) as PostCell;
			var data = DataProvider.SharedInstance.PostsData.GetItem<NSDictionary>((uint)indexPath.Row);
			cell.ConfigureWithData(data);
			if (BlackTheme)
			{
				cell.ChangeStylToBlack();
			}
			return cell;
		}

		public IndicatorInfo GetIndicatorInfo(PagerTabStripViewController pagerTabStripController)
		{
			return ItemInfo;
		}

		#endregion
	}
}
