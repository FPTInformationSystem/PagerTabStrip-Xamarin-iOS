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
	[Register ("ReloadExampleViewController")]
	partial class ReloadExampleViewController
	{
		[Action ("closeTapped:")]
		partial void closeTapped (Foundation.NSObject sender);

		[Action ("reloadTapped:")]
		partial void reloadTapped (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (titleLabel != null) {
				titleLabel.Dispose ();
				titleLabel = null;
			}
		}
	}
}
