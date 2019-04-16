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

using System.Text;
using System.Collections;
using SixDegrees;
using UnityEngine;
using System;

public class SDKController : MonoBehaviour
{
    private int saveState = (int)SDPlugin.SDSaveState.None;

    private int loadState = (int)SDPlugin.SDLoadState.None;

    private int saveError = (int)SDPlugin.SDSaveError.None;

    private int loadError = (int)SDPlugin.SDLoadError.None;

    private Coroutine activeCoroutine = null;

    public event Action OnSaveSucceededEvent;

    public event Action OnLoadSucceededEvent;

    public event Action<int> OnSaveErrorEvent;

    public event Action<int> OnLoadErrorEvent;

    public event Action OnFindingLocationEvent;

    public event Action OnUploadingEvent;

    public event Action OnDownloadingEvent;

    public event Action OnRelocalizingEvent;

    public event Action OnCancelledEvent;

    public void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (!SDPlugin.IsSDKReady)
        {
            Initialize();
        }
    }

    public void Initialize()
    {
        StringBuilder sb = new StringBuilder(16);
        SDPlugin.SixDegreesSDK_GetVersion(sb, sb.Capacity);
        SDPlugin.Version = sb.ToString();

        Debug.Log("Will initialize SDK v" + SDPlugin.Version);
#if UNITY_IOS && !UNITY_EDITOR
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3) 
        {
            unsafe
            {
                SDPlugin.SixDegreesSDK_InitializeWithEAGL(null);
            }
        }
        else
#endif
        {
            SDPlugin.SixDegreesSDK_Initialize();
        }

        StartCoroutine(InitializeCoroutine());
    }

    public void OnDestroy()
    {
        if (SDPlugin.IsSDKReady && activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;

            SDPlugin.SixDegreesSDK_CancelLoad();
            SDPlugin.SixDegreesSDK_CancelSave();

            OnCancelledEvent();
        }
    }

    public void Load()
    {
        if (!SDPlugin.IsSDKReady)
        {
            return;
        }

        Debug.Log("Will load location map data from AR Cloud");

        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        SDPlugin.SixDegreesSDK_LoadFromARCloud();
        activeCoroutine = StartCoroutine(LoadCoroutine());
    }

    public void Save()
    {
        if (!SDPlugin.IsSDKReady)
        {
            return;
        }

        Debug.Log("Will save location map data to AR Cloud");

        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }

        SDPlugin.SixDegreesSDK_SaveToARCloud();
        activeCoroutine = StartCoroutine(SaveCoroutine());
    }

    private IEnumerator InitializeCoroutine()
    {
        while (!SDPlugin.IsSDKReady)
        {
            SDPlugin.IsSDKReady = SDPlugin.SixDegreesSDK_IsInitialized();

            yield return null;
        }
    }

    private void UpdateSaveStatus()
    {
        unsafe
        {
            fixed (int* saveStatePtr = &saveState, saveErrorPtr = &saveError)
            {
                SDPlugin.SixDegreesSDK_GetSaveStatus(saveStatePtr, saveErrorPtr, null);
            }
        }
    }

    private IEnumerator SaveCoroutine()
    {
        saveState = (int)SDPlugin.SDSaveState.None;

        yield return new WaitForSeconds(0.1f);

        bool saving = true;
        while (saving)
        {
            UpdateSaveStatus();

            switch (saveState)
            {
                case (int)SDPlugin.SDSaveState.DoneFailed:
                case (int)SDPlugin.SDSaveState.DoneSuccess:
                case (int)SDPlugin.SDSaveState.DoneCancelled:
                    saving = false;
                    break;

                case (int)SDPlugin.SDSaveState.Positioning:
                    if (OnFindingLocationEvent != null)
                    {
                        OnFindingLocationEvent();
                    }
                    break;

                case (int)SDPlugin.SDSaveState.Uploading:
                    if (OnUploadingEvent != null)
                    {
                        OnUploadingEvent();
                    }
                    break;

                default:
                    break;
            }

            yield return null;
        }

        if (saveState == (int)SDPlugin.SDSaveState.DoneSuccess)
        {
            StringBuilder sb = new StringBuilder(16);
            SDPlugin.SixDegreesSDK_GetLocationId(sb, sb.Capacity);
            SDPlugin.LocationID = sb.ToString();
            if (OnSaveSucceededEvent != null)
            {
                OnSaveSucceededEvent();
            }
        }
        else if (saveState == (int)SDPlugin.SDSaveState.DoneFailed)
        {
            if (OnSaveErrorEvent != null)
            {
                OnSaveErrorEvent(saveError);
            }
        }

        yield return null;
        activeCoroutine = null;
    }

    private void UpdateLoadStatus()
    {
        unsafe
        {
            fixed (int* loadStatePtr = &loadState, loadErrorPtr = &loadError)
            {
                SDPlugin.SixDegreesSDK_GetLoadStatus(loadStatePtr, loadErrorPtr, null);
            }
        }
    }

    private IEnumerator LoadCoroutine()
    {
        loadState = (int)SDPlugin.SDLoadState.None;

        yield return new WaitForSeconds(0.1f);

        bool loading = true;
        while (loading)
        {
            UpdateLoadStatus();

            switch (loadState)
            {
                case (int)SDPlugin.SDLoadState.DoneFailed:
                case (int)SDPlugin.SDLoadState.DoneSuccess:
                case (int)SDPlugin.SDLoadState.DoneCancelled:
                    loading = false;
                    break;

                case (int)SDPlugin.SDLoadState.Positioning:
                    if (OnFindingLocationEvent != null)
                    {
                        OnFindingLocationEvent();
                    }
                    break;

                case (int)SDPlugin.SDLoadState.Downloading:
                    if (OnDownloadingEvent != null)
                    {
                        OnDownloadingEvent();
                    }
                    break;

                case (int)SDPlugin.SDLoadState.Relocalizing:
                    if (OnRelocalizingEvent != null)
                    {
                        OnRelocalizingEvent();
                    }
                    break;

                default:
                    break;
            }

            yield return null;
        }

        if (loadState == (int)SDPlugin.SDLoadState.DoneSuccess)
        {
            StringBuilder sb = new StringBuilder(16);
            SDPlugin.SixDegreesSDK_GetLocationId(sb, sb.Capacity);
            SDPlugin.LocationID = sb.ToString();
            if (OnLoadSucceededEvent != null)
            {
                OnLoadSucceededEvent();
            }
        }
        else if (loadState == (int)SDPlugin.SDLoadState.DoneFailed)
        {
            if (OnLoadErrorEvent != null)
            {
                OnLoadErrorEvent(loadError);
            }
        }

        yield return null;
        activeCoroutine = null;
    }
}
