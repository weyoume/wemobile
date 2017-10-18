﻿using System;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Lilarcor.Cheeseknife;
using Steepshot.Adapter;
using Steepshot.Base;
using Steepshot.Core;
using Steepshot.Core.Models.Common;
using Steepshot.Core.Presenters;
using Steepshot.Utils;

namespace Steepshot.Fragment
{
    public class VotersFragment : BaseFragmentWithPresenter<VotersPresenter>
    {
        private FollowersAdapter<VotersResult> _votersAdapter;
        private string _url;

#pragma warning disable 0649, 4014
        [InjectView(Resource.Id.loading_spinner)] private ProgressBar _bar;
        [InjectView(Resource.Id.followers_list)] private RecyclerView _votersList;
        [InjectView(Resource.Id.btn_back)] ImageButton _backButton;
        [InjectView(Resource.Id.profile_login)] private TextView _viewTitle;
        [InjectView(Resource.Id.btn_switcher)] private ImageButton _switcher;
        [InjectView(Resource.Id.btn_settings)] private ImageButton _settings;
        [InjectView(Resource.Id.people_count)] private TextView _people_count;
#pragma warning restore 0649

        protected override void CreatePresenter()
        {
            _presenter = new VotersPresenter();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (!IsInitialized)
            {
                InflatedView = inflater.Inflate(Resource.Layout.lyt_followers, null);
                Cheeseknife.Inject(this, InflatedView);
            }
            return InflatedView;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            if (IsInitialized)
                return;
            base.OnViewCreated(view, savedInstanceState);

            var font = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, "OpenSans-Regular.ttf");
            var semibold_font = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, "OpenSans-Semibold.ttf");

            var count = Activity.Intent.GetIntExtra("count", 0);
            _people_count.Text = $"{count.ToString("N0")} people";

            _backButton.Visibility = ViewStates.Visible;
            _switcher.Visibility = ViewStates.Gone;
            _settings.Visibility = ViewStates.Gone;
            _viewTitle.Typeface = semibold_font;
            _people_count.Typeface = font;
            _viewTitle.Text = Localization.Messages.Voters;

            _url = Activity.Intent.GetStringExtra("url");
            _votersAdapter = new FollowersAdapter<VotersResult>(Activity, _presenter, new[] { font, semibold_font });
            _votersAdapter.UserAction += OnClick;
            _votersList.SetAdapter(_votersAdapter);
            var scrollListner = new ScrollListener();
            scrollListner.ScrolledToBottom += LoadNext;
            _votersList.AddOnScrollListener(scrollListner);
            _votersList.SetLayoutManager(new LinearLayoutManager(Activity));
            LoadNext();
        }

        [InjectOnClick(Resource.Id.btn_back)]
        public void GoBackClick(object sender, EventArgs e)
        {
            Activity.OnBackPressed();
        }

        public override void OnDestroy()
        {
            _presenter.LoadCancel();
            base.OnDestroy();
        }

        private async void LoadNext()
        {
            var errors = await _presenter.TryLoadNext(_url);

            if (errors != null && errors.Count > 0)
                ShowAlert(errors);
            else
                _votersAdapter?.NotifyDataSetChanged();

            if (_bar != null)
                _bar.Visibility = ViewStates.Gone;
        }

        private void OnClick(int pos)
        {
            var voiter = _presenter[pos];
            if (voiter == null)
                return;
            ((BaseActivity)Activity).OpenNewContentFragment(new ProfileFragment(voiter.Username));
        }

        public override void OnDetach()
        {
            base.OnDetach();
            Cheeseknife.Reset(this);
        }
    }
}