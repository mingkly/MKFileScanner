using MKFileScanner;
using Microsoft.Maui.Controls.Compatibility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
#nullable enable
namespace MKFileScanner.Platforms.Windows
{
    internal class MetadataReader
    {
        public static class File
        {
            /// <summary>
            /// The system-provided file system size of the item, in byte
            /// </summary>
            public const string Length = "System.Size";
            public const string FileName = "System.FileName";
            public const string MimeType = "System.MIMEType";
            /// <summary>
            /// The date and time the item was created on the file system where it is currently located.
            /// </summary>
            public const string CreateTime = "System.DateCreated";
            /// <summary>
            /// The date and time of the last modification to the item. The Indexing Service friendly name is 'write'.
            /// </summary>
            public const string ModifyTime = "System.DateModified";
            public const string Title = "System.Title";

            public static string[] Properties = new string[]
            {
                Length, FileName, MimeType, CreateTime, ModifyTime,Title
            };
        }
        public static class Media
        {
            /// <summary>
            /// Represents the actual play time of a media file and is measured in 100ns units, not milliseconds.
            /// </summary>
            public const string Duration = "System.Media.Duration";
            public const string Year = "System.Media.Year";
            public static string[] Properties = new string[]
            {
                Duration, Year,
            };
        }
        public static class Audio
        {
            /// <summary>
            /// Indicates the channel count for the audio file. Possible values are 1 for mono and 2 for stereo.
            /// </summary>
            public const string ChannelCount = "System.Audio.ChannelCount";
            /// <summary>
            /// Indicates the audio compression used on the audio file.
            /// </summary>
            public const string Compression = "System.Audio.Compression";
            /// <summary>
            /// Indicates the average data rate in Hertz (Hz) for the audio file in bits per second.
            /// </summary>
            public const string EncodingBitrate = "System.Audio.EncodingBitrate";
            /// <summary>
            /// ndicates the format of the audio file.
            /// </summary>
            public const string Format = "System.Audio.Format";
            /// <summary>
            /// Indicates whether the audio file had a variable or constant bit rate.
            /// </summary>
            public const string IsVariableBitRate = "System.Audio.IsVariableBitRate";
            public const string PeakValue = "System.Audio.PeakValue";
            /// <summary>
            /// Indicates the sample rate for the audio file in samples per second.
            /// </summary>
            public const string SampleRate = "System.Audio.SampleRate";
            /// <summary>
            /// Indicates the sample size for the audio file in bits per sample.
            /// </summary>
            public const string SampleSize = "System.Audio.SampleSize";
            /// <summary>
            /// Identifies the name of the stream for the audio file.
            /// </summary>
            public const string StreamName = "System.Audio.StreamName";
            /// <summary>
            /// Identifies the stream number of the audio file.
            /// </summary>
            public const string StreamNumber = "System.Audio.StreamNumber";

