using System;
using PureLayout.Net;
using Steepshot.iOS.Helpers;
using Steepshot.iOS.ViewControllers;
using UIKit;

namespace Steepshot.iOS.Views
{
    public class RegistrationViewController : BaseViewController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.SetNavigationBarHidden(false, false);

            SetBackButton();
            CreateView();
        }

        private void SetBackButton()
        {
            var leftBarButton = new UIBarButtonItem(UIImage.FromBundle("ic_back_arrow"), UIBarButtonItemStyle.Plain, GoBack);
            NavigationItem.LeftBarButtonItem = leftBarButton;
            NavigationController.NavigationBar.TintColor = Constants.R15G24B30;
        }

        private void CreateView()
        {
            var background = new UIView();
            background.BackgroundColor = Constants.R250G250B250;
            View.AddSubview(background);

            var steemit = CreateButton(UIImage.FromBundle("ic_steemit"), "Sign up");
            background.AddSubview(steemit);

            #region constraints

            background.AutoPinEdgesToSuperviewEdges();

            steemit.AutoPinEdgeToSuperviewEdge(ALEdge.Top, 20);
            steemit.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 20);
            steemit.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 20);
            steemit.AutoSetDimension(ALDimension.Height, 80);

            #endregion
        }

        private UIButton CreateButton(UIImage logoSource, string titleText)
        {
            var button = new UIButton();
            button.BackgroundColor = UIColor.White;
            button.Layer.BorderWidth = 1;
            button.Layer.BorderColor = Constants.R244G244B246.CGColor;
            button.Layer.CornerRadius = 16;

            var buttonLogo = new UIImageView(logoSource);
            buttonLogo.ContentMode = UIViewContentMode.ScaleAspectFill;
            button.AddSubview(buttonLogo);

            var buttonTitle = new UILabel();
            buttonTitle.Lines = 2;
            buttonTitle.UserInteractionEnabled = false;
            buttonTitle.Font = Constants.Semibold14;
            buttonTitle.TextColor = UIColor.Black;
            buttonTitle.BackgroundColor = UIColor.Clear;
            buttonTitle.Text = titleText;
            button.AddSubview(buttonTitle);

            var arrow = new UIImageView(UIImage.FromBundle("ic_forward"));
            button.AddSubview(arrow);

            #region button_constraints

            buttonLogo.AutoPinEdgeToSuperviewEdge(ALEdge.Left);
            buttonLogo.AutoAlignAxisToSuperviewAxis(ALAxis.Horizontal);
            buttonLogo.AutoSetDimensionsToSize(new CoreGraphics.CGSize(80, 80));
            arrow.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 20);
            arrow.AutoAlignAxisToSuperviewAxis(ALAxis.Horizontal);
            arrow.AutoSetDimensionsToSize(new CoreGraphics.CGSize(5, 9));
            buttonTitle.AutoPinEdge(ALEdge.Left, ALEdge.Right, buttonLogo, 9);
            buttonTitle.AutoPinEdge(ALEdge.Right, ALEdge.Left, arrow);
            buttonTitle.AutoAlignAxisToSuperviewAxis(ALAxis.Horizontal);

            #endregion

            return button;
        }

        private void GoBack(object sender, EventArgs e)
        {
            NavigationController.PopViewController(true);
        }
    }
}
