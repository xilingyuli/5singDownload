using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Net;

namespace Download5sing
{

    public delegate void FinishCallBack(bool isSuccess);

    public class Download
    {
        private SongInformation inf;
        private String rootPath;
        private FinishCallBack func;

        public Download(String songId, String songType, String rootPath, FinishCallBack func)
        {
            inf = new SongInformation();
            inf.songId = songId;
            inf.songType = songType;
            this.rootPath = rootPath;
            this.func = func;
        }

        public void StartDownload(Object o)
        {
            inf = CatchCommand.GetSongInformation(inf.songId, inf.songType);
            inf = CatchCommand.AddDownloadPath(inf, rootPath);
            int t = CatchCommand.DownloadSong(inf, new AsyncCompletedEventHandler(DownloadFinish));
            if (t == 0)
                func(true);
            else if (t == -1)
                func(false);
        }

        public void DownloadFinish(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled)
            {
                DownloadFailed();
                func(false);
            }
            else
            {
                DownloadSuccess();
                func(true);
            }
        }

        public void DownloadSuccess()
        {
            FileStream fs = new FileStream(rootPath + "\\Success.txt", FileMode.Append);
            byte[] data = Encoding.Default.GetBytes(inf.songId + " " + inf.songType + " "
                + inf.songName + " " + inf.authorName + " " + inf.fileName + " " + "下载成功\r\n");
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
        }

        public void DownloadFailed()
        {
            if (File.Exists(inf.path + "\\" + inf.file))
                File.Delete(inf.path + "\\" + inf.file);

            FileStream fs = new FileStream(rootPath + "\\Fail.txt", FileMode.Append);
            byte[] data = Encoding.Default.GetBytes(inf.songId +" "+ inf.songType + " " 
                + inf.songName + " " + inf.authorName + " " + inf.fileName + " " + "下载失败\r\n");
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
        }

        public bool CheckExist()
        {
            if (inf.fileName == "")
                inf = CatchCommand.GetSongInformation(inf.songId, inf.songType);
            if (inf.fileName == "")
                return false;
            if (inf.file == "")
                inf = CatchCommand.AddDownloadPath(inf, rootPath);
            if (inf.file == "")
                return false;
            if (File.Exists(inf.path + "\\" + inf.file))
                return true;
            return false;
        }
    }
}
