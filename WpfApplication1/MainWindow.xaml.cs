using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Shell;
using System.Windows.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
   
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //BackgroundWorker worker = new BackgroundWorker();
            //worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            //worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            //worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            //worker.WorkerReportsProgress = true;
            //worker.RunWorkerAsync();
        }
        //void MainWindow_Loaded(object sender, RoutedEventArgs e)
        //{
        //    BackgroundWorker worker = new BackgroundWorker();
        //    worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        //    worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
        //    worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        //    worker.WorkerReportsProgress = true;
        //    worker.RunWorkerAsync();
        //}

        public MainWindow()
        {
            InitializeComponent();
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
                var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));

               this.Left = corner.X - this.ActualWidth - 20;
                this.Top = corner.Y - this.ActualHeight-10;
            }));
            //TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;

            //Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }
        //void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    progressBar1.Value = e.ProgressPercentage;
        //    TaskbarItemInfo.ProgressValue = (double)e.ProgressPercentage / 100;
        //}

        //void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    progressBar1.Value = 100;
        //    TaskbarItemInfo.ProgressValue = 1.0;
        //}
        //void worker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    for (int i = 0; i < 100; i += 10)
        //    {
        //        Thread.Sleep(1000);
        //        ((BackgroundWorker)sender).ReportProgress(i);
        //    }
        //}
    }
}
