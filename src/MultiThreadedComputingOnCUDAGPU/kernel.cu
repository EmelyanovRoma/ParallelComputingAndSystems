#include <iostream>
#include <opencv2/opencv.hpp>
#include "cuda_runtime.h"
#include "device_launch_parameters.h"
#include <chrono>

using namespace std;
using namespace cv;
using namespace chrono;

__global__ void CalculateIntensity(const uchar3* input,float* intensity, Size imgSize);
__global__ void ApplyThreshold(float* intensity, uchar* binary, Size imgSize, float threshold);
__global__ void Erosion(const uchar* binary, uchar* output, Size imgSize, int step);
__global__ void CreateBinaryImage(const uchar* binary, uchar3* output, Size imgSize);

Mat ProcessImage(const Mat& input, Size imgSize);
bool CheckCUDACapableGPU();
void ShowDeviceProperties();
Mat OpenImage(string imgPath);
void SaveImage(Mat img);

int main()
{
    if (!CheckCUDACapableGPU())
        return 0;

    string imgPath = "image1.jpg";
    Mat img = OpenImage(imgPath);
    Size imgSize(img.cols, img.rows); 

    img = ProcessImage(img, imgSize);
    SaveImage(img);

    return 0;
}

Mat ProcessImage(const Mat& image, Size imgSize)
{
    uchar3* input;
    uchar3* output;  
    uchar* erosion; 
    uchar* binary;
    float* intensity;

    cudaMalloc(&input, imgSize.width * imgSize.height * sizeof(uchar3));
    cudaMalloc(&intensity, imgSize.width * imgSize.height * sizeof(float));
    cudaMalloc(&binary, imgSize.width * imgSize.height * sizeof(uchar));
    cudaMalloc(&erosion, imgSize.width * imgSize.height * sizeof(uchar));
    cudaMalloc(&output, imgSize.width * imgSize.height * sizeof(uchar3));

    cudaMemcpy(input, image.ptr<uchar3>(),
        imgSize.width * imgSize.height * sizeof(uchar3), cudaMemcpyHostToDevice);

    dim3 threads(16, 16);
    dim3 blocks(
        (imgSize.width + threads.x - 1) / threads.x,
        (imgSize.height + threads.y - 1) / threads.y);

    auto start = high_resolution_clock::now();
    CalculateIntensity << <blocks, threads >> > (input, intensity, imgSize);
    ApplyThreshold << <blocks, threads >> > (intensity, binary, imgSize, 150);
    Erosion << <blocks, threads >> > (binary, erosion, imgSize, 1);
    CreateBinaryImage << <blocks, threads >> > (erosion, output, imgSize);

    cudaDeviceSynchronize();

    auto end = high_resolution_clock::now();
    double processingTime = duration_cast<milliseconds>(end - start).count();
    cout << processingTime / 1000 << " sec." << endl;

    Mat result(imgSize.height, imgSize.width, CV_8UC3);
    cudaMemcpy(result.ptr<uchar3>(), output,
        imgSize.width * imgSize.height * sizeof(uchar3), cudaMemcpyDeviceToHost);

    cudaFree(input);
    cudaFree(intensity);
    cudaFree(binary);
    cudaFree(erosion);
    cudaFree(output);

    return result;
}

__global__ void CalculateIntensity(const uchar3* input, float* intensity, Size imgSize)
{
    int x = blockDim.x * blockIdx.x + threadIdx.x;
    int y = blockDim.y * blockIdx.y + threadIdx.y;

    if (x < imgSize.width)
        if (y < imgSize.height)
        {
            int index = y * imgSize.width + x;
            uchar3 pixel = input[index];
            intensity[index] = (pixel.x + pixel.y + pixel.z) / 3.0f;
        }        
}

__global__ void ApplyThreshold(float* intensity, uchar* binary, Size imgSize, float threshold) 
{
    int x = blockDim.x * blockIdx.x + threadIdx.x;
    int y = blockDim.y * blockIdx.y + threadIdx.y;

    if (x < imgSize.width)
        if (y < imgSize.height)
        {
            int index = y * imgSize.width + x;
            binary[index] = (intensity[index] >= threshold) ? 1 : 0;
        }        
}

__global__ void Erosion(const uchar* binary, uchar* output, Size imgSize, int step) 
{
    int x = blockDim.x * blockIdx.x + threadIdx.x;
    int y = blockDim.y * blockIdx.y + threadIdx.y;

    if (x < imgSize.width)
        if (y < imgSize.height)
        {
            int index = y * imgSize.width + x;
            int sum = 0;

            for (int i = -step; i <= step; i++)
                for (int j = -step; j <= step; j++) 
                {
                    int newX = x + j;
                    int newY = y + i;

                    if (newX >= 0 && newY >= 0 &&
                        newX < imgSize.width && newY < imgSize.height)
                    {
                        sum += binary[newY * imgSize.width + newX];
                    }
                }

            output[index] = (sum == (2 * step + 1) * (2 * step + 1)) ? 1 : 0;
        }
}

__global__ void CreateBinaryImage(const uchar* binary, uchar3* output, Size imgSize)
{
    int x = blockDim.x * blockIdx.x + threadIdx.x;
    int y = blockDim.y * blockIdx.y + threadIdx.y;

    if (x < imgSize.width)
        if (y < imgSize.height)
        {
            int index = y * imgSize.width + x;
            uchar color = binary[index] ? 255 : 0;
            output[index] = make_uchar3(color, color, color);
        }
}

bool CheckCUDACapableGPU()
{
    int deviceCount;
    cudaGetDeviceCount(&deviceCount);

    if (deviceCount == 0)
    {
        cerr << "No CUDA-capable GPU detected." << endl;
        return false;
    }

    return true;
}

void ShowDeviceProperties()
{
    int deviceCount;
    cudaGetDeviceCount(&deviceCount);

    for (int device = 0; device < deviceCount; ++device) {
        cudaDeviceProp deviceProp;
        cudaGetDeviceProperties(&deviceProp, device);

        cout << "Device " << device << ": " << deviceProp.name << endl;
        cout << "  Compute capability: " << deviceProp.major << "." <<deviceProp.minor << endl;
        cout << "  Total global memory: " << deviceProp.totalGlobalMem / (1024 * 1024) << " MB" << endl;
        cout << "  Multiprocessors: " << deviceProp.multiProcessorCount << endl;
        cout << "  CUDA Cores: " << deviceProp.multiProcessorCount * (deviceProp.major == 2 ? 48 : 128) << endl;
        cout << "  Max threads per block: " << deviceProp.maxThreadsPerBlock << endl;
        cout << "  Max threads per multiprocessor: " << deviceProp.maxThreadsPerMultiProcessor << endl;
        cout << "  Warp size: " << deviceProp.warpSize << endl;
    }
}

Mat OpenImage(string imgPath)
{
    Mat img = imread(imgPath, IMREAD_COLOR);

    if (img.empty())
    {
        cerr << "Error loading image." << endl;
        exit(1);
    }        

    return img;
}

void SaveImage(Mat img)
{
    if (!imwrite("result_" + to_string(img.cols) + "x" + to_string(img.rows) + ".jpg", img))
    {
        cerr << "Error saving image." << endl;
        exit(1);
    }       

    cout << "Image saved successfully." << endl;
}