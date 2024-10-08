using System;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MultiThreadedComputingOnCPU
{
    /// <summary>
    /// Описывает перенос изображения по осям X и Y.
    /// </summary>
    public static class ImageOffset
    {
        /// <summary>
        /// Задает или возвращает количество потоков процессора.
        /// </summary>
        public static int Threads { set; get; }

        /// <summary>
        /// Применяет шаг переноса по осям X и Y к изображению.
        /// </summary>
        /// <param name="image">Исходное изображение.</param>
        /// <param name="xOffset">Шаг переноса для оси X.</param>
        /// <param name="yOffset">Шаг переноса для оси Y.</param>
        /// <returns>Изображение с переносом пикселей.</returns>
        public static Bitmap ApplyOffset(
            Bitmap image,
            int xOffset,
            int yOffset)
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap newImage = new Bitmap(width, height);
            Color offsetColor = Color.FromArgb(187, 38, 73);

            BitmapData bitmapData = image.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                image.PixelFormat);

            BitmapData newBitmapData = newImage.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                image.PixelFormat);

            int bytesPerPixel = Image.GetPixelFormatSize(image.PixelFormat) / 8;
            int byteCount = bitmapData.Stride * height;
            byte[] originalPixels = new byte[byteCount];
            byte[] newPixels = new byte[byteCount];
            
            IntPtr ptrFirstPixel = bitmapData.Scan0;
            IntPtr ptrNewFirstPixel = newBitmapData.Scan0;

            Marshal.Copy(ptrFirstPixel, originalPixels, 0, originalPixels.Length);

            Parallel.For(
                0,
                height,
                new ParallelOptions { MaxDegreeOfParallelism = Threads },
                y =>
            {
                int yIndex = y * newBitmapData.Stride;

                for (int x = 0; x < width; x++)
                {
                    int xIndex = x * bytesPerPixel;
                    int index = yIndex + xIndex;

                    if (y < yOffset || x < xOffset)
                    {
                        newPixels[index] = offsetColor.B;
                        newPixels[index + 1] = offsetColor.G;
                        newPixels[index + 2] = offsetColor.R;
                    }
                }
            });

            Parallel.For(
                0,
                height - yOffset,
                new ParallelOptions { MaxDegreeOfParallelism = Threads },
                y =>
            {
                int yIndex = y * bitmapData.Stride;
                int newYIndex = (y + yOffset) * newBitmapData.Stride;

                for (int x = 0; x < width - xOffset; x++)
                {
                    int xIndex = x * bytesPerPixel;
                    int newXIndex = (x + xOffset) * bytesPerPixel;

                    int originalIndex = yIndex + xIndex;
                    int newIndex = newYIndex + newXIndex;

                    for (int i = 0; i < bytesPerPixel; i++)
                    {
                        newPixels[newIndex + i] = originalPixels[originalIndex + i];
                    }
                }
            });

            Marshal.Copy(newPixels, 0, ptrNewFirstPixel, newPixels.Length);

            image.UnlockBits(bitmapData);
            newImage.UnlockBits(newBitmapData);

            image.Dispose();

            return newImage;
        }
    }
}
