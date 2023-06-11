using Android.Content;
using Android.Net;
using Android.OS;
using Android.Provider;
using MKFileScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileResult = MKFileScanner.FileResult;
using Uri = Android.Net.Uri;
using Application = Android.App.Application;
using System.Diagnostics;
using AndroidX.DocumentFile.Provider;
using System.Runtime.InteropServices;
using static Java.Util.Jar.Attributes;
using Environment = Android.OS.Environment;
#nullable enable
namespace MKFileScanner.Platforms.Android.FileScanner
{
    internal class FileScanner
    {
        public static DateTime StartTime = new(1970, 1, 1);
        public static Context Context => Application.Context;
        public static ContentResolver? ContentResolver => Application.Context.ContentResolver;
        public static void ScanFile(string? folder, FileScanOption? scanOption, Action<FileResult>? onFileScannedCallback)
        {
            if (folder == null)
            {
                ScanUseMediaStore(scanOption, onFileScannedCallback);
            }
            else
            {
                var uri = Uri.Parse(folder) ?? throw new ArgumentNullException(nameof(folder), "folder not correct");
                ScanUseDocumentFile(uri, scanOption, onFileScannedCallback);
            }
        }
        public static void ScanUseDocumentFile(Uri folder, FileScanOption? scanOption, Action<FileResult>? onFileScannedCallback)
        {            
            using var folderDoc = DocumentFile.FromTreeUri(Context, folder);            
            if (folderDoc == null)
            {
                return;
            }

            Func<string, bool>? expression = null;
            expression = scanOption?.FileNameExpression?.Compile();
            ScanUseDocumentFile(folderDoc,scanOption, onFileScannedCallback);
            void ScanUseDocumentFile(DocumentFile folder, FileScanOption? scanOption, Action<FileResult>? onFileScannedCallback)
            {
                var files = folder.ListFiles();
                foreach (var file in files)
                {
                    using(file)
                    if (file.IsDirectory)
                    {
                        ScanUseDocumentFile(file, scanOption, onFileScannedCallback);
                    }
                    else
                    {
                        var fileLength = file.Length();
                        var modify = StartTime + TimeSpan.FromSeconds(file.LastModified()/1000);
                        if (scanOption != null)
                        {
                            if (scanOption?.Extensions?.Any() ?? false)
                            {
                                var ext = Path.GetExtension(file.Name)?.TrimStart('.');
                                var extensions = scanOption.Extensions.Select(s => s.ToLower());
                                if (!extensions.Contains(ext))
                                {
                                    continue;
                                }
                            }
                            if (expression != null && !expression.Invoke(file.Name ?? string.Empty))
                            {
                                continue;
                            }
                            if (fileLength < scanOption!.MinFileSize || fileLength > scanOption.MaxFileSize)
                            {
                                continue;
                            }
                            if (modify < scanOption.MinModifyTime || modify > scanOption.MaxModifyTime)
                            {
                                continue;
                            }
                        }
                        onFileScannedCallback?.Invoke(new FileResult(
                            file.Name!,
                            Path.Combine(Environment.ExternalStorageDirectory?.AbsolutePath ?? string.Empty, file.Uri.Path!.Split(':').Last()),
                            file.Uri.ToString()!,
                            fileLength,
                            modify,
                            modify));
                    }
                }
            }
        }

