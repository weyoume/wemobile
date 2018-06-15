using System;
using CoreGraphics;
using PureLayout.Net;
using SafariServices;
using Steepshot.Core.Localization;
using Steepshot.Core.Utils;
using Steepshot.iOS.Helpers;
using Steepshot.iOS.ViewControllers;
using UIKit;

namespace Steepshot.iOS.Views
{
    public class RegistrationViewController : BaseViewController, ISFSafariViewControllerDelegate
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
        }

        private void CreateView()
        {
            var background = new UIView();
            background.BackgroundColor = Constants.R250G250B250;
            View.AddSubview(background);

            var emailReg = CreateButton(UIImage.FromBundle("ic_email"), AppSettings.LocalizationManager.GetText(LocalizationKeys.RegisterWithEmail));
            background.AddSubview(emailReg);

            #region separator

            var separatorsContainer = new UIStackView();
            separatorsContainer.Axis = UILayoutConstraintAxis.Horizontal;
            separatorsContainer.Alignment = UIStackViewAlignment.Center;
            separatorsContainer.Distribution = UIStackViewDistribution.FillEqually;

            var separatorL = new UIView();
            separatorL.BackgroundColor = Constants.R240G240B240;
            var separatorR = new UIView();
            separatorR.BackgroundColor = Constants.R240G240B240;

            var orLabel = new UILabel();
            orLabel.Lines = 1;
            orLabel.UserInteractionEnabled = false;
            orLabel.Font = Constants.Semibold14;
            orLabel.TextColor = UIColor.Black;
            orLabel.Text = AppSettings.LocalizationManager.GetText(LocalizationKeys.Or);
            orLabel.TextAlignment = UITextAlignment.Center;

            separatorsContainer.AddArrangedSubview(separatorL);
            separatorsContainer.AddArrangedSubview(orLabel);
            separatorsContainer.AddArrangedSubview(separatorR);

            background.AddSubview(separatorsContainer);

            #endregion

            var stackView = new UIStackView();
            stackView.Axis = UILayoutConstraintAxis.Vertical;
            stackView.Spacing = 10;
            stackView.Alignment = UIStackViewAlignment.Fill;
            stackView.Distribution = UIStackViewDistribution.Fill;

            var steemitReg = CreateButton(UIImage.FromBundle("ic_steemit"), AppSettings.LocalizationManager.GetText(LocalizationKeys.RegisterThroughSteemit));
            var blocktradesReg = CreateButton(UIImage.FromBundle("ic_blocktrades"), AppSettings.LocalizationManager.GetText(LocalizationKeys.RegisterThroughBlocktrades));
            var steemcreateReg = CreateButton(UIImage.FromBundle("ic_steemcreate"), AppSettings.LocalizationManager.GetText(LocalizationKeys.RegisterThroughSteemCreate));
            var emptySpace = new UIView();

            steemitReg.TouchDown += (sender, e) =>
            {
                OpenBrowser(new Uri(Core.Constants.SteemitRegUrl));
            };

            blocktradesReg.TouchDown += (sender, e) =>
            {
                OpenBrowser(new Uri(Core.Constants.BlocktradesRegUrl));
            };

            steemcreateReg.TouchDown += (sender, e) =>
            {
                OpenBrowser(new Uri(Core.Constants.SteemCreateRegUrl));
            };

            stackView.AddArrangedSubview(steemitReg);
            stackView.AddArrangedSubview(blocktradesReg);
            stackView.AddArrangedSubview(steemcreateReg);
            stackView.AddArrangedSubview(emptySpace);

            background.AddSubview(stackView);

            #region constraints

            background.AutoPinEdgesToSuperviewEdges();

            emailReg.AutoPinEdgeToSuperviewEdge(ALEdge.Top, 20);
            emailReg.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 20);
            emailReg.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 20);
            emailReg.AutoSetDimension(ALDimension.Height, 80);
            steemitReg.AutoSetDimension(ALDimension.Height, 80);
            blocktradesReg.AutoSetDimension(ALDimension.Height, 80);
            steemcreateReg.AutoSetDimension(ALDimension.Height, 80);

            separatorsContainer.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 20);
            separatorsContainer.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 20);
            separatorsContainer.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, emailReg);
            separatorsContainer.AutoSetDimension(ALDimension.Height, 68);

            separatorL.AutoSetDimension(ALDimension.Height, 1);
            separatorR.AutoSetDimension(ALDimension.Height, 1);

            stackView.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 20);
            stackView.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 20);
            stackView.AutoPinEdgeToSuperviewEdge(ALEdge.Bottom);
            stackView.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, separatorsContainer);

            #endregion
        }

        private void OpenBrowser(Uri uri)
        { 
            var sv = new SFSafariViewController(uri);
            sv.Delegate = this;

            NavigationController.SetNavigationBarHidden(true, false);
            NavigationController.PushViewController(sv, false);
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
