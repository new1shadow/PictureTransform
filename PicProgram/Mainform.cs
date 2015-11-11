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

//use Ctrl+M,Ctrl+O to fold the code.

namespace PicProgram
{
    public partial class Mainform : Form
    {
        const int MAXWH = 476;
        Bitmap White = new Bitmap(MAXWH, MAXWH);
        Bitmap input;
        Bitmap output;
        Bitmap show;
        Pictransformer pictran;

        public Mainform()
        {
            InitializeComponent();
        }

        private void Mainform_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedItem = "最近邻插值";
            checkBox1.Checked = true;
            checkBox1.Checked = false;
            pictran = new Pictransformer();
            Graphics Background;
            Background = Graphics.FromImage(White);
            Background.FillRectangle(new SolidBrush(Color.White), 0, 0, MAXWH, MAXWH);
            pictureBox1.BackgroundImage = White;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Enabled = checkBox1.Checked;
            radioButton2.Enabled = checkBox1.Checked;
            textBox3.Enabled = checkBox1.Checked;
            checkBox2.Enabled = checkBox1.Checked;
        }

        private void open_Click(object sender, EventArgs e)
        {
            if(input != null)
            {
                pictran.stop();
            }
            if (JPGEncoder.OpenFile(out input))
            { 
                pictureBox1.Image = White;
                if (MathWork.max(input.Width, input.Height) > MAXWH)
                {
                    double resized = (double)MAXWH / MathWork.max(input.Width, input.Height);
                    MessageBox.Show("图像太大，将以" + MathWork.round(100 * resized).ToString() + "%的比例缩放显示图片。");
                    Bitmap bpt = new Bitmap(MAXWH, MAXWH);
                    Pictransformer ptmp = new Pictransformer();
                    ptmp.start(input, progressBar1);
                    ptmp.stretchpicture(resized, resized, Pictransformer.Stretching.Nearest, out bpt);
                    pictureBox1.Image = bpt;
                    ptmp.stop2();
                }
                else
                {
                    pictureBox1.Image = input;
                }
                pictran.start(input, progressBar1);
                save.Enabled = false;
                insert.Enabled = true;
            }
        }

        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (input != null)
                pictran.stop();
        }

        private void insert_Click(object sender, EventArgs e)
        {
            double strx, stry, angle = 0;
            try
            {
                strx = Double.Parse(textBox1.Text);
                stry = Double.Parse(textBox2.Text);
                if (checkBox1.Checked)
                {
                    if (radioButton1.Checked)
                        angle = Double.Parse(textBox3.Text) * MathWork.pi() / 180;
                    else
                        angle = Double.Parse(textBox3.Text);
                }
                if (MathWork.abs(angle) < 0.000001D)
                {
                    if ((string)comboBox1.SelectedItem == "最近邻插值")
                        pictran.stretchpicture(strx, stry, Pictransformer.Stretching.Nearest, out output);
                    else if ((string)comboBox1.SelectedItem == "双线性插值")
                        pictran.stretchpicture(strx, stry, Pictransformer.Stretching.Bilinear, out output);
                    else if ((string)comboBox1.SelectedItem == "双三次插值")
                        pictran.stretchpicture(strx, stry, Pictransformer.Stretching.Bicubic, out output);
                }
                else
                {
                    if ((string)comboBox1.SelectedItem == "最近邻插值")
                        pictran.rotate_stretch(strx, stry, Pictransformer.Stretching.Nearest, angle, out output);
                    else if ((string)comboBox1.SelectedItem == "双线性插值")
                        pictran.rotate_stretch(strx, stry, Pictransformer.Stretching.Bilinear, angle, out output);
                    else if ((string)comboBox1.SelectedItem == "双三次插值")
                        pictran.rotate_stretch(strx, stry, Pictransformer.Stretching.Bicubic, angle, out output);
                    if (checkBox2.Checked)
                        pictran.crop(out output);
                }
                if (output == null)
                    throw new Exception();
                pictureBox1.Image = White;
                if (MathWork.max(output.Width, output.Height) > MAXWH)
                {
                    double resized = (double)MAXWH / MathWork.max(output.Width, output.Height);
                    MessageBox.Show("图像太大，将以" + MathWork.round(100 * resized).ToString() + "%的比例缩放显示图片。若想查看图片全貌，请保存图片。");
                    Bitmap bpt = new Bitmap(MAXWH, MAXWH);
                    Pictransformer ptmp = new Pictransformer();
                    ptmp.start(output, progressBar1);
                    ptmp.stretchpicture(resized, resized, Pictransformer.Stretching.Nearest, out bpt);
                    pictureBox1.Image = bpt;
                    ptmp.stop2();
                }
                else
                {
                    pictureBox1.Image = output;
                }
                save.Enabled = true;
            }
            catch (Exception es)
            {
                MessageBox.Show("参数错误！（角度请小于360°，大于0°）");
                save.Enabled = false;
                return;
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            JPGEncoder.SaveFile(output);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictran.crop(out output);
            pictureBox1.Image = output;
        }
    }
}
