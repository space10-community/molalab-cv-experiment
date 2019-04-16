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

using UnityEngine;
namespace SixDegrees
{
    public class SDCameraUtils
    {
        public enum CameraMode { Fit, Fill };

        public static CameraMode Mode = CameraMode.Fill;

        public static Rect GetCameraRect(int width, int height)
        {
            bool portrait = Screen.orientation == ScreenOrientation.Portrait || 
                            Screen.orientation == ScreenOrientation.PortraitUpsideDown;

            float targetAspect = portrait ? (float)height / width : (float)width / height;
            float windowAspect = (float)Screen.width / Screen.height;
            float scaleHeight = windowAspect / targetAspect;

            switch (Mode) 
            {
                case CameraMode.Fit:
                return GetFitCameraRect(scaleHeight);

                case CameraMode.Fill:
                default:
                return GetFillCameraRect(scaleHeight);
            }
        }

        private static Rect GetFitCameraRect(float scaleHeight)
        {
            Rect rect = new Rect();

            if (scaleHeight < 1.0f)
            {
                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;
            }
            else
            {
                float scaleWidth = 1.0f / scaleHeight;
                rect.width = scaleWidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scaleWidth) / 2.0f;
                rect.y = 0;
            }

            return rect;
        }

        private static Rect GetFillCameraRect(float scaleHeight)
        {
            Rect rect = new Rect();

            if (scaleHeight < 1.0f)
            {
                float scaleWidth = 1.0f / scaleHeight;
                rect.width = scaleWidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scaleWidth) / 2.0f;
                rect.y = 0;
            }
            else
            {
                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;
            }

            return rect;
        }

        public static Rect GetPixelCameraRect(int width, int height)
        {
            Rect cameraRect = GetCameraRect(width, height);
            return new Rect(cameraRect.x * Screen.width, cameraRect.y * Screen.height, 
                            cameraRect.width * Screen.width, cameraRect.height * Screen.height);
        }

        public static Matrix4x4 GetProjectionMatrix()
        {
            const int bufferSize = 16;
            float[] projectionBuffer = new float[bufferSize];

            unsafe
            {
                fixed (float* ptr = &projectionBuffer[0])
                {
                    SDPlugin.SixDegreesSDK_GetProjection(ptr, bufferSize);
                }
            }

            Matrix4x4 projectionMatrix = new Matrix4x4();
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    projectionMatrix[row, col] = projectionBuffer[row + col * 4];
                }
            }

            bool portrait = Screen.orientation == ScreenOrientation.Portrait ||
                            Screen.orientation == ScreenOrientation.PortraitUpsideDown;

            if (Mode == CameraMode.Fill)
            {
                // fix the projection when filling the screen
                int width = 1920;
                int height = 1080;
                unsafe
                {
                    int* widthPtr = &width, heightPtr = &height;
                    SDPlugin.SixDegreesSDK_GetBackgroundTextureSize(widthPtr, heightPtr);
                }
                float texAspect = (float)width / height;
                float windowAspect = portrait ? (float)Screen.height / Screen.width 
                                              : (float)Screen.width / Screen.height;

                if (texAspect > windowAspect)
                {
                    float scaleFactor = texAspect / windowAspect;
                    projectionMatrix[0, 0] *= scaleFactor;
                    projectionMatrix[0, 2] *= scaleFactor;
                }
                if (windowAspect > texAspect)
                {
                    float scaleFactor = windowAspect / texAspect;
                    projectionMatrix[1, 1] *= scaleFactor;
                    projectionMatrix[1, 2] *= scaleFactor;
                }
            }

            if (portrait) 
            {
                // fix the projection in portrait
                float p00 = projectionMatrix[0, 0];
                projectionMatrix[0, 0] = projectionMatrix[1, 1];
                projectionMatrix[1, 1] = p00;
                float p02 = projectionMatrix[0, 2];
                projectionMatrix[0, 2] = projectionMatrix[1, 2];
                projectionMatrix[1, 2] = p02;
            }

            return projectionMatrix;
        }
    }
}
