using Porter.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExtractor;

namespace PorterTube.Helper
{
    public class CustomVideoInfo
    {

        public static ObservableCollection<VideoExtensionType> getVideoExtensionType(string url, bool custome = true)
        {
            var list = new ObservableCollection<VideoExtensionType>();
            //int _index = 0;

            try
            {

                IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(url);
                if (!custome)
                    videos = videos.Where(a => a.VideoType == VideoType.Mp4)
                        .OrderByDescending(a => a.Resolution)
                        .Take(1);

                var videosDistinct = videos.Select(w => new {
                    w.VideoType
                    ,
                    w.Title
                    ,
                    w.Resolution
                    ,
                    w.VideoExtension
                })
                    .Distinct()
                    .OrderBy(w => w.VideoType)
                    .ThenByDescending(w => w.Resolution)
                    .ToList();

                videosDistinct.ToList().ForEach(v =>
                {
                    list.Add(new VideoExtensionType()
                    {
                        //Index = _index,
                        Resolution = v.Resolution,
                        VideoExtension = v.VideoExtension,
                        VideoType = v.VideoType.ToString(),
                        Titel = "Video Type:" + v.VideoType.ToString() +
                                    new string(' ', 10 - v.VideoType.ToString().Length) +
                                    "Resolution: " + v.Resolution.ToString()
                    });

                   // _index++;
                });

            }
            catch (Exception)
            { }

            return list;
        }


    }
}
