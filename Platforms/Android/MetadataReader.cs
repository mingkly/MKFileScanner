using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Media;
using Android.Net;
using Android.OS;
using Android.Provider;
using AndroidX.DocumentFile.Provider;
using LibraryTest.Platforms.Android;
using MKFileScanner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Application = Android.App.Application;
using Encoding = Android.Media.Encoding;
using Uri = Android.Net.Uri;
#nullable enable

namespace MKFileScanner.Platforms.Android
{
    internal class MetadataReader
    {
        public static DateTime StartTime = new(1970, 1, 1);
        public static string[] Cols = new string[]
        {
            MediaStore.IMediaColumns.MimeType,
            MediaStore.IMediaColumns.Album,
            MediaStore.IMediaColumns.AlbumArtist,
            MediaStore.IMediaColumns.Artist,
            MediaStore.IMediaColumns.Composer,
            MediaStore.IMediaColumns.Duration,
            MediaStore.IMediaColumns.Genre,
            MediaStore.IMediaColumns.Title,
            MediaStore.IMediaColumns.Year,
            MediaStore.IMediaColumns.DisplayName,
            MediaStore.IMediaColumns.DateAdded,
            MediaStore.IMediaColumns.Bitrate,
            MediaStore.IMediaColumns.DateModified,
            MediaStore.IMediaColumns.Size,
            MediaStore.IMediaColumns.Width,
            MediaStore.IMediaColumns.Height,
            MediaStore.IMediaColumns.Author,
            MediaStore.IMediaColumns.Resolution,
        };
        public static string[] ColsUnder29 = new string[]
        {
            MediaStore.IMediaColumns.MimeType,
            MediaStore.IMediaColumns.DisplayName,
            MediaStore.IMediaColumns.DateAdded,
            MediaStore.IMediaColumns.DateModified,
            MediaStore.IMediaColumns.Size,
        };
        public static MetadataKey[] AudioMetadataKeys = new MetadataKey[]
        {
            MetadataKey.Album,
            MetadataKey.Artist,
            MetadataKey.Composer,
            MetadataKey.Duration,
            MetadataKey.Genre,
            MetadataKey.Title,
            MetadataKey.Albumartist,
            MetadataKey.Year,
            MetadataKey.Bitrate,
            MetadataKey.SampleRate,
           
        };
        public static MetadataKey[] ImageMetadataKeys = new MetadataKey[]
        {
            MetadataKey.Author,
            MetadataKey.ImageWidth,
            MetadataKey.ImageHeight,
            MetadataKey.Title,
        };
        public static MetadataKey[] VideoMetadataKeys = new MetadataKey[]
        {
            MetadataKey.Author,
            MetadataKey.VideoWidth,
            MetadataKey.VideoHeight,
            MetadataKey.Title,
            MetadataKey.CaptureFramerate,
            MetadataKey.Duration,
        };

