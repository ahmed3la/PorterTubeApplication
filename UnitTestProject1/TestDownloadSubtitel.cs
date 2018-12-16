using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using Porter.Model;
using Porter.Entity;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace UnitTestProject1
{
    [TestClass]
    public class TestDownloadSubtitel
    {

        [TestMethod]
        static void xr()
        {
            string input = "Hello World";
            string result = Regex.Replace(input, @"(\w+) (\w+)", "$2 $1");
        }
        [TestMethod]
        static void x()
        {
            PorterTube.VideoDetailsViewModel d = new PorterTube.VideoDetailsViewModel();
            //d.showNotifcation();
        }
        //[TestMethod]
        //public void TestMethod1()
        //{
        //    //v=MK-blHFwBX0
        //    var destination = @"https://www.youtube.com/watch?v=GwXnyf6N3sk&list=PL4cyC4G0M1RQ_Rm52cQ4CcOJ_T_HXeMB4";
        //    if (destination.Contains("list="))
        //    {
        //        var x = destination.Split('?')[1];
        //        var z = x.Split('&').ToList<String>();


        //        foreach (string item in z)
        //        {
        //            if (item.StartsWith("list="))
        //            {

        //            }
        //            if (item.StartsWith("v="))
        //            {
        //            }



        //        }

        //        string listName = z.Select(a => a.StartsWith("list=")).ToString().Replace("list=", "");
        //        string videoName = z.Select(a => a.StartsWith("v=")).ToString().Replace("v=", "");

        //    }
        //    else
        //    {

        //    }

        //}


        //enum urlType : int
        //{
        //    List = 0,
        //    Vidio = 1
        //}


        //Dictionary<urlType, string> s(string _fullUrl)
        //{
        //    Dictionary<urlType, string> UrlType = new Dictionary<urlType, string>();


        //    var destination = @"https://www.youtube.com/watch?v=GwXnyf6N3sk&list=PL4cyC4G0M1RQ_Rm52cQ4CcOJ_T_HXeMB4";
        //    if (destination.Contains("list="))
        //    {
        //        var x = destination.Split('?')[1];
        //        var z = x.Split('&').ToList<String>();


        //        foreach (string item in z)
        //        {
        //            if (item.StartsWith("list="))
        //                UrlType.Add(urlType.List, item.Replace("list=", ""));

        //            if (item.StartsWith("v="))
        //                UrlType.Add(urlType.Vidio, item.Replace("v=", ""));
        //        }
        //    }
        //    else
        //    {
        //        var x = destination.Split('?')[1];
        //        var z = x.Split('&').ToList<String>();

        //        foreach (string item in z)
        //        {
        //            if (item.StartsWith("v="))
        //                UrlType.Add(urlType.Vidio, item.Replace("v=", ""));
        //        }
        //    }
        //    return UrlType;
        //}

        [TestMethod]
        public void formatMS()
        {
            //double ratio = 73;
            //string result = string.Format("{00:00:00,000}",
            //    ratio);

            for (int i = 0; i < 100; i++)
            {
                MilliSeconds(123);
                var x4 = MilliSeconds(1234);
                var x5 = MilliSeconds(12345);
                var x6 = MilliSeconds(123456);
                var x7 = MilliSeconds(1234567);
                var x8 = MilliSeconds(12345678);
                var x9 = MilliSeconds(123456789);
            }
            
        }

        public string MilliSeconds(int ms)
        {
            string s_ms = ms.ToString();

            string format = "00:00:00,000";
            
            switch (s_ms.Length)
            {
                case 1:
                    format= "00:00:00,00" + s_ms;
                    break;
                case 2:
                    format = "00:00:00,0" + s_ms;
                    break;
                case 3:
                    format = "00:00:00," + s_ms;
                    break;
                case 4:
                    format = "00:00:0" + s_ms.Substring(0,1) + "," + s_ms.Substring(1, 3);
                    break;
                case 5:
                    format = "00:00:" + s_ms.Substring(0, 2) + "," + s_ms.Substring(2, 3);
                    break;
                case 6:
                    format = "00:0" + s_ms.Substring(0, 1) + ":" + s_ms.Substring(1, 2) + "," + s_ms.Substring(3, 3);
                    break;
                case 7://
                    format = "00:" + s_ms.Substring(0, 2) + ":" + s_ms.Substring(2, 2) + "," + s_ms.Substring(4, 3);
                    break;
                case 8://
                    format = "0" + s_ms.Substring(0, 1) + ":" + s_ms.Substring(1, 2)+ ":" + s_ms.Substring(3, 2) + "," + s_ms.Substring(5, 3);
                    break;
                case 9://
                    format = s_ms.Substring(0, 2) + ":" + s_ms.Substring(2, 2) + ":" + s_ms.Substring(4, 2) + "," + s_ms.Substring(6, 3);
                    break;
                default:

                    break;
            }
            //12:30:30,999
            // 844760
            //Debug.WriteLine(format);
             
            return "";
        }

        [TestMethod]
        public void TestMethod1()
        {
            //PorterTube.VideoDetailsViewModel d = new PorterTube.VideoDetailsViewModel();
            //d.showNotifcation();

            //TestDownloadSubtitelAsync();


        }
        [TestMethod]
        public static  void TestDownloadSubtitelAsync()
        {
            var v = new VideoDetails();
            v.Titel = "Difference between LOOK, WATCH & SEE - Learn English Grammar";
            v.VideoPath = "C:\\Users\\aola\\Desktop\\Download\\";
            v.VideoID = "JkURo4oTKNk";
            //JkURo4oTKNk
            for (int i = 0; i < 10; i++)
            {
                using (FetchSubtitelURL sub = new FetchSubtitelURL())
                {

                     //sub.DownloadSubtitel(v,);
                }

            }

        }



        private static async Task WriteFile()
        {
            string file = Path.GetTempFileName();
            using (FileStream stream = new FileStream(file, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose))
            {
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    List<Task> tasks = new List<Task>();
                    for (int i = 0; i < 1000; i++)
                        tasks.Add(WriteFileLine(streamWriter));
                    await Task.WhenAll(tasks);
                }
            }
            Console.WriteLine(File.ReadAllText(file));
        }
        private static async Task WriteFileLine(StreamWriter streamWriter)
        {
            await streamWriter.WriteLineAsync("Test 123 456 789");
            await streamWriter.FlushAsync();
        }

    }
 
}
