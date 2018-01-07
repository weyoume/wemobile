using System.Threading;
using System.Threading.Tasks;
using Steepshot.Core.Models.Common;
using Steepshot.Core.Models.Requests;
using Steepshot.Core.Models.Responses;

namespace Steepshot.Core.Presenters
{
    public class PostDescriptionPresenter : TagsPresenter
    {
        public async Task<OperationResult<UploadResponse>> TryUploadWithPrepare(UploadImageRequest request)
        {
            return await TryRunTask<UploadImageRequest, UploadResponse>(UploadWithPrepare, OnDisposeCts.Token, request);
        }

        private async Task<OperationResult<UploadResponse>> UploadWithPrepare(CancellationToken ct, UploadImageRequest request)
        {
            return await Api.UploadWithPrepare(request, ct);
        }


        public async Task<OperationResult<ImageUploadResponse>> TryCreatePost(UploadImageRequest request, UploadResponse uploadResponse)
        {
            return await TryRunTask<UploadImageRequest, UploadResponse, ImageUploadResponse>(CreatePost, OnDisposeCts.Token, request, uploadResponse);
        }

        private async Task<OperationResult<ImageUploadResponse>> CreatePost(CancellationToken ct, UploadImageRequest request, UploadResponse uploadResponse)
        {
            return await Api.CreatePost(request, uploadResponse, ct);
        }
    }
}
