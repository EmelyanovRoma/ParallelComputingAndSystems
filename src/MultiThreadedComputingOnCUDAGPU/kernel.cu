#include <iostream>
#include <opencv2/opencv.hpp>
#include "cuda_runtime.h"
#include "device_launch_parameters.h"

using namespace std;

bool CheckCUDACapableGPU();
void ShowDeviceProperties();

bool CheckCUDACapableGPU()
{
    int deviceCount;
    cudaGetDeviceCount(&deviceCount);

    if (deviceCount == 0)
    {
        cout << "No CUDA-capable GPU detected." << endl;
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

int main() 
{    
    if (!CheckCUDACapableGPU())
        return 0;

    ShowDeviceProperties();

    return 0;
}