        public static void ScanUseMediaStore(FileScanOption? scanOption, Action<FileResult>? onFileScannedCallback)
        {
            var granted = Permissions.CheckStatusAsync<Permissions.StorageRead>().Result;
            if (granted != PermissionStatus.Granted)
            {
                granted =MainThread.InvokeOnMainThreadAsync(()=>Permissions.RequestAsync<Permissions.StorageRead>()).Result;
                if (granted != PermissionStatus.Granted)
                {
                    return;
                }
            }
            bool converted = false;
            string? selection = null;
            List<string>? selectionArgs = null;
            if (scanOption?.FileNameExpression != null)
            {
                converted = ExpressionParser.TryConvert(scanOption.FileNameExpression, out selection, out selectionArgs);
            }
            if (scanOption != null)
            {

                selection = selection is null ? string.Empty : $"{selection} and ";
                selectionArgs ??= new List<string>();
                selection += $"{MediaStore.IMediaColumns.DateAdded} > ? and {MediaStore.IMediaColumns.DateAdded} < ? " +
                    $"and {MediaStore.IMediaColumns.DateModified} > ? and {MediaStore.IMediaColumns.DateModified} < ? " +
                    $"and {MediaStore.IMediaColumns.Size} > ? and {MediaStore.IMediaColumns.Size} < ? ";
                if (scanOption.AndroidMediaStoreQueryAppend != null)
                {
                    selection += scanOption.AndroidMediaStoreQueryAppend;
                }
                selectionArgs.Add(Math.Clamp((long)((scanOption.MinCreateTime - StartTime).TotalSeconds), 0, long.MaxValue).ToString());
                selectionArgs.Add(Math.Clamp((long)((scanOption.MaxCreateTime - StartTime).TotalSeconds), 0, long.MaxValue).ToString());
                selectionArgs.Add(Math.Clamp((long)((scanOption.MinModifyTime - StartTime).TotalSeconds), 0, long.MaxValue).ToString());
                selectionArgs.Add(Math.Clamp((long)((scanOption.MaxModifyTime - StartTime).TotalSeconds), 0, long.MaxValue).ToString());
                selectionArgs.Add(scanOption.MinFileSize.ToString());
                selectionArgs.Add(scanOption.MaxFileSize.ToString());
                if (scanOption.AndroidMediaStoreQueryArgsAppend != null)
                {
                    selectionArgs.AddRange(scanOption.AndroidMediaStoreQueryArgsAppend);
                }
            }
            var projection = new string[]
            {
                Build.VERSION.SdkInt>BuildVersionCodes.Q?MediaStore.IMediaColumns.RelativePath:MediaStore.IMediaColumns.Data,
                MediaStore.IMediaColumns.DateAdded,
                MediaStore.IMediaColumns.DateModified,
                MediaStore.IMediaColumns.DisplayName,
                MediaStore.IMediaColumns.Size,
                IBaseColumns.Id,
            };
            var uri = GetScanUri(scanOption?.FileType ?? FileType.Any);
            if (uri == null)
            {
                return;
            }
            var cursor = ContentResolver?.Query(uri, projection, selection, selectionArgs?.ToArray(), null);
            try
            {
                if (cursor != null && cursor.Count > 0)
                {
                    var idCol = cursor.GetColumnIndex(IBaseColumns.Id);
                    var nameCol = cursor.GetColumnIndex(MediaStore.IMediaColumns.DisplayName);
                    var sizeCol = cursor.GetColumnIndex(MediaStore.IMediaColumns.Size);
                    var addCol = cursor.GetColumnIndex(MediaStore.IMediaColumns.DateAdded);
                    var modifyCol = cursor.GetColumnIndex(MediaStore.IMediaColumns.DateModified);
                    var pathCol = cursor.GetColumnIndex(Build.VERSION.SdkInt > BuildVersionCodes.Q ? MediaStore.IMediaColumns.RelativePath : MediaStore.IMediaColumns.Data);
                    var expression = scanOption?.FileNameExpression?.Compile();
                    while (cursor.MoveToNext())
                    {
                        var id = cursor.GetInt(idCol);
                        var name = cursor.GetString(nameCol);
                        var size = cursor.GetLong(sizeCol);
                        var add = cursor.GetLong(addCol);
                        var modify = cursor.GetLong(modifyCol);
                        var path = cursor.GetString(pathCol);
                        if (scanOption?.Extensions?.Any() ?? false)
                        {
                            var ext = System.IO.Path.GetExtension(name)?.TrimStart('.');
                            var extensions = scanOption.Extensions.Select(s => s.ToLower());
                            if (!extensions.Contains(ext))
                            {
                                continue;
                            }
                        }
                        if (converted || (expression?.Invoke(name ?? string.Empty) ?? true))
                        {
                            System.Diagnostics.Debug.WriteLine($"add time:{add};modify time:{modify}");
                            var contentUri = ContentUris.WithAppendedId(uri, id);
                            onFileScannedCallback?.Invoke(new FileResult(
                                name!,
                                path!,
                                contentUri.ToString()!,
                                size,
                                StartTime + TimeSpan.FromSeconds(add),
                                StartTime + TimeSpan.FromSeconds(modify))
                            {
                                AndroidMediaId = id,
                            });
                        }
                    }
                }
            }
            finally
            {
                cursor?.Close();
                cursor?.Dispose();
            }
        }
        public static Uri? GetScanUri(FileType type)
        {
            bool low29 = Build.VERSION.SdkInt < BuildVersionCodes.Q;
            Uri? uri = type switch
            {
                FileType.Audio => low29 ? MediaStore.Audio.Media.ExternalContentUri : MediaStore.Audio.Media.GetContentUri(MediaStore.VolumeExternal),
                FileType.Video => low29 ? MediaStore.Video.Media.ExternalContentUri : MediaStore.Video.Media.GetContentUri(MediaStore.VolumeExternal),
                FileType.Image => low29 ? MediaStore.Images.Media.ExternalContentUri : MediaStore.Images.Media.GetContentUri(MediaStore.VolumeExternal),
                _ => MediaStore.Files.GetContentUri(low29 ? "external_primary" : MediaStore.VolumeExternalPrimary),
            };
            return uri;
        }
    }
}
