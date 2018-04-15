using Newtonsoft.Json;
using Porter.Entity;
using Porter.Model;
using Porter.Model.Command;
using PorterTube.Common;
using PorterTube.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using YoutubeExtractor;

namespace PorterTube
{
    public class VideoDetailsViewModel : INotifyPropertyChanged
    {
        private double progressValue;
        public double ProgressValue
        {
            get
            {
                return progressValue;
            }
            set
            {
                progressValue = value;
                RaisePropertyChanged("ProgressValue");
            }
        }

        public Visibility IsNetworkAvailable
        {
            get
            {
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                    return Visibility.Hidden;

                return Visibility.Visible;
            }
             
        }

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

        //private ObservableCollection<Porter.Entity.VideoDetails> videoDetails;

        public RelayCommand StopCommand { get;private set; }
        public RelayCommand StopAllCommand { get; private set; }

        public ObservableCollection<Porter.Entity.VideoDetails> VideoDetails
        {
            get; 
            set; 
        }

        public RelayCommand PlayCommand { get; private set; }
        
        public RelayCommand DownloadOneCommand { get; private set; }
        public RelayCommand FetchVideoCommand { get; private set; }
        
        public MyICommand<string> ActiveChecks { get; private set; }
        public MyICommand<string> OpenBrowser { get; private set; }
        public MyICommand<string> RemoveItem { get; private set; }
        public RelayCommand DownloadPush { get; private set; }
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
                    fullUrl = value;
                    RaisePropertyChanged("FullUrl"); 
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
        //private void OnFetchVideo(string s)
        //{
        //    if (Porter.Model.Helper.UrlIsValid(FullUrl))
        //    {

        //        var childWindow = new PorterTube.ChildWindowView.ChildWindowView();
        //        childWindow.Closed += (list =>
        //        {

        //            customDownloadResolver(list);
        //            if (list.Count > 0)
        //            {

        //                App.Current.Dispatcher.Invoke((Action)(() =>
        //                {
        //                    for (int i = VideoDetails.Count - 1; i >= 0; i--)
        //                    {
        //                        var videoID = VideoDetails[i].VideoID;
        //                        bool any = list.ToList().Any(l => l.VideoID == videoID);
        //                        if (any)
        //                            VideoDetails.RemoveAt(i);
        //                    }

        //                    list.ToList().ForEach(a =>
        //                    VideoDetails.Add(a));
        //                }
        //                ));
        //            }
        //        });
        //        childWindow.Show(FullUrl);
        //    }
        //    else
        //    {
        //        System.Windows.Forms.MessageBox.Show("Invalid URL");
        //    }
        //}

        private void customDownloadResolver(ObservableCollection<VideoDetails> list)
        {
            try
            {
                list.ToList().ForEach(a =>
                {
                    var select = a.SelectedVideoExtensionType;
                    a.VideoExtensionType = null;
                    a.SelectedVideoExtensionType = select;
                    VideoDetails.Add(a);
                });
            }
            catch (Exception)
            { }
        }

        public VideoDetailsViewModel()
        {
            //NavCommand = new MyICommand<string>(OnNav);
//            FetchVideoCommand = new MyICommand<string>(OnFetchVideo);
          //  DownloadPush = new MyICommand<string>(OnDownloadPush);
            OpenBrowser = new MyICommand<string>(OnBrowser);
            configRelayCommand_DownloadPush();
            configRelayCommand_Browser();
            configShowinFolderCommand();
            configRemoveCommand();
            configRemoveAllCommand();

            initializeListVideoDetails();
            configDownloadOneCommand();
            configPlayCommand();
            configStopCommand();
            configStopAllCommand();
            configRelayCommand_FetchVideoCommand();

            if (VideoDetails.Count == 0)
            {
                var logList = Loging.Get();
                if (logList != null)
                    VideoDetails = new ObservableCollection<Porter.Entity.VideoDetails>(logList);
            }
        }

