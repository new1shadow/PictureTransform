//MathWork and Matrix
//For normal calculate and matrix calculate.
//Author:ShadowK
//email:zhu.shadowk@gmail.com
//2015.10.11
//Use Ctrl+M,Ctrl+O to fold the code.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PicProgram
{
    public struct Matrix
    {
        //A matrix with only a element 0.
        public static readonly Matrix Zero = new Matrix(1, 1,true);

        private double[,] data;
        public int row;
        public int col;
        public bool locked;

        public Matrix(int rows, int cols, double[,] datas)
        {
            if (rows <= 0 || cols <= 0)
                throw new Exception("Matrix with zero row/col!");
            data = new double[rows, cols];
            row = rows;
            col = cols;
            Array.Copy(datas, data, rows * cols);
            locked = false;
            return;
        }
        public Matrix(int rows, int cols)
        {
            if (rows <= 0 || cols <= 0)
                throw new Exception("Matrix with zero row/col!");
            data = new double[rows, cols];
            row = rows;
            col = cols;
            Array.Clear(data, 0, rows * cols);
            locked = false;
            return;
        }
        public Matrix(int rows, int cols, double[,] datas, bool tolock)
        {
            if (rows <= 0 || cols <= 0)
                throw new Exception("Matrix with zero row/col!");
            data = new double[rows, cols];
            row = rows;
            col = cols;
            Array.Copy(datas, data, rows * cols);
            locked = tolock;
            return;
        }
        public Matrix(int rows, int cols, bool tolock)
        {
            if (rows <= 0 || cols <= 0)
                throw new Exception("Matrix with zero row/col!");
            data = new double[rows, cols];
            row = rows;
            col = cols;
            Array.Clear(data, 0, rows * cols);
            locked = tolock;
            return;
        }

        public double GetData(int m,int n)
        {
            return data[m,n];
        }
        public bool SetData(int m, int n, double num)
        {
            if (locked)
                return false;
            data[m, n] = num;
            return true;
        }
        public bool AddData(int m, int n, double num)
        {
            if (locked)
                return false;
            data[m, n] += num;
            return true;
        }

        //If the matrix is only a number.
        public bool Issingle()
        {
            return (row == 1 && col == 1) ? true : false;
        }
        //Matirx Transpose
        public void Transpose()
        {
            double[,] dataold = new double[row, col];
            Array.Copy(data,dataold,row*col);
            data = new double[col, row];
            int rowold = row;
            int colold = col;
            row = colold;
            col = rowold;
            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                    data[i, j] = dataold[j, i];
            return;
        }

        public override string ToString()
        {
            string s = string.Empty;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    s += data[i, j] + " ";
                }
                s += "\r\n";
            }
            return s;
        }
        public string ToStringWithMatlab()
        {
            string s = string.Empty;
            s += "[";
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (j != col - 1)
                        s += data[i, j] + " ";
                    else
                        s += data[i, j];
                }
                if (i != row - 1)
                    s += "; ";
            }
            s += "]";
            return s;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            Matrix c = Zero;
            if (MathWork.MatrixMultiply(a, b,ref c))
                return c;
            return Zero;
        }
        public static Matrix operator *(double a, Matrix b)
        {
            Matrix c = Zero;
            if (MathWork.MatrixMultiply(a, b, ref c))
                return c;
            return Zero;
        }
        public static Matrix operator *(Matrix a, double b)
        {
            Matrix c = Zero;
            if (MathWork.MatrixMultiply(a, b, ref c))
                return c;
            return Zero;
        }
        public static Matrix operator +(Matrix a, Matrix b)
        {
            Matrix c = Zero;
            if (MathWork.MatrixAddSub(a,b,ref c,MathWork.Addition.Add))
                return c;
            return Zero;
        }
        public static Matrix operator -(Matrix a, Matrix b)
        {
            Matrix c = Zero;
            if (MathWork.MatrixAddSub(a, b,ref c, MathWork.Addition.Sub))
                return c;
            return Zero;
        }
    }
    public static class MathWork
    {
        public enum Addition { Add, Sub };
        
        //return 1 when b <= 0.
        //Using Math as calculating class.
        public static int power(int a, int b)
        {
            if (b <= 0) return 1;
            //return a * power(a, b - 1);
            return (int)Math.Pow(a,b);
        }
        public static double power(double a, int b)
        {
            if (b <= 0) return 1;
            //return a * power(a, b - 1);
            return Math.Pow(a, b);
        }

        public static int abs(int a)
        {
            return a > 0 ? a : (-1) * a;
        }
        public static double abs(double a)
        {
            return a > 0 ? a : (-1) * a;
        }
        public static int max(int a, int b)
        {
            return a > b ? a : b;
        }
        public static double max(double a, double b)
        {
            return a > b ? a : b;
        }
        public static int min(int a, int b)
        {
            return a < b ? a : b;
        }
        public static double min(double a, double b)
        {
            return a < b ? a : b;
        }

        //calculate a location rotate
        public static bool rotate(double rx, double ry, double x0, double y0, double angle, ref double x, ref double y)
        {
            double sina = sin(angle);
            double cosa = cos(angle);
            x = (x0 - rx) * cosa - (y0 - ry) * sina + rx;
            y = (x0 - rx) * sina + (y0 - ry) * cosa + ry;
            return true;
        }

        //Arc system, using Math as calculating class.
        public static double sin(double angle)
        {
            return (double)Math.Sin(angle);
        }
        public static double cos(double angle)
        {
            return (double)Math.Cos(angle);
        }
        public static double tan(double angle)
        {
            return (double)Math.Tan(angle);
        }

        //Using Math as calculating class.
        public static double sqrt(double num)
        {
            if (num <= 0) return 0;
            return Math.Sqrt(num);
        }
        public static int round(double num)
        {
            return (int)Math.Round(num);
        }
        public static int floor(double num)
        {
            return (int)Math.Floor(num);
        }
        public static int ceiling(double num)
        {
            return (int)Math.Ceiling(num);
        }
        //keep the data in 0~255
        public static int upcolor(int num)
        {
            return (num >= 0) ? ((num < 256) ? num :255) : 0;
        }

        //The "ans" will be recreated in the function.
        public static bool MatrixAddSub(Matrix a, Matrix b,ref Matrix ans, Addition c)
        {
            if (a.col != b.col || a.row != b.row)
                return false;
            ans = new Matrix(a.row, a.col);
            for (int i = 0; i < a.row; i++)
                for (int j = 0; j < a.col; j++)
                    ans.SetData(i, j,(c == Addition.Add) ? (a.GetData(i, j) + b.GetData(i, j)) : (a.GetData(i, j) - b.GetData(i, j)));
            return true;
        }
        public static bool MatrixMultiply(Matrix a, Matrix b,ref Matrix ans)
        {
            if (a.Issingle())
                MatrixMultiply(a.GetData(0, 0), b,ref ans);
            if (b.Issingle())
                MatrixMultiply(b.GetData(0, 0), a,ref ans);
            if(a.col != b.row)
            return false;
            ans = new Matrix(a.row, b.col);
            for (int i = 0; i < a.row; i++)
                for (int j = 0; j < b.col; j++)
                    for (int k = 0; k < a.col; k++)
                        ans.AddData(i, j, a.GetData(i, k) * b.GetData(k, j));
            return true;
        }
        public static bool MatrixMultiply(double a, Matrix b,ref Matrix ans)
        {
            ans = new Matrix(b.row, b.col);
            for (int i = 0; i < b.row; i++)
                for (int j = 0; j < b.col; j++)
                    ans.SetData(i, j, a * b.GetData(i,j));
            return true;
        }
        public static bool MatrixMultiply(Matrix a, double b,ref Matrix ans)
        {
            return MatrixMultiply(b, a,ref ans);
        }

        public static double KernelFunS(double x)
        {
            if (abs(x) <= 1)
                return 1 - 2 * x * x + abs(x) * x * x;
            else if (abs(x) < 2)
                return 4 - 8 * abs(x) + 5 * x * x - abs(x) * x * x;
            else
                return 0;
        }

        public static double pi()
        {
            return Math.PI;
        }
    }
}
