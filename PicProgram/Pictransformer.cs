//Pictransformer
//Transform the picture by order.
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

namespace PicProgram
{

    public partial class Pictransformer
    {
        public enum Stretching { Nearest, Bilinear, Bicubic };

        //Initialize
        public bool start(Bitmap image, ProgressBar pbs)
        {
            origin = new Bitmap(image);
            orpb = new PointBitmap(origin);
            orpb.LockBits();
            pb = pbs;
            //Bigmapwidth = MathWork.round(2 * MathWork.sqrt(MathWork.power(origin.Width, 2) + MathWork.power(origin.Height, 2)));
            return true;
        }

        public bool stretchpicture(double strx, double stry, Stretching kind, out Bitmap ans)
        {
            laststrx = strx;
            laststry = stry;
            try
            {
                int diagonal = CalculateDiagonal(origin.Size, strx, stry);
                Size anssize = CalculateSizeStretching(origin.Size, strx, stry, diagonal);
                if (output != null)
                    output.Dispose();
                output = new Bitmap(anssize.Width, anssize.Height);
                pb.Maximum = anssize.Height;
                oupb = new PointBitmap(output);
                oupb.LockBits();
                double oripx, oripy;
                int orixint;
                int oriyint;
                double u, v, temp;
                int ansr = 0, ansg = 0, ansb = 0;
                int oldxint = 0, oldyint = 0;
                double Sup1 = 1, Sup0 = 1, Sus1 = 1, Sus2 = 1, Svp1 = 1, Svp0 = 2, Svs1 = 3, Svs2 = 1;
                double kk1r = 1, kk2r = 1, kk3r = 1, kk4r = 1, kk1g = 1, kk2g = 2, kk3g = 3, kk4g = 2, kk1b = 3, kk2b = 2, kk3b = 1, kk4b = 2;
                Color a11, a12, a13, a14, a21, a22, a23, a24, a31, a32, a33, a34, a41, a42, a43, a44;
                #region init a11 to a44
                a11 = Color.Empty;
                a12 = Color.Empty;
                a13 = Color.Empty;
                a14 = Color.Empty;
                a21 = Color.Empty;
                a22 = Color.Empty;
                a23 = Color.Empty;
                a24 = Color.Empty;
                a31 = Color.Empty;
                a32 = Color.Empty;
                a33 = Color.Empty;
                a34 = Color.Empty;
                a41 = Color.Empty;
                a42 = Color.Empty;
                a43 = Color.Empty;
                a44 = Color.Empty;
                ExtraGetPixelRef(0, 0, ref a11);
                ExtraGetPixelRef(0, 0, ref a12);
                ExtraGetPixelRef(1, 0, ref a13);
                ExtraGetPixelRef(2, 0, ref a14);
                ExtraGetPixelRef(0, 0, ref a21);
                ExtraGetPixelRef(0, 0, ref a22);
                ExtraGetPixelRef(1, 0, ref a23);
                ExtraGetPixelRef(2, 0, ref a24);
                ExtraGetPixelRef(0, 1, ref a31);
                ExtraGetPixelRef(0, 1, ref a32);
                ExtraGetPixelRef(1, 1, ref a33);
                ExtraGetPixelRef(2, 1, ref a34);
                ExtraGetPixelRef(0, 2, ref a41);
                ExtraGetPixelRef(0, 2, ref a42);
                ExtraGetPixelRef(1, 2, ref a43);
                ExtraGetPixelRef(2, 2, ref a44);
                #endregion
                Color x_y, x_1_y, x_y_1, x_1_y_1;
                #region init x_y to x_1_y_1
                x_y = ExtraGetPixel(0, 0);
                x_1_y = ExtraGetPixel(1, 0);
                x_y_1 = ExtraGetPixel(0, 1);
                x_1_y_1 = ExtraGetPixel(1, 1);
                #endregion
                #region ComputeEveryPixel
                for (int i = 0; i < anssize.Height; i++)
                {
                    pb.PerformStep();
                    oripy = (i / stry);
                    oriyint = MathWork.floor(oripy);
                    v = oripy - oriyint;
                    for (int j = 0; j < anssize.Width; j++)
                    {
                        //PointF orip = ReachOriginStretching(new Point(j, i), strx, stry, diagonal);
                        oripx = (j / strx);
                        orixint = MathWork.floor(oripx);
                        u = oripx - orixint;
                        switch (kind)
                        {
                            case Stretching.Nearest:
                                //output.SetPixel(j, i, ExtraGetPixel(orixint, oriyint));
                                oupb.SetPixel(j, i, ExtraGetPixel(orixint, oriyint));
                                break;
                            case Stretching.Bilinear:

                                //This kind of Matrix calculating seens to be very slow.
                                //Changing it into a faster type.
                                #region OldMethod
                                //Matrix m1 = new Matrix(1, 2, new double[,] { { 1 - u, u } });
                                //Matrix m2r = new Matrix(2, 2, new double[,] { { (double)ExtraGetPixel(orixint, oriyint).R, 
                                //                                                (double)ExtraGetPixel(orixint, oriyint + 1).R }, 
                                //                                              { (double)ExtraGetPixel(orixint + 1, oriyint).R, 
                                //                                                (double)ExtraGetPixel(orixint + 1, oriyint + 1).R } });
                                //Matrix m2g = new Matrix(2, 2, new double[,] { { (double)ExtraGetPixel(orixint, oriyint).G, 
                                //                                                (double)ExtraGetPixel(orixint, oriyint + 1).G }, 
                                //                                              { (double)ExtraGetPixel(orixint + 1, oriyint).G, 
                                //                                                (double)ExtraGetPixel(orixint + 1, oriyint + 1).G } });
                                //Matrix m2b = new Matrix(2, 2, new double[,] { { (double)ExtraGetPixel(orixint, oriyint).B, 
                                //                                                (double)ExtraGetPixel(orixint, oriyint + 1).B }, 
                                //                                              { (double)ExtraGetPixel(orixint + 1, oriyint).B, 
                                //                                                (double)ExtraGetPixel(orixint + 1, oriyint + 1).B } });
                                //Matrix m3 = new Matrix(2, 1, new double[,] { { 1 - v }, { v } });
                                //int ansr = MathWork.upcolor(MathWork.round((m1 * m2r * m3).GetData(0, 0)));
                                //int ansg = MathWork.upcolor(MathWork.round((m1 * m2g * m3).GetData(0, 0)));
                                //int ansb = MathWork.upcolor(MathWork.round((m1 * m2b * m3).GetData(0, 0)));

                                //int u_1000 = MathWork.round(u * 1000);
                                //int v_1000 = MathWork.round(v * 1000);                            
                                //int ansr = MathWork.upcolor((
                                //    (1000 - v_1000) * ((1000 - u_1000) * ExtraGetPixel(orixint, oriyint).R + u_1000 *     ExtraGetPixel(orixint + 1, oriyint).R) +
                                //            v_1000 * ((1 - u_1000) * ExtraGetPixel(orixint, oriyint + 1).R + u_1000 * ExtraGetPixel(orixint + 1, oriyint + 1).R)) / 1000000);
                                //int ansg = MathWork.upcolor((
                                //    (1000 - v_1000) * ((1000 - u_1000) * ExtraGetPixel(orixint, oriyint).G + u_1000 *     ExtraGetPixel(orixint + 1, oriyint).G) +
                                //            v_1000 * ((1 - u_1000) * ExtraGetPixel(orixint, oriyint + 1).G + u_1000 * ExtraGetPixel(orixint + 1, oriyint + 1).G)) / 1000000);
                                //int ansb = MathWork.upcolor((
                                //    (1000 - v_1000) * ((1000 - u_1000) * ExtraGetPixel(orixint, oriyint).B + u_1000 *     ExtraGetPixel(orixint + 1, oriyint).B) +
                                //            v_1000 * ((1 - u_1000) * ExtraGetPixel(orixint, oriyint + 1).B + u_1000 * ExtraGetPixel(orixint + 1, oriyint + 1).B)) / 1000000);

                                //int ansr = MathWork.upcolor(MathWork.round(
                                //    (1-v)*(    (1-u)*ExtraGetPixel(orixint, oriyint).R+    u*ExtraGetPixel(orixint + 1, oriyint).R)+
                                //        v*((1-u)*ExtraGetPixel(orixint, oriyint + 1).R+u*ExtraGetPixel(orixint + 1, oriyint + 1).R)));
                                //int ansg = MathWork.upcolor(MathWork.round(
                                //    (1-v)*(    (1-u)*ExtraGetPixel(orixint, oriyint).G+    u*ExtraGetPixel(orixint + 1, oriyint).G)+
                                //        v*((1-u)*ExtraGetPixel(orixint, oriyint + 1).G+u*ExtraGetPixel(orixint + 1, oriyint + 1).G)));
                                //int ansb = MathWork.upcolor(MathWork.round(
                                //    (1-v)*(    (1-u)*ExtraGetPixel(orixint, oriyint).B+    u*ExtraGetPixel(orixint + 1, oriyint).B)+
                                //        v*((1-u)*ExtraGetPixel(orixint, oriyint + 1).B+u*ExtraGetPixel(orixint + 1, oriyint + 1).B)));

                                //int ansg = MathWork.upcolor(MathWork.round((1 - v) * (x_y.G + u * (x_1_y.G - x_y.G)) + v * (x_y_1.G + u * (x_1_y_1.G - x_y_1.G))));
                                //int ansb = MathWork.upcolor(MathWork.round((1 - v) * (x_y.B + u * (x_1_y.B - x_y.B)) + v * (x_y_1.B + u * (x_1_y_1.B - x_y_1.B))));
                                //output.SetPixel(j, i, Color.FromArgb(ansr, ansg, ansb));
                                #endregion
                                if (oldxint != orixint && oldyint != oriyint)
                                {
                                    x_y = ExtraGetPixel(orixint, oriyint);
                                    x_1_y = ExtraGetPixel(orixint + 1, oriyint);
                                    x_y_1 = ExtraGetPixel(orixint, oriyint + 1);
                                    x_1_y_1 = ExtraGetPixel(orixint + 1, oriyint + 1);
                                }
                                else if (oldxint != orixint)
                                {
                                    x_y = x_1_y;
                                    x_y_1 = x_1_y_1;
                                    x_1_y = ExtraGetPixel(orixint + 1, oriyint);
                                    x_1_y_1 = ExtraGetPixel(orixint + 1, oriyint + 1);
                                }
                                temp = x_y.R + u * (x_1_y.R - x_y.R);
                                ansr = MathWork.upcolor(MathWork.round(temp + v * (x_y_1.R - temp + u * (x_1_y_1.R - x_y_1.R))));
                                temp = x_y.G + u * (x_1_y.G - x_y.G);
                                ansg = MathWork.upcolor(MathWork.round(temp + v * (x_y_1.G - temp + u * (x_1_y_1.G - x_y_1.G))));
                                temp = x_y.B + u * (x_1_y.B - x_y.B);
                                ansb = MathWork.upcolor(MathWork.round(temp + v * (x_y_1.B - temp + u * (x_1_y_1.B - x_y_1.B))));
                                oupb.SetPixel(j, i, Color.FromArgb(ansr, ansg, ansb));
                                break;
                            case Stretching.Bicubic:
                                //This kind of Matrix calculating seens to be very slow.
                                //Changing it into a faster type.
                                #region OldMethod
                                //double u2 = orip.X - (double)MathWork.floor(orip.X);
                                //double v2 = orip.Y - (double)MathWork.floor(orip.Y);
                                //Matrix A = new Matrix(1, 4, new double[,] { { MathWork.KernelFunS(1 + u2), MathWork.KernelFunS(u2), MathWork.KernelFunS(1 - u2), MathWork.KernelFunS(2 - u2) } });
                                //Matrix Br = new Matrix(4, 4, new double[,]{{(double)ExtraGetPixel(orixint-1,oriyint-1).R,(double)ExtraGetPixel(orixint-1,oriyint-0).R,(double)ExtraGetPixel(orixint-1,oriyint+1).R,(double)ExtraGetPixel(orixint-1,oriyint+2).R},
                                //                                           {(double)ExtraGetPixel(orixint-0,oriyint-1).R,(double)ExtraGetPixel(orixint-0,oriyint-0).R,(double)ExtraGetPixel(orixint-0,oriyint+1).R,(double)ExtraGetPixel(orixint-0,oriyint+2).R},
                                //                                           {(double)ExtraGetPixel(orixint+1,oriyint-1).R,(double)ExtraGetPixel(orixint+1,oriyint-0).R,(double)ExtraGetPixel(orixint+1,oriyint+1).R,(double)ExtraGetPixel(orixint+1,oriyint+2).R},
                                //                                           {(double)ExtraGetPixel(orixint+2,oriyint-1).R,(double)ExtraGetPixel(orixint+2,oriyint-0).R,(double)ExtraGetPixel(orixint+2,oriyint+1).R,(double)ExtraGetPixel(orixint+2,oriyint+2).R}});
                                //Matrix Bg = new Matrix(4, 4, new double[,]{{(double)ExtraGetPixel(orixint-1,oriyint-1).G,(double)ExtraGetPixel(orixint-1,oriyint-0).G,(double)ExtraGetPixel(orixint-1,oriyint+1).G,(double)ExtraGetPixel(orixint-1,oriyint+2).G},
                                //                                           {(double)ExtraGetPixel(orixint-0,oriyint-1).G,(double)ExtraGetPixel(orixint-0,oriyint-0).G,(double)ExtraGetPixel(orixint-0,oriyint+1).G,(double)ExtraGetPixel(orixint-0,oriyint+2).G},
                                //                                           {(double)ExtraGetPixel(orixint+1,oriyint-1).G,(double)ExtraGetPixel(orixint+1,oriyint-0).G,(double)ExtraGetPixel(orixint+1,oriyint+1).G,(double)ExtraGetPixel(orixint+1,oriyint+2).G},
                                //                                           {(double)ExtraGetPixel(orixint+2,oriyint-1).G,(double)ExtraGetPixel(orixint+2,oriyint-0).G,(double)ExtraGetPixel(orixint+2,oriyint+1).G,(double)ExtraGetPixel(orixint+2,oriyint+2).G}});
                                //Matrix Bb = new Matrix(4, 4, new double[,]{{(double)ExtraGetPixel(orixint-1,oriyint-1).B,(double)ExtraGetPixel(orixint-1,oriyint-0).B,(double)ExtraGetPixel(orixint-1,oriyint+1).B,(double)ExtraGetPixel(orixint-1,oriyint+2).B},
                                //                                           {(double)ExtraGetPixel(orixint-0,oriyint-1).B,(double)ExtraGetPixel(orixint-0,oriyint-0).B,(double)ExtraGetPixel(orixint-0,oriyint+1).B,(double)ExtraGetPixel(orixint-0,oriyint+2).B},
                                //                                           {(double)ExtraGetPixel(orixint+1,oriyint-1).B,(double)ExtraGetPixel(orixint+1,oriyint-0).B,(double)ExtraGetPixel(orixint+1,oriyint+1).B,(double)ExtraGetPixel(orixint+1,oriyint+2).B},
                                //                                           {(double)ExtraGetPixel(orixint+2,oriyint-1).B,(double)ExtraGetPixel(orixint+2,oriyint-0).B,(double)ExtraGetPixel(orixint+2,oriyint+1).B,(double)ExtraGetPixel(orixint+2,oriyint+2).B}});
                                //Matrix C = new Matrix(4, 1, new double[,] { { MathWork.KernelFunS(1 + v2) }, { MathWork.KernelFunS(v2) }, { MathWork.KernelFunS(1 - v2) }, { MathWork.KernelFunS(2 - v2) } });
                                //int fansr = MathWork.upcolor(MathWork.round((A * Br * C).GetData(0, 0)));
                                //int fansg = MathWork.upcolor(MathWork.round((A * Bg * C).GetData(0, 0)));
                                //int fansb = MathWork.upcolor(MathWork.round((A * Bb * C).GetData(0, 0)));
                                //output.SetPixel(j, i, Color.FromArgb(fansr, fansg, fansb));
                                #endregion

                                Sup1 = MathWork.KernelFunS(u + 1);
                                Sup0 = MathWork.KernelFunS(u + 0);
                                Sus1 = MathWork.KernelFunS(u - 1);
                                Sus2 = MathWork.KernelFunS(u - 2);
                                Svp1 = MathWork.KernelFunS(v + 1);
                                Svp0 = MathWork.KernelFunS(v + 0);
                                Svs1 = MathWork.KernelFunS(v - 1);
                                Svs2 = MathWork.KernelFunS(v - 2);
                                if (oldxint != orixint && oldyint != oriyint)
                                {
                                    ExtraGetPixelRef(orixint - 1, oriyint - 1, ref a11);
                                    ExtraGetPixelRef(orixint - 0, oriyint - 1, ref a12);
                                    ExtraGetPixelRef(orixint + 1, oriyint - 1, ref a13);
                                    ExtraGetPixelRef(orixint + 2, oriyint - 1, ref a14);
                                    ExtraGetPixelRef(orixint - 1, oriyint - 0, ref a21);
                                    ExtraGetPixelRef(orixint - 0, oriyint - 0, ref a22);
                                    ExtraGetPixelRef(orixint + 1, oriyint - 0, ref a23);
                                    ExtraGetPixelRef(orixint + 2, oriyint - 0, ref a24);
                                    ExtraGetPixelRef(orixint - 1, oriyint + 1, ref a31);
                                    ExtraGetPixelRef(orixint - 0, oriyint + 1, ref a32);
                                    ExtraGetPixelRef(orixint + 1, oriyint + 1, ref a33);
                                    ExtraGetPixelRef(orixint + 2, oriyint + 1, ref a34);
                                    ExtraGetPixelRef(orixint - 1, oriyint + 2, ref a41);
                                    ExtraGetPixelRef(orixint - 0, oriyint + 2, ref a42);
                                    ExtraGetPixelRef(orixint + 1, oriyint + 2, ref a43);
                                    ExtraGetPixelRef(orixint + 2, oriyint + 2, ref a44);
                                }
                                else if (oldxint != orixint)
                                {
                                    a11 = a12;
                                    a12 = a13;
                                    a13 = a14;
                                    ExtraGetPixelRef(orixint + 2, oriyint - 1, ref a14);
                                    a21 = a22;
                                    a22 = a23;
                                    a23 = a24;
                                    ExtraGetPixelRef(orixint + 2, oriyint - 0, ref a24);
                                    a31 = a32;
                                    a32 = a33;
                                    a33 = a34;
                                    ExtraGetPixelRef(orixint + 2, oriyint + 1, ref a34);
                                    a41 = a42;
                                    a42 = a43;
                                    a43 = a44;
                                    ExtraGetPixelRef(orixint + 2, oriyint + 2, ref a44);
                                }
                                kk1r = Sup1 * a11.R + Sup0 * a12.R + Sus1 * a13.R + Sus2 * a14.R;
                                kk2r = Sup1 * a21.R + Sup0 * a22.R + Sus1 * a23.R + Sus2 * a24.R;
                                kk3r = Sup1 * a31.R + Sup0 * a32.R + Sus1 * a33.R + Sus2 * a34.R;
                                kk4r = Sup1 * a41.R + Sup0 * a42.R + Sus1 * a43.R + Sus2 * a44.R;
                                kk1g = Sup1 * a11.G + Sup0 * a12.G + Sus1 * a13.G + Sus2 * a14.G;
                                kk2g = Sup1 * a21.G + Sup0 * a22.G + Sus1 * a23.G + Sus2 * a24.G;
                                kk3g = Sup1 * a31.G + Sup0 * a32.G + Sus1 * a33.G + Sus2 * a34.G;
                                kk4g = Sup1 * a41.G + Sup0 * a42.G + Sus1 * a43.G + Sus2 * a44.G;
                                kk1b = Sup1 * a11.B + Sup0 * a12.B + Sus1 * a13.B + Sus2 * a14.B;
                                kk2b = Sup1 * a21.B + Sup0 * a22.B + Sus1 * a23.B + Sus2 * a24.B;
                                kk3b = Sup1 * a31.B + Sup0 * a32.B + Sus1 * a33.B + Sus2 * a34.B;
                                kk4b = Sup1 * a41.B + Sup0 * a42.B + Sus1 * a43.B + Sus2 * a44.B;
                                ansr = MathWork.upcolor(MathWork.round(Svp1 * kk1r + Svp0 * kk2r + Svs1 * kk3r + Svs2 * kk4r));
                                ansg = MathWork.upcolor(MathWork.round(Svp1 * kk1g + Svp0 * kk2g + Svs1 * kk3g + Svs2 * kk4g));
                                ansb = MathWork.upcolor(MathWork.round(Svp1 * kk1b + Svp0 * kk2b + Svs1 * kk3b + Svs2 * kk4b));
                                oupb.SetPixel(j, i, Color.FromArgb(ansr, ansg, ansb));
                                break;
                        }
                        oldxint = orixint;
                    }
                    oldyint = oriyint;
#if MYDEBUG
                    DebugLogger.LogTimeStamp();
#endif
                }
                #endregion

                oupb.UnlockBits();
                ans = output;
                pb.Value = 0;
                return true;

            }
            catch (Exception e)
            {
                ans = null;
                return false;
            }
        }

