
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
    [BroadcastReceiver(Enabled = true, Exported = true, Permission = "android.permission.RECEIVE_BOOT_COMPLETED", DirectBootAware = true)]
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
            if (presenter.OpenApi.Gateway == null)
                Log.Error("#Insta", $"Null gateway");

            var connectionService = AppSettings.ConnectionService;
            if (connectionService.IsConnectionAvailable())
            {
                var module = new InstagramModule(presenter.OpenApi.Gateway, AppSettings.User);
                if (module.IsAuthorized())
                {
                    module.TryCreateNewPost(CancellationToken.None);
                }
            }
            else
            {
                Log.Error("#Insta", $"No internet connection :(");
            }

            var am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            var myIntent = new Intent(context, typeof(SocialReceiver));
            var pIntent = PendingIntent.GetBroadcast(context, 0, myIntent, PendingIntentFlags.CancelCurrent);
            am.Set(AlarmType.RtcWakeup, Java.Lang.JavaSystem.CurrentTimeMillis() + 15000, pIntent);

            Log.Debug("#Insta", $"...taskReceived!");
        }
    }
}
