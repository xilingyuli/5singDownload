using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Download5sing_Clear
{
    public class Clear
    {
        private List<String[]> list;
        private String path;
        public Clear(int start, int end, String path)
        {
            list = CatchCommand.GetCollectList(start - 1, end - 1);
            this.path = path;
        }
        public void DoClear()
        {
            StreamReader sr = new StreamReader(path + "\\Success.txt");
            String success = sr.ReadToEnd();
            sr.Close();
            FileStream fs = new FileStream(path + "\\Clear.txt", FileMode.Append);
            foreach (String[] s in list)
            {
                if (!success.Contains("\n" + s[0] + " " + s[1]))
                {
                    SongInformation inf = CatchCommand.AddDownloadPath(CatchCommand.GetSongInformation(s[0], s[1]), path);
                    if (inf.file != "" && File.Exists(inf.path + "\\" + inf.file))
                    {
                        File.Delete(inf.path + "\\" + inf.file);
                        byte[] data = Encoding.Default.GetBytes(inf.path + "\\" + inf.file);
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                    }
                }
            }
            fs.Close();
        }
    }
}
