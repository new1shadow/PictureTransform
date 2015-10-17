//#define DEBUG

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace PicProgram
{
    public partial class Mainform : Form
    {
        public Mainform()
        {
            InitializeComponent();
        }

        private void Mainform_Load(object sender, EventArgs e)
        {
            Bitmap image;
            Bitmap ansimage;
            DebugLogger.ResetTime();
            DebugLogger.LogTimeStamp();
            while (!JPGEncoder.OpenFile(out image)) if (image != null) image.Dispose();
            Pictransformer pictran = new Pictransformer();
            pictran.start(image);
            pictran.stretchpicture(2, 2, Pictransformer.Stretching.Bicubic, out ansimage);
            DebugLogger.LogTimeStamp();
            JPGEncoder.SaveFile(ansimage);
            pictran.stop();
            DebugLogger.ResetTime();
            DebugLogger.SaveFile();
            Close();
        }
    }
}
