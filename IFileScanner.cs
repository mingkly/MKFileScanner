using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKFileScanner
{
#nullable enable
    public interface IFileScanner
    {
        public void Scan(string? folder,FileScanOption? option, Action<FileResult> onFileScannedCallback);
        public Task<MetadataMap?> ReadMetadataAsync(string platformPath);
    }
}
