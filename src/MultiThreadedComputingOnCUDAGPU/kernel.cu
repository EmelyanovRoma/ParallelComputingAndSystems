#include <iostream>
#include <opencv2/opencv.hpp>
#include "cuda_runtime.h"
#include "device_launch_parameters.h"

const int THRESHOLD = 150;
const int EROSION_STEP = 1;

using namespace std;
using namespace cv;

bool CheckCUDACapableGPU();
void ShowDeviceProperties();
Mat OpenImage(string imgPath);
void SaveImage(Mat img);

int main()
{
    if (!CheckCUDACapableGPU())
        return 0;

    ShowDeviceProperties();

    string imgPath = "image1.jpg";
    Mat img = OpenImage(imgPath);
    Size imgSize;    

    imgSize.width = img.cols;
    imgSize.height = img.rows;

    cout << "Image Dimensions: " << imgSize.width << "x" << imgSize.height << endl;

    SaveImage(img);

    return 0;
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