        public static string[] ExifTags = new string[]
        {
            ExifInterface.TagBrightnessValue,
            ExifInterface.TagColorSpace,
            ExifInterface.TagImageWidth,
            ExifInterface.TagImageLength,
        };
        public static string MapTags(string tag)
        {
            return tag switch
            {
                ExifInterface.TagImageLength => nameof(ImageMetadataMap.Height),
                ExifInterface.TagImageWidth => nameof(ImageMetadataMap.Width),
                ExifInterface.TagColorSpace => ExifInterface.TagColorSpace,
                _ => tag
            };
        }
        public static string? MapColumns(string columns)
        {
            return columns switch
            {
                MediaStore.IMediaColumns.Album => nameof(AudioMetadataMap.Album),
                MediaStore.IMediaColumns.AlbumArtist => nameof(AudioMetadataMap.AlbumArtist),
                MediaStore.IMediaColumns.Artist => nameof(AudioMetadataMap.Artist),
                MediaStore.IMediaColumns.Composer => nameof(AudioMetadataMap.Composer),
                MediaStore.IMediaColumns.Year=> nameof(AudioMetadataMap.Year),
                MediaStore.IMediaColumns.Genre => nameof(AudioMetadataMap.Genre),
                MediaStore.IMediaColumns.Bitrate => nameof(AudioMetadataMap.Bitrate),
                MediaStore.IMediaColumns.Size => nameof(AudioMetadataMap.Length),
                MediaStore.IMediaColumns.Duration => nameof(AudioMetadataMap.Duration),
                MediaStore.IMediaColumns.Title => nameof(AudioMetadataMap.Title),
                MediaStore.IMediaColumns.DisplayName=>nameof(AudioMetadataMap.Name),
                MediaStore.IMediaColumns.DateAdded=>nameof(AudioMetadataMap.CreateTime),
                MediaStore.IMediaColumns.DateModified=>nameof(AudioMetadataMap.ModifyTime),
                MediaStore.IMediaColumns.Width=>nameof(VideoMetadataMap.Width),
                MediaStore.IMediaColumns.Height=>nameof(VideoMetadataMap.Height),
                MediaStore.IMediaColumns.Resolution=> nameof(VideoMetadataMap.Resolution),
                MediaStore.IMediaColumns.CaptureFramerate=>nameof(VideoMetadataMap.FrameRate),
                MediaStore.IMediaColumns.Author=>nameof(VideoMetadataMap.Author),
                _ => columns,
            };
        }
        public static string? MapColumns(MetadataKey columns)
        {
            return columns switch
            {
                MetadataKey.Album => nameof(AudioMetadataMap.Album),
                MetadataKey.Albumartist => nameof(AudioMetadataMap.AlbumArtist),
                MetadataKey.Artist => nameof(AudioMetadataMap.Artist),
                MetadataKey.Composer => nameof(AudioMetadataMap.Composer),
                MetadataKey.Year => nameof(AudioMetadataMap.Year),
                MetadataKey.Genre => nameof(AudioMetadataMap.Genre),
                MetadataKey.Bitrate => nameof(AudioMetadataMap.Bitrate),
                MetadataKey.Duration => nameof(AudioMetadataMap.Duration),
                MetadataKey.Title => nameof(AudioMetadataMap.Title),
                MetadataKey.ImageWidth or MetadataKey.VideoWidth => nameof(VideoMetadataMap.Width),
                MetadataKey.ImageHeight or MetadataKey.VideoHeight => nameof(VideoMetadataMap.Height),
                MetadataKey.CaptureFramerate => nameof(VideoMetadataMap.FrameRate),
                MetadataKey.Author => nameof(VideoMetadataMap.Author),
                _ => null,
            };
        }
        static Context Context=>Application.Context;
        public static MetadataMap? Read(string path)
        {
            var uri=Uri.Parse(path);
            if(uri != null)
            {
                if(DocumentsContract.IsDocumentUri(Context, uri)||Build.VERSION.SdkInt<BuildVersionCodes.Q)
                {
                    return UseRetriver(uri);
                }             
                else
                {
                    return UseQuery(uri);
                }
            }
            return null;
        }
        static MetadataMap? UseQuery(Uri uri)
        {
            using var cursor = Context.ContentResolver?.Query(uri, Cols, null, null, null);
            if(cursor != null&&cursor.MoveToFirst())
            {
                int[] indexes=Cols.Select(c=>cursor.GetColumnIndex(c)).ToArray();
                var type = cursor.GetString(indexes[0]);               
                MetadataMap map;
                if(type == null)
                {
                    map = new MetadataMap();
                }
                else if (type.StartsWith("audio"))
                {
                    map = new AndroidAudioMetadataMap(uri);
                }
                else if(type.StartsWith("video"))
                {
                    map = new VideoMetadataMap();
                }
                else if (type.StartsWith("image"))
                {
                    map = new ImageMetadataMap();
                }
                else
                {
                    map=new MetadataMap();
                }
                for(int i=0; i<indexes.Length; i++)
                {                    
                    if (indexes[i] < 0)
                    {
                        continue;
                    }
                    if (Cols[i]==MediaStore.IMediaColumns.DateAdded|| Cols[i] == MediaStore.IMediaColumns.DateModified)
                    {
                        var time = cursor.GetLong(indexes[i]);
                        var value=StartTime+TimeSpan.FromSeconds(time);
                        var key = MapColumns(Cols[i]);
                        if(key != null)
                        {
                            map.TryAdd(key, value);
                        }                     
                    }
                    else
                    {
                        var value = cursor.GetString(indexes[i]);
                        var key = MapColumns(Cols[i]);
                        if (key != null)
                        {
                            map.TryAdd(key, value);
                        }
                    }                                      
                }
                return map;
            }
            return null;
        }
        static MetadataMap? UseQueryUnder29(Uri uri)
        {
            using var cursor = Context.ContentResolver?.Query(uri, ColsUnder29, null, null, null);
            if (cursor != null && cursor.MoveToFirst())
            {
                int[] indexes = ColsUnder29.Select(c => cursor.GetColumnIndex(c)).ToArray();
                var type = cursor.GetString(indexes[0]);
                MetadataMap map;
                if (type == null)
                {
                    map = new MetadataMap();
                }
                else if (type.StartsWith("audio"))
                {
                    map = new AndroidAudioMetadataMap(uri);
                }
                else if (type.StartsWith("video"))
                {
                    map = new VideoMetadataMap();
                }
                else if (type.StartsWith("image"))
                {
                    map = new ImageMetadataMap();
                }
                else
                {
                    map = new MetadataMap();
                }
                for (int i = 0; i < indexes.Length; i++)
                {
                    if (indexes[i] < 0)
                    {
                        continue;
                    }
                    if (ColsUnder29[i] == MediaStore.IMediaColumns.DateAdded || ColsUnder29[i] == MediaStore.IMediaColumns.DateModified)
                    {
                        var time = cursor.GetLong(indexes[i]);
                        var value = StartTime + TimeSpan.FromSeconds(time);
                        var key = MapColumns(ColsUnder29[i]);
                        if (key != null)
                        {
                            map.TryAdd(key, value);
                        }
                    }
                    else
                    {
                        var value = cursor.GetString(indexes[i]);
                        var key = MapColumns(ColsUnder29[i]);
                        if (key != null)
                        {
                            map.TryAdd(key, value);
                        }
                    }
                }
                return map;
            }
            return null;
        }
        static MetadataMap? UseRetriver(Uri uri)
        {
            DocumentFile? file=null;
            if(DocumentsContract.IsDocumentUri(Context, uri))
            {
                file=DocumentFile.FromSingleUri(Context, uri);
            }
            else
            {
                var map=UseQueryUnder29(uri);
                if(map != null)
                {
                    if(map is AudioMetadataMap)
                    {
                        ReadUseRetriver(AudioMetadataKeys, uri, map);
                    }
                    else if(map is VideoMetadataMap)
                    {
                        ReadUseRetriver(VideoMetadataKeys, uri, map);
                    }
                    else if(map is ImageMetadataMap)
                    {
                        var map0 = new ImageMetadataMap();
                        map = map0;
                        ReadUseExif(uri, map0);
                    }
                    return map;
                }
            }
            if(file != null)
            {
                try
                {
                    MetadataMap map;
                    var type = file.Type;
                    if (type == null)
                    {
                        map = new MetadataMap();
                    }
                    else if (type.StartsWith("audio"))
                    {
                        map = new AudioMetadataMap();
                        ReadUseRetriver(AudioMetadataKeys, uri, map);
                    }
                    else if (type.StartsWith("video"))
                    {
                        map = new VideoMetadataMap();
                        ReadUseRetriver(VideoMetadataKeys, uri, map);
                    }
                    else if (type.StartsWith("image"))
                    {
                        var map0= new ImageMetadataMap();
                        map = map0;
                        ReadUseExif(uri, map0);
                    }
                    else
                    {
                        map = new MetadataMap();
                    }
                    ReadUseDocument(file, map);                   
                    return map;
                }
                finally
                {
                    file?.Dispose();
                }
            }
            return null;
        }
        static void ReadUseDocument(DocumentFile file,MetadataMap map)
        {
            map.Add(nameof(MetadataMap.Name), file.Name ?? string.Empty);
            map.Add(nameof(MetadataMap.Length), file.Length());
            map.Add(nameof(MetadataMap.CreateTime), StartTime + TimeSpan.FromMilliseconds(file.LastModified()));
            map.Add(nameof(MetadataMap.ModifyTime), StartTime + TimeSpan.FromMilliseconds(file.LastModified()));
        }
        static void ReadUseRetriver(MetadataKey[]? keys, Uri uri,MetadataMap map)
        {
            if (keys != null)
            {
                try
                {
                    using var metadataRetriver = new MediaMetadataRetriever();
                    metadataRetriver.SetDataSource(Context, uri);
                    foreach (var key in keys)
                    {
                        var value = metadataRetriver.ExtractMetadata(key);
                        var key0 = MapColumns(key);
                        if (key0 != null)
                        {
                            map.TryAdd(key0, value);
                        }
                    }
                }
                catch
                {

                }
            }
        }
        static void ReadUseExif(Uri uri,ImageMetadataMap map)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.N)
            {
                return;
            }
            using var stream=Context.ContentResolver?.OpenInputStream(uri);
            if(stream == null)
            {
                return;
            }
#pragma warning disable CA1416 // Validate platform compatibility
            ExifInterface exif=new ExifInterface(stream);
#pragma warning restore CA1416 // Validate platform compatibility
            foreach(var tag in ExifTags)
            {
                var key=MapTags(tag);
                if (tag == ExifInterface.TagBrightnessValue)
                {
                    map.TryAdd(key, exif.GetAttributeDouble(tag, 0));
                }
                else
                {
                    map.TryAdd(key, exif.GetAttributeInt(tag, 0));
                }
                
            }
        }
    }
}
