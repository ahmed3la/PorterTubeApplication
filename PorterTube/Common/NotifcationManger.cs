using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PorterTube.Common
{
    public class NotificationManager
    {
        public  double Left;
        public  double Top { get; set; }
        
        public static List<Window> ListWindow = new List<Window>();

        private static int counter = 1;
        private static NotificationManager instance;

        public static NotificationManager Instance()
        {
            if (instance == null)
                return new Common.NotificationManager();
            return instance;
        }
        public static NotificationManager Instance(Window win)
        {
            if (instance == null)
            {
                instance = new NotificationManager();
            }

            var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;

            if (PresentationSource.FromVisual(win) == null)
            {  
                instance.Left = workingArea.Width - win.ActualWidth - 5;
                instance.Top = workingArea.Height - (win.ActualHeight * counter) - 5;
                return instance;
            }

            var transform = PresentationSource.FromVisual(win).CompositionTarget.TransformFromDevice;
            var corner =  transform.Transform(new Point(workingArea.Right, workingArea.Bottom));//new Point(500, 500);

            
            instance.Left = corner.X - win.ActualWidth - 5;
            instance.Top = corner.Y - (win.ActualHeight * counter) - 5;

             
            counter++;
            //win.Top = instance.Top;
            //win.Left = instance.Left;

            //Add(win);

            if (counter > 5)
                instance.Close();

            return instance;
        }

        public static Point Corner(Window win)
        {
            var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            var transform = PresentationSource.FromVisual(win).CompositionTarget.TransformFromDevice;
            var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));
            return corner;
        }


        public  void Add( Window Win)
        { 
            ListWindow.Add(Win);

            // Win.Visibility = Visibility.Visible;
            ListWindow[ListWindow.Count - 1].ShowInTaskbar = true;
                ListWindow[ListWindow.Count - 1].Show();
             
            //Win.ShowDialog();
        }


        //public   void Close()
        //{

        //    System.Windows.Application.Current.Dispatcher.Invoke(() =>
        //    {

        //        ListWindow[0].Close();
        //        foreach (Window win in System.Windows.Application.Current.Windows)
        //        {
        //            win.Close(); //or whatever
        //        }

        //    });


        //}

        //private void Close()
        //{
        //    foreach (Window win in System.Windows.Application.Current.Windows)
        //    {
        //        win.Close(); //or whatever
        //    }
        //}

 
 

        private void Close()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke( new System.Threading.ThreadStart(() => {
            foreach (Window win in ListWindow)
            {
                win.Close(); //or whatever
            }
        }));


        }


    }
}
