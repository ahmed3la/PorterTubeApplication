using Newtonsoft.Json;
using Porter.Entity;
using Porter.Model;
using Porter.Model.Command;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using YoutubeExtractor;

namespace PorterTube
{
    public class VideoDetailsViewModel : INotifyPropertyChanged
    {


        #region '======Property======'
        private Visibility windowVisibility;
        public Visibility WindowVisibility
        {
            get { return windowVisibility; }
            set
            {
                windowVisibility = value;
                RaisePropertyChanged("WindowVisibility");
            }
        }

        private ObservableCollection<Porter.Entity.VideoDetails> videoDetails;

        public ObservableCollection<Porter.Entity.VideoDetails> VideoDetails
        {
            get
            {
                if (videoDetails.Count == 0)
                {
                    var logList = Loging.Get();
                    if (logList != null)
                        videoDetails = new ObservableCollection<Porter.Entity.VideoDetails>(Loging.Get());
                }

                return videoDetails;
            }
            set
            {
                videoDetails = value;
                RaisePropertyChanged("VideoDetails");
            }
        }

        public RelayCommand PlayCommand { get; private set; }
        public RelayCommand DownloadOneCommand { get; private set; }
        public MyICommand<string> DownloadCommand { get; private set; }
        public MyICommand<string> DownloadPush { get; private set; }
        public MyICommand<string> ActiveChecks { get; private set; }
        public MyICommand<string> OpenBrowser { get; private set; }
        public MyICommand<string> RemoveItem { get; private set; }

        public RelayCommand DownloadByBrowserCommand { get; set; }
        public RelayCommand ShowinFolderCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }
        public RelayCommand RemoveAllCommand { get; set; }


        private bool isDownloadAll;
        public bool IsDownloadAll
        {
            get { return isDownloadAll; }
            set { isDownloadAll = value; RaisePropertyChanged("IsDownloadAll"); }
        }
        private string fullUrl;
        public string FullUrl
        {
            get
            {
                return fullUrl;
            }
            set
            {
                if (fullUrl != value)
                {
                    fullUrl = value;
                    RaisePropertyChanged("Titel");
                }
            }
        }

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

        #endregion


        #region '======Methods======'
        private void OnDownload(string s)
        {
             

            var childWindow = new PorterTube.ChildWindowView.ChildWindowView();
            childWindow.Closed += (list =>
            {
                customDownloadResolver(list);
                if (list.Count > 0)
                {

                    for (int i = VideoDetails.Count - 1; i >= 0; i--)
                    {
                        var videoID = VideoDetails[i].VideoID;
                        bool any = list.ToList().Any(l => l.VideoID == videoID);
                        if (any)
                            VideoDetails.RemoveAt(i);
                    }

                    list.ToList().ForEach(a =>
                    VideoDetails.Add(a));
                }

                Loging.SaveList(list, true);
            });
            childWindow.Show(FullUrl);
        }




        private void customDownloadResolver(ObservableCollection<VideoDetails> list)
        {
            try
            {
                list.ToList().ForEach(a =>
                {
                    a.VideoExtensionType = null;
                    VideoDetails.Add(a);
                });
            }
            catch (Exception)
            { }
        }




        public VideoDetailsViewModel()
        {
            

            //NavCommand = new MyICommand<string>(OnNav);
            DownloadCommand = new MyICommand<string>(OnDownload);
            DownloadPush = new MyICommand<string>(OnDownloadPush);
            OpenBrowser = new MyICommand<string>(OnBrowser);
            
            configRelayCommand_Browser();
            configShowinFolderCommand();
            configRemoveCommand();
            configRemoveAllCommand();

            initializeListVideoDetails();
            configDownloadOneCommand();
            configPlayCommand();

            
        }

        private void configPlayCommand()
        {
            PlayCommand = new RelayCommand(o =>
            {
                var video = ((Porter.Entity.VideoDetails)o);
                if (video != null)
                {
                    string argument = "/run, \"" + video.VideoPath + video.Titel + video.SelectedVideoExtensionType.VideoExtension + "\"";
                    System.Diagnostics.Process.Start("explorer.exe", argument);
                }

            }, o => true);
        }
        private void configDownloadOneCommand()
        {
            DownloadOneCommand = new RelayCommand(o =>
            {
                IsDownloadAll = false;

                var video = ((VideoDetails)o);
                if (video != null)
                {
                    using (Porter.Model.FetchVideoURL cls = new Porter.Model.FetchVideoURL())
                    {
                        VideoDownloader(video.Url, video.SelectedVideoExtensionType, video.VideoPath);
                    }
                }


            }, o => true);
        }

