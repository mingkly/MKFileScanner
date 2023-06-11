using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
#nullable enable
namespace MKFileScanner
{
    public class FileScanOption
    {

        /// <summary>
        /// the input fileNameContains extension.
        /// expression support startWith,endWith,contains,
        /// =,!=,,||,! in android mediastore query,unsupport expression will be used after name has been queried.
        /// </summary>
        public Expression<Func<string,bool>>? FileNameExpression {  get; set; }
        /// <summary>
        /// extensions need.
        /// </summary>
        public IEnumerable<string>? Extensions { get; set; }
        public FileType? FileType { get; set; }
        public long MinFileSize { get; set; } = 0;
        public long MaxFileSize { get; set;}=long.MaxValue;
        public DateTime MinCreateTime { get; set; } = DateTime.MinValue;
        public DateTime MaxCreateTime { get; set; }=DateTime.MaxValue;
        public DateTime MinModifyTime { get; set; } = DateTime.MinValue;
        public DateTime MaxModifyTime { get; set; } = DateTime.MaxValue;
        /// <summary>
        /// this will be append to contentResolver.query() selection parameter,
        /// u need to add "and" or "or" in start.
        /// </summary>
        public string? AndroidMediaStoreQueryAppend { get; set; }
        /// <summary>
        /// this will be append to contentResolver.query() selectionArgs parameter,
        /// u need to add "and" or "or" in start.
        /// </summary>
        public string[]? AndroidMediaStoreQueryArgsAppend { get; set; }
    }
}
