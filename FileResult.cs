using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace MKFileScanner
{
    public class FileResult
    {
        public string FileName { get; }
        public string AbsolutePath { get; }
        public string PlatformPath { get; }
        public long FileSize { get; }
        public DateTime? CreateTime { get; }
        public DateTime? ModifyTime { get; }
        public int AndroidMediaId { get; set; }
        public FileResult(string fileName, string absolutePath, string platformPath, long fileSize, DateTime? createTime, DateTime? modifyTime)
        {
            FileName = fileName;
            AbsolutePath = absolutePath;
            PlatformPath = platformPath;
            FileSize = fileSize;
            CreateTime = createTime;
            ModifyTime = modifyTime;
        }
    }
}
