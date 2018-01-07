using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Steepshot.Core.Models.Requests;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Steepshot.Core.Errors;
using Steepshot.Core.Models.Common;
using Steepshot.Core.Serializing;

namespace Steepshot.Core.HttpClient
{
    public class ApiGateway
    {
        #region SupportedMIME

        private HashSet<string> SupportedMIME { get; set; } = new HashSet<string>
        {
            "image/gif",// GIF(RFC 2045 и RFC 2046)
            "image/jpeg",// JPEG (RFC 2045 и RFC 2046)
            "image/pjpeg",// JPEG[8]
            "image/png",// Portable Network Graphics[9](RFC 2083)
            "image/svg+xml",// SVG[10]
            "image/tiff",// TIFF(RFC 3302)
            "image/vnd.microsoft.icon",// ICO[11]
            "image/vnd.wap.wbmp",// WBMP
            "image/webp",// WebP
            
            "video/mpeg",// MPEG-1 (RFC 2045 и RFC 2046)
            "video/mp4",// MP4 (RFC 4337)
            "video/ogg",// Ogg Theora или другое видео (RFC 5334)
            "video/quicktime",// QuickTime[12]
            "video/webm",// WebM
            "video/x-ms-wmv",// Windows Media Video[6]
            "video/x-flv",// FLV
            "video/3gpp",// .3gpp .3gp [13]
            "video/3gpp2",// .3gpp2 .3g2 [13]
        };

        #endregion SupportedMIME

        private readonly Regex _errorJson = new Regex("(?<=^{\"[a-z_0-9]*\":\\[\").*(?=\"]}$)");
        private readonly Regex _errorJson2 = new Regex("(?<=^{\"[a-z_0-9]*\":\").*(?=\"}$)");
        private readonly Regex _errorHtml = new Regex(@"<[^>]+>");
        protected readonly JsonNetConverter JsonNetConverter = new JsonNetConverter();

        public int ConnectionTimeout { get; set; } = 10000;

        private readonly System.Net.Http.HttpClient _client;
        public string Url { get; set; }

        public ApiGateway() : this(256000) { }

        public ApiGateway(long maxResponseContentBufferSize)
        {
            var httpHendler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            _client = new System.Net.Http.HttpClient(httpHendler)
            {
                MaxResponseContentBufferSize = maxResponseContentBufferSize,
            };
        }


