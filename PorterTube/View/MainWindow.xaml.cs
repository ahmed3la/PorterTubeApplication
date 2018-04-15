using PorterTube.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace PorterTube
{
  
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
         
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        public MainWindow()
        {
            InitializeComponent();
            //--------------------------------------
            childWindow.DataContext = ChildWindowManager.Instance;
            this.DataContext = new VideoDetailsViewModel();
            //--------------------------------------
            this.Closed += MainWindow_Closed;
            //--------------------------------------
            // initialise code here
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "The PorterTube application has been minimised.";
            m_notifyIcon.BalloonTipTitle = "PorterTube";
            m_notifyIcon.Text = "PorterTube";
            m_notifyIcon.MouseMove += M_notifyIcon_BalloonTipShown;
             
            m_notifyIcon.Icon = PorterTube.Properties.Resources.DownloadPorter;
 
            m_notifyIcon.Click += m_notifyIcon_Click;

            //--------------------------------------
            Title = "Youtube Porter 0." + DateTime.Now.ToString("yyyyMMdd");
        }

        private void M_notifyIcon_BalloonTipShown(object sender, EventArgs e)
        {//double.Parse($"{string.Format("{0:0.##}", listProgressPercentage.Average())}") * .01;
            m_notifyIcon.Text = "Progress " + string.Format("{0:0.##}", tbii.ProgressValue = 100) + " %";
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }



        void OnClose(object sender, CancelEventArgs args)
        {
            m_notifyIcon.Dispose();
            m_notifyIcon = null;
        }

        private WindowState m_storedWindowState = WindowState.Normal;
        void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                if (m_notifyIcon != null)
                    m_notifyIcon.ShowBalloonTip(2000);
            }
            else
                m_storedWindowState = WindowState;
        }
        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            CheckTrayIcon();
        }

        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = m_storedWindowState;
        }
        void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        void ShowTrayIcon(bool show)
        {
            if (m_notifyIcon != null)
                m_notifyIcon.Visible = show;
        }
    } 


}