            public static string[] Properties = new string[]
            {
                ChannelCount,
                EncodingBitrate,
                Format,
                IsVariableBitRate,
                SampleRate,
                SampleSize,
            };
        }
        public static class Music
        {
            public const string AlbumArtist = "System.Music.AlbumArtist";
            /// <summary>
            /// This optional string value allows for overriding the standard sort order of System.Music.AlbumArtist.This is very important for proper sorting of music files in Japanese which cannot becorrectly sorted phonetically (the user-expected ordering) without this field.It can also be used for customizing sorting in non-East Asian scenarios,such as allowing the user to remove articles for sorting purposes.
            /// </summary>
            public const string AlbumArtistSortOverride = "System.Music.AlbumArtistSortOverride";
            /// <summary>
            /// This property differentiates albums with the same title from different artists. It is the concatenation of System.Music.AlbumArtist and System.Music.AlbumTitle.
            /// </summary>
            public const string AlbumID = "System.Music.AlbumID";
            /// <summary>
            /// 
            /// </summary>
            public const string AlbumTitle = "System.Music.AlbumTitle";
            /// <summary>
            /// This optional string value allows for overriding the standard sort order of System.Music.Album.This is very important for proper sorting of music files in Japanese which cannot becorrectly sorted phonetically (the user-expected ordering) without this field.It can also be used for customizing sorting in non-East Asian scenarios,such as allowing the user to remove articles for sorting purposes.
            /// </summary>
            public const string AlbumTitleSortOverride = "System.Music.AlbumTitleSortOverride";
            public const string Artist = "System.Music.Artist";
            /// <summary>
            /// This optional string value allows for overriding the standard sort order of System.Music.Artist.This is very important for proper sorting of music files in Japanese which cannot becorrectly sorted phonetically (the user-expected ordering) without this field.It can also be used for customizing sorting in non-East Asian scenarios,such as allowing the user to remove articles for sorting purposes.
            /// </summary>
            public const string ArtistSortOverride = "System.Music.ArtistSortOverride";
            public const string BeatsPerMinute = "System.Music.BeatsPerMinute";
            public const string Composer = "System.Music.Composer";
            /// <summary>
            /// This optional string value allows for overriding the standard sort order of System.Music.Composer.This is very important for proper sorting of music files in Japanese which cannot becorrectly sorted phonetically (the user-expected ordering) without this field.It can also be used for customizing sorting in non-East Asian scenarios,such as allowing the user to remove articles for sorting purposes.
            /// </summary>
            public const string ComposerSortOverride = "System.Music.ComposerSortOverride";
            public const string Conductor = "System.Music.Conductor";
            public const string ContentGroupDescription = "System.Music.ContentGroupDescription";
            public const string DiscNumber = "System.Music.DiscNumber";
            /// <summary>
            /// This property returns the best representation of the album artist for a specific music file based upon System.Music.AlbumArtist, System.Music.Artist, and System.Music.IsCompilation information.
            /// </summary>
            public const string DisplayArtist = "System.Music.DisplayArtist";
            public const string Genre = "System.Music.Genre";
            public const string InitialKey = "System.Music.InitialKey";
            public const string IsCompilation = "System.Music.IsCompilation";
            public const string Lyrics = "System.Music.Lyrics";
            public const string Mood = "System.Music.Mood";
            public const string PartOfSet = "System.Music.PartOfSet";
            public const string Period = "System.Music.Period";
            public const string SynchronizedLyrics = "System.Music.SynchronizedLyrics";
            public const string TrackNumber = "System.Music.TrackNumber";

