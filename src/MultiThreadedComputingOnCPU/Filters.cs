using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MultiThreadedComputingOnCPU
{
    /// <summary>
    /// Описывает морфологичесикий фильтр эрозии и
    /// фильтр размытия со смещением пикселей изображения.
    /// </summary>
    public static class Filters
    {
        /// <summary>
        /// Задает или возвращает количество потоков процессора.
        /// </summary>
        public static int Threads { set; get; }

        /// <summary>
        /// Описывает фильтр эрозии.
        /// </summary>
        public static class Erosion
        {
            /// <summary>
            /// Вычисляет интенсивность пикселей изображения.
            /// </summary>
            /// <param name="image">Исходное изображение.</param>
            /// <returns>Матрица интенсивности пикселей.</returns>
            public static double[,] CalculateIntensivityOfImage(Bitmap image)
            {
                BitmapData bitmapData = image.LockBits(
                    new Rectangle(0, 0, image.Width, image.Height),
                    ImageLockMode.ReadWrite,
                    image.PixelFormat);
                
                int bytesPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
                int byteCount = bitmapData.Stride * image.Height;
                byte[] pixels = new byte[byteCount];
                double[,] intensivityMatrixImage = new double[image.Width, image.Height];                
                int widtn = image.Width;
                IntPtr ptrFirstPixel = bitmapData.Scan0;

                Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length); 
                
                Parallel.For(
                    0,
                    image.Height,
                    new ParallelOptions { MaxDegreeOfParallelism = Threads },
                    y =>
                {
                    int yIndex = y * bitmapData.Stride;
                    for (int x = 0; x < widtn; x++)
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
                image.UnlockBits(bitmapData);

                return intensivityMatrixImage;
            }

            /// <summary>
            /// Применяет значение порога для формирования матрицы из 0 и 1.
            /// </summary>
            /// <param name="intensivityMat">Матрица интенсивности пикселей.</param>
            /// <param name="threshold">Значение порога.</param>
            /// <returns>Матрица 0 и 1 (бинарное изображение).</returns>
            private static int[,] ApplyThreshold(double[,] intensivityMat, int threshold)
            {
                int width = intensivityMat.GetLength(0);
                int height = intensivityMat.GetLength(1);
                var binaryImage = new int[width, height];

                Parallel.For(
                    0,
                    width,
                    new ParallelOptions { MaxDegreeOfParallelism = Threads },
                    x =>
                {
                    for (int y = 0; y < height; y++)
                    {
                        binaryImage[x, y] = intensivityMat[x, y] < threshold ? 0 : 1;
                    }
                });

                return binaryImage;
            }

            /// <summary>
            /// Конвертирует бинарное изображение в <see cref="Bitmap"/>.
            /// </summary>
            /// <param name="binaryImage">Бинарное изображение.</param>
            /// <returns>24-битное изображение.</returns>
            private static Bitmap ConvertBinaryImageToBitmap(int[,] binaryImage)
            {
                int width = binaryImage.GetLength(0);
                int height = binaryImage.GetLength(1);
                Bitmap resultImage = new Bitmap(
                    width, height, PixelFormat.Format24bppRgb);

                BitmapData bitmapData = resultImage.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly,
                    resultImage.PixelFormat);

                int bytesPerPixel = Image.GetPixelFormatSize(resultImage.PixelFormat) / 8;
                int stride = bitmapData.Stride;                
                int byteCount = stride * height;
                byte[] pixels = new byte[byteCount];
                IntPtr ptrFirstPixel = bitmapData.Scan0;

                Parallel.For(
                    0,
                    height,
                    new ParallelOptions { MaxDegreeOfParallelism = Threads },
                    y =>
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

            /// <summary>
            /// Выполняет алгоритм эрозии изображения.
            /// </summary>
            /// <param name="binaryImage">Бинарное изображение.</param>
            /// <param name="step">Шаг.</param>
            /// <returns>Бинарная матрица.</returns>
            private static int[,] Erose(int[,] binaryImage, int step)
            {
                int width = binaryImage.GetLength(0);
                int height = binaryImage.GetLength(1);
                int[,] intermediateImage = new int[width, height];
                int[,] erodedImage = new int[width, height];

                Parallel.For(
                    0,
                    height,
                    new ParallelOptions { MaxDegreeOfParallelism = Threads },
                    y =>
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

                Parallel.For(0,
                    width,
                    new ParallelOptions { MaxDegreeOfParallelism = Threads },
                    x =>
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

            /// <summary>
            /// Применяет фильтр эрозии к изображению.
            /// </summary>
            /// <param name="image">Исходное изображение.</param>
            /// <returns>Изображение с эрозией.</returns>
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
