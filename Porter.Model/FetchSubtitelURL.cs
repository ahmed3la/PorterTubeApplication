using Newtonsoft.Json;
using Porter.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YoutubeExtractor;

namespace Porter.Model
{
    public class FetchSubtitelURL : IDisposable
    {
        public bool DownloadSubtitel(VideoDetails videoDetails, List<CaptionTracks> listCaptionTracks)
        {
            string url = "";
            if (listCaptionTracks == null || listCaptionTracks.Count == 0)
                return false;
            else
            {
                url = listCaptionTracks[0].baseUrl;
            }
            TimedText timeText = getTimedText(url, videoDetails.Titel);

            if (timeText != null)
            {
                FileStream file = new FileStream(videoDetails.VideoPath + Helper.RemoveSpatialCharacter(videoDetails.Titel) + ".srt",
                FileMode.Create,
                FileAccess.Write);


                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < timeText.events.Count; i++)
                {
                    var _event = timeText.events[i];

                    builder.AppendLine((i + 1).ToString());

                    builder
                        .Append(Helper.MilliSeconds(_event.tStartMs))
                        .Append(" --> ")
                        .AppendLine(Helper.MilliSeconds(_event.tStartMs + _event.dDurationMs));

                    if (_event.segs != null && _event.segs.Count != 0)
                        builder.AppendLine(_event.segs[0].utf8);
                    builder.AppendLine();
                }

                UnicodeEncoding uniencoding = new UnicodeEncoding();
                byte[] result = uniencoding.GetBytes(builder.ToString());

                file.Write(result, 0, result.Length);
                //---------

                file.Close();
                file.Dispose();
            }
            return true;
        }
         
        private static TimedText getTimedText(string captionTracks, string titel)
        {
            //https://www.youtube.com/api/timedtext?v=h0e2HAPTGF4&lang=en&name=CC&fmt=json3
            // List<VideoDetails> list = new List<VideoDetails>();
            TimedText timeText = new TimedText();

            //var url = string.Format("https://www.youtube.com/api/timedtext?v={0}&lang=en&fmt={1}", videoId, "json3");
            var url = string.Format(captionTracks + "&fmt={0}", "json3");
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

                ////////////////////////
                timeText = JsonConvert.DeserializeObject<TimedText>(data);
            }

            return timeText;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        ~FetchSubtitelURL()
        {
            // Finalizer calls Dispose(false)  
            Dispose();
        }


    }

}
