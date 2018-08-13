﻿using System;
using CoreGraphics;
using Foundation;
using PureLayout.Net;
using Steepshot.iOS.Helpers;
using UIKit;

namespace Steepshot.iOS.CustomViews
{
    public class BaseTextField : UITextField
    {
        private UIEdgeInsets _insets = new UIEdgeInsets();

        public BaseTextField(UIEdgeInsets insets)
        {
            _insets = insets;
        }

        public override CGRect TextRect(CGRect forBounds)
        {
            return base.TextRect(CalculateRect(forBounds));
        }

        public override CGRect EditingRect(CGRect forBounds)
        {
            return base.EditingRect(CalculateRect(forBounds));
        }

        public override CGRect RightViewRect(CGRect forBounds)
        {
            return base.RightViewRect(CalculateRect(forBounds));;
        }

        private CGRect CalculateRect(CGRect forBounds)
        {
            return new CGRect(_insets.Left,
                              _insets.Top,
                              forBounds.Width - _insets.Left - _insets.Right,
                              forBounds.Height - _insets.Top - _insets.Bottom);
        }
    }

    public class SearchTextField : BaseTextField
    {
        public Action ClearButtonTapped;

        public UIButton ClearButton
        {
            get;
            private set;
        }

        public UIActivityIndicatorView Loader
        {
            get;
            private set;
        }

        public SearchTextField(Action returnButtonTapped, string placeholder, BaseTextFieldDelegate deleg = null) : base(new UIEdgeInsets(0, 20, 0, 20))
        {
            var rightView = new UIView();

            Loader = new UIActivityIndicatorView();
            Loader.Color = Constants.R231G72B0;
            Loader.HidesWhenStopped = true;

            ClearButton = new UIButton();
            ClearButton.Hidden = true;
            ClearButton.SetImage(UIImage.FromBundle("ic_delete_tag"), UIControlState.Normal);
            ClearButton.TouchDown += (sender, e) =>
            {
                Clear();
                ClearButtonTapped?.Invoke();
            };

            rightView.AddSubview(Loader);
            rightView.AddSubview(ClearButton);

            ClearButton.AutoCenterInSuperview();
            ClearButton.AutoSetDimensionsToSize(new CGSize(37,37));
            ClearButton.AutoAlignAxis(ALAxis.Horizontal, Loader);
            Loader.AutoSetDimensionsToSize(new CGSize(16, 16));
            Loader.AutoPinEdgeToSuperviewEdge(ALEdge.Top);
            Loader.AutoPinEdgeToSuperviewEdge(ALEdge.Right, -10);

            RightView = rightView;
            rightView.AutoSetDimensionsToSize(new CGSize(37, 37));
            RightViewMode = UITextFieldViewMode.Always;

            var _searchPlaceholderAttributes = new UIStringAttributes
            {
                Font = Constants.Regular14,
                ForegroundColor = Constants.R151G155B158,
            };

            var at = new NSMutableAttributedString();
            at.Append(new NSAttributedString(placeholder, _searchPlaceholderAttributes));
            AttributedPlaceholder = at;
            AutocorrectionType = UITextAutocorrectionType.No;
            AutocapitalizationType = UITextAutocapitalizationType.None;
            BackgroundColor = Constants.R245G245B245;
            Font = Constants.Regular14;
            Layer.CornerRadius = 20;
            TintColor = Constants.R255G71B5;

            Delegate = deleg ?? new TagFieldDelegate() { DoneTapped = returnButtonTapped };
            EditingChanged += DoEditingChanged;
        }

        private void DoEditingChanged(object sender, EventArgs e)
        {
            ClearButton.Hidden = Text.Length == 0;
        }

        public void Clear()
        {
            Text = string.Empty;
            ClearButton.Hidden = true;
            ((BaseTextFieldDelegate)Delegate).ChangeBackground(this);
        }
    }
}
