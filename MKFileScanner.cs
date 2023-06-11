using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKFileScanner
{
    public static class MKFileScanner
    {
        static IFileScanner? fileScanner;
        public static IFileScanner Default
        {
            get
            {
                fileScanner ??= new FileScanner();
                return fileScanner;
            }
        }
        public static void Scan(string? folder, FileScanOption? option, Action<FileResult> onFileScannedCallback)=>Default.Scan(folder, option, onFileScannedCallback);
        public static Task<MetadataMap?> ReadMetadataAsync(string platformPath)=>Default.ReadMetadataAsync(platformPath);

    }
}
