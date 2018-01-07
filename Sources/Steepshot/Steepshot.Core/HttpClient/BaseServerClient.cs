using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ditch.Core.Helpers;
using Steepshot.Core.Models.Common;
using Steepshot.Core.Models.Requests;
using Steepshot.Core.Models.Responses;
using Steepshot.Core.Serializing;
using Steepshot.Core.Errors;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Steepshot.Core.HttpClient
{
    public abstract class BaseServerClient
    {
        public volatile bool EnableRead;
        protected ApiGateway Gateway;
        protected JsonNetConverter JsonConverter;

        #region Get requests

        public async Task<OperationResult<ListResponse<Post>>> GetUserPosts(UserPostsRequest request, CancellationToken ct)
        {
            if (!EnableRead)
                return null;

            var results = Validate(request);
            if (results.Any())
                return new OperationResult<ListResponse<Post>>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var parameters = new Dictionary<string, object>();
            AddOffsetLimitParameters(parameters, request.Offset, request.Limit);
            AddLoginParameter(parameters, request.Login);
            AddCensorParameters(parameters, request);

            var endpoint = $"user/{request.Username}/posts";
            return await Gateway.Get<ListResponse<Post>>(GatewayVersion.V1P1, endpoint, parameters, ct);
        }

        public async Task<OperationResult<ListResponse<Post>>> GetUserRecentPosts(CensoredNamedRequestWithOffsetLimitFields request, CancellationToken ct)
        {
            if (!EnableRead)
                return null;

            var results = Validate(request);
            if (results.Any())
                return new OperationResult<ListResponse<Post>>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var parameters = new Dictionary<string, object>();
            AddOffsetLimitParameters(parameters, request.Offset, request.Limit);
            AddLoginParameter(parameters, request.Login);
            AddCensorParameters(parameters, request);

            var endpoint = "recent";
            return await Gateway.Get<ListResponse<Post>>(GatewayVersion.V1P1, endpoint, parameters, ct);
        }

        public async Task<OperationResult<ListResponse<Post>>> GetPosts(PostsRequest request, CancellationToken ct)
        {
            if (!EnableRead)
                return null;

            var results = Validate(request);
            if (results.Any())
                return new OperationResult<ListResponse<Post>>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var parameters = new Dictionary<string, object>();
            AddOffsetLimitParameters(parameters, request.Offset, request.Limit);
            AddLoginParameter(parameters, request.Login);
            AddCensorParameters(parameters, request);

            var endpoint = $"posts/{request.Type.ToString().ToLowerInvariant()}";
            return await Gateway.Get<ListResponse<Post>>(GatewayVersion.V1P1, endpoint, parameters, ct);
        }

        public async Task<OperationResult<ListResponse<Post>>> GetPostsByCategory(PostsByCategoryRequest request, CancellationToken ct)
        {
            if (!EnableRead)
                return null;

            var results = Validate(request);
            if (results.Any())
                return new OperationResult<ListResponse<Post>>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var parameters = new Dictionary<string, object>();
            AddOffsetLimitParameters(parameters, request.Offset, request.Limit);
            AddLoginParameter(parameters, request.Login);
            AddCensorParameters(parameters, request);

            var endpoint = $"posts/{request.Category}/{request.Type.ToString().ToLowerInvariant()}";
            return await Gateway.Get<ListResponse<Post>>(GatewayVersion.V1P1, endpoint, parameters, ct);
        }

        public async Task<OperationResult<ListResponse<UserFriend>>> GetPostVoters(VotersRequest request, CancellationToken ct)
        {
            if (!EnableRead)
                return null;

            var results = Validate(request);
            if (results.Any())
                return new OperationResult<ListResponse<UserFriend>>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var parameters = new Dictionary<string, object>();
            AddOffsetLimitParameters(parameters, request.Offset, request.Limit);
            AddVotersTypeParameters(parameters, request.Type);
            if (!string.IsNullOrEmpty(request.Login))
                AddLoginParameter(parameters, request.Login);

            var endpoint = $"post/{request.Url}/voters";
            return await Gateway.Get<ListResponse<UserFriend>>(GatewayVersion.V1P1, endpoint, parameters, ct);
        }

        public async Task<OperationResult<ListResponse<Post>>> GetComments(NamedInfoRequest request, CancellationToken ct)
        {
            if (!EnableRead)
                return null;

            var results = Validate(request);
            if (results.Any())
                return new OperationResult<ListResponse<Post>>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var parameters = new Dictionary<string, object>();
            AddOffsetLimitParameters(parameters, request.Offset, request.Limit);
            AddLoginParameter(parameters, request.Login);

            var endpoint = $"post/{request.Url}/comments";
            return await Gateway.Get<ListResponse<Post>>(GatewayVersion.V1P1, endpoint, parameters, ct);
        }

        public async Task<OperationResult<UserProfileResponse>> GetUserProfile(UserProfileRequest request, CancellationToken ct)
        {
            if (!EnableRead)
                return null;

            var results = Validate(request);
            if (results.Any())
                return new OperationResult<UserProfileResponse>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var parameters = new Dictionary<string, object>();
            AddLoginParameter(parameters, request.Login);
            parameters.Add("show_nsfw", Convert.ToInt32(request.ShowNsfw));
            parameters.Add("show_low_rated", Convert.ToInt32(request.ShowLowRated));

            var endpoint = $"user/{request.Username}/info";
            return await Gateway.Get<UserProfileResponse>(GatewayVersion.V1P1, endpoint, parameters, ct);
        }

        public async Task<OperationResult<ListResponse<UserFriend>>> GetUserFriends(UserFriendsRequest request, CancellationToken ct)
        {
            if (!EnableRead)
                return null;

            var results = Validate(request);
            if (results.Any())
                return new OperationResult<ListResponse<UserFriend>>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var parameters = new Dictionary<string, object>();
            AddOffsetLimitParameters(parameters, request.Offset, request.Limit);
            AddLoginParameter(parameters, request.Login);

            var endpoint = $"user/{request.Username}/{request.Type.ToString().ToLowerInvariant()}";
            return await Gateway.Get<ListResponse<UserFriend>>(GatewayVersion.V1P1, endpoint, parameters, ct);
        }

        public async Task<OperationResult<Post>> GetPostInfo(NamedInfoRequest request, CancellationToken ct)
        {
            if (!EnableRead)
                return null;

            var results = Validate(request);
            if (results.Any())
                return new OperationResult<Post>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var parameters = new Dictionary<string, object>();
            AddLoginParameter(parameters, request.Login);
            AddCensorParameters(parameters, request);

            var endpoint = $"post/{request.Url}/info";
            return await Gateway.Get<Post>(GatewayVersion.V1P1, endpoint, parameters, ct);
        }

        public async Task<OperationResult<ListResponse<UserFriend>>> SearchUser(SearchWithQueryRequest request, CancellationToken ct)
        {
            if (!EnableRead)
                return null;

            var results = Validate(request);
            if (results.Any())
                return new OperationResult<ListResponse<UserFriend>>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var parameters = new Dictionary<string, object>();
            AddLoginParameter(parameters, request.Login);
            AddOffsetLimitParameters(parameters, request.Offset, request.Limit);
            parameters.Add("query", request.Query);

            var endpoint = "user/search";
            return await Gateway.Get<ListResponse<UserFriend>>(GatewayVersion.V1P1, endpoint, parameters, ct);
        }

        public async Task<OperationResult<UserExistsResponse>> UserExistsCheck(UserExistsRequests request, CancellationToken ct)
        {
            if (!EnableRead)
                return null;

            var results = Validate(request);
            if (results.Any())
                return new OperationResult<UserExistsResponse>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var parameters = new Dictionary<string, object>();
            var endpoint = $"user/{request.Username}/exists";
            return await Gateway.Get<UserExistsResponse>(GatewayVersion.V1, endpoint, parameters, ct);
        }

        public async Task<OperationResult<ListResponse<SearchResult>>> GetCategories(OffsetLimitFields request, CancellationToken ct)
        {
            if (!EnableRead)
                return null;

            var results = Validate(request);
            if (results.Any())
                return new OperationResult<ListResponse<SearchResult>>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var parameters = new Dictionary<string, object>();
            AddOffsetLimitParameters(parameters, request.Offset, request.Limit);
            var endpoint = "categories/top";
            var result = await Gateway.Get<ListResponse<SearchResult>>(GatewayVersion.V1, endpoint, parameters, ct);

            if (result.IsSuccess)
            {
                foreach (var category in result.Result.Results)
                {
                    category.Name = Transliteration.ToRus(category.Name);
                }
            }
            return result;
        }

        public async Task<OperationResult<ListResponse<SearchResult>>> SearchCategories(SearchWithQueryRequest request, CancellationToken ct)
        {
            if (!EnableRead)
                return null;

            var results = Validate(request);
            if (results.Any())
                return new OperationResult<ListResponse<SearchResult>>(new ValidationError(string.Join(Environment.NewLine, results.Select(i => i.ErrorMessage))));

            var query = Transliteration.ToEng(request.Query);
            if (query != request.Query)
            {
                query = $"ru--{query}";
            }
            request.Query = query;

            var parameters = new Dictionary<string, object>();
            AddOffsetLimitParameters(parameters, request.Offset, request.Limit);
            parameters.Add("query", request.Query);
            var endpoint = "categories/search";
            var result = await Gateway.Get<ListResponse<SearchResult>>(GatewayVersion.V1, endpoint, parameters, ct);

            if (result.IsSuccess)
            {
                foreach (var categories in result.Result.Results)
                {
                    categories.Name = Transliteration.ToRus(categories.Name);
                }
            }

            return result;
        }

        protected async Task Trace(string endpoint, string login, ErrorBase resultErrors, string target, CancellationToken ct)
        {
            if (!EnableRead)
                return;

            try
            {
                var parameters = new Dictionary<string, object>();
                AddLoginParameter(parameters, login);
                parameters.Add("error", resultErrors == null ? string.Empty : resultErrors.Message);
                if (!string.IsNullOrEmpty(target))
                    parameters.Add("target", target);
                await Gateway.Post<VoidResponse>(GatewayVersion.V1, $@"log/{endpoint}", parameters, ct);
            }
            catch
            {
                //todo nothing
            }
        }

        public async Task<OperationResult<BeneficiariesResponse>> GetBeneficiaries(bool isNeedRewards, CancellationToken ct)
        {
            if (!EnableRead)
                return null;

            var parameters = new Dictionary<string, object>();
            SetBeneficiaryParameters(parameters, isNeedRewards);

            var endpoint = "beneficiaries";
            return await Gateway.Get<BeneficiariesResponse>(GatewayVersion.V1, endpoint, parameters, ct);
        }

        #endregion Get requests


        protected async Task<OperationResult<UploadResponse>> Upload(UploadImageRequest request, CancellationToken ct)
        {
            if (!EnableRead)
                return null;

            OperationHelper.PrepareTags(request.Tags);
            return await Gateway.Upload<UploadResponse>(GatewayVersion.V1, "post/prepare", request, ct);
        }

        private void AddOffsetLimitParameters(Dictionary<string, object> parameters, string offset, int limit)
        {
            if (!string.IsNullOrWhiteSpace(offset))
                parameters.Add("offset", offset);

            if (limit > 0)
                parameters.Add("limit", limit);
        }

        private void SetBeneficiaryParameters(Dictionary<string, object> parameters, bool isNeedRewards)
        {
            if (!isNeedRewards)
                parameters.Add("set_beneficiary", "steepshot_no_rewards");
        }

        private void AddVotersTypeParameters(Dictionary<string, object> parameters, VotersType type)
        {
            if (type != VotersType.All)
                parameters.Add(type == VotersType.Likes ? "likes" : "flags", 1);
        }

        private void AddLoginParameter(Dictionary<string, object> parameters, string login)
        {
            if (!string.IsNullOrEmpty(login))
                parameters.Add("username", login);
        }

        private void AddCensorParameters(Dictionary<string, object> parameters, CensoredNamedRequestWithOffsetLimitFields request)
        {
            parameters.Add("show_nsfw", Convert.ToInt32(request.ShowNsfw));
            parameters.Add("show_low_rated", Convert.ToInt32(request.ShowLowRated));
        }

        protected List<ValidationResult> Validate<T>(T request)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(request);
            Validator.TryValidateObject(request, context, results, true);
            return results;
        }
    }
}
