﻿using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ditch.Core.JsonRpc;
using Newtonsoft.Json;
using Steepshot.Core.Authorization;
using Steepshot.Core.Clients;
using Steepshot.Core.Exceptions;
using Steepshot.Core.Localization;
using Steepshot.Core.Models.Common;
using Steepshot.Core.Models.Requests;
using Steepshot.Core.Utils;

namespace Steepshot.Core.Integration
{
    public abstract class BaseModule
    {
        protected User User;
        protected readonly SteepshotApiClient Client;


        protected BaseModule(SteepshotApiClient client, User user)
        {
            Client = client;
            User = user;
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
                    return new OperationResult<VoidResponse>(uploadResult.Exception);

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
                return new OperationResult<MediaModel>(new InternalException(LocalizationKeys.PhotoUploadError, ex));
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
    }
}