        public bool rotatepicture(double angle, Stretching kind, out Bitmap ans)
        {
            return rotate_stretch(1, 1, kind, angle, out ans);
        }

        public bool rotate_stretch(double strx, double stry, Stretching kind, double angle, out Bitmap ans)
        {
            if (angle > MathWork.pi() * 2 || angle < 0)
            {
                if (output != null)
                    output.Dispose();
                output = null;
                ans = null;
                return false;
            }
            try
            {
                laststrx = strx;
                laststry = stry;
                int diagonal = CalculateDiagonal(origin.Size, strx, stry);
                Size anssize = CalculateSize(origin.Size, strx, stry, angle, diagonal);
                int rkind = 0;
                int basement = 0;
                double xend = 0, yend = 0;
                if (angle < MathWork.pi() / 2)
                {
                    rkind = 1;
                    MathWork.rotate(diagonal, diagonal, diagonal, diagonal + origin.Height * stry, angle, ref xend, ref yend);
                    basement = MathWork.round(xend);
                }
                else if (angle < MathWork.pi())
                {
                    rkind = 2;
                    MathWork.rotate(diagonal, diagonal, diagonal, diagonal + origin.Height * stry, angle, ref xend, ref yend);
                    basement = MathWork.round(yend);
                }
                else if (angle < MathWork.pi() * 3 / 2)
                {
                    rkind = 3;
                    MathWork.rotate(diagonal, diagonal, diagonal + origin.Width * strx, diagonal, angle, ref xend, ref yend);
                    basement = MathWork.round(xend);
                }
                else
                {
                    rkind = 4;
                    MathWork.rotate(diagonal, diagonal, diagonal + origin.Width * strx, diagonal, angle, ref xend, ref yend);
                    basement = MathWork.round(yend);
                }

                if (output != null)
                    output.Dispose();
                output = new Bitmap(anssize.Width, anssize.Height);
                pb.Maximum = anssize.Height;
                oupb = new PointBitmap(output);
                oupb.LockBits();
                #region ComputeEveryPixel
                double u, v, temp;
                int ansr = 0, ansg = 0, ansb = 0;
                int oldxint = 0, oldyint = 0;
                int oriyint = 0, orixint = 0;
                double Sup1 = 1, Sup0 = 1, Sus1 = 1, Sus2 = 1, Svp1 = 1, Svp0 = 2, Svs1 = 3, Svs2 = 1;
                double kk1r = 1, kk2r = 1, kk3r = 1, kk4r = 1, kk1g = 1, kk2g = 2, kk3g = 3, kk4g = 2, kk1b = 3, kk2b = 2, kk3b = 1, kk4b = 2;
                Color a11, a12, a13, a14, a21, a22, a23, a24, a31, a32, a33, a34, a41, a42, a43, a44;
                #region init a11 to a44
                a11 = Color.Empty;
                a12 = Color.Empty;
                a13 = Color.Empty;
                a14 = Color.Empty;
                a21 = Color.Empty;
                a22 = Color.Empty;
                a23 = Color.Empty;
                a24 = Color.Empty;
                a31 = Color.Empty;
                a32 = Color.Empty;
                a33 = Color.Empty;
                a34 = Color.Empty;
                a41 = Color.Empty;
                a42 = Color.Empty;
                a43 = Color.Empty;
                a44 = Color.Empty;
                ExtraGetPixelRef(0, 0, ref a11);
                ExtraGetPixelRef(0, 0, ref a12);
                ExtraGetPixelRef(1, 0, ref a13);
                ExtraGetPixelRef(2, 0, ref a14);
                ExtraGetPixelRef(0, 0, ref a21);
                ExtraGetPixelRef(0, 0, ref a22);
                ExtraGetPixelRef(1, 0, ref a23);
                ExtraGetPixelRef(2, 0, ref a24);
                ExtraGetPixelRef(0, 1, ref a31);
                ExtraGetPixelRef(0, 1, ref a32);
                ExtraGetPixelRef(1, 1, ref a33);
                ExtraGetPixelRef(2, 1, ref a34);
                ExtraGetPixelRef(0, 2, ref a41);
                ExtraGetPixelRef(0, 2, ref a42);
                ExtraGetPixelRef(1, 2, ref a43);
                ExtraGetPixelRef(2, 2, ref a44);
                #endregion
                Color x_y, x_1_y, x_y_1, x_1_y_1;
                #region init x_y to x_1_y_1
                x_y = ExtraGetPixel(0, 0);
                x_1_y = ExtraGetPixel(1, 0);
                x_y_1 = ExtraGetPixel(0, 1);
                x_1_y_1 = ExtraGetPixel(1, 1);
                #endregion
                int i, j;
                for (i = 0; i < anssize.Height; i++)
                {
                    pb.PerformStep();
                    for (j = 0; j < anssize.Width; j++)
                    {
                        Point now = new Point();
                        if (rkind == 1)
                        {
                            now.X = basement + j;
                            now.Y = anssize.Height - i + diagonal;
                            //now.Y = diagonal + i;
                        }
                        else if (rkind == 2)
                        {
                            now.X = j + diagonal - anssize.Width;
                            now.Y = basement + anssize.Height - i;
                            //now.Y = basement + i;
                        }
                        else if (rkind == 3)
                        {
                            now.X = basement + j;
                            now.Y = diagonal - i;
                            //now.Y = i + diagonal - origin.Height;
                        }
                        else
                        {
                            now.X = diagonal + j;
                            now.Y = basement + anssize.Height - i;
                            //now.Y = basement + i;
                        }
                        PointF orip = ReachOrigin(now, strx, stry, angle, diagonal);
                        if (orip == BlackPoint)
                        {
                            oupb.SetPixel(j, i, Color.Black);
                            //oldxint = orixint;
                            continue;
                        }
                        oriyint = MathWork.floor(orip.Y);
                        orixint = MathWork.floor(orip.X);
                        v = orip.Y - oriyint;
                        u = orip.X - orixint;
                        #region switch
                        switch (kind)
                        {
                            case Stretching.Nearest:

                                oupb.SetPixel(j, i, ExtraGetPixel(orixint, oriyint));
                                break;
                            case Stretching.Bilinear:

                                if (oldxint != orixint || oldyint != oriyint)
                                {
                                    x_y = ExtraGetPixel(orixint, oriyint);
                                    x_1_y = ExtraGetPixel(orixint + 1, oriyint);
                                    x_y_1 = ExtraGetPixel(orixint, oriyint + 1);
                                    x_1_y_1 = ExtraGetPixel(orixint + 1, oriyint + 1);
                                }
                                temp = x_y.R + u * (x_1_y.R - x_y.R);
                                ansr = MathWork.upcolor(MathWork.round(temp + v * (x_y_1.R - temp + u * (x_1_y_1.R - x_y_1.R))));
                                temp = x_y.G + u * (x_1_y.G - x_y.G);
                                ansg = MathWork.upcolor(MathWork.round(temp + v * (x_y_1.G - temp + u * (x_1_y_1.G - x_y_1.G))));
                                temp = x_y.B + u * (x_1_y.B - x_y.B);
                                ansb = MathWork.upcolor(MathWork.round(temp + v * (x_y_1.B - temp + u * (x_1_y_1.B - x_y_1.B))));
                                oupb.SetPixel(j, i, Color.FromArgb(ansr, ansg, ansb));
                                break;
                            case Stretching.Bicubic:

                                Sup1 = MathWork.KernelFunS(u + 1);
                                Sup0 = MathWork.KernelFunS(u + 0);
                                Sus1 = MathWork.KernelFunS(u - 1);
                                Sus2 = MathWork.KernelFunS(u - 2);
                                Svp1 = MathWork.KernelFunS(v + 1);
                                Svp0 = MathWork.KernelFunS(v + 0);
                                Svs1 = MathWork.KernelFunS(v - 1);
                                Svs2 = MathWork.KernelFunS(v - 2);
                                if (oldxint != orixint || oldyint != oriyint)
                                {
                                    ExtraGetPixelRef(orixint - 1, oriyint - 1, ref a11);
                                    ExtraGetPixelRef(orixint - 0, oriyint - 1, ref a12);
                                    ExtraGetPixelRef(orixint + 1, oriyint - 1, ref a13);
                                    ExtraGetPixelRef(orixint + 2, oriyint - 1, ref a14);
                                    ExtraGetPixelRef(orixint - 1, oriyint - 0, ref a21);
                                    ExtraGetPixelRef(orixint - 0, oriyint - 0, ref a22);
                                    ExtraGetPixelRef(orixint + 1, oriyint - 0, ref a23);
                                    ExtraGetPixelRef(orixint + 2, oriyint - 0, ref a24);
                                    ExtraGetPixelRef(orixint - 1, oriyint + 1, ref a31);
                                    ExtraGetPixelRef(orixint - 0, oriyint + 1, ref a32);
                                    ExtraGetPixelRef(orixint + 1, oriyint + 1, ref a33);
                                    ExtraGetPixelRef(orixint + 2, oriyint + 1, ref a34);
                                    ExtraGetPixelRef(orixint - 1, oriyint + 2, ref a41);
                                    ExtraGetPixelRef(orixint - 0, oriyint + 2, ref a42);
                                    ExtraGetPixelRef(orixint + 1, oriyint + 2, ref a43);
                                    ExtraGetPixelRef(orixint + 2, oriyint + 2, ref a44);
                                }
                                kk1r = Sup1 * a11.R + Sup0 * a12.R + Sus1 * a13.R + Sus2 * a14.R;
                                kk2r = Sup1 * a21.R + Sup0 * a22.R + Sus1 * a23.R + Sus2 * a24.R;
                                kk3r = Sup1 * a31.R + Sup0 * a32.R + Sus1 * a33.R + Sus2 * a34.R;
                                kk4r = Sup1 * a41.R + Sup0 * a42.R + Sus1 * a43.R + Sus2 * a44.R;
                                kk1g = Sup1 * a11.G + Sup0 * a12.G + Sus1 * a13.G + Sus2 * a14.G;
                                kk2g = Sup1 * a21.G + Sup0 * a22.G + Sus1 * a23.G + Sus2 * a24.G;
                                kk3g = Sup1 * a31.G + Sup0 * a32.G + Sus1 * a33.G + Sus2 * a34.G;
                                kk4g = Sup1 * a41.G + Sup0 * a42.G + Sus1 * a43.G + Sus2 * a44.G;
                                kk1b = Sup1 * a11.B + Sup0 * a12.B + Sus1 * a13.B + Sus2 * a14.B;
                                kk2b = Sup1 * a21.B + Sup0 * a22.B + Sus1 * a23.B + Sus2 * a24.B;
                                kk3b = Sup1 * a31.B + Sup0 * a32.B + Sus1 * a33.B + Sus2 * a34.B;
                                kk4b = Sup1 * a41.B + Sup0 * a42.B + Sus1 * a43.B + Sus2 * a44.B;
                                ansr = MathWork.upcolor(MathWork.round(Svp1 * kk1r + Svp0 * kk2r + Svs1 * kk3r + Svs2 * kk4r));
                                ansg = MathWork.upcolor(MathWork.round(Svp1 * kk1g + Svp0 * kk2g + Svs1 * kk3g + Svs2 * kk4g));
                                ansb = MathWork.upcolor(MathWork.round(Svp1 * kk1b + Svp0 * kk2b + Svs1 * kk3b + Svs2 * kk4b));
                                oupb.SetPixel(j, i, Color.FromArgb(ansr, ansg, ansb));
                                break;
                        }
                        #endregion
                        oldxint = orixint;
                    }
                    oldyint = oriyint;
#if MYDEBUG
                    DebugLogger.LogTimeStamp();
#endif
                }
                #endregion
                oupb.UnlockBits();
                ans = output;
                pb.Value = 0;
                return true;
            }
            catch (Exception e)
            {
                ans = null;
                return false;
            }
        }