        public async Task<OperationResult<T>> DownloadAsync<T>(string url, Func<DounloadModel, Task<T>> readDelegate, CancellationToken token)
        {
            var ctsTimeout = new CancellationTokenSource(ConnectionTimeout);
            var cts = CancellationTokenSource.CreateLinkedTokenSource(token, ctsTimeout.Token);
            token.ThrowIfCancellationRequested();
            try
            {

                var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                return await CreateResult<T>(response, readDelegate, token);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OperationResult<T>> Get<T>(GatewayVersion version, string endpoint, Dictionary<string, object> parameters, CancellationToken token)
        {
            var ctsTimeout = new CancellationTokenSource(ConnectionTimeout);
            var cts = CancellationTokenSource.CreateLinkedTokenSource(token, ctsTimeout.Token);
            token.ThrowIfCancellationRequested();

            var url = GetUrl(version, endpoint, parameters);
            var response = await _client.GetAsync(url, cts.Token);
            return await CreateResult<T>(response, null, token);
        }

        public async Task<OperationResult<T>> Post<T>(GatewayVersion version, string endpoint, Dictionary<string, object> parameters, CancellationToken token)
        {
            var url = GetUrl(version, endpoint);
            HttpContent content = null;
            if (parameters != null && parameters.Count > 0)
            {
                var param = JsonNetConverter.Serialize(parameters);
                content = new StringContent(param, Encoding.UTF8, "application/json");
            }

            var ctsTimeout = new CancellationTokenSource(ConnectionTimeout);
            var cts = CancellationTokenSource.CreateLinkedTokenSource(token, ctsTimeout.Token);
            token.ThrowIfCancellationRequested();

            var response = await _client.PostAsync(url, content, cts.Token);
            return await CreateResult<T>(response, null, token);
        }

        public async Task<OperationResult<T>> Upload<T>(GatewayVersion version, string endpoint, UploadImageRequest request, CancellationToken ct)
        {
            var url = GetUrl(version, endpoint);
            var fTitle = Guid.NewGuid().ToString(); //request.Title.Length > 20 ? request.Title.Remove(20) : request.Title;

            var multiContent = new MultipartFormDataContent();
            multiContent.Add(new ByteArrayContent(request.Photo), "photo", fTitle);
            multiContent.Add(new StringContent(request.Title), "title");
            multiContent.Add(new StringContent($"@{request.Login}/{request.PostUrl}"), "post_permlink");
            if (!string.IsNullOrWhiteSpace(request.Description))
                multiContent.Add(new StringContent(request.Description), "description");
            if (!string.IsNullOrWhiteSpace(request.Login))
                multiContent.Add(new StringContent(request.Login), "username");
            if (!string.IsNullOrWhiteSpace(request.VerifyTransaction))
                multiContent.Add(new StringContent(request.VerifyTransaction), "trx");
            if (!request.IsNeedRewards)
                multiContent.Add(new StringContent("steepshot_no_rewards"), "set_beneficiary");
            foreach (var tag in request.Tags)
                multiContent.Add(new StringContent(tag), "tags");

            var response = await _client.PostAsync(url, multiContent, ct);
            return await CreateResult<T>(response, null, ct);
        }

        private string GetUrl(GatewayVersion version, string endpoint, Dictionary<string, object> parameters = null)
        {
            var sb = new StringBuilder(Url);

            switch (version)
            {
                case GatewayVersion.V1:
                    sb.Append("/v1/");
                    break;
                case GatewayVersion.V1P1:
                    sb.Append("/v1_1/");
                    break;
            }

            sb.Append(endpoint);
            if (parameters != null && parameters.Count > 0)
            {

                var isFirst = true;
                foreach (var parameter in parameters)
                {
                    if (isFirst)
                    {
                        sb.Append("?");
                        isFirst = false;
                    }
                    else
                    {
                        sb.Append("&");
                    }
                    sb.Append(parameter.Key);
                    sb.Append("=");
                    sb.Append(parameter.Value);
                }
            }

            return sb.ToString();
        }

        protected virtual async Task<OperationResult<T>> CreateResult<T>(HttpResponseMessage response, Func<DounloadModel, Task<T>> readDelegate, CancellationToken ct)
        {
            var result = new OperationResult<T>();

            // HTTP error
            if (response.StatusCode == HttpStatusCode.InternalServerError ||
                response.StatusCode != HttpStatusCode.OK &&
                response.StatusCode != HttpStatusCode.Created)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(content))
                {
                    result.Error = new ServerError((int)response.StatusCode, Localization.Errors.EmptyResponseContent);
                    return result;
                }
                if (_errorHtml.IsMatch(content))
                {
                    result.Error = new ServerError((int)response.StatusCode, Localization.Errors.HttpErrorCodeToMessage(response.StatusCode, content));
                    return result;
                }
                var match = _errorJson.Match(content);
                if (match.Success)
                {
                    var txt = match.Value.Replace("\",\"", Environment.NewLine);
                    result.Error = new ServerError((int)response.StatusCode, txt);
                    return result;
                }

                match = _errorJson2.Match(content);
                if (match.Success)
                {
                    result.Error = new ServerError((int)response.StatusCode, match.Value);
                    return result;
                }

                result.Error = new HttpError((int)response.StatusCode, Localization.Errors.UnexpectedError);
                return result;
            }

            if (response.Content == null)
                return result;

            var mediaType = response.Content.Headers?.ContentType?.MediaType.ToLower();

            if (mediaType != null)
            {
                if (mediaType.Equals("application/json"))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    result.Result = JsonNetConverter.Deserialize<T>(content);
                }
                else if (SupportedMIME.Contains(mediaType))
                {
                    result.Result = await Download<T>(response, readDelegate, ct);
                }
                else
                {
                    result.Error = new ApplicationError(Localization.Errors.UnsupportedMime);
                }
            }

            return result;
        }

        private async Task<T> Download<T>(HttpResponseMessage response, Func<DounloadModel, Task<T>> readDelegate, CancellationToken token)
        {
            DounloadModel dounloadModel = null;

            try
            {
                dounloadModel = new DounloadModel();
                dounloadModel.Total = response.Content.Headers.ContentLength ?? -1;
                dounloadModel.Token = token;
                dounloadModel.SourceStream = await response.Content.ReadAsStreamAsync();
                dounloadModel.MediaType = response.Content.Headers.ContentType.MediaType;

                if (readDelegate != null)
                    return await readDelegate.Invoke(dounloadModel);
                return default(T);
            }
            finally
            {
                dounloadModel?.Dispose();
            }
        }
    }
}
