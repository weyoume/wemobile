using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using PureLayout.Net;
using Steepshot.Core.Localization;
using Steepshot.Core.Utils;
using Steepshot.iOS.Cells;
using Steepshot.iOS.Helpers;
using Steepshot.iOS.ViewSources;
using UIKit;

namespace Steepshot.iOS.Views
{
    public partial class PlagiarismViewController : UIViewController
    {
        private List<Tuple<NSDictionary, UIImage>> ImageAssets;
        private UICollectionView photoCollection;
        private UIImageView photoView;
        private UIDeviceOrientation rotation;
        private UIButton noButton;
        private UIButton yesButton;

        private string ImageExtension;

        public bool _isFromCamera => ImageAssets.Count == 1 && ImageAssets[0].Item1 == null;

        public PlagiarismViewController(List<Tuple<NSDictionary, UIImage>> imageAssets, string extension, UIDeviceOrientation rotation = UIDeviceOrientation.Portrait) : base("PlagiarismViewController", null)
        {
            ImageAssets = imageAssets;
            ImageExtension = extension;
            this.rotation = rotation;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetBackButton();

            CreateView();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            Constants.CreateGradient(noButton, 25, GradientType.Orange);
            Constants.CreateGradient(yesButton, 25, GradientType.Blue);

            Constants.CreateShadow(noButton, Constants.R231G72B0, 0.5f, 25, 10, 12);
            Constants.CreateShadow(yesButton, Constants.R231G72B0, 0.5f, 25, 10, 12);
        }

        private void CreateView()
        {
            if (ImageAssets.Count == 1)
            {
                photoView = new UIImageView();
                photoView.ContentMode = UIViewContentMode.ScaleAspectFill;
                photoView.Layer.CornerRadius = 8;
                photoView.ClipsToBounds = true;
                photoView.Image = ImageAssets[0].Item2;
                mainScroll.AddSubview(photoView);

                photoView.AutoPinEdgeToSuperviewEdge(ALEdge.Top, 30f);
                photoView.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 30f);
                photoView.AutoMatchDimension(ALDimension.Height, ALDimension.Width, photoView);
                photoView.AutoSetDimension(ALDimension.Width, 150);

                photoView.Layer.BorderColor = Constants.R255G71B5.CGColor;
                photoView.Layer.BorderWidth = 4;
            }
            else
            {
                photoCollection = new UICollectionView(CGRect.Null, new UICollectionViewFlowLayout()
                {
                    ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                    ItemSize = new CGSize(150, 150),
                    SectionInset = new UIEdgeInsets(0, 15, 0, 0),
                    MinimumInteritemSpacing = 10,
                });

                mainScroll.AddSubview(photoCollection);

                photoCollection.AutoPinEdgeToSuperviewEdge(ALEdge.Left);
                photoCollection.AutoPinEdgeToSuperviewEdge(ALEdge.Top, 30f);
                photoCollection.AutoPinEdgeToSuperviewEdge(ALEdge.Right);
                photoCollection.AutoSetDimension(ALDimension.Height, 150f);
                photoCollection.AutoSetDimension(ALDimension.Width, UIScreen.MainScreen.Bounds.Width);

                photoCollection.Bounces = false;
                photoCollection.ShowsHorizontalScrollIndicator = false;
                photoCollection.RegisterClassForCell(typeof(PhotoGalleryCell), nameof(PhotoGalleryCell));
                var galleryCollectionViewSource = new PhotoGalleryViewSource(ImageAssets);
                photoCollection.Source = galleryCollectionViewSource;
                photoCollection.BackgroundColor = UIColor.White;
            }

            // Separator

            var separator = new UIView();
            separator.BackgroundColor = Constants.R245G245B245;
            mainScroll.AddSubview(separator);

