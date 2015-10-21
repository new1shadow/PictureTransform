//#define MYDEBUG
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
            while (!JPGEncoder.OpenFile(out image)) if (image != null) image.Dispose();
            DebugLogger.ResetTime();
            DebugLogger.LogTimeStamp();
            Pictransformer pictran = new Pictransformer();
            pictran.start(image);
            //pictran.stretchpicture(0.5, 0.5, Pictransformer.Stretching.Bicubic, out ansimage);
            pictran.rotate_stretch(2, 2, Pictransformer.Stretching.Bicubic, 2.0, out ansimage);
            DebugLogger.LogTimeStamp();
            JPGEncoder.SaveFile(ansimage);
            pictran.stop();
            DebugLogger.ResetTime();
            DebugLogger.SaveFile();
            Close();
        }
    }
}
