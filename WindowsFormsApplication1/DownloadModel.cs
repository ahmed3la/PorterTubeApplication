using Porter.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Porter.Model
{
    public class DownloadModel : IDisposable
    {

        public static bool ValidDownloadAll(ObservableCollection<Porter.Entity.VideoDetails> videoDetails)
        {
            var any = GetListValidVideoDownload(videoDetails).Any();

            return any;
        }

        private static List<Porter.Entity.VideoDetails> GetListValidVideoDownload(ObservableCollection<Porter.Entity.VideoDetails> videoDetails,int take=1)
        {
            var push = videoDetails.Where(a =>
            (a.DownlaodStatus == DownlaodStatus.None || a.DownlaodStatus == DownlaodStatus.None || a.DownlaodStatus == DownlaodStatus.Cancel))//&& a.IsActive == true
                            .Take(take).ToList();

            return push;
        }
        public static List<Porter.Entity.VideoDetails> ConfigBeforeVideoDownload(ObservableCollection<Porter.Entity.VideoDetails> videoDetails,int take)
        {
            var push = GetListValidVideoDownload(videoDetails, take);
            push.ForEach(a =>
            {
                a.DownlaodStatus = DownlaodStatus.Begin;

                var path = a.VideoPath + a.Titel + a.VideoExtensionType;
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

            });

            return push;
        }

        public static List<Porter.Entity.VideoDetails> GetListVideoDownload(ObservableCollection<Porter.Entity.VideoDetails> videoDetails, bool isDownloadAll=false)
        {
            int countThreadStart = 1;
            if (isDownloadAll)
            {
                var Log = ConfigurationManager.AppSettings["CountThreadStart"];
                
                if (!string.IsNullOrEmpty(Log) && !string.IsNullOrWhiteSpace(Log))
                    countThreadStart = int.Parse(Log);
            }
            var list = ConfigBeforeVideoDownload(videoDetails, countThreadStart);

            return list;
        }



        public static Dictionary<UrlType, string> GetUrlType(string _fullUrl)
        {
            Dictionary<UrlType, string> UrlType = new Dictionary<UrlType, string>();


            //  var destination = @"https://www.youtube.com/watch?v=GwXnyf6N3sk&list=PL4cyC4G0M1RQ_Rm52cQ4CcOJ_T_HXeMB4";
            if (_fullUrl.Contains("list="))
            {
                var x = _fullUrl.Split('?')[1];
                var z = x.Split('&').ToList<String>();


                foreach (string item in z)
                {
                    if (item.StartsWith("list="))
                        UrlType.Add(Porter.Entity.UrlType.List, item.Replace("list=", ""));

                    if (item.StartsWith("v="))
                        UrlType.Add(Porter.Entity.UrlType.Video, item.Replace("v=", ""));
                }
            }
            else
            {
                var x = _fullUrl.Split('?')[1];
                var z = x.Split('&').ToList<String>();

                foreach (string item in z)
                {
                    if (item.StartsWith("v="))
                        UrlType.Add(Porter.Entity.UrlType.Video, item.Replace("v=", ""));
                }
            }
            return UrlType;
        }

        public static ObservableCollection<VideoExtensionType> GetListVideoDetailsCustom(ObservableCollection<Porter.Entity.VideoDetails> listVideoDetails)
        {
            var listVideoExtensionTypeResult = new ObservableCollection<VideoExtensionType>();

            var ListIntersect = new List<VideoExtensionType>();
            var currentList = new List<VideoExtensionType>();

            var list = listVideoDetails.Where(a => a.VideoExtensionType.Count > 0).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                if (!ListIntersect.Any())
                {
                    ListIntersect = list[i].VideoExtensionType.Select(a => new VideoExtensionType {
                        Resolution = a.Resolution,
                        Titel = a.Titel,
                        VideoExtension = a.VideoExtension,
                        VideoType = a.VideoType,
                    }).ToList();
                    //break;
                }
                else
                {
                    currentList = list[i].VideoExtensionType.Select(a => new VideoExtensionType
                    {
                        Resolution = a.Resolution,
                        Titel = a.Titel,
                        VideoExtension = a.VideoExtension,
                        VideoType = a.VideoType,
                    }).ToList();
                    ListIntersect = ListIntersect.Intersect(currentList, new VideoExtensionType()).ToList();
                }
                    
            }

            var lll = ListIntersect.Select(a => new VideoExtensionType
            {
                Resolution = a.Resolution,
                Titel = a.Titel,
                VideoExtension = a.VideoExtension,
                VideoType = a.VideoType,
            }).ToList();

            listVideoExtensionTypeResult = new ObservableCollection<VideoExtensionType>(lll);

            return new ObservableCollection<VideoExtensionType>(listVideoExtensionTypeResult);
             
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        ~DownloadModel()
        {
            // Finalizer calls Dispose(false)  
            Dispose();
        }
    }
}
