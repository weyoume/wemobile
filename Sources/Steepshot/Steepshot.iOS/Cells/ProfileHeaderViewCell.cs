﻿using System;
using CoreGraphics;
using Foundation;
using PureLayout.Net;
using Steepshot.Core.Localization;
using Steepshot.Core.Models.Enums;
using Steepshot.Core.Models.Responses;
using Steepshot.Core.Extensions;
using Steepshot.Core.Utils;
using Steepshot.iOS.Helpers;
using Steepshot.iOS.Views;
using UIKit;
using Xamarin.TTTAttributedLabel;

namespace Steepshot.iOS.Cells
{
    public class ProfileHeaderViewCell : UICollectionViewCell
    {
        protected ProfileHeaderViewCell(IntPtr handle) : base(handle)
        {
        }
    }

    public class ProfileHeaderCellBuilder : UIView
    {
        private const int mainMargin = 30;
        private const int extraMargin = 20;
        private const int verticalSpacing = 26;
        private const int horizontalSpacing = 48;
        private const int topViewSpacing = 25;

        private const int topViewHeight = 90;
        private const int powerFrameSide = 90;
        private const int photoSide = 80;

        private int descriptionY;

        private CircleFrame powerFrame;
        private UIStackView infoView;
        private UIStackView statsView;
        private UIImageView avatar;
        private UIView topView;
        private UIView originalityContainer;
        private UIView statsContainer;
        private UIView balanceContainer;
        private UILabel originalityLabel;
        private UILabel userName;
        private UILabel userLocation;
        private UILabel originality;
        private UILabel balance;
        private UIButton followButton;
        private UIButton photos;
        private UIButton following;
        private UIButton followers;
        private TTTAttributedLabel attributedLabel;
        private NSMutableAttributedString at;
        private UIActivityIndicatorView followProgress;

        private UserProfileResponse _userData;

        public bool IsProfileActionSet => ProfileAction != null;
        public event Action<ActionType> ProfileAction;

        public ProfileHeaderCellBuilder()
        {
            CreateView();
        }

