/***********************************************************
* Copyright (C) 2018 6degrees.xyz Inc.
*
* This file is part of the 6D.ai Beta SDK and is not licensed
* for commercial use.
*
* The 6D.ai Beta SDK can not be copied and/or distributed without
* the express permission of 6degrees.xyz Inc.
*
* Contact developers@6d.ai for licensing requests.
***********************************************************/

using System;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SixDegrees
{
    public class SDPlugin : MonoBehaviour
    {
        public static bool IsSDKReady = false;
        public static string LocationID = "";
        public static string Version = "";

        public enum SDTrackingQuality
        {
            None = 0,
            Limited = 1,
            Good = 2
        };

        public enum SDSaveState
        {
            None = 0,
            Positioning = 1,
            Uploading = 2,
            DoneSuccess = 3,
            DoneFailed = 4,
            DoneCancelled = 5
        };

        public enum SDSaveError
        {
            None = 0,
            Unknown = 1,
            NotEnoughSpace = 2,
            Offline = 3,
            CloudNotAvailable = 4,
            NotAuthorized = 5,
            LocationNotAvailable = 6
        };

        public enum SDLoadState
        {
            None = 0,
            Positioning = 1,
            Downloading = 2,
            Relocalizing = 3,
            DoneSuccess = 4,
            DoneFailed = 5,
            DoneCancelled = 6
        };

        public enum SDLoadError
        {
            None = 0,
            Unknown = 1,
            NotEnoughSpace = 2,
            Offline = 3,
            CloudNotAvailable = 4,
            NotAuthorized = 5,
            LocationNotAvailable = 6,
            DataNotAvailable = 7,
            FailedToRelocalize = 8
        };

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void SixDegreesSDK_GetVersion(StringBuilder versionOut, int bufferSize);

        [DllImport("__Internal")]
        public static extern bool SixDegreesSDK_IsDeviceSupported();

        [DllImport("__Internal")]
        public static extern void SixDegreesSDK_Initialize();

        [DllImport("__Internal")]
        public static unsafe extern void SixDegreesSDK_InitializeWithEAGL(void* eaglContext);

        [DllImport("__Internal")]
        public static extern bool SixDegreesSDK_IsInitialized();

        [DllImport("__Internal")]
        public static extern IntPtr SixDegreesSDK_GetBackgroundTexture();

        [DllImport("__Internal")]
        public static extern int SixDegreesSDK_GetEAGLBackgroundTexture();

        [DllImport("__Internal")]
        public unsafe static extern void SixDegreesSDK_GetBackgroundTextureSize(int* widthOut, int* heightOut);

        [DllImport("__Internal")]
        public unsafe static extern int SixDegreesSDK_GetPose(float* poseDataOut, int bufferSize);

        [DllImport("__Internal")]
        public unsafe static extern void SixDegreesSDK_GetProjection(float* projectionDataOut, int bufferSize);

        [DllImport("__Internal")]
        public static extern void SixDegreesSDK_GetLocationId(StringBuilder locationIdOut, int bufferSize);

        [DllImport("__Internal")]
        public static extern void SixDegreesSDK_SaveToARCloud();

        [DllImport("__Internal")]
        public static extern void SixDegreesSDK_CancelSave();

        [DllImport("__Internal")]
        public unsafe static extern void SixDegreesSDK_GetSaveStatus(int* stateOut, int* errorOut, long* timestampOut);

        [DllImport("__Internal")]
        public static extern void SixDegreesSDK_LoadFromARCloud();

        [DllImport("__Internal")]
        public static extern void SixDegreesSDK_CancelLoad();

        [DllImport("__Internal")]
        public unsafe static extern void SixDegreesSDK_GetLoadStatus(int* stateOut, int* errorOut, long* timestampOut);

        [DllImport("__Internal")]
        public unsafe static extern int SixDegreesSDK_GetMeshBlockInfo(int* blockBufferSizeOut, int* vertexBufferSizeOut, int* indexBufferSizeOut);

        [DllImport("__Internal")]
        public static extern float SixDegreesSDK_GetMeshBlockSize();

        [DllImport("__Internal")]
        public unsafe static extern int SixDegreesSDK_GetMeshBlocks(int* blockBuffer, float* vertexBuffer, int* indexBuffer, int blockBufferSize, int vertexBufferSize, int indexBufferSize);
#else
        public static void SixDegreesSDK_GetVersion(StringBuilder versionOut, int bufferSize) {}
        public static bool SixDegreesSDK_IsDeviceSupported() { return false; }
        public static void SixDegreesSDK_Initialize() {}
        public static bool SixDegreesSDK_IsInitialized() { return false; }
        public static IntPtr SixDegreesSDK_GetBackgroundTexture() { return IntPtr.Zero; }
        public unsafe static void SixDegreesSDK_GetBackgroundTextureSize(int* widthOut, int* heightOut) {}
        public unsafe static int SixDegreesSDK_GetPose(float* poseDataOut, int bufferSize) { return 0; }
        public unsafe static void SixDegreesSDK_GetProjection(float* projectionDataOut, int bufferSize) {}
        public static void SixDegreesSDK_GetLocationId(StringBuilder locationIdOut, int bufferSize) {}
        public static void SixDegreesSDK_SaveToARCloud() {}
        public static void SixDegreesSDK_CancelSave() {}
        public unsafe static void SixDegreesSDK_GetSaveStatus(int* state, int* error, long* timestamp) {}
        public static void SixDegreesSDK_LoadFromARCloud() {}
        public static void SixDegreesSDK_CancelLoad() {}
        public unsafe static void SixDegreesSDK_GetLoadStatus(int* state, int* error, long* timestamp) {}
        public unsafe static int SixDegreesSDK_GetMeshBlockInfo(int* blockBufferSizeOut, int* vertexBufferSizeOut, int* indexBufferSizeOut) { return -1; }
        public static float SixDegreesSDK_GetMeshBlockSize() { return 0.0f; }
        public unsafe static int SixDegreesSDK_GetMeshBlocks(int* blockBuffer, float* vertexBuffer, int* indexBuffer, int blockBufferSize, int vertexBufferSize, int indexBufferSize) { return 0; }
#endif

    }
}

