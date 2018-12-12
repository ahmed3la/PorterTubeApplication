using Newtonsoft.Json;
using Porter.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Porter.Model
{
    public class Loging
    {

        public static async Task<bool> SaveList(ObservableCollection<VideoDetails> list,bool getOld=false)
        {
            if (getOld)
            {
                var oldList = Get();
                if (oldList != null)
                {
                    oldList.ForEach(a =>
                    {
                        Application.Current.Dispatcher.Invoke(()=> // <--- HERE
                        {
                            list.Add(a);
                        });

                    });
                }
            }
            var Log = ConfigurationManager.AppSettings["Log"];
            var path = AppDomain.CurrentDomain.BaseDirectory + Log;

            //List<VideoDetails> anotherlist = new List<VideoDetails>();
            //anotherlist.AddRange(list);

            list.ToList().ForEach(a =>
            {
                //if (a.DownlaodStatus == DownlaodStatus.Cancel)
                //    a.DownlaodStatus = DownlaodStatus.None;

                a.VideoExtensionType = null;
            });

            string output = JsonConvert.SerializeObject(list);
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Config"))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Config");
            }

            File.WriteAllText(path, output);

            return await Task.FromResult(true);
        }

        public static async Task< bool> Remove(ObservableCollection<VideoDetails> list, string url) //, out int index
        {

            bool result = false;
//            index = 0;
            var Log = ConfigurationManager.AppSettings["Log"];
            var path = System.AppDomain.CurrentDomain.BaseDirectory + Log;

            if (list.Count > 0 && !string.IsNullOrEmpty(url))
            {
                var videoItem = list.ToList().FirstOrDefault(a => a.Url.ToLower() == url.ToLower());

                result = list.Remove(videoItem);

                if (result)
                { 
                    result = true;
                    await SaveList(list);
                }
            }

            return result;
        }
        public static bool RemoveAll()
        {
            var Log = ConfigurationManager.AppSettings["Log"];
            var path = System.AppDomain.CurrentDomain.BaseDirectory + Log;

            File.WriteAllText(path, "");
            return true;
        }
        public static List<VideoDetails> Get()
        {
            var Log = ConfigurationManager.AppSettings["Log"];
            var path = System.AppDomain.CurrentDomain.BaseDirectory + Log;
            var list = new List<VideoDetails>();

            try
            {
                if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "Config")
                    || !File.Exists(path))
                    return new List<VideoDetails>();
                else
                {
                    var log = File.ReadAllText(path);
                    list = JsonConvert.DeserializeObject<List<VideoDetails>>(log);
  
                } 

            }
            catch (Exception)
            {

                return new List<VideoDetails>(); ;
            }

            return list;

        }



    }
}
