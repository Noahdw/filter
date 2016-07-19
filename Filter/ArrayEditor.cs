using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filter
{
    class ArrayEditor
    {
        public double[,] averageArray(double[,] src, int height, int width, double rate,double[] arrayT)
        {
            double[,] kernel = new double[3, 3] { { 1, 1, 1 }, {1,1,1}, {1,1,1} };

            double[,] newImage = src;

             //MainWindow.To2dArray(arrayT, width, height);;
            for (int i = 1; i < height-2; i++)
            {
                for (int j = 1; j < width- 2; j++)
                {
                    double total = 0;
                    for (int y = i-1; y <= i+1; y++)
                    {
                        for (int x = j-1; x <=j+1; x++)
                        {
                            total += src[y, x];
                        }
                    }
                    total = (total / 9)*rate;
                    newImage[i, j] = total;
                }
            }

            return newImage;
        }
    }
}
