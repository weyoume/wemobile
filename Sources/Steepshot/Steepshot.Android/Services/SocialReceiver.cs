
using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Steepshot.Core.Integration;
using Steepshot.Core.Presenters;
using Steepshot.Core.Utils;

namespace Steepshot.Services
{
    [BroadcastReceiver(Enabled = true, Permission = "android.permission.RECEIVE_BOOT_COMPLETED", DirectBootAware = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted, "android.intent.action.QUICKBOOT_POWERON" })]
    public class SocialReceiver : BroadcastReceiver
    {
        private TestPresenter presenter;

        public SocialReceiver() : base()
        {
            presenter = new TestPresenter();
        }

        public override void OnReceive(Context context, Intent intent)
        {
            var connectionService = AppSettings.ConnectionService;
            if (connectionService.IsConnectionAvailable())
            {
                var module = new InstagramModule(presenter.OpenApi.Gateway, AppSettings.User);
                if (module.IsAuthorized())
                {
                    Log.Debug("#Insta", $"Create new post");
                    module.TryCreateNewPost(CancellationToken.None);
                }
            }
            else
            {
                Log.Error("#Insta", $"No internet connection :(");
            }

            if (Intent.ActionBootCompleted.Equals(intent.Action))
            {
                var am = (AlarmManager)context.GetSystemService(Context.AlarmService);
                var myIntent = new Intent(context, typeof(SocialReceiver));
                var pIntent = PendingIntent.GetBroadcast(context, 0, myIntent, 0);

                am.SetRepeating(AlarmType.ElapsedRealtimeWakeup, SystemClock.ElapsedRealtime() + 61000, 61000, pIntent); // 300000 - 5min
            }

            Log.Warn("#Insta", $"Task received");
        }
    }
}
