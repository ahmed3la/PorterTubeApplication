using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Porter.Entity
{

    //
    // Summary:
    //     Specifies the display state of an element.
    //public enum MyVisibility : byte
    //{
    //    //
    //    // Summary:
    //    //     Display the element.
    //    Visible = 0,
    //    //
    //    // Summary:
    //    //     Do not display the element, but reserve space for the element in layout.
    //    Hidden = 1,
    //    //
    //    // Summary:
    //    //     Do not display the element, and do not reserve space for it in layout.
    //    Collapsed = 2
    //}


    public enum DownlaodStatus : int
    {
        None = 0,
        Begin = 1,
        End = 2,
        Cancel = 4,
        Failure = 5
    }

    public class VideoDetails : INotifyPropertyChanged
    {
        public VideoDetails()
        {
            VideoExtensionType = new ObservableCollection<Entity.VideoExtensionType>();
            
        }
        private string titel;
        private string imageUrl;
        private double progressPercentage;
        private string url;

        private bool isActive;

        //-------------------------------
        //private MyVisibility canSelected;

        //public MyVisibility CanSelected
        //{
        //    get {

        //        if (VideoExtensionType == null || VideoExtensionType.Count == 0)
        //            canSelected = MyVisibility.Hidden;
        //        else return MyVisibility.Visible;
        //        return canSelected;
        //    }
        //    set { canSelected = value; }
        //}

        //private MyVisibility visibiltyEndDownload;

        //public MyVisibility VisibiltyEndDownload
        //{
        //    get { return visibiltyEndDownload; }
        //    set { visibiltyEndDownload = value; RaisePropertyChanged("VisibiltyEndDownload"); }
        //}

        private DownlaodStatus downlaodStatus;
        public DownlaodStatus DownlaodStatus
        {
            get { return downlaodStatus; }
            set { downlaodStatus = value; RaisePropertyChanged("DownlaodStatus"); }
        }


        private string videoID;

        public string VideoID
        {
            get { return videoID; }
            set { videoID = value; RaisePropertyChanged("VideoID"); }
        }

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; RaisePropertyChanged("isActive"); }
        }
        //public bool BeginDownload
        //{
        //    get { return beginDownload; }
        //    set { beginDownload = value; RaisePropertyChanged("BeginWonload"); }
        //}
        //private bool beginDownload;
        //private bool endDownload;
        //public bool EndDownload
        //{
        //    get {
        //        //if (endDownload)
        //        //    VisibiltyEndDownload = MyVisibility.Collapsed;

        //        return endDownload;
        //    }
        //    set {
        //        endDownload = value;
        //        RaisePropertyChanged("EndDownload");
        //        //if (endDownload)
        //        //    VisibiltyEndDownload = MyVisibility.Collapsed;
        //    }
        //}
        //private bool stopDownload;
        //public bool StopDownload
        //{
        //    get
        //    { 
        //        return stopDownload;
        //    }
        //    set
        //    {
        //        stopDownload = value;
        //        RaisePropertyChanged("StopDownload");
        //        //if (endDownload)
        //        //    VisibiltyEndDownload = MyVisibility.Collapsed;
        //    }
        //}
        public string Url
        {
            get { return url; }
            set { url = value; RaisePropertyChanged("Url"); }
        }
        private string videoPath;

        public string VideoPath
        {
            get { return videoPath; }
            set { videoPath = value; RaisePropertyChanged("VideoPath"); }
        }

        public string Titel
        {
            get
            { 
                return titel;
            }

            set
            {
                if (titel != value)
                {
                    titel = value;
                    RaisePropertyChanged("Titel");

                }
            }
        }
        public string ImageUrl
        {
            get
            {
                return imageUrl;
            }

            set
            {
                if (imageUrl != value)
                {
                    imageUrl = value;
                    RaisePropertyChanged("ImageUrl");

                }
            }
        }

        private VideoExtensionType selectedVideoExtensionType;

        public VideoExtensionType SelectedVideoExtensionType
        {
            get {
                return selectedVideoExtensionType;
            }
            set {
                 
                selectedVideoExtensionType = value;
                RaisePropertyChanged("SelectedVideoExtensionType");
            }
        }

        private ObservableCollection<VideoExtensionType> videoExtensionType;

        public ObservableCollection<VideoExtensionType> VideoExtensionType
        {
            get { return videoExtensionType; }
            set
            {
                videoExtensionType = value;
                RaisePropertyChanged("VideoExtensionType");
            }
        }

        public double ProgressPercentage
        {
            get
            {
                return progressPercentage;
            }

            set
            {
                if (progressPercentage != value)
                {
                    progressPercentage = value;
                    RaisePropertyChanged("ProgressPercentage");

                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
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
            }
        }
    }
}
