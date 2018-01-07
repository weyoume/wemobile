using System;
using System.IO;
using System.Threading;

namespace Steepshot.Core.HttpClient
{
    public class DounloadModel : IDisposable
    {
        public Stream SourceStream { get; set; }

        public long Total { get; set; }

        public CancellationToken Token { get; set; }

        public string MediaType { get; set; }

        public void Dispose()
        {
            SourceStream?.Dispose();
        }
    }
}