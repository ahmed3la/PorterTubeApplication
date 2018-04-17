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
