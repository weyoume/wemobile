
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

namespace Steepshot.Services
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class SocialReceiver : BroadcastReceiver
    {
        public SocialReceiver() : base()
        {
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

            var am = (AlarmManager)context.GetSystemService(Context.AlarmService);
            var myIntent = new Intent(context, typeof(SocialReceiver));
            var pIntent = PendingIntent.GetBroadcast(context, 0, myIntent, PendingIntentFlags.CancelCurrent);
            am.Set(AlarmType.RtcWakeup, Java.Lang.JavaSystem.CurrentTimeMillis() + 5000, pIntent);

            Log.Debug("#Debug", "TaskReceived!");
        }
    }
}
