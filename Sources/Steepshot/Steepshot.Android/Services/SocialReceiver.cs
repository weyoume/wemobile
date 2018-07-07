
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
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class SocialReceiver : BroadcastReceiver
    {
        private TestPresenter presenter;

        public SocialReceiver() : base()
        {
            presenter = new TestPresenter();
        }

        public override void OnReceive(Context context, Intent intent)
        {
            //var nm = (NotificationManager)context.GetSystemService(Context.NotificationService);
            //int icon = Resource.Drawable.ic_steem;
            //string text = "Test";
            //long when = Java.Lang.JavaSystem.CurrentTimeMillis();

            //var notification = new Notification(icon, text, when);

            //var intentTL = new Intent(context, typeof(Activity.RootActivity));
            //notification.SetLatestEventInfo(context, "Test", "Do something", PendingIntent.GetActivity(context, 0, intentTL, PendingIntentFlags.CancelCurrent));
            //notification.Flags = NotificationFlags.AutoCancel;
            //notification.Defaults = NotificationDefaults.Lights;
            //nm.Notify(1, notification);

            Log.Debug("#Debug", $"TaskStarted...");
            Log.Debug("#Debug", $"Data: {presenter.OpenApi.Gateway.ToString()}, {AppSettings.User}");

            var module = new InstagramModule(presenter.OpenApi.Gateway, AppSettings.User);
            if (module.IsAuthorized())
            {
                module.TryCreateNewPost(CancellationToken.None);
            }

            var am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            var myIntent = new Intent(context, typeof(SocialReceiver));
            var pIntent = PendingIntent.GetBroadcast(context, 0, myIntent, PendingIntentFlags.CancelCurrent);
            am.Set(AlarmType.RtcWakeup, Java.Lang.JavaSystem.CurrentTimeMillis() + 15000, pIntent);

            Log.Debug("#Debug", $"TaskReceived! {module.IsAuthorized().ToString()}");
        }
    }
}
