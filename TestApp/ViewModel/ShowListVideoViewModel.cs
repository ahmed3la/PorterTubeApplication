using Porter.Entity;
using Porter.Model.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using TestApp;
using YoutubeExtractor;

namespace PorterTube.ViewModel
{
    class ShowListVideoViewModel : INotifyPropertyChanged
    {

        #region '======Property======'
        private bool canSelected;

        public bool CanSelected
        {
            get
            {
                return !isCustome;
            }
            set { canSelected = !isCustome; RaisePropertyChanged("CanSelected"); }
        }

        private bool isCustome;
        public bool IsNotCustome
        {
            get
            {
                hasCustome(isCustome);
                return isCustome;
            }
            set { isCustome = value; RaisePropertyChanged("IsCustome"); }
        }


        public ObservableCollection<Porter.Entity.VideoDetails> VideoDetails { get; set; }

        private bool isAll;
        public bool IsAll
        {
            get { return isAll; }
            set
            {
                isAll = value;
                VideoDetails.ToList().ForEach(a => a.IsActive = value);
            }
        }


        private int heightRowCustom;

        public int HeightRowCustom
        {
            get { return heightRowCustom; }
            set { heightRowCustom = value; RaisePropertyChanged("HeightRowCustom"); }
        }
        public MyICommand<string> CommandGetList { get; private set; }
        public MyICommand<string> CommandCancel { get; private set; }

        #endregion


        #region '======Methods======'
        void OnGetList(string x)
        {
            if (Closed != null)
            {
                var list = VideoDetails.Where(a => a.IsActive).ToList();

                VideoDetails = new ObservableCollection<VideoDetails>(list);

                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                    VideoDetails.ToList().ForEach(a => a.VideoPath = dialog.SelectedPath + "\\");
                }

                Closed(VideoDetails);
            }
        }
        public ShowListVideoViewModel(string url)
        {
            VideoDetails = new ObservableCollection<Porter.Entity.VideoDetails>();
            CommandGetList = new MyICommand<string>(OnGetList);
            CommandCancel = new MyICommand<string>(OnCancel);

            OnDownload(url);
            IsNotCustome = true;
        }

        private void hasCustome(bool custome)
        {
            HeightRowCustom = custome ? 160 : 0;
            CanSelected = !custome;

            if (!custome)
            {
                Thread thread = new Thread(() =>
                {
                    VideoDetails.ToList().ForEach(a =>
                    {
                        if (a.VideoExtensionType.Count == 0)
                        {
                            var l = getVideoExtensionType(a.Url).ToList();
                            l.ForEach(q =>
                            {
                                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                                {
                                    a.VideoExtensionType.Add(q);
                                });
                            });

                        }

                    });
                })
                {
                    IsBackground = true
                };

                thread.Start();
            }

        }

        private void OnCancel(string url)
        {
            if (Closed != null)
            {
                VideoDetails = new ObservableCollection<VideoDetails>();

                Closed(VideoDetails);
            }
        }

        private void OnDownload(string destination)
        {
            string listName = "";
            string videoName = "";


            var type = GetUrlType(destination);
            if (type != null)
            {
                var l = type.FirstOrDefault(a => a.Key == UrlType.List);
                if (l.Value != null)
                    listName = l.Value;

                var v = type.FirstOrDefault(a => a.Key == UrlType.video);
                if (v.Value != null)
                    videoName = v.Value;


                listName = type[UrlType.List];
                videoName = type[UrlType.video];
            }

            if (!string.IsNullOrWhiteSpace(listName))
                fillList(listName);
            else
            {

            }


        }
        Dictionary<UrlType, string> GetUrlType(string _fullUrl)
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
                        UrlType.Add(Porter.Entity.UrlType.video, item.Replace("v=", ""));
                }
            }
            else
            {
                var x = _fullUrl.Split('?')[1];
                var z = x.Split('&').ToList<String>();

                foreach (string item in z)
                {
                    if (item.StartsWith("v="))
                        UrlType.Add(Porter.Entity.UrlType.video, item.Replace("v=", ""));
                }
            }
            return UrlType;
        }

        void fillList(string listName)
        {
            using (Porter.Model.FetchVideoURL cls = new Porter.Model.FetchVideoURL())
            {
                var x = cls.GetList(listName);
                x.ToList().ForEach(a =>
                {
                    VideoDetails.Add(new Porter.Entity.VideoDetails
                    {
                        Url = a.Url,
                        Titel = a.Titel,
                        ImageUrl = a.ImageUrl,
                        ProgressPercentage = 0,
                        VideoID = a.VideoID
                    });


                });
            }
        }


        ObservableCollection<VideoExtensionType> getVideoExtensionType(string url)
        {
            var list = new ObservableCollection<VideoExtensionType>();
            int _index = 0;

            try
            {

                IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(url);
                var videosDistinct = videos.Select(w => new
                {
                    w.VideoType
                    ,
                    w.Title
                    ,
                    w.Resolution
                    ,
                    w.VideoExtension
                })
                    .Distinct().OrderBy(w => w.VideoType).ThenByDescending(w => w.Resolution).ToList();

                videosDistinct.ToList().ForEach(v =>
                {
                    list.Add(new VideoExtensionType()
                    {
                        Index = _index,
                        Resolution = v.Resolution,
                        VideoExtension = v.VideoExtension,
                        VideoType = v.VideoType.ToString(),
                        Titel = "Video Type:" + v.VideoType.ToString() +
                                    new string(' ', 10 - v.VideoType.ToString().Length) +
                                    "Resolution: " + v.Resolution.ToString()
                    });

                    _index++;
                });

            }
            catch (Exception)
            { }

            return list;
        }


        private void customDownloadResolver(ObservableCollection<VideoDetails> list)
        {
            try
            {

                list.ToList().ForEach(a =>
                {
                    int _index = 0;
                    IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(a.Url);

                    videos.ToList().ForEach(v =>
                    {

                        var isExist = a.VideoExtensionType.Any(e => e.Resolution == v.Resolution && e.VideoExtension == v.VideoExtension);
                        if (!isExist)
                        {
                            a.VideoExtensionType.Add(new VideoExtensionType()
                            {
                                Index = _index,
                                Resolution = v.Resolution,
                                VideoExtension = v.VideoExtension,
                                VideoType = v.VideoType.ToString(),
                                Titel = "Video Type:" + v.VideoType.ToString() +
                                    new string(' ', 10 - v.VideoType.ToString().Length) +
                                    "Resolution: " + v.Resolution.ToString()
                            });
                            _index++;
                        }


                    });


                    VideoDetails.Add(a);
                        //a.SVideoExtensionType = a.VideoExtensionType.FirstOrDefault();

                    });

                //VideoDetails = list;
            }
            catch (Exception)
            {
            }

        }



        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion


        #region '======Fields======'
        public event Action<ObservableCollection<Porter.Entity.VideoDetails>> Closed;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


    }
}
