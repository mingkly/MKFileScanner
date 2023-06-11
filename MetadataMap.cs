using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
#nullable enable
namespace MKFileScanner
{
    public class MetadataMap : Dictionary<string, object?>
    {
        public virtual string? Title => GetString();
        public virtual string? Name => GetString();
        public virtual long? Length => GetLong();
        public virtual DateTime CreateTime => GetTime();
        public virtual DateTime ModifyTime => GetTime();


        protected string? GetString([CallerMemberName] string key = "")
        {
            if (TryGetValue(key, out var value))
            {
                if (value is string s)
                {
                    return s;
                }
            }
            return default;
        }
        protected long GetLong([CallerMemberName] string key = "")
        {
            if (TryGetValue(key, out var value))
            {
                if (value is long l)
                {
                    return l;
                }
                else if (long.TryParse(value?.ToString(), out l))
                {
                    return l;
                }
            }
            return default;
        }
        protected int GetInt([CallerMemberName] string key = "")
        {
            if (TryGetValue(key, out var value))
            {
                
                if (value is int l)
                {
                    return l;
                }
                else if (value is uint ui)
                {
                    return (int)ui;
                }
                else if (value is ulong ul)
                {
                    return (int)ul;
                }
                else if (value is long l0)
                {
                    return (int)l0;
                }
                else if (int.TryParse(value?.ToString(), out l))
                {
                    return l;
                }
            }
            return default;
        }
        protected DateTime GetTime([CallerMemberName] string key = "")
        {
            if (TryGetValue(key, out var value))
            {
                if (value is long l)
                {
                    return DateTime.MinValue + TimeSpan.FromMicroseconds(l);
                }
                else if (value is string s)
                {
                    if (DateTime.TryParse(s, out var t))
                    {
                        return t;
                    }
                }
            }
            return default;
        }
    }
    public class AudioMetadataMap : MetadataMap
    {
        public virtual string? Album => GetString();
        public virtual string? AlbumArtist => GetString();
        public virtual string? Artist => GetString();
        public virtual string[]? Artists => Artist?.Split('/', ',', ';', '\\', '|', ':');
        public virtual int Bitrate => GetInt();
        public virtual string? Composer => GetString();
        public virtual string? Genre => GetString();
        public virtual int SampleRate => GetInt();
        public virtual int ChannelCount => GetInt();
        public virtual string? Encoding => GetString();
        public virtual int Year => GetInt();
        public virtual long Duration => GetLong();
    }
    public class ImageMetadataMap : MetadataMap
    {
        public virtual int Height => GetInt();
        public virtual int Width => GetInt();
        public virtual string? Resolution => GetString();
    }
    public class VideoMetadataMap : MetadataMap
    {
        public virtual int Height => GetInt();
        public virtual int Width => GetInt();
        public virtual string? Resolution => GetString();
        public virtual string? Author => GetString();
        public virtual long Duration => GetLong();
        public virtual int FrameRate => GetInt();
    }
}
