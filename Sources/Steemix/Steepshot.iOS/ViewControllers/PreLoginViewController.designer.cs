// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Steepshot.iOS
{
	[Register ("PreLoginViewController")]
	partial class PreLoginViewController
	{
		[Outlet]
		UIKit.UISwitch devSwitch { get; set; }

		[Outlet]
		UIKit.UIImageView golosImg { get; set; }

		[Outlet]
		UIKit.UIButton loginButton { get; set; }

		[Outlet]
		UIKit.UILabel loginLabel { get; set; }

		[Outlet]
		UIKit.UITextField loginText { get; set; }

		[Outlet]
		UIKit.UIImageView logo { get; set; }

		[Outlet]
		UIKit.UISwitch networkSwitch { get; set; }

		[Outlet]
		UIKit.UILabel signLabel { get; set; }

		[Outlet]
		UIKit.UIButton signUpButton { get; set; }

		[Outlet]
		UIKit.UIImageView steemImg { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (golosImg != null) {
				golosImg.Dispose ();
				golosImg = null;
			}

			if (loginButton != null) {
				loginButton.Dispose ();
				loginButton = null;
			}

			if (loginLabel != null) {
				loginLabel.Dispose ();
				loginLabel = null;
			}

			if (loginText != null) {
				loginText.Dispose ();
				loginText = null;
			}

			if (networkSwitch != null) {
				networkSwitch.Dispose ();
				networkSwitch = null;
			}

			if (signLabel != null) {
				signLabel.Dispose ();
				signLabel = null;
			}

			if (signUpButton != null) {
				signUpButton.Dispose ();
				signUpButton = null;
			}

			if (steemImg != null) {
				steemImg.Dispose ();
				steemImg = null;
			}

			if (devSwitch != null) {
				devSwitch.Dispose ();
				devSwitch = null;
			}

			if (logo != null) {
				logo.Dispose ();
				logo = null;
			}
		}
	}
}