using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Download5sing
{
    public class SongInformation
    {
        public String songId = "";
        public String songType = "";
        public String songName = "";
        public String authorName = "";
        public String fileName = "";
        public String path = "";
        public String file = "";
    }
    public static class CatchCommand
    {
        private static CookieContainer cookie;
        private static char[] illegalPath = Path.GetInvalidPathChars();
        private static char[] illegalFile = Path.GetInvalidFileNameChars();

        public static bool CheckCookie()
        {
            if (cookie == null || cookie.Count == 0)
                cookie = CookieUtil.ReadCookiesFromDisk(Environment.CurrentDirectory + "\\cookie");
            if (cookie == null || cookie.Count == 0)
                return false;
            return true;
        }

        public static bool Login(String id, String password)
        {
            Encoding myEncoding = Encoding.GetEncoding("utf-8");
            String url = "http://5sing.kugou.com/login/";
            Byte[] data = myEncoding.GetBytes("txtUserName=" + id + "&txtPassword=" + password);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.Timeout = 5000;
            req.ContentType = "application/x-www-form-urlencoded";
            req.Headers.Add("Accept-Encoding", "gzip, deflate");
            req.CookieContainer = new CookieContainer();
            req.AllowAutoRedirect = false;

            //req.Referer = "http://5sing.kugou.com/login/";
            //req.Host = "5sing.kugou.com";
            //req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            //req.Headers.Add("Origin", "http://5sing.kugou.com");
            //req.Headers.Add("Upgrade-Insecure-Requests", "1");
            //req.Headers.Add("RA-Ver", "3.0.7");
            //req.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");

            bool flag = false;
            try
            {
                Stream stream = req.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), myEncoding);
                String str = reader.ReadToEnd();
                cookie = new CookieContainer();
                cookie.Add(myResponse.Cookies);
                if (cookie.Count != 0)
                    flag = true;
                CookieUtil.WriteCookiesToDisk(Environment.CurrentDirectory+"\\cookie", cookie);
            }
            catch (Exception e)
            {
            }
            return flag;
        }

        public static List<String[]> GetCollectList(int start, int end)  //下标由0起
        {
            if (start > end)
                return null;
            List<String[]> list = new List<String[]>();
            int l = start / 20;
            int h = end / 20;
            if (l == h)
            {
                list.AddRange(GetCollectPageList(l + 1, start % 20, end % 20));
                return list;
            }
            list.AddRange(GetCollectPageList(l + 1, start % 20, 19));
            for(int i=l+1; i<h; i++)
            {
                list.AddRange(GetCollectPageList(i + 1, 0, 19));
            }
            list.AddRange(GetCollectPageList(h + 1, 0, end % 20));
            return list;
        }

        public static List<String[]> GetCollectPageList(int page, int start, int end)  //下标由0起
        {
            if (start > end || page <= 0)
                return new List<string[]>();
            String url = "http://5sing.kugou.com/my/writing/favoritesong?type=&page="+page;
            Encoding myEncoding = Encoding.GetEncoding("utf-8");
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.Timeout = 5000;
            req.CookieContainer = cookie;
            try
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), myEncoding);
                String str = reader.ReadToEnd();
                reader.Close();
                MatchCollection match = Regex.Matches(str, @"MainSong-..-[0-9]+");
                List<String[]> list = new List<string[]>();
                for(int i=start;i<= end;i++)
                {
                    String s = match[i].Value;
                    String t = "yc";
                    if (s.Contains("fc"))
                        t = "fc";
                    else if (s.Contains("bz"))
                        t = "bz";
                    list.Add(new string[] { s.Substring(s.LastIndexOf("-") + 1), t });
                }
                return list;
            }
            catch (Exception e)
            {
                return new List<string[]>();
            }
        }

        public static SongInformation GetSongInformation(String songId, String songType)
        {
            /*String url = "http://service.5sing.kugou.com/song/getPermission?songId="+songId+"&songType="+songType;
            Encoding myEncoding = Encoding.GetEncoding("utf-8");
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.Timeout = 5000;
            req.CookieContainer = cookie;
            try
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream());
                String str = reader.ReadToEnd();
                reader.Close();

                //写入class
                SongInformation result = new SongInformation();
                JObject jo = JObject.Parse(str);
                result.songId = songId;
                result.songType = songType;
                if (jo["success"].ToString() == "false")
                    return result;
                result.songName = jo["data"]["songName"].ToString();
                result.authorName = jo["data"]["authorName"].ToString();
                result.fileName = jo["data"]["fileName"].ToString();
                return result;
            }
            catch (Exception e)
            {
                return null;
            }*/

            String url = "http://5sing.kugou.com/m/detail/" + songType + "-" + songId + "-1.html";
            Encoding myEncoding = Encoding.GetEncoding("utf-8");
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.Timeout = 5000;
            try
            {
                HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream());
                String str = reader.ReadToEnd();
                reader.Close();

                //写入class
                SongInformation result = new SongInformation();
                result.songId = songId;
                result.songType = songType;
                String res = Regex.Match(str, @"http://.+preload").ToString();
                res = res.Substring(0, res.Length - 9);
                result.fileName = res;
                String s = Regex.Match(str, @"<title>.+ - 手机原创音乐基地</title>").ToString();
                s = s.Substring(0, s.IndexOf(" - 手机原创音乐基地</title>"));
                s = s.Substring(0, s.LastIndexOf(" - "));
                s = s.Substring(7);
                result.songName = s.Substring(0, s.LastIndexOf(" - "));
                result.authorName = s.Substring(s.LastIndexOf(" - ") + 3);
                return result;
            }
            catch (Exception e)
            {
                SongInformation result = new SongInformation();
                result.songId = songId;
                result.songType = songType;
                return result;
            }

        }

        public static SongInformation AddDownloadPath(SongInformation inf, String rootPath)
        {
            if (inf.songName == "" || inf.authorName == "")
                return inf;
            inf.path = rootPath + "\\" + MakeDirPath(inf.authorName);
            inf.file = MakeFilePath(inf.authorName + " - " + inf.songName + inf.fileName.Substring(inf.fileName.LastIndexOf(".")));
            return inf;
        }

        public static int DownloadSong(SongInformation inf, AsyncCompletedEventHandler handler)
        {
            if (inf == null || inf.fileName == "" || inf.path == "" || inf.file == "")
                return -1;

            if (!Directory.Exists(inf.path))
            {
                Directory.CreateDirectory(inf.path);
            }
            if (File.Exists(inf.path + "\\" + inf.file))
                return 0;

            WebClient webClient = new WebClient();
            webClient.DownloadFileAsync(new Uri(inf.fileName), inf.path + "\\" + inf.file);
            webClient.DownloadFileCompleted += handler;
            return 1;
        }

        public static String MakeDirPath(String path)
        {
            foreach (char c in illegalPath)
                path = path.Replace(c.ToString(), "");
            return path;
        }

        public static String MakeFilePath(String path)
        {
            foreach (char c in illegalFile)
                path = path.Replace(c.ToString(), "");
            return path;
        }

    }
}
