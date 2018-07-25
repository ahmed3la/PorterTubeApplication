using Porter.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Porter.Model
{
    public static class Helper
    {
        public static string RemoveSpatialCharacter(string str)
        { 
            return str.Replace('/', ' ')
                    .Replace('\\', ' ')
                    .Replace('"', ' ')
                    .Replace('?', ' ')
                    .Replace('*', ' ')
                    .Replace('|', ' ')
                    .Replace('>', ' ')
                    .Replace('<', ' ')
                    .Replace(':', ' ');
        }

        public static string MilliSeconds(int tStartMs, int dDurationMs = 0)
        {
            string s_ms = ConvertNumberToTime(tStartMs, dDurationMs);

            string result = "00:00:00,000";
            StringBuilder builder = new StringBuilder();
            
            switch (s_ms.Length)
            {
                case 1:
                    builder.Append("00:00:00,00").Append(s_ms);
                    break;
                case 2:
                    builder.Append("00:00:00,0").Append(s_ms);
                    break;
                case 3:
                    builder.Append("00:00:00,").Append(s_ms);
                    break;
                case 4:
                    builder.Append("00:00:0").Append(s_ms.Substring(0, 1)).Append(",").Append(s_ms.Substring(1, 3));
                    break;
                case 5:
                    builder.Append("00:00:").Append(s_ms.Substring(0, 2)).Append(",").Append(s_ms.Substring(2, 3));
                    break;
                case 6:
                    builder.Append("00:0").Append(s_ms.Substring(0, 1)).Append(":").Append(s_ms.Substring(1, 2)).Append(",").Append(s_ms.Substring(3, 3));
                    break;
                case 7://
                    builder.Append("00:").Append(s_ms.Substring(0, 2)).Append(":").Append(s_ms.Substring(2, 2)).Append(",").Append(s_ms.Substring(4, 3));
                    break;
                case 8://
                    builder.Append("0").Append(s_ms.Substring(0, 1)).Append(":").Append(s_ms.Substring(1, 2)).Append(":").Append(s_ms.Substring(3, 2)).Append(",").Append(s_ms.Substring(5, 3));
                    break;
                case 9://
                    builder.Append(s_ms.Substring(0, 2)).Append(":").Append(s_ms.Substring(2, 2)).Append(":").Append(s_ms.Substring(4, 2)).Append(",").Append(s_ms.Substring(6, 3));
                    break;
                default:

                    break;
            }
            result = builder.ToString();

            return result;
        }

        private static string ConvertNumberToTime(int tStartMs, int dDurationMs = 0)
        {
            var tim = new DateTime().AddMilliseconds(tStartMs).AddMilliseconds(dDurationMs);

            var output = tim.Hour.ToString("00") + tim.Minute.ToString("00") + tim.Second.ToString("00") + tim.Millisecond.ToString("000");

            return output;
        }

        public static bool UrlIsValid(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                if (url.ToLower().Contains("youtube.com") &&
                    (url.ToLower().Contains("v=")
                    || url.ToLower().Contains("list="))
                    )
                    return true;

            }
            return false;
        }

        public static KeyValuePair<UrlType, string>? ConvertStringToKeyValuePair(string strObj)
        {
            if (!string.IsNullOrEmpty(strObj))
            {
                var objSplit = strObj.Substring(1, strObj.Length - 2).Split(',');

                KeyValuePair<UrlType, string> kvp =
                new KeyValuePair<UrlType, string>((UrlType)Enum.Parse(typeof(UrlType), objSplit[0]), objSplit[1]);
                return kvp;
            }
            else
                return null;
            
        }

    }

}
