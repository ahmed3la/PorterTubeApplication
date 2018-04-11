using System.ComponentModel;
using System.Windows;

namespace PorterTube.Common
{
    public class ChildWindowManager : INotifyPropertyChanged
    {
        public ChildWindowManager()
        {
            WindowVisibility = Visibility.Collapsed;
            XmlContent = null;
        }
        private static ChildWindowManager instance;
        public static ChildWindowManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ChildWindowManager();
                }
                return instance;
            }
        }


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

        private FrameworkElement xmlContent;
        public FrameworkElement XmlContent
        {
            get { return xmlContent; }
            set
            {
                xmlContent = value;
                RaisePropertyChanged("XmlContent");
            }
        }

        public void ShowChildWindow(FrameworkElement content)
        {
            XmlContent = content;
            RaisePropertyChanged("XmlContent");
            WindowVisibility = Visibility.Visible;
            RaisePropertyChanged("WindowVisibility");
        }

        public void CloseChildWindow()
        {
            WindowVisibility = Visibility.Collapsed;
            RaisePropertyChanged("WindowVisibility");
            XmlContent = null;
            RaisePropertyChanged("XmlContent");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }



    }

    }