        public UIView CreateView()
        {
            var screenWidth = UIScreen.MainScreen.Bounds.Width;
            UserInteractionEnabled = true;

            #region topPanel

            topView = new UIView();
            topView.BackgroundColor = UIColor.Clear;
            topView.Frame = new CGRect(mainMargin - 5, mainMargin, screenWidth - mainMargin * 2, topViewHeight);

            avatar = new UIImageView(new CGRect(5, 5, photoSide, photoSide));
            avatar.Layer.CornerRadius = photoSide / 2;
            avatar.ClipsToBounds = true;
            avatar.UserInteractionEnabled = true;
            avatar.ContentMode = UIViewContentMode.ScaleAspectFill;
            avatar.BackgroundColor = UIColor.Clear;
            powerFrame = new CircleFrame(avatar, new CGRect(0, 0, powerFrameSide, powerFrameSide));

            infoView = new UIStackView();
            infoView.Axis = UILayoutConstraintAxis.Vertical;
            infoView.Alignment = UIStackViewAlignment.Fill;
            infoView.Distribution = UIStackViewDistribution.FillEqually;
            infoView.Frame = new CGRect(powerFrameSide + topViewSpacing, 0, screenWidth - mainMargin * 2 - (powerFrameSide + topViewSpacing), topViewHeight + 8);

            userName = new UILabel();
            userName.UserInteractionEnabled = false;
            userName.TextColor = Helpers.Constants.R15G24B30;
            userName.Font = Helpers.Constants.Semibold20;
            userName.Lines = 1;
            userName.LineBreakMode = UILineBreakMode.TailTruncation;
            userName.BackgroundColor = UIColor.Clear;

            userLocation = new UILabel();
            userLocation.UserInteractionEnabled = false;
            userLocation.TextColor = Helpers.Constants.R151G155B158;
            userLocation.Font = Helpers.Constants.Regular14;
            userLocation.Lines = 1;
            userLocation.LineBreakMode = UILineBreakMode.TailTruncation;
            userLocation.BackgroundColor = UIColor.Clear;

            var topEmptyView = new UIView();
            var bottomEmptyView = new UIView();

            infoView.AddArrangedSubview(topEmptyView);
            infoView.AddArrangedSubview(userName);
            infoView.AddArrangedSubview(userLocation);
            infoView.AddArrangedSubview(bottomEmptyView);

            topView.AddSubview(powerFrame);
            topView.AddSubview(infoView);
            AddSubview(topView);

            #endregion

            attributedLabel = new TTTAttributedLabel();
            attributedLabel.Lines = 0;
            attributedLabel.EnabledTextCheckingTypes = NSTextCheckingType.Link;

            var prop = new NSDictionary();
            attributedLabel.LinkAttributes = prop;
            attributedLabel.ActiveLinkAttributes = prop;

            attributedLabel.Delegate = new TTTAttributedLabelCustomDelegate();

            at = new NSMutableAttributedString();

            followButton = new UIButton();
            followButton.BackgroundColor = UIColor.Clear;
            AddSubview(followButton);

            followProgress = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
            followProgress.HidesWhenStopped = false;
            followProgress.StartAnimating();
            AddSubview(followProgress);

            #region originality

            originalityContainer = new UIView();
            originalityContainer.BackgroundColor = Helpers.Constants.R250G250B250;
            originalityContainer.Layer.CornerRadius = 10;

            originalityLabel = new UILabel();
            originalityLabel.UserInteractionEnabled = false;
            //originalityLabel.Text = AppSettings.LocalizationManager.GetText(LocalizationKeys.Originality);
            originalityLabel.TextColor = UIColor.Black;
            originalityLabel.Font = Helpers.Constants.Regular14;
            originalityLabel.Lines = 1;
            originalityLabel.LineBreakMode = UILineBreakMode.TailTruncation;

            originality = new UILabel();
            originality.UserInteractionEnabled = false;
            originality.Text = "100%";
            originality.TextColor = Helpers.Constants.R255G34B5;
            originality.Font = Helpers.Constants.Semibold14;
            originality.Lines = 1;

            originalityContainer.AddSubview(originalityLabel);
            originalityContainer.AddSubview(originality);

            //contentView.AddSubview(originalityContainer);

            #endregion

            #region stats

            statsView = new UIStackView();
            statsView.Axis = UILayoutConstraintAxis.Horizontal;
            statsView.Alignment = UIStackViewAlignment.Fill;
            statsView.Distribution = UIStackViewDistribution.Fill;
            statsView.TranslatesAutoresizingMaskIntoConstraints = false;

            photos = new UIButton();
            following = new UIButton();
            followers = new UIButton();

            var emptySpace = new UIView();

            var firstSpacing = new UIView();
            var secondSpacing = new UIView();

            statsView.AddArrangedSubview(photos);
            statsView.AddArrangedSubview(firstSpacing);
            statsView.AddArrangedSubview(following);
            statsView.AddArrangedSubview(secondSpacing);
            statsView.AddArrangedSubview(followers);
            statsView.AddArrangedSubview(emptySpace);

            statsContainer = new UIView();
            statsContainer.AddSubview(statsView);

            AddSubview(statsContainer);

            #endregion

            #region balance

            balanceContainer = new UIView();
            balanceContainer.UserInteractionEnabled = true;

            var topSeparator = new UIView();
            var bottomSeparator = new UIView();
            topSeparator.BackgroundColor = bottomSeparator.BackgroundColor = Helpers.Constants.R245G245B245;

            var balanceImage = new UIImageView();
            balanceImage.Image = UIImage.FromBundle("ic_balance");

            var balanceLabel = new UILabel();
            balanceLabel.UserInteractionEnabled = false;
            balanceLabel.Text = AppSettings.LocalizationManager.GetText(LocalizationKeys.AccountBalance);
            balanceLabel.TextColor = UIColor.Black;
            balanceLabel.Font = Helpers.Constants.Regular14;
            balanceLabel.Lines = 1;

            balance = new UILabel();
            balance.UserInteractionEnabled = false;
            balance.TextColor = Helpers.Constants.R255G34B5;
            balance.TextAlignment = UITextAlignment.Right;
            balance.Font = Helpers.Constants.Semibold14;
            balance.Lines = 1;

            var balanceArrow = new UIImageView();
            balanceArrow.Image = UIImage.FromBundle("ic_forward");

            balanceContainer.AddSubview(topSeparator);
            balanceContainer.AddSubview(bottomSeparator);
            balanceContainer.AddSubview(balanceImage);
            balanceContainer.AddSubview(balanceLabel);
            balanceContainer.AddSubview(balanceArrow);
            balanceContainer.AddSubview(balance);

            UITapGestureRecognizer balanceTap = new UITapGestureRecognizer(() =>
            {
                ProfileAction?.Invoke(ActionType.Balance);
            });
            balanceContainer.AddGestureRecognizer(balanceTap);
#if DEBUG
            AddSubview(balanceContainer);
#endif
            #endregion

            AddSubview(bottomSeparator);

            #region constraints

            /*
            originalityLabel.AutoPinEdgeToSuperviewEdge(ALEdge.Left, extraMargin);
            originalityLabel.AutoAlignAxis(ALAxis.Horizontal, originalityContainer);
            originality.AutoPinEdgeToSuperviewEdge(ALEdge.Right, extraMargin);
            originality.AutoAlignAxis(ALAxis.Horizontal, originalityContainer);
            */

            statsView.AutoPinEdgesToSuperviewEdges();

            topSeparator.AutoSetDimension(ALDimension.Height, 1);
            topSeparator.AutoPinEdgeToSuperviewEdge(ALEdge.Top);
            topSeparator.AutoPinEdgeToSuperviewEdge(ALEdge.Left);
            topSeparator.AutoPinEdgeToSuperviewEdge(ALEdge.Right);

            bottomSeparator.AutoSetDimension(ALDimension.Height, 1);
            bottomSeparator.AutoPinEdgeToSuperviewEdge(ALEdge.Left);
            bottomSeparator.AutoPinEdgeToSuperviewEdge(ALEdge.Right);
            bottomSeparator.AutoPinEdgeToSuperviewEdge(ALEdge.Bottom);

            followProgress.AutoAlignAxis(ALAxis.Horizontal, followButton);
            followProgress.AutoAlignAxis(ALAxis.Vertical, followButton);
#if DEBUG
            balanceImage.AutoSetDimensionsToSize(new CGSize(10, 10));
            balanceImage.AutoPinEdgeToSuperviewEdge(ALEdge.Left, mainMargin);
            balanceImage.AutoAlignAxisToSuperviewAxis(ALAxis.Horizontal);
            balanceLabel.AutoPinEdge(ALEdge.Left, ALEdge.Right, balanceImage, 20);
            balanceLabel.AutoAlignAxisToSuperviewAxis(ALAxis.Horizontal);
            balanceArrow.AutoSetDimensionsToSize(new CGSize(6, 10));
            balanceArrow.AutoPinEdgeToSuperviewEdge(ALEdge.Right, mainMargin);
            balanceArrow.AutoAlignAxisToSuperviewAxis(ALAxis.Horizontal);
            balance.AutoPinEdgeToSuperviewEdge(ALEdge.Right, 55);
            balance.AutoAlignAxisToSuperviewAxis(ALAxis.Horizontal);
#endif
            firstSpacing.AutoSetDimension(ALDimension.Width, 48);
            secondSpacing.AutoSetDimension(ALDimension.Width, 48);

            #endregion

            return this;
        }

