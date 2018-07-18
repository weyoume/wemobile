﻿using System;
using Android.App;
using Android.Content;
using Android.Util;
using Newtonsoft.Json;
using Steepshot.Core.Authorization;
using Steepshot.Core.HttpClient;
using Steepshot.Core.Utils;
using Steepshot.Services;
using Xamarin.Auth;

namespace Steepshot.Integration
{
    public class InstagramModule : Steepshot.Core.Integration.InstagramModule
    {
        protected const string AccessTokenKeyName = "access_token";
        protected readonly Uri AuthorizeUrl = new Uri("https://api.instagram.com/oauth/authorize/");
        private readonly ModuleConfig _moduleConfig;

        public string UserToken => GetOptionsOrDefault<ModuleOptionsModel>(AppId).AccessToken;


        public InstagramModule(ApiGateway gateway, User user)
            : base(gateway, user)
        {
            var dic = AppSettings.AssetHelper.IntegrationModuleConfig();
            if (dic != null && dic.ContainsKey(AppId))
                _moduleConfig = JsonConvert.DeserializeObject<ModuleConfig>(dic[AppId]);
        }

        public void AuthToInstagram(Context context)
        {
            Log.Warn("#Insta", "Auth to instagram...");

            if (_moduleConfig == null)
                return;

            var opt = GetOptionsOrDefault<ModuleOptionsModel>(AppId);
            if (string.IsNullOrEmpty(opt.AccessToken))
            {
                var auth = new OAuth2Authenticator(_moduleConfig.ClientId, _moduleConfig.Scope, AuthorizeUrl, new Uri(_moduleConfig.RedirectUrl));
                auth.Completed += AuthOnCompleted;
                var intent = auth.GetUI(context);
                context.StartActivity(intent);
            }
        }

        public void CheckInstagram(Context context)
        {
            var when = Java.Util.Calendar.Instance;
            when.Add(Java.Util.CalendarField.Second, 61);

            var am = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
            var myIntent = new Intent(context, typeof(SocialReceiver));
            var pIntent = PendingIntent.GetBroadcast(context, 0, myIntent, 0);
            am.SetRepeating(AlarmType.RtcWakeup, 0, when.TimeInMillis, pIntent);

            Log.Debug("#Insta", "_Alarm service started_");
        }

        private void AuthOnCompleted(object o, AuthenticatorCompletedEventArgs args)
        {
            if (args.IsAuthenticated)
            {
                var opt = GetOptionsOrDefault<ModuleOptionsModel>(AppId);

                if (args.Account.Properties.ContainsKey(AccessTokenKeyName))
                    opt.AccessToken = args.Account.Properties[AccessTokenKeyName];

                Log.Warn("#Insta", "Auth completed!");

                User.Integration[AppId] = JsonConvert.SerializeObject(opt);
                User.Save();

                CheckInstagram(Application.Context);
            }
        }

        private class ModuleConfig
        {
            public string ClientId { get; set; }

            public string ClientSecret { get; set; }

            public string Scope { get; set; }

            public string RedirectUrl { get; set; }
        }
    }
}
