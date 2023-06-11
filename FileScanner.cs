using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MKFileScanner
{
    public partial class FileScanner : IFileScanner
    {
        public static void ScanUseFileAPI(string folder, FileScanOption? scanOption, Action<FileResult> onFileScannedCallback)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories);
            if (scanOption?.FileNameExpression != null)
            {
                files = files.Where(scanOption.FileNameExpression.Compile());
            }
            if (scanOption?.Extensions?.Any() ?? false)
            {
                var extensions = scanOption.Extensions.Select(s => s.ToLower());
                files = files.Where(s => extensions.Contains(Path.GetExtension(s).TrimStart('.').ToLower()));
            }

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (
                    scanOption == null ||
                    (fileInfo.Length > scanOption.MinFileSize && fileInfo.Length < scanOption.MaxFileSize
                    &&
                    fileInfo.CreationTime > scanOption.MinCreateTime && fileInfo.CreationTime < scanOption.MaxCreateTime
                    &&
                    fileInfo.LastWriteTime > scanOption.MinModifyTime && fileInfo.LastWriteTime < scanOption.MaxModifyTime
                    ))
                {
                    onFileScannedCallback?.Invoke(new FileResult(
                        Path.GetFileName(file),
                        file,
                        file,
                        fileInfo.Length,
                        fileInfo.CreationTime,
                        fileInfo.LastWriteTime));
                }
            }
        }

        public void Scan(string? folder, FileScanOption? option, Action<FileResult> onFileScannedCallback)
        {
#if ANDROID
            if (folder == null || folder.StartsWith("content:"))
            {
                Platforms.Android.FileScanner.FileScanner.ScanFile(folder, option, onFileScannedCallback);
            }
            else if (folder.StartsWith("file:"))
            {
                FileScanner.ScanUseFileAPI(folder.Replace("file://", ""), option, onFileScannedCallback);
            }
            else
            {
                FileScanner.ScanUseFileAPI(folder, option, onFileScannedCallback);
            }
#else
            if (folder == null)
            {
                throw new ArgumentNullException(nameof(folder));
            }
            ScanUseFileAPI(folder, option, onFileScannedCallback);
#endif
        }

        public Task<MetadataMap?> ReadMetadataAsync(string platformPath)
        {
#if ANDROID
            return Task.Run(()=>Platforms.Android.MetadataReader.Read(platformPath));
#elif WINDOWS
            return Platforms.Windows.MetadataReader.ReadMetadata(platformPath);
#else
            return Task.FromResult<MetadataMap?>(null);
#endif
        }
    }
}