        public nfloat UpdateProfile(UserProfileResponse userData)
        {
            if (userData == null && _userData == null)
                return 0;

            descriptionY = topViewHeight + mainMargin + verticalSpacing;

            if(userData != null)
                _userData = userData;

            if (!string.IsNullOrEmpty(_userData.ProfileImage))
                ImageLoader.Load(_userData.ProfileImage, avatar, size: new CGSize(300, 300));
            else
                avatar.Image = UIImage.FromBundle("ic_noavatar");

            if (_userData.Username == AppSettings.User.Login)
                powerFrame.ChangePercents((int)_userData.VotingPower);
            else
                powerFrame.ChangePercents(0);

            if (string.IsNullOrEmpty(_userData.Name))
                userName.Hidden = true;
            else
            {
                userName.Hidden = false;
                userName.Text = _userData.Name;
            }

            if (string.IsNullOrEmpty(_userData.Location))
                userLocation.Hidden = true;
            else
            {
                userLocation.Hidden = false;
                userLocation.Text = _userData.Location;
            }

            if (AppSettings.User.HasPostingPermission && _userData.Username != AppSettings.User.Login)
            {
                followButton.Frame = new CGRect(new CGPoint(mainMargin, descriptionY),
                                                new CGSize(UIScreen.MainScreen.Bounds.Width - mainMargin * 2, 40));
                descriptionY += verticalSpacing + 40;

                DecorateFollowButton();
            }
            else
            {
                followButton.Hidden = true;
                followProgress.Hidden = true;
            }

            at.SetString(new NSAttributedString(string.Empty));

            var noLinkAttribute = new UIStringAttributes
            {
                Font = Helpers.Constants.Regular14,
                ForegroundColor = Helpers.Constants.R15G24B30
            };

            if (!string.IsNullOrEmpty(_userData.About))
            {
                at.Append(new NSAttributedString(_userData.About, noLinkAttribute));
            }

            if (!string.IsNullOrEmpty(_userData.Website))
            {
                var linkAttribute = new UIStringAttributes
                {
                    Link = new NSUrl(_userData.Website),
                    Font = Helpers.Constants.Semibold14,
                    ForegroundColor = Helpers.Constants.R255G34B5
                };

                at.Append(new NSAttributedString(Environment.NewLine));
                at.Append(new NSAttributedString(Environment.NewLine));
                at.Append(new NSAttributedString(_userData.Website, linkAttribute));
            }

            attributedLabel.SetText(at);

            var textHeight = attributedLabel.SizeThatFits(new CGSize(UIScreen.MainScreen.Bounds.Width - mainMargin * 2, 0)).Height;
            attributedLabel.Frame = new CGRect(new CGPoint(mainMargin, descriptionY),
                                               new CGSize(UIScreen.MainScreen.Bounds.Width - mainMargin * 2, textHeight));

            AddSubview(attributedLabel);

            //originalityContainer.Frame = new CGRect(new CGPoint(extraMargin, attributedLabel.Frame.Bottom + verticalSpacing),
            //new CGSize(UIScreen.MainScreen.Bounds.Width - extraMargin * 2, 50));

            statsContainer.Frame = new CGRect(new CGPoint(mainMargin, attributedLabel.Frame.Bottom + verticalSpacing),//originalityContainer.Frame.Bottom + verticalSpacing),
                                         new CGSize(UIScreen.MainScreen.Bounds.Width - mainMargin * 2, 45));

            SetupStats();
#if DEBUG
            balanceContainer.Frame = new CGRect(new CGPoint(0, statsContainer.Frame.Bottom + verticalSpacing),
                                                new CGSize(UIScreen.MainScreen.Bounds.Width, 70));
            balance.Text = $"$ {_userData.EstimatedBalance}";

            Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, balanceContainer.Frame.Bottom);
            return balanceContainer.Frame.Bottom;
#else
            Frame = new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, statsContainer.Frame.Bottom + verticalSpacing);
            return statsContainer.Frame.Bottom + verticalSpacing;
#endif
        }

        private void SetupStats()
        {
            var buttonsAttributes = new UIStringAttributes
            {
                Font = Helpers.Constants.Semibold20,
                ForegroundColor = Helpers.Constants.R15G24B30,
            };

            var textAttributes = new UIStringAttributes
            {
                Font = Helpers.Constants.Regular12,
                ForegroundColor = Helpers.Constants.R151G155B158,
            };

            NSMutableAttributedString photosString = new NSMutableAttributedString();
            photosString.Append(new NSAttributedString(_userData.PostCount.CounterFormat(), buttonsAttributes));
            photosString.Append(new NSAttributedString(Environment.NewLine));
            photosString.Append(new NSAttributedString("Photos", textAttributes));

            NSMutableAttributedString followingString = new NSMutableAttributedString();
            followingString.Append(new NSAttributedString(_userData.FollowingCount.CounterFormat(), buttonsAttributes));
            followingString.Append(new NSAttributedString(Environment.NewLine));
            followingString.Append(new NSAttributedString("Following", textAttributes));

            NSMutableAttributedString followersString = new NSMutableAttributedString();
            followersString.Append(new NSAttributedString(_userData.FollowersCount.CounterFormat(), buttonsAttributes)); 
            followersString.Append(new NSAttributedString(Environment.NewLine));
            followersString.Append(new NSAttributedString("Followers", textAttributes));

            photos.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
            photos.TitleLabel.TextAlignment = UITextAlignment.Left;
            photos.SetAttributedTitle(photosString, UIControlState.Normal);

            following.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
            following.TitleLabel.TextAlignment = UITextAlignment.Left;
            following.SetAttributedTitle(followingString, UIControlState.Normal);

            followers.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
            followers.TitleLabel.TextAlignment = UITextAlignment.Left;
            followers.SetAttributedTitle(followersString, UIControlState.Normal);

            UITapGestureRecognizer followTap = new UITapGestureRecognizer(() =>
            {
                ProfileAction?.Invoke(ActionType.Follow);
            });
            followButton.AddGestureRecognizer(followTap);

            UITapGestureRecognizer followersTap = new UITapGestureRecognizer(() =>
            {
                ProfileAction?.Invoke(ActionType.Followers);
            });
            followers.AddGestureRecognizer(followersTap);

            UITapGestureRecognizer followingTap = new UITapGestureRecognizer(() =>
            {
                ProfileAction?.Invoke(ActionType.Following);
            });
            following.AddGestureRecognizer(followingTap);

            UITapGestureRecognizer avatarTap = new UITapGestureRecognizer(() =>
            {
                ProfileAction?.Invoke(ActionType.ProfilePower);
            });
            avatar.AddGestureRecognizer(avatarTap);
        }

        public void DecorateFollowButton()
        {
            followButton.Hidden = false;
            followButton.Layer.CornerRadius = 20;
            followButton.TitleLabel.Font = Helpers.Constants.Semibold14;

            BringSubviewToFront(followProgress);

            if (_userData.FollowedChanging)
            {
                followButton.Selected = false;
                followButton.Enabled = false;
                followButton.SetTitle("", UIControlState.Normal);
                followProgress.Hidden = false;
                followProgress.StartAnimating();
            }
            else
            {
                followButton.Enabled = true;
                followButton.Selected = _userData.HasFollowed;
                followProgress.StopAnimating();
                followProgress.Hidden = true;

                if (_userData.HasFollowed)
                {
                    Helpers.Constants.RemoveGradient(followButton);
                    followButton.Layer.BorderWidth = 1;
                    followButton.SetTitle(AppSettings.LocalizationManager.GetText(LocalizationKeys.Unfollow), UIControlState.Normal);
                    followButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
                    followButton.Layer.BorderColor = Helpers.Constants.R244G244B246.CGColor;
                    followButton.TitleLabel.Font = Helpers.Constants.Semibold14;
                }
                else
                {
                    followButton.Layer.BorderWidth = 0;
                    followButton.LayoutIfNeeded();
                    followButton.SetTitle(AppSettings.LocalizationManager.GetText(LocalizationKeys.Follow), UIControlState.Normal);
                    followButton.SetTitleColor(UIColor.White, UIControlState.Normal);
                    followButton.TitleLabel.Font = Helpers.Constants.Bold14;

                    Helpers.Constants.CreateShadow(followButton, Helpers.Constants.R231G72B0, 0.3f, 20, 10, 10);
                    Helpers.Constants.CreateGradient(followButton, 20);
                }
            }
        }
    }
}