        private void configRemoveAllCommand()
        {
            RemoveAllCommand = new RelayCommand(o =>
            {
                if (Loging.RemoveAll())
                {
                    for (int i = VideoDetails.Count - 1; i >= 0; i--)
                    {
                        VideoDetails.RemoveAt(i);
                    }

                }
            }, o => true);
        }


        private void configRemoveCommand()
        {
            RemoveCommand = new RelayCommand(o =>
            {
                if (o != null && !string.IsNullOrEmpty(((Porter.Entity.VideoDetails)o).Url))
                {
                    int index;
                    bool deletedDone = Loging.Remove(VideoDetails, ((Porter.Entity.VideoDetails)o).Url, out index);
                }

            }, o => true);
        }
        private void configShowinFolderCommand()
        {
            ShowinFolderCommand = new RelayCommand(o =>
            {
                var video = ((Porter.Entity.VideoDetails)o);
                string argument = "/select, \"" + video.VideoPath + video.Titel + video.SelectedVideoExtensionType.VideoExtension + "\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);
            }, o => true);
        }

        private void configRelayCommand_Browser()
        {
            //  ActiveChecks = new MyICommand<string>(OnActiveChecks);
            DownloadByBrowserCommand = new RelayCommand(o =>
            {
                Porter.Entity.VideoDetails v = (Porter.Entity.VideoDetails)o;
                OnBrowser(v.VideoID);

            }, o => true);

        }

        private void OnBrowser(string videoId)
        {
            Porter.Entity.VideoDetails videoDetails = new Porter.Entity.VideoDetails();

            videoDetails = VideoDetails.FirstOrDefault(a => a.VideoID == videoId);

            IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(videoDetails.Url);
            //Get url video
            var video = videos.First(p => p.VideoExtension == videoDetails.SelectedVideoExtensionType.VideoExtension
                                                    && p.Resolution == videoDetails.SelectedVideoExtensionType.Resolution);
            if (video != null && !string.IsNullOrWhiteSpace(video.DownloadUrl))
            {
                System.Diagnostics.Process.Start(video.DownloadUrl);
            }
        }

        private void OnDownloadPush(object ModalDialog)
        { 
            IsDownloadAll = true;
            startDownload(5);

        }
        void startDownload(int take)
        {

            var push = VideoDetails.Where(a => a.BeginDownload == false && a.IsActive == true && a.EndDownload == false).Take(take).ToList();

            push.ForEach(a =>
            {
                a.BeginDownload = true;
            });

            push.ForEach(a =>
            {
                VideoDownloader(a.Url, a.SelectedVideoExtensionType, a.VideoPath);
            });


        }
        private void OnNav(string destination)
        {
            IsDownloadAll = false;

            var url = destination;

            using (Porter.Model.FetchVideoURL cls = new Porter.Model.FetchVideoURL())
            {
                var v = VideoDetails.FirstOrDefault(a => a.Url == destination);

                VideoDownloader(v.Url, v.SelectedVideoExtensionType, v.VideoPath);
            }

        }

        private bool removeExistFile(string filePath, string videoExtension = ".MP4")
        {
            bool result = false;
            try
            {
                if (File.Exists(filePath + videoExtension))
                {
                    File.Delete(filePath + videoExtension);
                    result = true;
                }

                if (File.Exists(filePath + ".srt"))
                {
                    File.Delete(filePath + ".srt");
                    result = true;
                }

            }
            catch (Exception)
            {
            }
            return result;
        }