        //Only after the rotate has been done,will cut the image.
        public bool crop(out Bitmap ans)
        {
            if (output == null)
            {
                ans = null;
                return false;
            }
            else
            {
                Bitmap b = new Bitmap(MathWork.round(laststrx * origin.Width), MathWork.round(laststry * origin.Height));
                Graphics g = Graphics.FromImage(b);
                int startx = MathWork.round((output.Width - b.Width) / 2);
                int starty = MathWork.round((output.Height - b.Height) / 2);
                g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, b.Width, b.Height));
                g.DrawImage(output, new Rectangle(0, 0, b.Width, b.Height), new Rectangle(startx, starty, b.Width, b.Height), GraphicsUnit.Pixel);
                output.Dispose();
                output = (Bitmap)b.Clone();
                g.Dispose();
                b.Dispose();
                ans = output;
                return true;
            }
        }

        //Will release the input image and output image.
        public void stop()
        {
            if (origin != null)
            {
                orpb.UnlockBits();
                origin.Dispose();
            }
            if (output != null)
                output.Dispose();
        }

        //Will not release the input image and output image.
        public void stop2()
        {
            if (origin != null)
            {
                orpb.UnlockBits();
            }
        }
    }


    public partial class Pictransformer
    {
        private readonly PointF BlackPoint = new PointF(-1, -1);
        private Bitmap origin;
        private Bitmap output;
        private PointBitmap orpb;
        private PointBitmap oupb;
        private ProgressBar pb;
        private double laststrx = 1;
        private double laststry = 1;

        //Return BlackPoint when out of origin range.
        //The new base point will be set at the furthest possible edge of the picture.
        //The center of the rotate(also the base point of the original picture) will be at 
        //(diagonal,diagonal) at this coordinate system.

        //Return the original point in the original image by using the point now given.

        //For a faster speed, did not use Stretching function at last.
        private PointF ReachOriginRotate(Point now, double angle, int diagonal)
        {
            return ReachOrigin(now, 1, 1, angle, diagonal);
        }
        private PointF ReachOriginStretching(Point now, double strx, double stry, int diagonal)
        {
            return ReachOrigin(now, strx, stry, 0, diagonal);
        }
        private PointF ReachOrigin(Point now, double strx, double stry, double angle, int diagonal)
        {
            double xrotate = 0, yrotate = 0;
            if (MathWork.abs(angle) > 0.000001D)
                MathWork.rotate(diagonal, diagonal, (double)now.X, (double)now.Y, (-1) * angle, ref xrotate, ref yrotate);
            else
            {
                xrotate = (double)now.X + diagonal;
                yrotate = (double)now.Y + diagonal;
            }
            if (xrotate < diagonal || (xrotate - diagonal) > strx * origin.Width) return BlackPoint;
            if (yrotate < diagonal || (yrotate - diagonal) > stry * origin.Height) return BlackPoint;
            PointF ret = new PointF();
            ret.X = (float)((xrotate - diagonal) / strx);
            ret.Y = (float)(origin.Height - ((yrotate - diagonal) / stry));
            return ret;
        }
        //a rectangle:
        //  3 ----------- 2
        //  |             |
        //  |             |
        //  0 ----------- 1
        //Give the real size of the picture after transform.
        private Size CalculateSizeRotate(Size now, double angle, int diagonal)
        {
            return CalculateSize(now, 1, 1, angle, diagonal);
        }
        private Size CalculateSizeStretching(Size now, double strx, double stry, int diagonal)
        {
            return CalculateSize(now, strx, stry, 0, diagonal);
        }
        private Size CalculateSize(Size now, double strx, double stry, double angle, int diagonal)
        {
            double x0 = diagonal, y0 = diagonal, x1 = 0, y1 = 0, x2 = 0, y2 = 0, x3 = 0, y3 = 0;
            MathWork.rotate(diagonal, diagonal, diagonal + now.Width * strx, diagonal, angle, ref x1, ref y1);
            MathWork.rotate(diagonal, diagonal, diagonal + now.Width * strx, diagonal + now.Height * stry, angle, ref x2, ref y2);
            MathWork.rotate(diagonal, diagonal, diagonal, diagonal + now.Height * stry, angle, ref x3, ref y3);
            Size s = new Size();
            s.Width = MathWork.round(MathWork.max(MathWork.abs(x2 - x0), MathWork.abs(x3 - x1)));
            s.Height = MathWork.round(MathWork.max(MathWork.abs(y2 - y0), MathWork.abs(y3 - y1)));
            return s;
        }

        private int CalculateDiagonal(Size now, double strx, double stry)
        {
            return MathWork.ceiling(MathWork.sqrt(MathWork.power(now.Width * strx, 2) + MathWork.power(now.Height * stry, 2)));
        }
        //If the point is out of the original picture,return black.
        private Color ExtraGetPixel(int x, int y)
        {
            if (x < 0 || x >= origin.Width || y < 0 || y >= origin.Height)
                return Color.Black;
            return orpb.GetPixel(x, y);
        }
        //A little bit "faster" way.
        private void ExtraGetPixelRef(int x, int y, ref Color c)
        {
            if (x < 0 || x >= origin.Width || y < 0 || y >= origin.Height)
                c = Color.Black;
            else
                orpb.GetPixelRef(x, y, ref c);
            return;
        }
    }

}