            public static string[] Properties = new string[]
            {
                AlbumArtist,
                AlbumTitle,
                Composer,
                Conductor,
                DisplayArtist,
                Genre,
                Media.Year,
                Media.Duration,
                Audio.ChannelCount,
                Audio.EncodingBitrate,
                Audio.Format,
                Audio.IsVariableBitRate,
                Audio.SampleRate,
                Audio.SampleSize,
            };
        }
        public static class Image
        {
            /// <summary>
            /// Indicates how many bits are used in each pixel of the image. (Usually 8, 16, 24, or 32).
            /// </summary>
            public const string BitDepth = "System.Image.BitDepth";
            /// <summary>
            /// The colorspace embedded in the image. Taken from the Exchangeable Image File (EXIF) information.
            /// </summary>
            public const string ColorSpace = "System.Image.ColorSpace";
            /// <summary>
            /// Indicates the image compression level.
            /// </summary>
            public const string CompressedBitsPerPixel = "System.Image.CompressedBitsPerPixel";
            /// <summary>
            /// The denominator of PKEY_Image_CompressedBitsPerPixel.
            /// </summary>
            public const string CompressedBitsPerPixelDenominator = "System.Image.CompressedBitsPerPixelDenominator";
            /// <summary>
            /// The numerator of PKEY_Image_CompressedBitsPerPixel.
            /// </summary>
            public const string CompressedBitsPerPixelNumerator = "System.Image.CompressedBitsPerPixelNumerator";
            /// <summary>
            /// The algorithm used to compress the image.
            /// </summary>
            public const string Compression = "System.Image.Compression";
            /// <summary>
            /// The user-friendly form of System.Image.Compression. Not intended to be parsed programmatically.
            /// </summary>
            public const string CompressionText = "System.Image.CompressionText";
            /// <summary>
            /// The image dimensions in string format as horizontal pixels x vertical pixels. For example, 3080x2100.
            /// </summary>
            public const string Dimensions = "System.Image.Dimensions";
            /// <summary>
            /// Indicates the number of pixels per resolution unit in the image width.
            /// </summary>
            public const string HorizontalResolution = "System.Image.HorizontalResolution";
            /// <summary>
            /// The horizontal size of the image, in pixels.
            /// </summary>
            public const string HorizontalSize = "System.Image.HorizontalSize";
            /// <summary>
            /// 
            /// </summary>
            public const string ImageID = "System.Image.ImageID";
            /// <summary>
            /// Indicates the resolution units. Used for images with a non-square aspect ratio, but without meaningful absolute dimensions. 1 = No absolute unit of measurement. 2 = Inches. 3 = Centimeters. The default value is 2 (Inches).
            /// </summary>
            public const string ResolutionUnit = "System.Image.ResolutionUnit";
            /// <summary>
            /// Indicates the number of pixels per resolution unit in the image height.
            /// </summary>
            public const string VerticalResolution = "System.Image.VerticalResolution";
            /// <summary>
            /// The vertical size of the image, in pixels.
            /// </summary>
            public const string VerticalSize = "System.Image.VerticalSize";
            public static string[] Properties = new string[]
            {
                Dimensions,
                HorizontalResolution,
                VerticalResolution,
            };
        }
        public static class Video
        {
            /// <summary>
            /// Specifies the video compression format.
            /// </summary>
            public const string Compression = "System.Video.Compression";
            /// <summary>
            /// Indicates the person who directed the video.
            /// </summary>
            public const string Director = "System.Video.Director";
            /// <summary>
            /// Indicates the data rate in "bits per second" for the video stream. "DataRate".
            /// </summary>
            public const string EncodingBitrate = "System.Video.EncodingBitrate";
            /// <summary>
            /// Specifies the FOURCC code for the video stream.
            /// </summary>
            public const string FourCC = "System.Video.FourCC";
            /// <summary>
            /// Indicates the frame height for the video stream.
            /// </summary>
            public const string FrameHeight = "System.Video.FrameHeight";
            /// <summary>
            /// Indicates the frame rate for the video stream, in frames per 1000 seconds.
            /// </summary>
            public const string FrameRate = "System.Video.FrameRate";
            /// <summary>
            /// Indicates the frame width for the video stream.
            /// </summary>
            public const string FrameWidth = "System.Video.FrameWidth";
            /// <summary>
            /// Indicates the horizontal portion of the pixel aspect ratio. The X portion of XX:YY. For example, 10 is the X portion of 10:11.
            /// </summary>
            public const string HorizontalAspectRatio = "System.Video.HorizontalAspectRatio";
            /// <summary>
            /// Indicates whether the media file has a spherical video stream.
            /// </summary>
            public const string IsSpherical = "System.Video.IsSpherical";
            /// <summary>
            /// 
            /// </summary>
            public const string IsStereo = "System.Video.IsStereo";
            /// <summary>
            /// This is the video orientation in degrees.
            /// </summary>
            public const string Orientation = "System.Video.Orientation";
            /// <summary>
            /// Indicates the sample size in bits for the video stream. "SampleSize".
            /// </summary>
            public const string SampleSize = "System.Video.SampleSize";
            /// <summary>
            /// Indicates the name for the video stream. "StreamName".
            /// </summary>
            public const string StreamName = "System.Video.StreamName";
            /// <summary>
            /// Indicates the ordinal number of the stream being played.
            /// </summary>
            public const string StreamNumber = "System.Video.StreamNumber";
            /// <summary>
            /// Indicates the total data rate in "bits per second" for all video and audio streams.
            /// </summary>
            public const string TotalBitrate = "System.Video.TotalBitrate";
            /// <summary>
            /// Indicates the vertical portion of the aspect ratio.
            /// </summary>
            public const string TranscodedForSync = "System.Video.TranscodedForSync";
            /// <summary>
            /// Indicates the horizontal portion of the pixel aspect ratio. The Y portion of XX:YY. For example, 11 is the Y portion of 10:11 .
            /// </summary>
            public const string VerticalAspectRatio = "System.Video.VerticalAspectRatio";
            public static string[] Properties = new string[]
            {
                Director,
                EncodingBitrate,
                FrameHeight,
                FrameRate,
                FrameWidth,
                HorizontalAspectRatio,
                IsStereo,
                Orientation,
                Media.Year,
                Media.Duration,
            };
        }
        public static string? MapKey(string property)
        {
            return property switch
            {
                File.Title=>nameof(AudioMetadataMap.Title),
                File.Length=>nameof(MetadataMap.Length),
                File.FileName=> nameof(MetadataMap.Name),
                File.ModifyTime => nameof(MetadataMap.ModifyTime),
                File.CreateTime => nameof(MetadataMap.CreateTime),
                Media.Duration=>nameof(AudioMetadataMap.Duration),
                Media.Year=>nameof(AudioMetadataMap.Year),
                Audio.ChannelCount => nameof(AudioMetadataMap.ChannelCount),
                Audio.EncodingBitrate => nameof(AudioMetadataMap.Bitrate),
                Audio.SampleRate => nameof(AudioMetadataMap.SampleRate),
                Music.AlbumArtist => nameof(AudioMetadataMap.AlbumArtist),
                Music.AlbumTitle=> nameof(AudioMetadataMap.Album),
                Music.Composer=> nameof(AudioMetadataMap.Composer),
                Music.Genre=> nameof(AudioMetadataMap.Genre),
                Music.DisplayArtist=> nameof(AudioMetadataMap.Artist),
                Image.Dimensions=>nameof(VideoMetadataMap.Resolution),
                Image.HorizontalResolution or Video.FrameWidth=>nameof(VideoMetadataMap.Width),
                Image.VerticalResolution or Video.FrameHeight => nameof(VideoMetadataMap.Height),
                Video.FrameRate=> nameof(VideoMetadataMap.FrameRate),
                Video.Director=> nameof(VideoMetadataMap.Author),
                _ =>null
            };
        }


