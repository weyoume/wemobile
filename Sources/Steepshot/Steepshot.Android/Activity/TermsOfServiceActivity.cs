using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Com.Lilarcor.Cheeseknife;
using Steepshot.Base;
using Steepshot.Core.Presenters;

namespace Steepshot.Activity
{
    [Activity(NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public sealed class TermsOfServiceActivity : BaseActivityWithPresenter<TermOfServicePresenter>
    {
        private TextView _termsOfService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.lyt_terms_of_service);
            Cheeseknife.Inject(this);

            _termsOfService = FindViewById<TextView>(Resource.Id.tv_terms_of_service);
            LoadText();
        }

        protected override void CreatePresenter()
        {
            _presenter = new TermOfServicePresenter();
        }

        [InjectOnClick(Resource.Id.go_back)]
        public void GoBackClick(object sender, EventArgs e)
        {
            Finish();
        }

        private async void LoadText()
        {
            var response = await _presenter.TryGetTermsOfService();
            if (response != null && response.Success)
                _termsOfService.Text = response.Result.Text;
            else
                ShowAlert(response);
        }
    }
}