            if (ImageAssets.Count == 1)
                separator.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, photoView, 30);
            else
                separator.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, photoCollection, 30);

            separator.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 30);
            separator.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 30);
            separator.AutoSetDimension(ALDimension.Height, 1f);

            // Title

            var titleTextView = new UITextView();
            titleTextView.Editable = false;
            titleTextView.ScrollEnabled = false;
            titleTextView.Font = Constants.Semibold20;
            titleTextView.TextAlignment = UITextAlignment.Left;
            titleTextView.Text = AppSettings.LocalizationManager.GetText(LocalizationKeys.PlagiarismAlertTitle);
            titleTextView.BackgroundColor = UIColor.Clear;
            titleTextView.TextContainerInset = UIEdgeInsets.Zero;

            mainScroll.AddSubview(titleTextView);

            titleTextView.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, separator, 24);
            titleTextView.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 30);
            titleTextView.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 30);

            // Message

            var messageTextView = new UITextView();
            messageTextView.Editable = false;
            messageTextView.ScrollEnabled = false;
            messageTextView.Font = Constants.Regular14;
            messageTextView.TextAlignment = UITextAlignment.Left;
            messageTextView.Text = AppSettings.LocalizationManager.GetText(LocalizationKeys.PlagiarismAlertMessage);
            messageTextView.BackgroundColor = UIColor.Clear;
            messageTextView.TextContainerInset = UIEdgeInsets.Zero;

            mainScroll.AddSubview(messageTextView);

            messageTextView.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, titleTextView, 19);
            messageTextView.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 30);
            messageTextView.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 30);

            // Separator

            separator = new UIView();
            separator.BackgroundColor = Constants.R245G245B245;
            mainScroll.AddSubview(separator);

            separator.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, messageTextView, 26);
            separator.AutoAlignAxisToSuperviewAxis(ALAxis.Vertical);
            separator.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 30);
            separator.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 30);
            separator.AutoSetDimension(ALDimension.Height, 1f);

            // Guideline text

            var guidelineText = new UITextView();
            guidelineText.Editable = false;
            guidelineText.ScrollEnabled = false;
            guidelineText.Font = Constants.Semibold14;
            guidelineText.TextAlignment = UITextAlignment.Left;
            guidelineText.Text = AppSettings.LocalizationManager.GetText(LocalizationKeys.PlagiarismGuidelineText);
            guidelineText.TextColor = Constants.R255G34B5;
            guidelineText.BackgroundColor = UIColor.Clear;
            guidelineText.TextContainerInset = UIEdgeInsets.Zero;
            guidelineText.UserInteractionEnabled = true;

            mainScroll.AddSubview(guidelineText);

            guidelineText.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, separator, 24);
            guidelineText.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 30);
            guidelineText.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 30);

            // Arrow

            var arrow = new UIImageView(UIImage.FromBundle("ic_forward"));
            arrow.ContentMode = UIViewContentMode.ScaleAspectFill;
            arrow.ClipsToBounds = true;
            arrow.UserInteractionEnabled = true;

            mainScroll.AddSubview(arrow);

            arrow.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 30);
            arrow.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, separator, 29);
            arrow.AutoSetDimensionsToSize(new CGSize(6, 10));

            // Separator

            separator = new UIView();
            separator.BackgroundColor = Constants.R245G245B245;
            mainScroll.AddSubview(separator);

            separator.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, guidelineText, 24);
            separator.AutoAlignAxisToSuperviewAxis(ALAxis.Vertical);
            separator.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 30);
            separator.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 30);
            separator.AutoSetDimension(ALDimension.Height, 1f);

            // No button

            noButton = CreateButton(AppSettings.LocalizationManager.GetText(LocalizationKeys.PlagiarismNOButtonText), Constants.R231G72B0, UIColor.White);

            mainScroll.AddSubview(noButton);

            noButton.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, separator, 40);
            noButton.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 30);
            noButton.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 30);
            noButton.AutoSetDimension(ALDimension.Height, 50);

            // yes button

            yesButton = CreateButton(AppSettings.LocalizationManager.GetText(LocalizationKeys.PlagiarismYESButtonText), UIColor.Blue, UIColor.White);

            mainScroll.AddSubview(yesButton);

            yesButton.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, noButton, 10);
            yesButton.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 30);
            yesButton.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 30);
            yesButton.AutoSetDimension(ALDimension.Height, 50);

            // claim your rights button

            var claimRightsButton = CreateButton(AppSettings.LocalizationManager.GetText(LocalizationKeys.PlagiarismClaimRights), UIColor.White, UIColor.Black);
            claimRightsButton.Layer.BorderWidth = 1f;
            claimRightsButton.Layer.BorderColor = Constants.R245G245B245.CGColor;

            mainScroll.AddSubview(claimRightsButton);

            claimRightsButton.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, yesButton, 20);
            claimRightsButton.AutoPinEdgeToSuperviewEdge(ALEdge.Bottom, 10);
            claimRightsButton.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 30);
            claimRightsButton.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 30);
            claimRightsButton.AutoSetDimension(ALDimension.Height, 50);
        }

        private UIButton CreateButton(string title, UIColor backgroundColor, UIColor titleColor)
        {
            var button = new UIButton();
            button.SetTitle(title, UIControlState.Normal);
            button.Font = Constants.Bold14;
            button.SetTitleColor(titleColor, UIControlState.Normal);
            button.BackgroundColor = backgroundColor;
            button.Layer.CornerRadius = 25;

            return button;
        }

        private void SetBackButton()
        {
            var leftBarButton = new UIBarButtonItem(UIImage.FromBundle("ic_back_arrow"), UIBarButtonItemStyle.Plain, GoBack);
            NavigationItem.LeftBarButtonItem = leftBarButton;
            NavigationItem.Title = AppSettings.LocalizationManager.GetText(LocalizationKeys.PlagiarismCheck);
            NavigationController.NavigationBar.TintColor = Constants.R15G24B30;
        }

        private void GoBack(object sender, EventArgs e)
        {
            NavigationController.PopViewController(true);
        }
    }
}

