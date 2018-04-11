using Newtonsoft.Json;
using Porter.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Porter.Model
{

    public class FetchVideoURL :IDisposable
    {
         
        private string configListUrl(string playlistName, int resultsPerPage = 50, string nextPageToken = "")
        {
            string key = "&key=AIzaSyC2HrJLjYz8vMvNGEgFI1tksNVycl1plSM";// "&key=AIzaSyAGPYIA5Zz3m1WavVFmw35Fw5mvLUkUyeY";
                               
            string url = string.Format(@"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet{0}&maxResults={1}&playlistId={2}"
                              , string.IsNullOrEmpty(nextPageToken) ? "" : "&pageToken=" + nextPageToken
                              , "50"
                              , playlistName
                              , nextPageToken);

            string fullURL = url + key;
            return fullURL;
        }
        private string configVideoUrl(string videoId)
        { 
            string key = "AIzaSyAGPYIA5Zz3m1WavVFmw35Fw5mvLUkUyeY";
            //https://www.googleapis.com/youtube/v3/videos?id=GwXnyf6N3sk&key=AIzaSyAGPYIA5Zz3m1WavVFmw35Fw5mvLUkUyeY&part=snippet,contentDetails
            //https://www.googleapis.com/youtube/v3/videos?id=GwXnyf6N3sk&key=AIzaSyAGPYIA5Zz3m1WavVFmw35Fw5mvLUkUyeY&part=snippet,contentDetailsAIzaSyAGPYIA5Zz3m1WavVFmw35Fw5mvLUkUyeY
            string url = string.Format(@"https://www.googleapis.com/youtube/v3/videos?id={0}&key={1}&part=snippet,contentDetails"
                               , videoId
                              , key);

           
            return url;
        }




        [STAThread]
        public List<VideoDetails> GetList(string playlistName)
        {
            List<VideoDetails> list = new List<VideoDetails>();

            List<RootObjectPlayList> listRoot = new List<RootObjectPlayList>();
            string insert = "https://www.youtube.com/watch?v=";

            string nextPageToken = "";
            do
            {
                string url = configListUrl(playlistName, 50, nextPageToken);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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


                    RootObjectPlayList example1Model = new RootObjectPlayList();
                    example1Model = JsonConvert.DeserializeObject<RootObjectPlayList>(data);

                    
                    listRoot.Add(example1Model);

                     
                    nextPageToken = example1Model.nextPageToken;

                }

            } while (!string.IsNullOrEmpty(nextPageToken));
             
            var x = listRoot.SelectMany(a => a.items).ToList();

            list = x.Where(a=> !a.snippet.title.ToLower().Equals("deleted video") && a.snippet.thumbnails != null).Select(a => new VideoDetails
            {
                ImageUrl = a.snippet.thumbnails==null?"": a.snippet.thumbnails.@default.url,
                Url = insert + a.snippet.resourceId.videoId,
                VideoID = a.snippet.resourceId.videoId,
                Titel = a.snippet.title,

            }).ToList();

            var s = JsonConvert.SerializeObject(list);



            return list;
        }



        //[STAThread]
        //public ObservableCollection<VideoDetails> GetListVideosDetails(string playlistName,int resultsPerPage=10, string nextPageToken="")
        //{ 
        //    try
        //    {
        //        ObservableCollection<VideoDetails> list = null;


        //            //pageToken
        //            string False = "False";
        //        string S1 = "https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&nextPageToken=CAUQAA&maxResults=50&playlistId=";

        //        if (string.IsNullOrEmpty(nextPageToken))
        //        {

        //        }


        //        string listConfigUrl = string.Format(@"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet
        //                                &nextPageToken=CAUQAA
        //                                &maxResults=50
        //                                &playlistId=", 1);




        //        string S2 = "&key=AIzaSyAGPYIA5Zz3m1WavVFmw35Fw5mvLUkUyeY";
        //        string URLFormat = @"https://www.youtube.com/playlist?list=";
        //        //string insert = "www.youtubeinmp3.com/fetch/?video=https://www.youtube.com/watch?v=";
        //        string insert = "https://www.youtube.com/watch?v=";

        //        //Console.Title = "YouTube Playlist URL parser by Frazzlee";
        //        //Console.WriteLine("Removing previous files...");
        //        File.Delete("lines.txt");
        //        File.Delete("Dump.txt");
        //        File.Delete("LinesNEW.txt");
        //        File.Delete("RAWid.txt");
        //        File.Delete("PreOpen.txt");

        //        Thread.Sleep(100);
        //        bool isInternetConnected = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        //        Console.WriteLine("Is Internet Connected / Are connections available :     {0}", isInternetConnected);

        //        if (String.ReferenceEquals(isInternetConnected, False))
        //        {
        //            Console.WriteLine("Cannot connect to internet, please check connections");
        //            Thread.Sleep(1000);
        //            Environment.Exit(0);
        //        }

        //        Console.WriteLine(@"Only works up to 50 videos, since the API only gives me the videos that are loaded as standard on the browser (load more button) , will fix soon");
        //        Console.WriteLine("Enter full playlist Link:");

        //        string fullURLuser = @"https://www.youtube.com/playlist?list=PL4cyC4G0M1RQ_Rm52cQ4CcOJ_T_HXeMB4";
        //        System.IO.File.WriteAllText("Playlist.txt", fullURLuser);

        //        string isYouTube = fullURLuser.Remove(fullURLuser.Length - 34);
        //        if (!isYouTube.Equals(URLFormat))
        //        {
        //            Console.WriteLine("Wrong URL Format (most likely isn't a YouTube link), please check documentation for more details");
        //            Thread.Sleep(1000);
        //            Environment.Exit(0);
        //        }
        //        else
        //            Console.WriteLine("URL seems to be ok...");

        //        string playlistID = playlistName;//fullURLuser.Substring(38);
        //        string fullURL = S1 + playlistID + S2;
        //        Console.WriteLine("REMOVE LATER: Full URL: {0}", fullURL);
        //        Console.WriteLine("Parsing HTML Code");
        //        Thread.Sleep(1000);

        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullURL);
        //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            Stream receiveStream = response.GetResponseStream();
        //            StreamReader readStream = null;
        //            if (response.CharacterSet == null)
        //            {
        //                readStream = new StreamReader(receiveStream);
        //            }
        //            else
        //            {
        //                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
        //            }
        //            string data = readStream.ReadToEnd();
        //            response.Close();
        //            readStream.Close();
        //            // Console.WriteLine(data);
        //            //  JObject jObject = JObject.Parse(data);

        //            RootObject example1Model = JsonConvert.DeserializeObject<RootObject>(data);

        //            //Console.Clear();
        //            Thread.Sleep(800);
        //            Console.WriteLine(@"Dumping API result to ""dump.txt""");
        //            System.IO.File.WriteAllText("dump.txt", data);
        //            Thread.Sleep(500);
        //            Console.WriteLine("Dump finished");

        //            string[] dumpFile = System.IO.File.ReadAllLines("dump.txt");
        //            string[] selected = dumpFile.Where(line => line.StartsWith(@"     ""videoId"":")).ToArray();
        //            System.IO.File.AppendAllLines("linesNEW.txt", selected);

        //            Console.WriteLine("VideoID Positions succesfully found and extracted");
        //            string[] videoID = System.IO.File.ReadAllLines("LinesNEW.txt");

        //            for (int i = 0; i < videoID.Length; i++)
        //            {
        //                videoID[i].Substring(17);
        //            }

        //            NewMethod(insert, videoID);


        //            if (videoID.Length > 0)
        //            {
        //                //listBox1.Items.AddRange(videoID);
        //                var x = example1Model.items.Select(a => new VideoDetails
        //                {
        //                    ImageUrl = a.snippet.thumbnails.standard.url,
        //                    Url = insert + a.snippet.resourceId.videoId,
        //                    Titel = a.snippet.title,

        //                });

        //                //list = new ObservableCollection<VideoDetails>();
        //                ObservableCollection<VideoDetails> listx = new ObservableCollection<VideoDetails>(x.ToList() as List<VideoDetails>);
        //                return listx;

        //            }

        //            System.IO.File.AppendAllLines("RAWid.txt", videoID);
        //            Console.WriteLine("All done! : ) don't forget to star");
        //        }
        //        else
        //        {
        //            Console.WriteLine("Connection could not be established , please check internet connection or firewall settings");
        //        }
        //        Console.ReadLine();

        //        return list;
        //    }
        //    catch (Exception )
        //    {

        //        throw;
        //    }



        //}

        [STAThread]
        public VideoDetails GetVideoDetail(string videoID)
        {
            VideoDetails video = new VideoDetails();
            string insert = "https://www.youtube.com/watch?v=";

            RootObjectPlayList Root = new RootObjectPlayList();

            string url = configVideoUrl(videoID);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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


                RootObjectVideo example1Model = new RootObjectVideo();
                example1Model = JsonConvert.DeserializeObject<RootObjectVideo>(data);

                var videoItem = example1Model.items.FirstOrDefault();
                if (videoItem != null)
                {
                    video = new VideoDetails()
                    {
                        ImageUrl = videoItem.snippet.thumbnails.@default.url,
                        Url = insert + videoItem.id,
                        VideoID = videoItem.id,
                        Titel = videoItem.snippet.title
                    };
                }
            }

            return video;
        }

        //public VideoDetails GetVideoDetail(string videoId)
        //{
        //    try
        //    {
        //        VideoDetails video = null;
        //        string insert = "https://www.youtube.com/watch?v=";
        //        string key = "AIzaSyAGPYIA5Zz3m1WavVFmw35Fw5mvLUkUyeY";
        //        string fullURL = string.Format(@"https://www.googleapis.com/youtube/v3/videos?id={0}&key={1}&part=snippet,contentDetails"
        //                            , videoId, key);

        //        bool isInternetConnected = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullURL);
        //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            Stream receiveStream = response.GetResponseStream();
        //            StreamReader readStream = null;
        //            if (response.CharacterSet == null)
        //            {
        //                readStream = new StreamReader(receiveStream);
        //            }
        //            else
        //            {
        //                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
        //            }
        //            string data = readStream.ReadToEnd();
        //            response.Close();
        //            readStream.Close();


        //            RootObject example1Model = JsonConvert.DeserializeObject<RootObject>(data);



        //            //listBox1.Items.AddRange(videoID);
        //            var x = example1Model.items.Select(a => new VideoDetails
        //            {
        //                ImageUrl = a.snippet.thumbnails.standard.url,
        //                Url = insert + a.snippet.resourceId.videoId,
        //                Titel = a.snippet.title,

        //            });

        //        }

        //        return video;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //private static void NewMethod(string insert, string[] videoID)
        //{
        //    for (int i = 0; i < videoID.Length; i++)
        //        videoID[i] = videoID[i].Replace(@"     ""videoId"": """, "INSERT");
        //    for (int i = 0; i < videoID.Length; i++)
        //        videoID[i] = videoID[i].Replace(@"""", string.Empty);
        //    for (int i = 0; i < videoID.Length; i++)
        //        videoID[i] = videoID[i].Replace(@"INSERT", insert);
        //    System.IO.File.AppendAllLines("PreOpen.txt", videoID);

        //}


        private static string getgetTimedText(string videoUrl, string format = "json3")
        {
            var videoId = videoUrl.Substring(10);
            return videoId;
        }






        public void Dispose()
        { 
            GC.SuppressFinalize(this);
        }
        ~FetchVideoURL ()
        {
            // Finalizer calls Dispose(false)  
            Dispose();
        }
    }

}
