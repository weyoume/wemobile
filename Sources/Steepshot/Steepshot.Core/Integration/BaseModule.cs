﻿using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ditch.Core.JsonRpc;
using Newtonsoft.Json;
using Steepshot.Core.Authorization;
using Steepshot.Core.Errors;
using Steepshot.Core.HttpClient;
using Steepshot.Core.Localization;
using Steepshot.Core.Models.Common;
using Steepshot.Core.Models.Requests;
using Steepshot.Core.Utils;

namespace Steepshot.Core.Integration
{
    public abstract class BaseModule
    {
        protected User User;
        public SteepshotApiClient Client;


        protected BaseModule(User user)
        {
            User = user;
            Client = new SteepshotApiClient();
            Client.InitConnector(User.Chain, false);
        }

        public abstract bool IsAuthorized();

        public abstract void TryCreateNewPost(CancellationToken token);


        protected async Task<OperationResult<VoidResponse>> CreatePost(PreparePostModel model, CancellationToken token)
        {
            for (var i = 0; i < model.Media.Length; i++)
            {
                var media = model.Media[i];
                var uploadResult = await UploadPhoto(media.Url, token);
                if (!uploadResult.IsSuccess)
                    return new OperationResult<VoidResponse>(uploadResult.Error);

                model.Media[i] = uploadResult.Result;
            }

            return await Client.CreateOrEditPost(model, token);
        }

        private async Task<OperationResult<MediaModel>> UploadPhoto(string url, CancellationToken token)
        {
            MemoryStream stream = null;
            WebClient client = null;

            try
            {
                client = new WebClient();
                var bytes = client.DownloadData(new Uri(url));

                stream = new MemoryStream(bytes);
                var request = new UploadMediaModel(User.UserInfo, stream, Path.GetExtension(MimeTypeHelper.Jpg));
                var serverResult = await Client.UploadMedia(request, token);
                return serverResult;
            }
            catch (Exception ex)
            {
                AppSettings.Reporter.SendCrash(ex);
                return new OperationResult<MediaModel>(new AppError(LocalizationKeys.PhotoUploadError));
            }
            finally
            {
                stream?.Flush();
                client?.Dispose();
                stream?.Dispose();
            }
        }


        protected T GetOptionsOrDefault<T>(string appId)
            where T : new()
        {
            T model;
            if (!User.Integration.ContainsKey(appId))
            {
                model = new T();
                User.Integration.Add(appId, JsonConvert.SerializeObject(model));
            }
            else
            {
                var json = User.Integration[appId];
                model = JsonConvert.DeserializeObject<T>(json);
            }

            return model;
        }

        protected void SaveOptions<T>(string appId, T model)
        {
            if (User.Integration.ContainsKey(appId))
                User.Integration[appId] = JsonConvert.SerializeObject(model);
            else
                User.Integration.Add(appId, JsonConvert.SerializeObject(model));

            User.Save();
        }


        protected async void SendLog(LinkedLog log, CancellationToken token)
        {
            await Client.Trace("external_ref", log, token);
        }


        protected class RecentMedia
        {
            public string Id { get; set; } = string.Empty;
            public string CreatedTime { get; set; } = string.Empty;
            public int Likes { get; set; }
            public int Comments { get; set; }
            public string Type { get; set; } = string.Empty;
        }

        protected class LinkedLog
        {
            public string Login { get; set; }
            public string Username { get; set; } = string.Empty;
            public string UserMail { get; set; } = string.Empty;
            public DateTime Time { get; set; }
            public RecentMedia[] RecentMedia { get; set; } = new RecentMedia[0];

            public LinkedLog(User user)
            {
                Login = user.Login;
                Time = DateTime.Now;
            }
        }
    }
}
