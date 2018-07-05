
using System;

using Android.App;
using Android.Content;
using Android.OS;

namespace Steepshot.Services
{
    [Service(Label = "SocialService")]
    [IntentFilter(new String[] { "com.Steepshot.SocialService" })]
    public class SocialService : Service
    {
        IBinder binder;

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            // start your service logic here

            // Return the correct StartCommandResult for the type of service you are building
            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            binder = new SocialServiceBinder(this);
            return binder;
        }
    }

    public class SocialServiceBinder : Binder
    {
        readonly SocialService service;

        public SocialServiceBinder(SocialService service)
        {
            this.service = service;
        }

        public SocialService GetSocialService()
        {
            return service;
        }
    }
}
