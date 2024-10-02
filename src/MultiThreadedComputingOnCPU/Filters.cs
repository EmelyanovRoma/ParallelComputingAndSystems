using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MultiThreadedComputingOnCPU
{
    public static class Filters
    {
        public static int Threads { set; get; }


        public static class Erosion
        {          
            public static double[,] CalculateIntensivityOfImage(Bitmap bitmap)
            {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                                    ImageLockMode.ReadWrite, bitmap.PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                int byteCount = bitmapData.Stride * bitmap.Height;
                byte[] pixels = new byte[byteCount];

                IntPtr ptrFirstPixel = bitmapData.Scan0;
                Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);

                int height = bitmap.Height;
                int width = bitmap.Width;
                double[,] intensivityMatrixImage = new double[width, height];

                Parallel.For(0, height, new ParallelOptions { MaxDegreeOfParallelism = Threads }, y =>
                {
                    int yIndex = y * bitmapData.Stride;
                    for (int x = 0; x < width; x++)
                    {
                        int xIndex = x * bytesPerPixel;
                        int index = yIndex + xIndex;

                        byte blue = pixels[index];
                        byte green = pixels[index + 1];
                        byte red = pixels[index + 2];

                        double intensity = (red + green + blue) / 3;
                        intensivityMatrixImage[x, y] = intensity;
                    }
                });

                Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
                bitmap.UnlockBits(bitmapData);

                return intensivityMatrixImage;
            }

            private static int[,] ApplyThreshold(double[,] intensivity, int threshold)
            {
                int width = intensivity.GetLength(0);
                int height = intensivity.GetLength(1);
                var binaryImage = new int[width, height];

                Parallel.For(0, width, new ParallelOptions { MaxDegreeOfParallelism = Threads }, x =>
                {
                    for (int y = 0; y < height; y++)
                    {
                        binaryImage[x, y] = intensivity[x, y] < threshold ? 0 : 1;
                    }
                });

                return binaryImage;
            }

            private static Bitmap ConvertBinaryImageToBitmap(int[,] binaryImage)
            {
                int width = binaryImage.GetLength(0);
                int height = binaryImage.GetLength(1);
                Bitmap resultImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                BitmapData bitmapData = resultImage.LockBits(new Rectangle(0, 0, width, height),
                                                             ImageLockMode.WriteOnly, resultImage.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(resultImage.PixelFormat) / 8;
                int stride = bitmapData.Stride;
                IntPtr ptrFirstPixel = bitmapData.Scan0;
                int byteCount = stride * height;
                byte[] pixels = new byte[byteCount];

                Parallel.For(0, height, new ParallelOptions { MaxDegreeOfParallelism = Threads }, y =>
                {
                    int yOffset = y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int xOffset = x * bytesPerPixel;
                        int pixelIndex = yOffset + xOffset;

                        byte colorValue = (binaryImage[x, y] == 1) ? (byte)255 : (byte)0;

                        pixels[pixelIndex] = colorValue;     // Blue
                        pixels[pixelIndex + 1] = colorValue; // Green
                        pixels[pixelIndex + 2] = colorValue; // Red
                    }
                });

                Marshal.Copy(pixels, 0, ptrFirstPixel, byteCount);
                resultImage.UnlockBits(bitmapData);

                return resultImage;
            }            

            private static int[,] Erose(int[,] binaryImage, int step)
            {
                int width = binaryImage.GetLength(0);
                int height = binaryImage.GetLength(1);
                int[,] intermediateImage = new int[width, height];
                int[,] erodedImage = new int[width, height];

                Parallel.For(0, height, new ParallelOptions { MaxDegreeOfParallelism = Threads }, y =>
                {
                    for (int x = 0; x < width; x++)
                    {
                        bool erode = true;
                        for (int i = -step; i <= step; i++)
                        {
                            int newX = x + i;
                            if (newX < 0 || newX >= width || binaryImage[newX, y] == 0)
                            {
                                erode = false;
                                break;
                            }
                        }
                        intermediateImage[x, y] = erode ? 1 : 0;
                    }
                });

                Parallel.For(0, width, new ParallelOptions { MaxDegreeOfParallelism = Threads }, x =>
                {
                    for (int y = 0; y < height; y++)
                    {
                        bool erode = true;
                        for (int j = -step; j <= step; j++)
                        {
                            int newY = y + j;
                            if (newY < 0 || newY >= height || intermediateImage[x, newY] == 0)
                            {
                                erode = false;
                                break;
                            }
                        }
                        erodedImage[x, y] = erode ? 1 : 0;
                    }
                });

                return erodedImage;
            }            

            public static Bitmap ApplyErossion(Bitmap image)
            {
                var intensivityMat = CalculateIntensivityOfImage(image);
                var binaryImage = ApplyThreshold(intensivityMat, 150);
                var erodedImage = Erose(binaryImage, 1);
                image.Dispose();
                return ConvertBinaryImageToBitmap(erodedImage);
            }
        }
    }
}
