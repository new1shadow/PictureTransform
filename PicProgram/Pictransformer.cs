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

namespace PicProgram
{
    public partial class Pictransformer
    {
        private readonly PointF BlackPoint = new PointF(-1, -1);
        private Bitmap origin;
        private Bitmap output;
        private PointBitmap orpb;
        private PointBitmap oupb;

        //Return BlackPoint when out of origin range.
        //The new base point will be set at the furthest possible edge of the picture.
        //The center of the rotate(also the base point of the original picture) will be at 
        //(diagonal,diagonal) at this coordinate system.
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
            double xrotate = 0,yrotate = 0;
            if (MathWork.abs(angle) > 0.000001D)
                MathWork.rotate(diagonal, diagonal, (double)now.X, (double)now.Y, (-1) * angle, ref xrotate, ref yrotate);
            else
            {
                xrotate = (double)now.X;
                yrotate = (double)now.Y;
            }
            if (xrotate < 0 || xrotate > strx * origin.Width) return BlackPoint;
            if (yrotate < 0 || yrotate > stry * origin.Height) return BlackPoint;
            PointF ret = new PointF();
            ret.X = (float)(xrotate / strx);
            ret.Y = (float)(yrotate / stry);
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
            double x0 = 0,y0 = 0,x1 = 0,y1 = 0,x2 = 0,y2 = 0,x3 = 0,y3 = 0;
            MathWork.rotate(diagonal, diagonal, now.Width * strx, 0, angle, ref x1, ref y1);
            MathWork.rotate(diagonal, diagonal, now.Width * strx, now.Height * stry, angle, ref x2, ref y2);
            MathWork.rotate(diagonal, diagonal, 0, now.Height * stry, angle, ref x1, ref y1);
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
    }

