using System;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Components.Forms;

namespace WMS_UnitedTracors_Blazor.Helpers
{
    public class MemoryBrowserFile : IBrowserFile
    {
        private readonly byte[] _data;

        public MemoryBrowserFile(string name, string contentType, byte[] data)
        {
            Name = name;
            ContentType = contentType;
            _data = data;
            Size = data.Length;
            LastModified = DateTimeOffset.Now;
        }

        public string Name { get; }
        public DateTimeOffset LastModified { get; }
        public long Size { get; }
        public string ContentType { get; }

        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
        {
            return new MemoryStream(_data);
        }
    }
}