        private void Downloader_DownloadProgressChanged(object sender, ProgressEventArgs e)
        {
            //https://video.google.com/timedtext?lang=en&v=5MgBikgcWnY
            //https://r4---sn-jtcxgp5-g0ie.googlevideo.com/videoplayback?v=GwXnyf6N3sk&signature=7C1F9D7F42857DAD397EC93DE05FC9A28FAA84A2.513E68EDC99EBAD6EE2F46A01EFDCC7770AB5380&initcwndbps=177500&key=yt6&mime=video/mp4&c=WEB&expire=1519910404&ei=pKmXWu_uFsfi1gKH3qbQCQ&lmt=1507227283906759&dur=652.109&itag=18&clen=14976452&gir=yes&ratebypass=yes&sparams=clen,dur,ei,gir,id,initcwndbps,ip,ipbits,itag,lmt,mime,mm,mn,ms,mv,pl,ratebypass,requiressl,source,expire&requiressl=yes&fvip=1&source=youtube&pl=24&id=o-AD56wpv4EjKfi_M-PoSffwhEYduVcqVRw5aVbst4ZSp0&mv=m&mt=1519888664&ms=au,rdu&mn=sn-jtcxgp5-g0ie,sn-5hne6nsy&mm=31,29&ip=212.119.82.100&ipbits=0

            Dispatcher.CurrentDispatcher.Invoke((Action)delegate // <--- HERE
            {

                var videoInfo = ((VideoDownloader)sender).Video;

                var video = VideoDetails.FirstOrDefault(a => a.VideoID == getVideoID(videoInfo));
                if (video != null)
                    video.ProgressPercentage = double.Parse($"{string.Format("{0:0.##}", e.ProgressPercentage)}");
                else
                    e.Cancel = true;
            });
        }
        private string getVideoID(VideoInfo videoInfo)
        {
            return (videoInfo.DownloadUrl.Split('&'))[0].Substring(61);
        }

        private void Downloader_DownloadFinished(object sender, EventArgs e)
        {

            Dispatcher.CurrentDispatcher.Invoke((Action)delegate // <--- HERE
            {
                var videoInfo = ((VideoDownloader)sender).Video;

                var video = VideoDetails.FirstOrDefault(a => a.Titel == videoInfo.Title);
                if (video != null)
                {
                    video.EndDownload = true;

                    using (FetchSubtitelURL sub = new FetchSubtitelURL())
                    {
                        sub.DownloadSubtitel(video);
                    }

                }
                Loging.SaveList(VideoDetails);

                if (IsDownloadAll)
                    startDownload(1);
            });

            //Dispatcher.CurrentDispatcher.Invoke(new MethodInvoker(delegate ()
            //{ 

            //}));


        }

        public VideoDownloader VideoDownloader(string url, VideoExtensionType videoExtensionType, string videoPath = "")
        {

            IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(url);


            //Get url video
            VideoInfo video;
            if (videoExtensionType != null)
                video = videos.First(p => p.VideoExtension == videoExtensionType.VideoExtension
                                                        && p.Resolution == videoExtensionType.Resolution);
            else
                video = videos.FirstOrDefault();

            if (video.RequiresDecryption)
                DownloadUrlResolver.DecryptDownloadUrl(video);

            if (string.IsNullOrWhiteSpace(videoPath))
                videoPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Download\\";


            var titel = Porter.Model.Helper.RemoveSpatialCharacter(video.Title);

            //Download video
            VideoDownloader downloader = new VideoDownloader(video, videoPath + titel + video.VideoExtension);
            removeExistFile(videoPath + titel, video.VideoExtension);

            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            downloader.DownloadFinished += Downloader_DownloadFinished;
            try
            {
                ////Create a new thread to download file
                Thread thread = new Thread(() =>
                {
                    downloader.Execute();

                })
                { IsBackground = true };
                thread.Start();
            }
            catch (Exception)
            {

            }

            return downloader;

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

        //private void xOnDownload(string destination)
        //{
        //    string listName = "";
        //    string videoName = "";


        //    var type = GetUrlType(FullUrl);
        //    if (type != null)
        //    {
        //        var l = type.FirstOrDefault(a => a.Key == UrlType.List);
        //        if (l.Value != null)
        //            listName = l.Value;

        //        var v = type.FirstOrDefault(a => a.Key == UrlType.video);
        //        if (v.Value != null)
        //            videoName = v.Value;


        //        listName = type[UrlType.List];
        //        videoName = type[UrlType.video];
        //    }

        //    if (!string.IsNullOrWhiteSpace(listName))
        //        fillList(listName);
        //    else
        //    {

        //    }


        //}

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
                        VideoID = a.VideoID,
                        ImageUrl = a.ImageUrl,
                        ProgressPercentage = 0
                    });
                });
            }
        }

        public void initializeListVideoDetails()
        {
            VideoDetails = new ObservableCollection<Porter.Entity.VideoDetails>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion


        #region '======Fields======'

        #endregion


    }

}
