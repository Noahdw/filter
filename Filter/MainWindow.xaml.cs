using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Filter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ArrayEditor edit;
        Bitmap bmp;
        public static bool imageIsOriginal = true;
        public static double[,] originalImage;
        MemoryStream ms = new MemoryStream();
        public MainWindow()
        {
            edit = new ArrayEditor();
            InitializeComponent();
            initialDisplay();
        }

        public void initialDisplay()
        {
            try
            {
                imageIsOriginal = true;
                bmp = new Bitmap(@"C:\Users\Puter\Pictures\Wallpapers\noah.jpg", true);
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
                Bitmap clone = bmp.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppRgb);


                BitmapData data = clone.LockBits(rect, ImageLockMode.ReadWrite, clone.PixelFormat);
                IntPtr ptr = data.Scan0;

                //declare an array to hold the doubles of the bitmap
                int numBytes = data.Stride * clone.Height;

                byte[] tempByteArray = new byte[numBytes];
                Marshal.Copy(ptr, tempByteArray, 0, numBytes);

                double[] doubles = new double[numBytes];
                Array.Copy(tempByteArray, doubles, numBytes);

                clone.UnlockBits(data);
                clone = editImage(doubles, bmp.Width, bmp.Height, 0, null);
                displayImage(clone);
                clone.Dispose();


            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }


        }
        public Bitmap editImage(double[] doublesArg, int width, int height, double rate, double[,] origArray)
        {
            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            System.Drawing.Rectangle grayRect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData data = bmp.LockBits(grayRect, ImageLockMode.ReadWrite, bmp.PixelFormat);

            IntPtr ptr = data.Scan0;

            int numBytes = data.Stride * bmp.Height;

            double[,] array2D;
            double[] array1D = new double[numBytes];
            double[] arrayT = new double[numBytes];


            //creates 2d array for editing
            if (imageIsOriginal == true)
            {

                array2D = To2dArray(doublesArg, data.Stride, height);
                originalImage = array2D; //Master image to save computation time
                Buffer.BlockCopy(array2D, 0, arrayT, 0, numBytes);
                imageIsOriginal = false;
                
            }
            else
            {
                array2D = edit.averageArray(origArray, height, data.Stride, rate, arrayT);
            }
            //converts 2d array into 1d
            array1D = To1dArray(array2D, data.Stride, height);

            //Converts 1d back into byte for transfer
            byte[] tempByteArray = array1D.Select(x => Convert.ToByte(x)).ToArray();

            Marshal.Copy(tempByteArray, 0, ptr, numBytes);
            bmp.UnlockBits(data);
            
            return bmp;
        }
        public void displayImage(Bitmap clone)
        {
            //Set wpf image to bitmap
            ms.Dispose();
            image.Source = null;
            ms = new MemoryStream();

            clone.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            image.Source = bi;
            
            clone.Dispose();
        }

        public static double[,] To2dArray(double[] source, int width, int height)
        {
            int count = 0;
            double[,] result = new double[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    result[i, j] = source[count];
                    count++;
                }
            }
            return result;
        }
        public static double[] To1dArray(double[,] source, int width, int height)
        {
            double[] result = new double[height * width];
            int count = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    result[count] = source[i, j];
                    count++;
                }
            }
            return result;
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
               
                Bitmap Tbmp;
                Tbmp = editImage(null, originalImage.GetLength(1)/4, originalImage.GetLength(0), slider.Value, originalImage);
                displayImage(Tbmp);
                Tbmp.Dispose();
            }
            catch (NullReferenceException t)
            {

                Console.WriteLine(t);
            }

        }

        private void filterChange_Click(object sender, RoutedEventArgs e)
        {
            initialDisplay();
           
        }
    }
}