        public static async Task<MetadataMap?> ReadMetadata(string path)
        {
            var file=await StorageFile.GetFileFromPathAsync(path);          
            var type = file.ContentType;
            IDictionary<string, object> properties;
            IDictionary<string, object>? properties2=null;
            var ps = await file.GetBasicPropertiesAsync();
            properties = await ps.RetrievePropertiesAsync(File.Properties);
            MetadataMap map;
            if (type == null)
            {
                map=new MetadataMap();
            }
            else if (type.StartsWith("audio"))
            {
                map=new AudioMetadataMap();
                properties2 = await (await file.Properties.GetMusicPropertiesAsync()).RetrievePropertiesAsync(Music.Properties);
            }
            else if (type.StartsWith("video"))
            {
                map=new VideoMetadataMap();
                properties2 = await (await file.Properties.GetVideoPropertiesAsync()).RetrievePropertiesAsync(Video.Properties);
            }
            else if (type.StartsWith("image"))
            {
                map=new ImageMetadataMap();
                properties2 = await (await file.Properties.GetImagePropertiesAsync()).RetrievePropertiesAsync(Image.Properties);
            }
            else
            {
                map = new MetadataMap();
            }
            foreach(var property in properties)
            {
                var key=MapKey(property.Key);
                if (key != null)
                {
                    map.TryAdd(key, property.Value);
                }
            }
            if (properties2 != null)
            {
                foreach (var property in properties2)
                {
                    var key = MapKey(property.Key);
                    if (key != null)
                    {
                        if (property.Key == Video.FrameRate)
                        {
                            map.TryAdd(key, (int)property.Value/1000);
                        }
                        else if (property.Key == Media.Duration)
                        {
                            map.TryAdd(key, (ulong)property.Value / 10000);
                        }
                        else
                        {
                            map.TryAdd(key, property.Value);
                        }
                        
                    }
                }
            }
            return map;
        }
    }
}