        private void configPlayCommand()
        {
            PlayCommand = new RelayCommand(o =>
            {

                var video = ((Porter.Entity.VideoDetails)o);
                if (video != null)
                {
                    var fullPath = video.VideoPath + video.Titel + video.SelectedVideoExtensionType.VideoExtension;
                    if (File.Exists(fullPath))
                    {
                        string argument = "/run, \"" + video.VideoPath + video.Titel + video.SelectedVideoExtensionType.VideoExtension + "\"";
                        System.Diagnostics.Process.Start("explorer.exe", argument);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("File Not Found.", caption: "Porter Youtube.");
                    }
                }

            }, o => checkDownloadStatusIsCompleted(o,DownlaodStatus.End));
        }

        bool checkDownloadStatusIsCompleted(object o,params DownlaodStatus[] status)
        {
            var video = (VideoDetails)o;
            if (video != null)
            {
                if (status.Contains(video.DownlaodStatus))
                    return true;
            }

            return false;
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
                        ProgressValue = 0;
                        //video.BeginDownload = true;
                        //video.EndDownload = false;
                        video.IsActive = true;
                        video.ProgressPercentage = 0;
                        video.DownlaodStatus = DownlaodStatus.Begin;
                        try
                        {
                            VideoDownloader(video.Url, video.SelectedVideoExtensionType, video.VideoPath);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }


            }, o => checkDownloadStatusIsCompleted(o, DownlaodStatus.None,
                                                      DownlaodStatus.End,
                                                      DownlaodStatus.Cancel));
        }

        private void configStopAllCommand()
        {
            StopAllCommand = new RelayCommand(o =>
            {
                VideoDetails.Where(a => a.DownlaodStatus != DownlaodStatus.End).ToList()
                            .ForEach(a => a.DownlaodStatus = DownlaodStatus.Cancel);
                 
            }, o => true);
        }
        private void configStopCommand()
        {
            StopCommand = new RelayCommand(o =>
            {
                var videoSelected = ((Porter.Entity.VideoDetails)o);

                var video = VideoDetails.FirstOrDefault(a => a.Titel == videoSelected.Titel);
                if (video != null)
                    video.DownlaodStatus = DownlaodStatus.Cancel;

            }, o => checkDownloadStatusIsCompleted(o, DownlaodStatus.Begin));
        }
        private void configRelayCommand_DownloadPush()
        {
            DownloadPush = new RelayCommand(o =>
            { 
                IsDownloadAll = true; 
                startDownload(IsDownloadAll);
            }, o => DownloadModel.ValidDownloadAll(VideoDetails));
        }
        private void configRelayCommand_FetchVideoCommand()
        {
            FetchVideoCommand = new RelayCommand(o =>
            {
                var copyText = "";
                 

                if (string.IsNullOrWhiteSpace(FullUrl) )
                {
                    copyText = System.Windows.Clipboard.GetText(System.Windows.TextDataFormat.Text);
                    if (Porter.Model.Helper.UrlIsValid(copyText))
                        FullUrl = copyText;
                }
 

                if (Porter.Model.Helper.UrlIsValid(FullUrl))
                {

                    var childWindow = new PorterTube.ChildWindowView.ChildWindowView();
                    childWindow.Closed += ( list =>
                    {

                        customDownloadResolver(list);
                        if (list.Count > 0)
                        {

                            System.Windows.Application.Current.Dispatcher.Invoke(async () =>
                            {
                                for (int i = VideoDetails.Count - 1; i >= 0; i--)
                                {
                                    var videoID = VideoDetails[i].VideoID;
                                    bool any = list.ToList().Any(l => l.VideoID == videoID);
                                    if (any)
                                        VideoDetails.RemoveAt(i);
                                }


                                list.ToList().ForEach(a => VideoDetails.Add(a));

                                await Loging.SaveList(VideoDetails);

                            } );
                        }

                    });
                    //-------------------
                    var type = DownloadModel.GetUrlType(FullUrl);
                    KeyValuePair<UrlType, string> urlType = new KeyValuePair<UrlType, string>();

                    if (type != null)
                    {
                        if (type.Count == 2)
                        {
                            var ruslt = System.Windows.Forms.MessageBox.Show(text: "This video is a part of a playlist. Would you like to download play list?", caption: "PorterTube Application.", buttons: MessageBoxButtons.YesNoCancel);
                            if (ruslt.ToString() == "Yes")
                            {
                                urlType = type.FirstOrDefault(a => a.Key == UrlType.List);

                                childWindow.Show(urlType);
                            }
                            else if (ruslt.ToString() == "No")
                            {
                                urlType = type.FirstOrDefault(a => a.Key == UrlType.Video);
                                childWindow.Show(urlType);
                            }
                        }
                        else if (type.Count == 1)
                        {
                            var resultKVP = (from kvp in type select new KeyValuePair<UrlType, string>(kvp.Key, kvp.Value)).FirstOrDefault();
                            childWindow.Show(resultKVP);
                        }

                    }
                    //-------------------
                    
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Invalid URL");
                }

            }, o => true);
        }

        private bool UrlIsValid()
        {
            return Porter.Model.Helper.UrlIsValid(FullUrl);
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
            RemoveCommand = new RelayCommand(async o =>
            {
                if (o != null && !string.IsNullOrEmpty(((Porter.Entity.VideoDetails)o).Url))
                {
                    //int index;
                    bool deletedDone = await Loging.Remove(VideoDetails, ((Porter.Entity.VideoDetails)o).Url);//, out index
                }

            }, o => true);
        }
        private void configShowinFolderCommand()
        {
            ShowinFolderCommand = new RelayCommand(o =>
            {
                var video = ((Porter.Entity.VideoDetails)o);
                var fullPath = video.VideoPath + video.Titel + video.SelectedVideoExtensionType.VideoExtension;
                if (File.Exists(fullPath))
                {
                    string argument = "/select, \"" + fullPath + "\"";
                    System.Diagnostics.Process.Start("explorer.exe", argument);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("File Not Found.", caption: "Porter Youtube.");
                }
            }, o => checkDownloadStatusIsCompleted(o, DownlaodStatus.End));
        }

        private void configRelayCommand_Browser()
        {
            //  ActiveChecks = new MyICommand<string>(OnActiveChecks);
            DownloadByBrowserCommand = new RelayCommand(o =>
            {
                //showNotifcation();
                Porter.Entity.VideoDetails v = (Porter.Entity.VideoDetails)o;
                if (v != null)
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

        void startDownload(bool isDownloadAll=false)
        {
             
                var push = DownloadModel.GetListVideoDownload(VideoDetails, isDownloadAll);

                push.ForEach(a =>
                {
                    VideoDownloader(a.Url, a.SelectedVideoExtensionType, a.VideoPath);
                });

             
        }
        //private void OnNav(string destination)
        //{
        //    IsDownloadAll = false;

        //    var url = destination;

        //    using (Porter.Model.FetchVideoURL cls = new Porter.Model.FetchVideoURL())
        //    {
        //        var v = VideoDetails.FirstOrDefault(a => a.Url == destination);

        //        VideoDownloader(v.Url, v.SelectedVideoExtensionType, v.VideoPath);
        //    }

        //}

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

                var video = VideoDetails.FirstOrDefault(a => a.Titel.ToLower() == videoInfo.Title.ToLower());
                if (video != null)
                {
                    //Command Stop Download
                    if (video.DownlaodStatus== DownlaodStatus.Cancel)
                    {
                        e.Cancel = true;
                        if (video.SelectedVideoExtensionType != null)
                            removeExistFile(video.VideoPath + video.Titel,
                                video.SelectedVideoExtensionType.VideoExtension);
                    }

                    if (video.SelectedVideoExtensionType == null)
                    {
                        var listVideoExtensionType = Helper.CustomVideoInfo.getVideoExtensionType(video.Url, false);
                        if (listVideoExtensionType != null && listVideoExtensionType.Count != 0)
                            video.SelectedVideoExtensionType = listVideoExtensionType[0];
                    }

                    if (video != null)
                    {
                        video.ProgressPercentage = double.Parse($"{string.Format("{0:0.##}", e.ProgressPercentage)}");
                        //ProgressValue
                        var listProgressPercentage = VideoDetails
                                    .Where(a => a.DownlaodStatus == DownlaodStatus.Begin
                                                && a.IsActive == true
                                                ) //&& a.EndDownload == false
                                    .ToList().Select(a => a.ProgressPercentage).ToList();

                        if (listProgressPercentage.Any())
                        {
                            ProgressValue = double.Parse($"{string.Format("{0:0.##}", listProgressPercentage.Average())}") * .01;

                        }

                    }
                    else
                        e.Cancel = true;
                }



                
            });
        }
        //private string getVideoID(VideoInfo videoInfo)
        //{
        //    return (videoInfo.DownloadUrl.Split('&'))[0].Substring(61);
        //}
        
        [STAThread]
        public void showNotifcation()
        {
      

            Thread t = new Thread(() => {

                //    //NotificationWin win = new View.NotificationWin();
                //    //NotificationManager.Instance().Add( win);
                //View.Window1 w = new Window1();
                //w.Show();
                //w.ShowInTaskbar=true;

            });
            t.SetApartmentState(ApartmentState.STA);

            t.Start();

        }
 
        private void Downloader_DownloadFinished(object sender, EventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke((Action) async delegate // <--- HERE
            { 
                var videoInfo = ((VideoDownloader)sender).Video;

                var video = VideoDetails.FirstOrDefault(a => a.Titel == videoInfo.Title);
                if (video != null )
                {
                    if (video.ProgressPercentage == 100)
                    {
                        using (FetchSubtitelURL sub = new FetchSubtitelURL())
                            sub.DownloadSubtitel(video);

                        video.DownlaodStatus = DownlaodStatus.End;
                    }
                    else
                        video.DownlaodStatus = DownlaodStatus.Cancel;

                    await Loging.SaveList(VideoDetails);

                    if (IsDownloadAll && video.ProgressPercentage == 100)
                        startDownload(false);
                }

                
            });

            //Thread t = new Thread(() => {

            //    w = new NotificationWin();
            //    w.Show(); 
            //});
            //t.SetApartmentState(ApartmentState.STA);

            //t.Start();

        }
        //NotificationWin w = new NotificationWin();

        public VideoDownloader VideoDownloader(string url, VideoExtensionType videoExtensionType, string videoPath = "")
        {
            VideoDownloader downloader = null;
            try
            {
                IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(url);


                //Get url video
                VideoInfo video;
                if (videoExtensionType != null)
                    video = videos.First(p => p.VideoExtension == videoExtensionType.VideoExtension
                                                            && p.Resolution == videoExtensionType.Resolution);
                else
                {
                    video = videos.FirstOrDefault(a => a.VideoType == VideoType.Mp4);

                }
                if (video.RequiresDecryption)
                    DownloadUrlResolver.DecryptDownloadUrl(video);

                if (string.IsNullOrWhiteSpace(videoPath))
                    videoPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Download\\";


                var titel = Porter.Model.Helper.RemoveSpatialCharacter(video.Title);


                if (video.RequiresDecryption)
                    DownloadUrlResolver.DecryptDownloadUrl(video);
                //Download video
                downloader = new VideoDownloader(video, videoPath + titel + video.VideoExtension);
                //removeExistFile(videoPath + titel, video.VideoExtension);

                downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
                downloader.DownloadFinished += Downloader_DownloadFinished;

                ////Create a new thread to download file
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        downloader.Execute();
                    }
                    catch (Exception)
                    {
                        var v = VideoDetails.FirstOrDefault(a => a.Url.ToLower() == url.ToLower());
                        v.DownlaodStatus = DownlaodStatus.Failure;
                        v.ImageUrl = System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\sadyoutube.jpg";
                    }
                })
                { IsBackground = true };

                thread.Start();
            }
            catch (Exception)
            {
            }

            return downloader;
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
