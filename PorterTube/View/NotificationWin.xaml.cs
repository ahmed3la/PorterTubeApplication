using PorterTube.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PorterTube.View
{
    /// <summary>
    /// Interaction logic for NotificationWin.xaml
    /// </summary>
    
    public partial class NotificationWin : Window
    {
       
        public NotificationWin()
        {
            InitializeComponent();

          //  config();


            //Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            //{

            //}));

        }

        public void config()
        {
            var notifyManger = NotificationManager.Instance(this);

            this.Left = notifyManger.Left;
            this.Top = notifyManger.Top;
            this.Visibility = Visibility.Visible;
        }


    }
}
