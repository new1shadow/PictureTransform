//JPGEncoder
//load and save jpg files.
//Author:ShadowK
//email:zhu.shadowk@gmail.com
//2015.10.11
//Use Ctrl+M,Ctrl+O to fold the code.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace PicProgram
{
    //Read Jpg and turn it into Bmp(for calculate).
    public static class JPGEncoder
    {
        public static bool OpenFile(out Bitmap image)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "*.jpg|*.jpg";
            openFileDialog.Title = "打开图像文件";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                image = new Bitmap(fileName);
                return true;
            }
            else
            {
                image = null;
                return false;
            }
        }
        public static bool SaveFile(Bitmap image)
        {
            if (image == null)
                return false;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "保存图片";
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.Filter = "jpg格式图片(*.jpg)|*.jpg" + "|" + "jpeg格式图片(*.jpeg)|*.jpeg";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                string filePath = Path.GetDirectoryName(fileName);
                filePath += "\\log.txt";
                DebugLogger.SaveFile(filePath);
                image.Save(fileName, ImageFormat.Jpeg);
                return true;
            }
            return false;
        }
    }
}
