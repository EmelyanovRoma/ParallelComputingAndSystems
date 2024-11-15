using OpenCL.Net;
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
            ShowGPUProperties();
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

        /// <summary>
        /// Отображает некоторые характеристики уствновленных в системе GPU.
        /// </summary>
        public static void ShowGPUProperties()
        {
            ErrorCode error;
            var platforms = Cl.GetPlatformIDs(out error);

            if (error != ErrorCode.Success)
            {
                Console.WriteLine("Не удалось получить платформы.");
                return;
            }

            foreach (var platform in platforms)
            {
                Console.WriteLine(
                    $"Платформа: " +
                    $"{Cl.GetPlatformInfo(platform, PlatformInfo.Name, out _)}");

                var devices = Cl.GetDeviceIDs(platform, DeviceType.Gpu, out error);

                if (error != ErrorCode.Success)
                {
                    Console.WriteLine("Не удалось получить устройства на платформе.");
                    continue;
                }

                foreach (var device in devices)
                {
                    Console.WriteLine("Устройство:");
                    Console.WriteLine($"  Имя: {Cl.GetDeviceInfo(device, DeviceInfo.Name, out _)}");
                    Console.WriteLine($"  Производитель: {Cl.GetDeviceInfo(device, DeviceInfo.Vendor, out _)}");
                    Console.WriteLine($"  Драйвер: {Cl.GetDeviceInfo(device, DeviceInfo.DriverVersion, out _)}");
                    Console.WriteLine($"  Версия OpenCL: {Cl.GetDeviceInfo(device, DeviceInfo.Version, out _)}");
                    Console.WriteLine($"  Ядер вычисления: {Cl.GetDeviceInfo(device, DeviceInfo.MaxComputeUnits, out _)}");
                    Console.WriteLine($"  Глобальная память: {Cl.GetDeviceInfo(device, DeviceInfo.GlobalMemSize, out _)} байт");
                    Console.WriteLine($"  Частота процессора: {Cl.GetDeviceInfo(device, DeviceInfo.MaxClockFrequency, out _)} MHz");
                }
            }
        }
    }
}
