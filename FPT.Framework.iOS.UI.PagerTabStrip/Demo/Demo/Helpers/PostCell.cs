using System;

using Foundation;
using UIKit;

namespace Demo
{
	public partial class PostCell : UITableViewCell
	{

		#region PROPERTIES

		public static readonly NSString Key = new NSString("PostCell");
		public static readonly UINib Nib;

		#endregion

		#region CONSTRUCTORS

		static PostCell()
		{
			Nib = UINib.FromName("PostCell", NSBundle.MainBundle);
		}

		protected PostCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		#endregion

		#region FUNCTIONS

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
			userImage.Layer.CornerRadius = 10f;
		}

		internal void ConfigureWithData(NSDictionary data)
		{
			var post = data["name"] as NSDictionary;
			var user = post["text"] as NSDictionary;

			if (post != null && user != null)
			{
				postName.Text = user["name"] as NSString;
				postText.Text = post["text"] as NSString;
				userImage.Image = UIImage.FromBundle(postName.Text.Replace(" ", "_"));
			}
		}

		internal void ChangeStylToBlack()
		{
			userImage.Layer.CornerRadius = 30f;
			postText.Text = null;
			postName.Font = UIFont.FromName("HelveticaNeue-Light", 18f) ?? UIFont.SystemFontOfSize(18f);
			postName.TextColor = UIColor.White;
			BackgroundColor = new UIColor(red: 15 / 255f, green: 16 / 255f, blue: 16 / 255f, alpha: 1f);
		}

		#endregion

	}
}
