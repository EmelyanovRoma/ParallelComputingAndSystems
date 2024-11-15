using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace MultiThreadedComputingOnGPU
{
    public class Program
    {
        public static void Main(string[] args)
        {            
            Bitmap bmp = OpenImage("image1.jpg");
            SaveImage(bmp);
        }

        /// <summary>
        /// Сохраняет изображение.
        /// </summary>
        /// <param name="img">Изображение для сохранения.</param>
        public static void SaveImage(Bitmap img)
        {
            if (img == null)
            {
                Console.WriteLine("No Image to save.");
                return;
            }

            try
            {
                img.Save(
                    $"result_{img.Width}x{img.Height}.jpg",
                    ImageFormat.Jpeg);

                Console.WriteLine("Image saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image: {ex.Message}");
            }            
        }

        /// <summary>
        /// Открывает изображение.
        /// </summary>
        /// <param name="path">Путь до изображения.</param>
        /// <returns>Загруженное изображение.</returns>
        public static Bitmap OpenImage(string path)
        {
            Bitmap img = null;

            try
            {
                img = new Bitmap(path);
            }
            catch
            {
                Console.WriteLine("Error loading image. Check image path.");
                System.Environment.Exit(1);
            }
            
            return img;
        }
    }
}
