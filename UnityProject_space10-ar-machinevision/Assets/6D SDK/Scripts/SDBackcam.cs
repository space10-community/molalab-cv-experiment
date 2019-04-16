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
using System.Collections;
using UnityEngine.UI;
using System;

namespace SixDegrees
{
    public class SDBackcam : MonoBehaviour
    {
        public SDCameraUtils.CameraMode cameraMode = SDCameraUtils.CameraMode.Fill;
        public RawImage background;
        private Texture2D mBackgroundTexture;
        private Camera mCamera;
        private ScreenOrientation mOrientation;

        void Awake()
        {
            background.gameObject.SetActive(false);
            mCamera = GetComponent<Camera>();

            if (!mCamera)
            {
                Debug.LogWarning("SDBackcam must be attached to a camera object!");
                return;
            }

            if (!background)
            {
                Debug.LogWarning("SDBackcam must have a background RawImage!");
                mCamera.enabled = false;
                return;
            }

            SDCameraUtils.Mode = cameraMode;
            mCamera.rect = SDCameraUtils.GetCameraRect(1920, 1080);
            mOrientation = ScreenOrientation.AutoRotation;
        }

        void SetupBackgroundTexture()
        {
            background.gameObject.SetActive(true);
            IntPtr texturePtr = IntPtr.Zero;

#if UNITY_IOS && !UNITY_EDITOR
            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3) 
            {
                int textureId = SDPlugin.SixDegreesSDK_GetEAGLBackgroundTexture();
                texturePtr = new IntPtr(textureId);
            }
            else
#endif
            {
                texturePtr = SDPlugin.SixDegreesSDK_GetBackgroundTexture();
            }

            if (texturePtr != IntPtr.Zero)
            {
                int width = 1920;
                int height = 1080;
                unsafe
                {
                    int* widthPtr = &width, heightPtr = &height;
                    SDPlugin.SixDegreesSDK_GetBackgroundTextureSize(widthPtr, heightPtr);
                }

                Debug.Log("Create External Texture:" + texturePtr + "(" + width + "x" + height + ")");
                mBackgroundTexture = Texture2D.CreateExternalTexture(
                    width,
                    height,
                    TextureFormat.RGBA32,
                    false,
                    false,
                    texturePtr);
                mBackgroundTexture.filterMode = FilterMode.Point;
                mBackgroundTexture.name = "camera_texture";
                background.color = Color.black;
                background.texture = mBackgroundTexture;
                background.material.SetTexture("_MainTex", mBackgroundTexture);
            }
        }

        void UpdateAspectRatio()
        {
            mOrientation = Screen.orientation;

            int width = mBackgroundTexture.width;
            int height = mBackgroundTexture.height;
            Vector2 pixelSize = SDCameraUtils.GetPixelCameraRect(width, height).size;
            Vector2 rotatedPixelSize = new Vector2(pixelSize.y, pixelSize.x); // for portrait mode

            mCamera.rect = SDCameraUtils.GetCameraRect(width, height);
            background.uvRect = new Rect(0f, 1f, 1f, -1f);

            switch (mOrientation)
            {
                case ScreenOrientation.LandscapeRight:
                    background.rectTransform.sizeDelta = pixelSize;
                    background.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 180f);
                    break;
                case ScreenOrientation.Portrait:
                    background.rectTransform.sizeDelta = rotatedPixelSize;
                    background.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -90f);
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                    background.rectTransform.sizeDelta = rotatedPixelSize;
                    background.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                    break;
                default:
                case ScreenOrientation.LandscapeLeft:
                    background.rectTransform.sizeDelta = pixelSize;
                    background.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    break;
            }
        }

        void OnPreRender()
        {
            GL.Clear(false, true, Color.black, 0f);

            if (!SDPlugin.IsSDKReady)
            {
                return;
            }

            if (!mBackgroundTexture)
            {
                SetupBackgroundTexture();
            }

            if (mBackgroundTexture && mOrientation != Screen.orientation)
            {
                UpdateAspectRatio();
            }
        }
    }
}