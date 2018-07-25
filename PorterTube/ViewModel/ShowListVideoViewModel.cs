using Porter.Entity;
using Porter.Model;
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
using YoutubeExtractor;

namespace PorterTube.ViewModel
{
    class ShowListVideoViewModel : INotifyPropertyChanged
    {

        #region '======Property======'
        private string title;
        public string Title
        {
            get { return "Youtube Porter 1.1"; }
            set { title = value;
                RaisePropertyChanged("Title");
            }
        }

        private VideoExtensionType videoExtensionTypeCustome;

        public VideoExtensionType VideoExtensionTypeCustome
        {
            get { return videoExtensionTypeCustome; }
            set {


                videoExtensionTypeCustome = value;

                RaisePropertyChanged("VideoExtensionTypeCustome");

                if (ListVideoExtensionTypeCustom.Count > 0 )
                {
                    for (int i = 0; i < VideoDetails.Count - 1; i++)
                    {
                        var vd = VideoDetails[i];

                        vd.SelectedVideoExtensionType = vd.VideoExtensionType.FirstOrDefault(a => a.Resolution == value.Resolution &&
                                                                     a.VideoExtension.ToLower() == value.VideoExtension.ToLower());
                    } 
                }
                
            }
        }

        private ObservableCollection<VideoExtensionType> listVideoExtensionTypeCustom;
        public ObservableCollection<VideoExtensionType> ListVideoExtensionTypeCustom
        {
            get { return listVideoExtensionTypeCustom; }
            set
            {
                listVideoExtensionTypeCustom = value;
                RaisePropertyChanged("ListVideoExtensionTypeCustom");
            }
        }


        /// <summary>
        /// ProgressPercentage=(ProgressExtensionType * 100) / VideoDetails.Count;
        /// </summary>
        private string progressPercentage;

        public string ProgressPercentage
        {
            get { return progressPercentage; }
            set { progressPercentage = value; RaisePropertyChanged("ProgressPercentage"); }
        }


        private int progressExtensionType;

        public int ProgressExtensionType
        {
            get { return progressExtensionType; }
            set {
                progressExtensionType = value;
                RaisePropertyChanged("ProgressExtensionType");
                //
                if (ProgressExtensionType != VideoDetails.Count && IsCustome && value!=0)
                    CustomEnabled = false; 
                else
                    CustomEnabled = true;
                //
            }
        }

        private bool customEnabled = true;

        public bool CustomEnabled
        {
            get { return customEnabled; }
            set { customEnabled = value; RaisePropertyChanged("CustomEnabled"); }
        }


        private bool isCustome;
        public bool IsCustome
        {
            get
            { 
                return isCustome;
            }
            set
            {

                isCustome = value;
                RaisePropertyChanged("IsCustome");
 
                Task t = new Task(() =>
                 {
                     customeVideoExtensionType(value);
                 });
                t.Start();
 
            }
        }
        private ObservableCollection<Porter.Entity.VideoDetails> videoDetails;
        public ObservableCollection<Porter.Entity.VideoDetails> VideoDetails
        { get {
                return videoDetails;
            }
            set {
                videoDetails = value;
                RaisePropertyChanged("VideoDetails");
            } }

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


        //private int heightRowCustom;

        //public int HeightRowCustom
        //{
        //    get { return heightRowCustom; }
        //    set { heightRowCustom = value; RaisePropertyChanged("HeightRowCustom"); }
        //} 
        public RelayCommand CommandGetList { get; private set; }
        public MyICommand<string> CommandCancel { get; private set; }




        #endregion


        #region '======Methods======'
     

        private void configGetSelectedVideosCommand()
        {
            CommandGetList = new RelayCommand(o =>
            {
                if (Closed != null)
                {
                    var list = VideoDetails.Where(a => a.IsActive);

                  //  VideoDetails = new ObservableCollection<VideoDetails>(list);

                    using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                        list.ToList().ForEach(a =>  
                        {
                            a.VideoPath = dialog.SelectedPath + "\\";                             
                            if (VideoExtensionTypeCustome != null)
                                a.SelectedVideoExtensionType = VideoExtensionTypeCustome;
                        });
                    }

                    Closed(new ObservableCollection<Porter.Entity.VideoDetails>(list));
                }

            }, o => true);
        }

