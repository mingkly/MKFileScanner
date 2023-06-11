using Android.Content;
using Android.Media;
using Android.Net;
using MKFileScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uri = Android.Net.Uri;
using Application = Android.App.Application;
using Encoding = Android.Media.Encoding;

namespace LibraryTest.Platforms.Android
{
    internal class AndroidAudioMetadataMap : AudioMetadataMap
    {
        readonly Uri uri;
        public AndroidAudioMetadataMap(Uri uri)
        {
            this.uri = uri;
        }
        bool otherInfoReaded;
        int channelCount;
        int sampleRate;
        string? encoding;
        public override int SampleRate
        {
            get
            {
                ReadOtherInfo();
                return sampleRate;
            }
        }
        public override int ChannelCount
        {
            get
            {
                ReadOtherInfo();
                return channelCount;
            }
        }
        public override string? Encoding
        {
            get
            {
                ReadOtherInfo();
                return encoding;
            }
        }
        void ReadOtherInfo()
        {
            if (otherInfoReaded)
            {
                return;
            }
            try
            {
                using (var mediaExtrctor = new MediaExtractor())
                {
                    mediaExtrctor.SetDataSource(Application.Context, uri, null);
                    mediaExtrctor.SelectTrack(0);
                    var format = mediaExtrctor.GetTrackFormat(0);
                    var mimeType = format.GetString(MediaFormat.KeyMime);
                    if(!(mimeType?.StartsWith("audio")??true))
                    {
                        return;
                    }
                    try
                    {
                        channelCount = format.GetInteger(MediaFormat.KeyChannelCount);
                        sampleRate = format.GetInteger(MediaFormat.KeySampleRate);
                        var pcmEncoding = format.GetInteger(MediaFormat.KeyPcmEncoding);
                        encoding = ((Encoding)pcmEncoding).ToString();
                    }
                    catch
                    {
                        encoding ="Pcm16bit";
                    }
                }
            }
            catch{}
            otherInfoReaded = true;
        }
    }
}
