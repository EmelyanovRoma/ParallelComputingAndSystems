using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using OpenCL.Net;

namespace MultiThreadedComputingOnGPU
{
    public class Program
    {
        /// <summary>
        /// Ядро для преобразования изображения.
        /// </summary>
        private static string _openCLKernel = @"
        __kernel void Downscale(
            __global const uchar* input,
            __global uchar* output,
            int width,
            int height) 
       {
            int x = get_global_id(0);
            int y = get_global_id(1);
            int newWidth = width / 2;
            int newHeight = height / 2;

            if (x < newWidth && y < newHeight) {
                int idx = (y * 2 * width + x * 2) * 3;
                int outIdx = (y * newWidth + x) * 3;

                for (int i = 0; i < 3; i++) {
                    output[outIdx + i] = input[idx + i];
                }
            }
        }";

        public static void Main(string[] args)
        {
            ShowGPUProperties();
            Bitmap img = OpenImage("image1.jpg");
            img = ImageProcess(img);
            SaveImage(img);
            Console.Read();
        }

        /// <summary>
        /// Строит следующий уровень гауссовой пирамиды для изображения.
        /// </summary>
        /// <param name="img">Исходное изображение.</param>
        /// <param name="imgSize">Размер изображения.</param>
        /// <param name="imgBytes">Массив байтов исходного изображения.</param>
        /// <returns>Массив байтов нового изображения.</returns>
        private static byte[] GaussianPyramid(Bitmap img, int imgSize, byte[] imgBytes)
        {
            Platform[] platforms = Cl.GetPlatformIDs(out _);
            Device[] devices = Cl.GetDeviceIDs(platforms[0], DeviceType.Gpu, out _);
            Context context = Cl.CreateContext(null, 1, devices, null, IntPtr.Zero, out _);
            CommandQueue commandQueue = Cl.CreateCommandQueue(
                context, devices[0], CommandQueueProperties.None, out _);

            byte[] outputImgBytes = new byte[imgSize / 4];
            IMem inputBuffer = Cl.CreateBuffer(context,
                MemFlags.ReadOnly | MemFlags.CopyHostPtr, imgBytes, out _);  
            
            IMem outputBuffer = Cl.CreateBuffer(context, MemFlags.WriteOnly,
                outputImgBytes.Length, out _);          

            OpenCL.Net.Program program = Cl.CreateProgramWithSource(
                context, 1, new[] { _openCLKernel }, null, out _);
            Cl.BuildProgram(program, 1, devices, string.Empty, null, IntPtr.Zero);

            Kernel kernel = Cl.CreateKernel(program, "Downscale", out _);
            Cl.SetKernelArg(kernel, 0, inputBuffer);
            Cl.SetKernelArg(kernel, 1, outputBuffer);
            Cl.SetKernelArg(kernel, 2, img.Width);
            Cl.SetKernelArg(kernel, 3, img.Height);

            var stopwatch = Stopwatch.StartNew();

            Event clevent;
            IntPtr[] globalWorkSize = new IntPtr[] 
            { 
                (IntPtr)(img.Width / 2),
                (IntPtr)(img.Height / 2) 
            };
            Cl.EnqueueNDRangeKernel(commandQueue, kernel, 2, null,
                globalWorkSize, null, 0, null, out clevent);
            Cl.Finish(commandQueue);

            stopwatch.Stop();
            Console.WriteLine(
                $"Image processing time: " +
                $"{stopwatch.Elapsed.TotalMilliseconds / 1000} sec.");

            Cl.EnqueueReadBuffer(commandQueue, outputBuffer, Bool.True, IntPtr.Zero,
                new IntPtr(outputImgBytes.Length), outputImgBytes, 0, null, out clevent);

            Cl.ReleaseMemObject(inputBuffer);
            Cl.ReleaseMemObject(outputBuffer);
            Cl.ReleaseKernel(kernel);
            Cl.ReleaseProgram(program);
            Cl.ReleaseCommandQueue(commandQueue);
            Cl.ReleaseContext(context);

            return outputImgBytes;
        }

        /// <summary>
        /// Запускает <see cref="GaussianPyramid"/> для обработки изображения.
        /// </summary>
        /// <param name="img">Исходное изображение.</param>
        /// <returns>Обработанное изображение.</returns>
        public static Bitmap ImageProcess(Bitmap img)
        {
            BitmapData inputData = img.LockBits(
                new Rectangle(0, 0, img.Width, img.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            int imgSize = inputData.Stride * img.Height;
            byte[] imgBytes = new byte[imgSize];

            Marshal.Copy(inputData.Scan0, imgBytes, 0, imgSize);
            img.UnlockBits(inputData);

            byte[] newImgBytes = GaussianPyramid(img, imgSize, imgBytes);

            Bitmap resultImg = new Bitmap(
                img.Width / 2, img.Height / 2, PixelFormat.Format24bppRgb);
            BitmapData outputData = resultImg.LockBits(
                new Rectangle(0, 0, resultImg.Width, resultImg.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);

            Marshal.Copy(newImgBytes, 0, outputData.Scan0, newImgBytes.Length);
            resultImg.UnlockBits(outputData);

            return resultImg;
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
                Console.WriteLine("Failed to obtain platforms.");
                return;
            }

            foreach (var platform in platforms)
            {
                Console.WriteLine(
                    $"Platform: " +
                    $"{Cl.GetPlatformInfo(platform, PlatformInfo.Name, out _)}");

                var devices = Cl.GetDeviceIDs(platform, DeviceType.Gpu, out error);

                if (error != ErrorCode.Success)
                {
                    Console.WriteLine("Failed to get devices on the platform.");
                    continue;
                }

                foreach (var device in devices)
                {
                    Console.WriteLine("Device:");
                    Console.WriteLine($"  Name: {Cl.GetDeviceInfo(device, DeviceInfo.Name, out _)}");
                    Console.WriteLine($"  Manufacture: {Cl.GetDeviceInfo(device, DeviceInfo.Vendor, out _)}");
                    Console.WriteLine($"  Driver: {Cl.GetDeviceInfo(device, DeviceInfo.DriverVersion, out _)}");
                    Console.WriteLine($"  OpenCL version: {Cl.GetDeviceInfo(device, DeviceInfo.Version, out _)}");
                    Console.WriteLine($"  Compute cores: {Cl.GetDeviceInfo(device, DeviceInfo.MaxComputeUnits, out _)}");
                    Console.WriteLine($"  VRAM: {Cl.GetDeviceInfo(device, DeviceInfo.GlobalMemSize, out _)} byte");
                    Console.WriteLine($"  Core clock: {Cl.GetDeviceInfo(device, DeviceInfo.MaxClockFrequency, out _)} MHz");
                }
            }
        }
    }
}
