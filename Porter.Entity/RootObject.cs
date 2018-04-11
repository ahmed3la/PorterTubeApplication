using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Porter.Entity
{

    public enum UrlType : int
    {
        List = 0,
        Video = 1
    }
    //
    // Summary:
    //     The video type. Also known as video container.
    public class VideoExtensionType : IEqualityComparer<VideoExtensionType>
    {
        //private int index;

        //public int Index
        //{
        //    get {
        //        return index;
        //    }
        //    set {
        //        index = value;
        //    }
        //}

        public int Resolution { get; set; }
        public string VideoType { get; set; }
        public string VideoExtension { get; set; }
        public string Titel { get; set; }

        public bool Equals(VideoExtensionType x, VideoExtensionType y)
        {
            bool result = false;

            result = string.Equals(x.Titel, y.Titel)
                  && string.Equals(x.Resolution, y.Resolution)
                  && string.Equals(x.VideoExtension, y.VideoExtension)
                  && string.Equals(x.VideoType, y.VideoType);

            return result;


        }

        public int GetHashCode(VideoExtensionType obj)
        {
            return obj.Titel.GetHashCode();
        }
    }



    public class PageInfo
    {
        public int totalResults { get; set; }
        public int resultsPerPage { get; set; }
    }

    public class Default
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Medium
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class High
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Standard
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Thumbnails
    {
        public Default @default { get; set; }
        public Medium medium { get; set; }
        public High high { get; set; }
        public Standard standard { get; set; }
    }

    public class ResourceId
    {
        public string kind { get; set; }
        public string videoId { get; set; }
    }

    public class Snippet
    {
        public DateTime publishedAt { get; set; }
        public string channelId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public Thumbnails thumbnails { get; set; }
        public string channelTitle { get; set; }
        public string playlistId { get; set; }
        public int position { get; set; }
        public ResourceId resourceId { get; set; }
        public List<string> tags { get; set; }
        public string categoryId { get; set; }
        public string liveBroadcastContent { get; set; }
        public Localized localized { get; set; }
    }

    public class Item
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public Snippet snippet { get; set; }
        public ContentDetails contentDetails { get; set; }
    }

    public class RootObjectPlayList
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string nextPageToken { get; set; }
        public PageInfo pageInfo { get; set; }
        public List<Item> items { get; set; }
    }

    public class RootObjectVideo
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public PageInfo pageInfo { get; set; }
        public List<Item> items { get; set; }
    }

    public class Localized
    {
        public string title { get; set; }
        public string description { get; set; }
    }

    public class ContentDetails
    {
        public string duration { get; set; }
        public string dimension { get; set; }
        public string definition { get; set; }
        public string caption { get; set; }
        public bool licensedContent { get; set; }
        public string projection { get; set; }
    }
}
