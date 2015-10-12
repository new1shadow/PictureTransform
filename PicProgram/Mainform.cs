#define DEBUG

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
            //Matrix a = new Matrix(3, 3, new float[,] { { 1, 4, 6 }, { 6, 2, 1 }, { 6, 2, 6 } });
            //Matrix b = new Matrix(3, 2, new float[,] { { 4, 6 }, { 6, 22 }, { 6.2F, 6 } });
            //Matrix c = Matrix.Zero;
            //MathWork.MatrixMultiply(a, b,ref c);
            //textBox1.Text = (5*b).ToString();
            //textBox1.Text += Matrix.Zero.ToStringWithMatlab();
            Bitmap image;
            Bitmap ansimage;
            DebugLogger.LogTimeStamp();
            while (!JPGEncoder.OpenFile(out image)) if(image != null)   image.Dispose();
            Pictransformer pictran = new Pictransformer();
            pictran.start(image);
            DebugLogger.LogTimeStamp();
            pictran.stretchpicture(2, 2, Pictransformer.Stretching.Bilinear,out ansimage);
            DebugLogger.LogTimeStamp();
            //DebugLogger.LogLine("TEST");
            JPGEncoder.SaveFile(ansimage);
            DebugLogger.LogTimeStamp();
            Close();
        }
    }
}
