using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExtractor;


namespace Porter.Model
{
    public partial class Form1 : Form
    {



        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {


            //---------------------
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;

            //Get url video
            IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(txtUrl.Text);
            VideoInfo video = videos.First(p => p.VideoType == VideoType.Mp4 && p.Resolution == Convert.ToInt32(cboResolution.Text));
            if (video.RequiresDecryption)
                DownloadUrlResolver.DecryptDownloadUrl(video);

            //Download video
            VideoDownloader downloader = new VideoDownloader(video, Path.Combine(Application.StartupPath + "\\", video.Title + video.VideoExtension));
            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            //Create a new thread to download file
            Thread thread = new Thread(() => { downloader.Execute(); }) { IsBackground = true };
            thread.Start();

        }

        private void Downloader_DownloadProgressChanged(object sender, ProgressEventArgs e)
        {
            //Update progressbar
            Invoke(new MethodInvoker(delegate ()
            {
                progressBar.Value = (int)e.ProgressPercentage;
                lblPercentage.Text = $"{string.Format("{0:0.##}", e.ProgressPercentage)}%";//C# 6.0
                progressBar.Update();
            }));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(txtUrl.Text);


            foreach (var items in videoInfos)
            {
                listBox1.Items.Add(items.DownloadUrl);
                downloadLis(items);
            }



        }


        bool downloadLis(VideoInfo videos)
        {
            return true;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {


                test();


            }
            catch (Exception )
            {


            }

        }




        [STAThread]
         void test()
        {

            try
            {
                string False = "False";
                string S1 = "https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=50&playlistId=";
                //string S1 = "https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&pageToken=1&playlistId=";
                string S2 = "&key=AIzaSyAGPYIA5Zz3m1WavVFmw35Fw5mvLUkUyeY";
                string URLFormat = @"https://www.youtube.com/playlist?list=";
                //string insert = "www.youtubeinmp3.com/fetch/?video=https://www.youtube.com/watch?v=";
                string insert = "https://www.youtube.com/watch?v=";
 
                // Console.SetWindowSize(width, height);

                //Console.Title = "YouTube Playlist URL parser by Frazzlee";
                //Console.WriteLine("Removing previous files...");
                File.Delete("lines.txt");
                File.Delete("Dump.txt");
                File.Delete("LinesNEW.txt");
                File.Delete("RAWid.txt");
                File.Delete("PreOpen.txt");
                Thread.Sleep(100);
                bool isInternetConnected = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
                Console.WriteLine("Is Internet Connected / Are connections available :     {0}", isInternetConnected);

                if (String.ReferenceEquals(isInternetConnected, False))
                {
                    Console.WriteLine("Cannot connect to internet, please check connections");
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }

                Console.WriteLine(@"Only works up to 50 videos, since the API only gives me the videos that are loaded as standard on the browser (load more button) , will fix soon");
                Console.WriteLine("Enter full playlist Link:");
                string fullURLuser = @"https://www.youtube.com/playlist?list=PL4cyC4G0M1RQ_Rm52cQ4CcOJ_T_HXeMB4";
                System.IO.File.WriteAllText("Playlist.txt", fullURLuser);

                string isYouTube = fullURLuser.Remove(fullURLuser.Length - 34);
                if (!isYouTube.Equals(URLFormat))
                {
                    Console.WriteLine("Wrong URL Format (most likely isn't a YouTube link), please check documentation for more details");
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }
                else
                    Console.WriteLine("URL seems to be ok...");

                string playlistID = fullURLuser.Substring(38);
                string fullURL = S1 + playlistID + S2;
                Console.WriteLine("REMOVE LATER: Full URL: {0}", fullURL);
                Console.WriteLine("Parsing HTML Code");
                Thread.Sleep(1000);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullURL);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;
                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    }
                    string data = readStream.ReadToEnd();
                    response.Close();
                    readStream.Close();
                    Console.WriteLine(data);

                    //Console.Clear();
                    Thread.Sleep(800);
                    Console.WriteLine(@"Dumping API result to ""dump.txt""");
                    System.IO.File.WriteAllText("dump.txt", data);
                    Thread.Sleep(500);
                    Console.WriteLine("Dump finished");

                    string[] dumpFile = System.IO.File.ReadAllLines("dump.txt");
                    string[] selected = dumpFile.Where(line => line.StartsWith(@"     ""videoId"":")).ToArray();
                    System.IO.File.AppendAllLines("linesNEW.txt", selected);

                    Console.WriteLine("VideoID Positions succesfully found and extracted");
                    string[] videoID = System.IO.File.ReadAllLines("LinesNEW.txt");

                    for (int i = 0; i < videoID.Length; i++)
                    {
                        videoID[i].Substring(17);
                    }

                    for (int i = 0; i < videoID.Length; i++)
                        videoID[i] = videoID[i].Replace(@"     ""videoId"": """, "INSERT");
                    for (int i = 0; i < videoID.Length; i++)
                        videoID[i] = videoID[i].Replace(@"""", string.Empty);
                    for (int i = 0; i < videoID.Length; i++)
                        videoID[i] = videoID[i].Replace(@"INSERT", insert);
                    System.IO.File.AppendAllLines("PreOpen.txt", videoID);


                    //  WebClient wb = new WebClient();

                    if (videoID.Length >0)
                    {
                        listBox1.Items.AddRange(videoID);
                    }

                    //for (int i = 0; i < videoID.Length; i++)
                    //{
                    //    //System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", videoID[i].ToString());
                    //    // download(videoID[i].ToString());
                    //    listBox1.Items.AddRange(videoID);
                    //}
                        
                    Thread.Sleep(5000);

                    System.IO.File.AppendAllLines("RAWid.txt", videoID);
                    Console.WriteLine("All done! : ) don't forget to star");
                }
                else
                {
                    Console.WriteLine("Connection could not be established , please check internet connection or firewall settings");
                }
                Console.ReadLine();


            }
            catch (Exception )
            {

                throw;
            }
        }


        bool download(string url)
        {
            //---------------------
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;

            //Get url video
            IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(url);
            VideoInfo video = videos.First(p => p.VideoType == VideoType.Mp4 && p.Resolution == Convert.ToInt32(cboResolution.Text));
            if (video.RequiresDecryption)
                DownloadUrlResolver.DecryptDownloadUrl(video);

            //Download video
            VideoDownloader downloader = new VideoDownloader(video, Path.Combine(Application.StartupPath + "\\Download\\", video.Title + video.VideoExtension));
            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            
            
            //Create a new thread to download file
            Thread thread = new Thread(() => { downloader.Execute(); }) { IsBackground = true };
            thread.Start();




            return true;
        }



    }
}
