using Android.App;
using Android.Runtime;
using Autofac;
using Square.Picasso;
using Steepshot.Core.Localization;
using Steepshot.Core.Sentry;
using Steepshot.Core.Services;
using Steepshot.Core.Utils;
using Steepshot.Services;
using Steepshot.Utils;
using System;
using Steepshot.Core.Authorization;
using Steepshot.Core.HttpClient;
using Android.Content;
using Java.Util;

namespace Steepshot.Base
{
    [Application]
    public class App : Application
    {
        public App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public static LruCache Cache;

        public override void OnCreate()
        {
            base.OnCreate();
            InitIoC(Context.Assets);
            InitPicassoCache();
            CheckInstagram();
        }

        private void InitPicassoCache()
        {
            if (Cache == null)
            {
                Cache = new LruCache(this);
                var d = new Picasso.Builder(this);
                d.MemoryCache(Cache);
                Picasso.SetSingletonInstance(d.Build());
            }
        }

        public void CheckInstagram()
        {
            var when = Calendar.Instance;
            when.Add(CalendarField.Second, 61);

            var am = (AlarmManager)Context.GetSystemService(AlarmService);
            var myIntent = new Intent(Context, typeof(SocialReceiver));
            var pIntent = PendingIntent.GetBroadcast(Context, 0, myIntent, 0);
            am.SetRepeating(AlarmType.RtcWakeup, when.TimeInMillis, 61000, pIntent);

            Android.Util.Log.Debug("#Insta", "_Alarm service started_");
        }

        public static void InitIoC(Android.Content.Res.AssetManager assetManagerssets)
        {
            if (AppSettings.Container == null)
            {
                var builder = new ContainerBuilder();
                var saverService = new SaverService();
                var dataProvider = new UserManager(saverService);
                var appInfo = new AppInfo();
                var assetsHelper = new AssetHelper(assetManagerssets);
                var connectionService = new ConnectionService();

                var localizationManager = new LocalizationManager(saverService, assetsHelper);
                var configManager = new ConfigManager(saverService, assetsHelper);

                builder.RegisterInstance(assetsHelper).As<IAssetHelper>().SingleInstance();
                builder.RegisterInstance(appInfo).As<IAppInfo>().SingleInstance();
                builder.RegisterInstance(saverService).As<ISaverService>().SingleInstance();
                builder.RegisterInstance(dataProvider).As<UserManager>().SingleInstance();
                builder.RegisterInstance(connectionService).As<IConnectionService>().SingleInstance();
                builder.RegisterInstance(connectionService).As<IConnectionService>().SingleInstance();
                builder.RegisterInstance(localizationManager).As<LocalizationManager>().SingleInstance();
                builder.RegisterInstance(configManager).As<ConfigManager>().SingleInstance();
                var configInfo = assetsHelper.GetConfigInfo();
                var reporterService = new ReporterService(appInfo, configInfo.RavenClientDsn);
                builder.RegisterInstance(reporterService).As<IReporterService>().SingleInstance();
                AppSettings.Container = builder.Build();
            }
        }
    }
}