    public partial class Pictransformer
    {
        public enum Stretching {Nearest, Bilinear, Bicubic};
        public Pictransformer() { }
        public bool start(Bitmap image)
        {
            origin = new Bitmap(image);
            orpb = new PointBitmap(origin);
            orpb.LockBits();
            //Bigmapwidth = MathWork.round(2 * MathWork.sqrt(MathWork.power(origin.Width, 2) + MathWork.power(origin.Height, 2)));
            return true;
        }
        public bool stretchpicture(double strx, double stry, Stretching kind, out Bitmap ans)
        {
            try
            {
                int diagonal = CalculateDiagonal(origin.Size, strx, stry);
                Size anssize = CalculateSizeStretching(origin.Size, strx, stry, diagonal);
                if (output != null)
                    output.Dispose();
                output = new Bitmap(anssize.Width, anssize.Height);
                oupb = new PointBitmap(output);
                oupb.LockBits();
                for (int i = 0; i < anssize.Height; i++)
                {
                    for (int j = 0; j < anssize.Width; j++)
                    {
                        PointF orip = ReachOriginStretching(new Point(j, i), strx, stry, diagonal);
                        int orixint = MathWork.floor(orip.X);
                        int oriyint = MathWork.floor(orip.Y);
                        switch (kind)
                        {
                            case Stretching.Nearest:
                                //output.SetPixel(j, i, ExtraGetPixel(orixint, oriyint));
                                oupb.SetPixel(j, i, ExtraGetPixel(orixint, oriyint));
                                break;
                            case Stretching.Bilinear:
                                double u = orip.X - (double)MathWork.floor(orip.X);
                                double v = orip.Y - (double)MathWork.floor(orip.Y);

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
                                #endregion

                                Color x_y = ExtraGetPixel(orixint, oriyint);
                                Color x_1_y = ExtraGetPixel(orixint + 1, oriyint);
                                Color x_y_1 = ExtraGetPixel(orixint, oriyint + 1);
                                Color x_1_y_1 = ExtraGetPixel(orixint + 1, oriyint + 1);
                                double temp;
                                temp = x_y.R + u * (x_1_y.R - x_y.R);
                                int ansr = MathWork.upcolor(MathWork.round(temp + v * (x_y_1.R + u * (x_1_y_1.R - x_y_1.R) - temp)));
                                temp = x_y.G + u * (x_1_y.G - x_y.G);
                                int ansg = MathWork.upcolor(MathWork.round(temp + v * (x_y_1.G + u * (x_1_y_1.G - x_y_1.G) - temp)));
                                temp = x_y.B + u * (x_1_y.B - x_y.B);
                                int ansb = MathWork.upcolor(MathWork.round(temp + v * (x_y_1.B + u * (x_1_y_1.B - x_y_1.B) - temp)));
                                //output.SetPixel(j, i, Color.FromArgb(ansr, ansg, ansb));
                                oupb.SetPixel(j, i, Color.FromArgb(ansr, ansg, ansb));
                                break;
                            case Stretching.Bicubic:
                                double u2 = orip.X - (double)MathWork.floor(orip.X);
                                double v2 = orip.Y - (double)MathWork.floor(orip.Y);
                                Matrix A = new Matrix(1, 4, new double[,] { { MathWork.KernelFunS(1 + u2), MathWork.KernelFunS(u2), MathWork.KernelFunS(1 - u2), MathWork.KernelFunS(2 - u2) } });
                                Matrix Br = new Matrix(4, 4, new double[,]{{(double)ExtraGetPixel(orixint-1,oriyint-1).R,(double)ExtraGetPixel(orixint-1,oriyint-0).R,(double)ExtraGetPixel(orixint-1,oriyint+1).R,(double)ExtraGetPixel(orixint-1,oriyint+2).R},
                                                                           {(double)ExtraGetPixel(orixint-0,oriyint-1).R,(double)ExtraGetPixel(orixint-0,oriyint-0).R,(double)ExtraGetPixel(orixint-0,oriyint+1).R,(double)ExtraGetPixel(orixint-0,oriyint+2).R},
                                                                           {(double)ExtraGetPixel(orixint+1,oriyint-1).R,(double)ExtraGetPixel(orixint+1,oriyint-0).R,(double)ExtraGetPixel(orixint+1,oriyint+1).R,(double)ExtraGetPixel(orixint+1,oriyint+2).R},
                                                                           {(double)ExtraGetPixel(orixint+2,oriyint-1).R,(double)ExtraGetPixel(orixint+2,oriyint-0).R,(double)ExtraGetPixel(orixint+2,oriyint+1).R,(double)ExtraGetPixel(orixint+2,oriyint+2).R}});
                                Matrix Bg = new Matrix(4, 4, new double[,]{{(double)ExtraGetPixel(orixint-1,oriyint-1).G,(double)ExtraGetPixel(orixint-1,oriyint-0).G,(double)ExtraGetPixel(orixint-1,oriyint+1).G,(double)ExtraGetPixel(orixint-1,oriyint+2).G},
                                                                           {(double)ExtraGetPixel(orixint-0,oriyint-1).G,(double)ExtraGetPixel(orixint-0,oriyint-0).G,(double)ExtraGetPixel(orixint-0,oriyint+1).G,(double)ExtraGetPixel(orixint-0,oriyint+2).G},
                                                                           {(double)ExtraGetPixel(orixint+1,oriyint-1).G,(double)ExtraGetPixel(orixint+1,oriyint-0).G,(double)ExtraGetPixel(orixint+1,oriyint+1).G,(double)ExtraGetPixel(orixint+1,oriyint+2).G},
                                                                           {(double)ExtraGetPixel(orixint+2,oriyint-1).G,(double)ExtraGetPixel(orixint+2,oriyint-0).G,(double)ExtraGetPixel(orixint+2,oriyint+1).G,(double)ExtraGetPixel(orixint+2,oriyint+2).G}});
                                Matrix Bb = new Matrix(4, 4, new double[,]{{(double)ExtraGetPixel(orixint-1,oriyint-1).B,(double)ExtraGetPixel(orixint-1,oriyint-0).B,(double)ExtraGetPixel(orixint-1,oriyint+1).B,(double)ExtraGetPixel(orixint-1,oriyint+2).B},
                                                                           {(double)ExtraGetPixel(orixint-0,oriyint-1).B,(double)ExtraGetPixel(orixint-0,oriyint-0).B,(double)ExtraGetPixel(orixint-0,oriyint+1).B,(double)ExtraGetPixel(orixint-0,oriyint+2).B},
                                                                           {(double)ExtraGetPixel(orixint+1,oriyint-1).B,(double)ExtraGetPixel(orixint+1,oriyint-0).B,(double)ExtraGetPixel(orixint+1,oriyint+1).B,(double)ExtraGetPixel(orixint+1,oriyint+2).B},
                                                                           {(double)ExtraGetPixel(orixint+2,oriyint-1).B,(double)ExtraGetPixel(orixint+2,oriyint-0).B,(double)ExtraGetPixel(orixint+2,oriyint+1).B,(double)ExtraGetPixel(orixint+2,oriyint+2).B}});
                                Matrix C = new Matrix(4, 1, new double[,] { { MathWork.KernelFunS(1 + v2) }, { MathWork.KernelFunS(v2) }, { MathWork.KernelFunS(1 - v2) }, { MathWork.KernelFunS(2 - v2) } });
                                int fansr = MathWork.upcolor(MathWork.round((A * Br * C).GetData(0, 0)));
                                int fansg = MathWork.upcolor(MathWork.round((A * Bg * C).GetData(0, 0)));
                                int fansb = MathWork.upcolor(MathWork.round((A * Bb * C).GetData(0, 0)));
                                output.SetPixel(j, i, Color.FromArgb(fansr, fansg, fansb));
                                break;
                        }
                    }
#if DEBUG
                    if (i % 5 == 0)
                        DebugLogger.LogLine("Processing:" + "x:0" + "y:" + i + "Timestamp:" + DebugLogger.GetTimeStamp());
#endif
                }
                oupb.UnlockBits();
                ans = output;
                return true;

            }
            catch (Exception e)
            {
                ans = null;
                return false;
            }
        }

        public void stop()
        {
            orpb.UnlockBits();
            origin.Dispose();
            output.Dispose();
        }
    }
}
