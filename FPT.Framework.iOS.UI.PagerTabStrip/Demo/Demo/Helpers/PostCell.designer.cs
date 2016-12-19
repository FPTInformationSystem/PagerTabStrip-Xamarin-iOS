// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Demo
{
	[Register ("PostCell")]
	partial class PostCell
	{
		[Outlet]
		UIKit.UILabel postName { get; set; }

		[Outlet]
		UIKit.UILabel postText { get; set; }

		[Outlet]
		UIKit.UIImageView userImage { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (userImage != null) {
				userImage.Dispose ();
				userImage = null;
			}

			if (postName != null) {
				postName.Dispose ();
				postName = null;
			}

			if (postText != null) {
				postText.Dispose ();
				postText = null;
			}
		}
	}
}
