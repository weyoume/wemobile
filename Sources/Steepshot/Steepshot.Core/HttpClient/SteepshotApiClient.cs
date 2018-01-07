using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Steepshot.Core.Extensions;
using Steepshot.Core.Models.Common;
using Steepshot.Core.Models.Requests;
using Steepshot.Core.Models.Responses;
using Steepshot.Core.Serializing;
using System.Linq;
using Steepshot.Core.Errors;

namespace Steepshot.Core.HttpClient
{
    public class SteepshotApiClient : BaseServerClient, ISteepshotApiClient
    {
        private readonly Dictionary<string, Beneficiary[]> _beneficiariesCash;
        private readonly object _synk;

        private CancellationTokenSource _ctsMain;
        private BaseDitchClient _ditchClient;

        public SteepshotApiClient()
        {
            Gateway = new ApiGateway();
            JsonConverter = new JsonNetConverter();
            _beneficiariesCash = new Dictionary<string, Beneficiary[]>();
            _synk = new object();
        }

        public void InitConnector(KnownChains chain, bool isDev, CancellationToken token)
        {
            var sUrl = string.Empty;
            switch (chain)
            {
                case KnownChains.Steem when isDev:
                    sUrl = Constants.SteemUrlQa;
                    break;
                case KnownChains.Steem:
                    sUrl = Constants.SteemUrl;
                    break;
                case KnownChains.Golos when isDev:
                    sUrl = Constants.GolosUrlQa;
                    break;
                case KnownChains.Golos:
                    sUrl = Constants.GolosUrl;
                    break;
            }

            lock (_synk)
            {
                if (!string.IsNullOrEmpty(Gateway.Url))
                {
                    _ditchClient.EnableWrite = false;
                    _ctsMain.Cancel();
                }

                _ctsMain = new CancellationTokenSource();

                _ditchClient = chain == KnownChains.Steem
                    ? (BaseDitchClient)new SteemClient(JsonConverter)
                    : new GolosClient(JsonConverter);

                Gateway.Url = sUrl;
                EnableRead = true;
            }
        }

        public bool TryReconnectChain(CancellationToken token)
        {
            return _ditchClient.TryReconnectChain(token);
        }

        public async Task<OperationResult<VoidResponse>> LoginWithPostingKey(AuthorizedRequest request, CancellationToken ct)
        {
            var results = Validate(request);
            if (results.Any())
                return new OperationResult<VoidResponse>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var result = await _ditchClient.LoginWithPostingKey(request, ct);
            Trace("login-with-posting", request.Login, result.Error, string.Empty, ct);//.Wait(5000);
            return result;
        }

        public async Task<OperationResult<VoteResponse>> Vote(VoteRequest request, CancellationToken ct)
        {
            var results = Validate(request);
            if (results.Any())
                return new OperationResult<VoteResponse>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var result = await _ditchClient.Vote(request, ct);
            Trace($"post/{request.Identifier}/{request.Type.GetDescription()}", request.Login, result.Error, request.Identifier, ct);//.Wait(5000);
            return result;
        }

        public async Task<OperationResult<VoidResponse>> Follow(FollowRequest request, CancellationToken ct)
        {
            var results = Validate(request);
            if (results.Any())
                return new OperationResult<VoidResponse>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var result = await _ditchClient.Follow(request, ct);
            Trace($"user/{request.Username}/{request.Type.ToString().ToLowerInvariant()}", request.Login, result.Error, request.Username, ct);//.Wait(5000);
            return result;
        }

        public async Task<OperationResult<CommentResponse>> CreateComment(CommentRequest request, CancellationToken ct)
        {
            var results = Validate(request);
            if (results.Any())
                return new OperationResult<CommentResponse>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var bKey = $"{_ditchClient.GetType()}{request.IsNeedRewards}";
            if (_beneficiariesCash.ContainsKey(bKey))
            {
                request.Beneficiaries = _beneficiariesCash[bKey];
            }
            else
            {
                var beneficiaries = await GetBeneficiaries(request.IsNeedRewards, ct);
                if (beneficiaries.IsSuccess)
                    _beneficiariesCash[bKey] = request.Beneficiaries = beneficiaries.Result.Beneficiaries;
            }

            var result = await _ditchClient.CreateComment(request, ct);
            Trace($"post/{request.Url}/comment", request.Login, result.Error, request.Url, ct);//.Wait(5000);
            return result;
        }

        public async Task<OperationResult<CommentResponse>> EditComment(CommentRequest request, CancellationToken ct)
        {
            var results = Validate(request);
            if (results.Any())
                return new OperationResult<CommentResponse>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var result = await _ditchClient.EditComment(request, ct);
            Trace($"post/{request.Url}/comment", request.Login, result.Error, request.Url, ct);//.Wait(5000);
            return result;
        }

        public async Task<OperationResult<ImageUploadResponse>> CreatePost(UploadImageRequest request, UploadResponse uploadResponse, CancellationToken ct)
        {
            var results = Validate(request);
            if (results.Any())
                return new OperationResult<ImageUploadResponse>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var result = await _ditchClient.CreatePost(request, uploadResponse, ct);
            Trace("post", request.Login, result.Error, uploadResponse.Payload.Permlink, ct);//.Wait(5000);
            return result;
        }

        public async Task<OperationResult<UploadResponse>> UploadWithPrepare(UploadImageRequest request, CancellationToken ct)
        {
            var results = Validate(request);
            if (results.Any())
                return new OperationResult<UploadResponse>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var trxResp = await _ditchClient.GetVerifyTransaction(request, ct);

            if (!trxResp.IsSuccess)
                return new OperationResult<UploadResponse>(trxResp.Error);

            request.VerifyTransaction = trxResp.Result;
            var response = await Upload(request, ct);

            if (response.IsSuccess)
                response.Result.PostUrl = request.PostUrl;
            return response;
        }

        public async Task<OperationResult<VoidResponse>> DeletePostOrComment(DeleteRequest request, CancellationToken ct)
        {
            var results = Validate(request);
            if (results.Any())
                return new OperationResult<VoidResponse>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var response = await _ditchClient.DeletePostOrComment(request, ct);
            // if (response.IsSuccess)
            return response;
        }
    }
}
