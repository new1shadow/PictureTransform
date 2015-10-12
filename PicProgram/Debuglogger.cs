using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PicProgram
{
    static class DebugLogger
    {
        private static string s = string.Empty;
        public static bool LogLine(string c)
        {
            s += "\r\n";
            s += c;
            return true;
        }
        public static bool Log(string c)
        {
            s += c;
            return true;
        }
        public static bool LogTimeStamp()
        {
            LogLine("TimeStamp: " + GetTimeStamp());
            return true;
        }
        public static string GetLog()
        {
            return s;
        }
        public static bool Clean()
        {
            s = string.Empty;
            return true;
        }
        public static bool SaveFile()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "保存日志";
                saveFileDialog.OverwritePrompt = true;
                saveFileDialog.CheckPathExists = true;
                saveFileDialog.Filter = "txt文本(*.txt)|*.txt";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = saveFileDialog.FileName;
                    FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(s);
                    sw.Close();
                    fs.Close();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static bool SaveFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName)) File.Delete(fileName);
                FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(s);
                sw.Close();
                fs.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(2015, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        } 
    }
}