        public ShowListVideoViewModel(KeyValuePair<UrlType,string> type)
        { 
            VideoDetails = new ObservableCollection<Porter.Entity.VideoDetails>();
             
            CommandCancel = new MyICommand<string>(OnCancel);

            OnDownload(type.Value, type.Key);


            configGetSelectedVideosCommand();
        }
        
        

        private void customeVideoExtensionType(bool custome)
        { 
            ProgressExtensionType = 0;
            ProgressPercentage = "0 %";

                VideoDetails.ToList().ForEach(a =>
                {
                    a.VideoExtensionType = new ObservableCollection<VideoExtensionType>();
                    a.SelectedVideoExtensionType = null;


                    if (custome)
                    {
                        Task task = new Task(() =>
                        {
                            ObservableCollection<VideoExtensionType> listVideoExtension = new ObservableCollection<VideoExtensionType>();
                           
                            listVideoExtension = Helper.CustomVideoInfo.getVideoExtensionType(a.Url, custome);
                            //Clear befor fill 
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>// <--- HERE
                            {

                                listVideoExtension.ToList().ForEach(q =>
                                {
                                    if (a.VideoExtensionType == null)
                                        a.VideoExtensionType = new ObservableCollection<VideoExtensionType>();
                                    a.VideoExtensionType.Add(q);
                                    a.IsCustome = custome;
                                });
                                //
                                if (a.VideoExtensionType != null && a.VideoExtensionType.Any())
                                    a.SelectedVideoExtensionType = a.VideoExtensionType[0];
                                //
                                ProgressExtensionType++;
                                ProgressPercentage = VideoDetails.Count == 0 ? "0" : string.Concat(((ProgressExtensionType * 100) / VideoDetails.Count).ToString(), " %");

                                if (ProgressExtensionType == VideoDetails.Count)
                                    ListVideoExtensionTypeCustom = DownloadModel.GetListVideoDetailsCustom(VideoDetails);
                            });
                        }, cts.Token);
                        //{
                        //    IsBackground = true
                        //};

                        task.Start();
                    }
                    else
                    {
                        a.IsCustome = custome;
                        ListVideoExtensionTypeCustom = new ObservableCollection<VideoExtensionType>();
                        VideoExtensionTypeCustome = null;
                    }

                });

        }



        private void OnCancel(string url)
        {
            if (Closed != null)
            {
                if (cts.Token.CanBeCanceled)
                    cts.Cancel();

                VideoDetails = new ObservableCollection<VideoDetails>();

                Closed(VideoDetails);
            }
        }

        private void OnDownload(string destination, UrlType type)
        {
            if (type == UrlType.List)
                fillList(destination);
            else
                fillVideo(destination);
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
        void fillVideo(string videoId)
        {
            using (Porter.Model.FetchVideoURL cls = new Porter.Model.FetchVideoURL())
            {
                var video = cls.GetVideoDetail(videoId);

                VideoDetails.Add(video);
            }
        }
         
        private void customDownloadResolver(ObservableCollection<VideoDetails> list)
        {
            try
            {

                list.ToList().ForEach(a =>
                {
                    //int _index = 0;
                    IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(a.Url);

                    videos.ToList().ForEach(v =>
                    {

                        var isExist = a.VideoExtensionType.Any(e => e.Resolution == v.Resolution && e.VideoExtension == v.VideoExtension);
                        if (!isExist)
                        {
                            a.VideoExtensionType.Add(new VideoExtensionType()
                            {
                                //Index = _index,
                                Resolution = v.Resolution,
                                VideoExtension = v.VideoExtension,
                                VideoType = v.VideoType.ToString(),
                                Titel = "Video Type:" + v.VideoType.ToString() +
                                    new string(' ', 10 - v.VideoType.ToString().Length) +
                                    "Resolution: " + v.Resolution.ToString()
                            });
                            //_index++;
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
        CancellationTokenSource cts = new CancellationTokenSource();
        #endregion


    }
}
