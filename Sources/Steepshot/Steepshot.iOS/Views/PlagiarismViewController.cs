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
            SetNavigationBar();

            CreateView();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            Constants.CreateGradient(noButton, 25, GradientType.Orange);
            Constants.CreateGradient(yesButton, 25, GradientType.Blue);

            Constants.CreateShadow(noButton, Constants.R231G72B0, 0.5f, 25, 10, 12);
            Constants.CreateShadow(yesButton, Constants.R18G148B246, 0.5f, 25, 10, 12);
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

                photoView.AutoPinEdgeToSuperviewEdge(ALEdge.Top, 15f);
                photoView.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 15f);
                photoView.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 15f);
                photoView.AutoMatchDimension(ALDimension.Height, ALDimension.Width, photoView);
                var photoMargin = 15;
                var photoViewSide = UIScreen.MainScreen.Bounds.Width - photoMargin * 2;
                photoView.AutoSetDimension(ALDimension.Width, photoViewSide);
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

            var titleTextView = new UILabel();
            titleTextView.Font = Constants.Semibold20;
            titleTextView.TextAlignment = UITextAlignment.Left;
            titleTextView.LineBreakMode = UILineBreakMode.WordWrap;
            titleTextView.Lines = 0;
            titleTextView.Text = AppSettings.LocalizationManager.GetText(LocalizationKeys.PlagiarismAlertGalleryTitle);
            titleTextView.BackgroundColor = UIColor.Clear;

            mainScroll.AddSubview(titleTextView);

            titleTextView.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, separator, 24);
            titleTextView.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 30);
            titleTextView.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 30);

            // Message

            var messageTextView = new UILabel();
            messageTextView.Font = Constants.Regular14;
            messageTextView.TextAlignment = UITextAlignment.Left;
            messageTextView.LineBreakMode = UILineBreakMode.WordWrap;
            messageTextView.Lines = 0;
            messageTextView.Text = AppSettings.LocalizationManager.GetText(LocalizationKeys.PlagiarismAlertMessage);
            messageTextView.BackgroundColor = UIColor.Clear;

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

            // IPFSLink text

            var ipfsTextView = new UILabel();
            ipfsTextView.Text = AppSettings.LocalizationManager.GetText(LocalizationKeys.IPFSLink);
            ipfsTextView.Font = Constants.Semibold14;
            ipfsTextView.TextColor = Constants.R255G34B5;
            ipfsTextView.TextAlignment = UITextAlignment.Left;
            ipfsTextView.UserInteractionEnabled = true;

            ipfsTextView.BackgroundColor = UIColor.Clear;

            UITapGestureRecognizer ipfsTap = new UITapGestureRecognizer(IPFSTap);
            ipfsTap.NumberOfTapsRequired = 1;
            ipfsTextView.AddGestureRecognizer(ipfsTap);

            mainScroll.AddSubview(ipfsTextView);

            ipfsTextView.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, separator, 0);
            ipfsTextView.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 30);
            ipfsTextView.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 30);
            ipfsTextView.AutoSetDimension(ALDimension.Height, 70f);

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

            separator.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, ipfsTextView, 0);
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
            noButton.TouchDown += (object sender, EventArgs e) => 
            {
                
            };

            // yes button

            yesButton = CreateButton(AppSettings.LocalizationManager.GetText(LocalizationKeys.PlagiarismYESButtonText), UIColor.Blue, UIColor.White);

            mainScroll.AddSubview(yesButton);

            yesButton.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, noButton, 10);
            yesButton.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 30);
            yesButton.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 30);
            yesButton.AutoPinEdgeToSuperviewEdge(ALEdge.Bottom, 10);
            yesButton.AutoSetDimension(ALDimension.Height, 50);

            // claim your rights button TODO: Add in future versions

            //var claimRightsButton = CreateButton(AppSettings.LocalizationManager.GetText(LocalizationKeys.PlagiarismClaimRights), UIColor.White, UIColor.Black);
            //claimRightsButton.Layer.BorderWidth = 1f;
            //claimRightsButton.Layer.BorderColor = Constants.R245G245B245.CGColor;
            //mainScroll.AddSubview(claimRightsButton);
            //claimRightsButton.AutoPinEdge(ALEdge.Top, ALEdge.Bottom, yesButton, 20);
            //claimRightsButton.AutoPinEdgeToSuperviewEdge(ALEdge.Bottom, 10);
            //claimRightsButton.AutoPinEdgeToSuperviewEdge(ALEdge.Left, 30);
            //claimRightsButton.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 30);
            //claimRightsButton.AutoSetDimension(ALDimension.Height, 50);
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

        private void SetNavigationBar()
        {
            var leftBarButton = new UIBarButtonItem(UIImage.FromBundle("ic_back_arrow"), UIBarButtonItemStyle.Plain, GoBack);
            NavigationItem.LeftBarButtonItem = leftBarButton;

            NavigationItem.Title = AppSettings.LocalizationManager.GetText(LocalizationKeys.PlagiarismCheck);
            NavigationController.NavigationBar.TintColor = Constants.R15G24B30;

            UITapGestureRecognizer guidelineTap = new UITapGestureRecognizer(() => 
            {
                UIApplication.SharedApplication.OpenUrl(new NSUrl(Core.Constants.Guide));
            });
            guidelineTap.NumberOfTapsRequired = 1;
            var guidelines = new UILabel()
            {
                Text = AppSettings.LocalizationManager.GetText(LocalizationKeys.PlagiarismGuidelineText),
                Font = Constants.Semibold14,
                TextColor = Constants.R255G34B5,
                UserInteractionEnabled = true,
            };
            guidelines.AddGestureRecognizer(guidelineTap);
            guidelines.SizeToFit();
            var rightBarButton = new UIBarButtonItem(guidelines);
            NavigationItem.RightBarButtonItem = rightBarButton;
        }

        void IPFSTap()
        {
            UIAlertController actionSheetAlert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);
            actionSheetAlert.AddAction(UIAlertAction.Create(AppSettings.LocalizationManager.GetText(LocalizationKeys.Cancel), UIAlertActionStyle.Cancel, null));
            PresentViewController(actionSheetAlert, true, null);
        }

        private void ContinuePublishing()
        { 
        
        }

        private void GoBack(object sender, EventArgs e)
        {
            NavigationController.PopViewController(true);
        }
    }
}

