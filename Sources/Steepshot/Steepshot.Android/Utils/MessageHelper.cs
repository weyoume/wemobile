﻿using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Steepshot.Core.Exceptions;
using Steepshot.Core.Localization;
using Steepshot.Core.Utils;

namespace Steepshot.Utils
{
    public static class MessageHelper
    {
        public static void ShowAlert(this Context context, Exception exception)
        {
            if (IsSkeepException(exception))
                return;

            var message = GetMsg(exception);
            if (string.IsNullOrWhiteSpace(message))
                return;

            var btnOk = AppSettings.LocalizationManager.GetText(LocalizationKeys.Ok);
            CreateAndShowDialog(context, message, btnOk);
        }

        public static void ShowAlert(this Context context, LocalizationKeys key)
        {
            var lm = AppSettings.LocalizationManager;
            var btnOk = lm.GetText(LocalizationKeys.Ok);
            var msg = lm.GetText(key);

            CreateAndShowDialog(context, msg, btnOk);
        }

        public static void ShowAlert(this Context context, LocalizationKeys keys, ToastLength length)
        {
            var message = AppSettings.LocalizationManager.GetText(keys);
            Toast.MakeText(context, message, length).Show();
        }

        public static void ShowAlert(this Context context, Exception exception, ToastLength length)
        {
            if (IsSkeepException(exception))
                return;

            var message = GetMsg(exception);

            if (string.IsNullOrWhiteSpace(message))
                return;

            Toast.MakeText(context, message, length).Show();
        }

        public static void ShowInteractiveMessage(this Context context, Exception exception, EventHandler<DialogClickEventArgs> tryAgainAction, EventHandler<DialogClickEventArgs> forgetAction)
        {
            if (IsSkeepException(exception))
                return;

            var message = GetMsg(exception);
            if (string.IsNullOrWhiteSpace(message))
                return;

            var lm = AppSettings.LocalizationManager;
            var pBtn = lm.GetText(LocalizationKeys.TryAgain);
            var nBtn = lm.GetText(LocalizationKeys.Forget);
            CreateAndShowDialog(context, message, pBtn, tryAgainAction, nBtn, forgetAction);
        }

        private static void CreateAndShowDialog(Context context, string msg, string positiveButtonText, EventHandler<DialogClickEventArgs> positiveButtonAction = null, string negativeButtonText = null, EventHandler<DialogClickEventArgs> negativeButtonAction = null)
        {
            var alert = new AlertDialog.Builder(context);
            if (msg.Contains("https:"))
            {
                var inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                var alertView = inflater.Inflate(Resource.Layout.lyt_alert, null);
                var textView = (TextView)alertView.FindViewById(Resource.Id.alert);
                textView.SetText(msg, TextView.BufferType.Normal);
                alert.SetView(alertView);
            }
            else
            {
                alert.SetMessage(msg);
            }

            if (positiveButtonAction == null)
                positiveButtonAction = (senderAlert, args) => { };

            alert.SetPositiveButton(positiveButtonText, positiveButtonAction);

            if (!string.IsNullOrEmpty(negativeButtonText))
                alert.SetNegativeButton(negativeButtonText, negativeButtonAction);

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private static string GetMsg(Exception exception)
        {
            var lm = AppSettings.LocalizationManager;

            if (exception is ValidationException validationException)
                return lm.GetText(validationException);

            var msg = string.Empty;

            if (exception is InternalException internalException)
            {
                msg = lm.GetText(internalException.Key);
            }
            else if (exception is RequestException requestException)
            {
                if (!string.IsNullOrEmpty(requestException.RawResponse))
                {
                    msg = lm.GetText(requestException.RawResponse);
                }
                else
                {
                    do
                    {
                        msg = lm.GetText(exception.Message);
                        exception = exception.InnerException;
                    } while (string.IsNullOrEmpty(msg) && exception != null);
                }
            }
            else
            {
                msg = lm.GetText(exception.Message);
            }

            return string.IsNullOrEmpty(msg) ? lm.GetText(LocalizationKeys.UnexpectedError) : msg;
        }

        private static bool IsSkeepException(Exception exception)
        {
            if (exception == null || exception is TaskCanceledException || exception is OperationCanceledException)
                return true;

            if (exception is RequestException requestException)
            {
                if (requestException.Exception is TaskCanceledException || requestException.Exception is OperationCanceledException)
                    return true;
            }

            return false;
        }
    }
}
