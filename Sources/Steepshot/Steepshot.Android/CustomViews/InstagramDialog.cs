using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Steepshot.Core.Localization;
using Steepshot.Core.Utils;
using Steepshot.Utils;

namespace Steepshot.CustomViews
{
    public class InstagramDialog : BottomSheetDialog
    {
        public Action InstagramConnect;

        public InstagramDialog(Context context) : base(context) { }

        // TODO: Feature popup 
        public override void Show()
        {
            using (var dialogView = LayoutInflater.From(Context).Inflate(Resource.Layout.lyt_instagram_popup, null))
            {
                dialogView.SetMinimumWidth((int)(Context.Resources.DisplayMetrics.WidthPixels * 0.8));

                var description = dialogView.FindViewById<TextView>(Resource.Id.instagram_description);
                description.Typeface = Style.Light;
                description.Text = AppSettings.LocalizationManager.GetText(LocalizationKeys.InstagramConnectionDescription);

                var connect = dialogView.FindViewById<Button>(Resource.Id.connect);
                connect.Typeface = Style.Semibold;
                connect.Text = AppSettings.LocalizationManager.GetText(LocalizationKeys.Connect);

                var cancel = dialogView.FindViewById<Button>(Resource.Id.cancel_button);
                cancel.Typeface = Style.Semibold;
                cancel.Text = AppSettings.LocalizationManager.GetText(LocalizationKeys.Cancel);

                connect.Click += (sender, e) =>
                {
                    InstagramConnect?.Invoke();
                };

                cancel.Click += (sender, e) => { Cancel(); };

                SetContentView(dialogView);
                Window.FindViewById(Resource.Id.design_bottom_sheet).SetBackgroundColor(Color.Transparent);
                var dialogPadding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 10, Context.Resources.DisplayMetrics);
                Window.DecorView.SetPadding(dialogPadding, dialogPadding, dialogPadding, dialogPadding);
                base.Show();

                var bottomSheet = FindViewById<FrameLayout>(Resource.Id.design_bottom_sheet);
                BottomSheetBehavior.From(bottomSheet).State = BottomSheetBehavior.StateExpanded;
            }
        }
    }
}
