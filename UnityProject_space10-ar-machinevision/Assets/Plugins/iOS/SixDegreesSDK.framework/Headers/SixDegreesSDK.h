/*!
@header SixDegreesSDK.h
@copyright Copyright (C) 2018 6degrees.xyz Inc.
@abstract This file is part of the 6D.ai Beta SDK and is not licensed
for commercial use.

The 6D.ai Beta SDK can not be copied and/or distributed without
the express permission of 6degrees.xyz Inc.

Contact developers\@6d.ai for licensing requests.
@version beta 0.18.0
@ignore FOUNDATION_EXPORT
 */

#pragma once

#import <UIKit/UIKit.h>

/*!
 @brief Populates versionOut with the current version number of the SDK, e.g. "0.18.0"
 @param versionOut pointer to a char buffer
 @param bufferSize size of the char buffer (recommended value: 16)
 */
FOUNDATION_EXPORT void SixDegreesSDK_GetVersion(char* versionOut, int bufferSize);

/*!
 @brief Returns whether the device is supported by this SDK.
 @discussion Depth estimation and real world mesh building are resource-intensive processes that are
 enabled on the most powerful devices only. @link SixDegreesSDK_Initialize() @/link will fail if this
 returns false.
 @return true if the device is supported, false otherwise.
 */
FOUNDATION_EXPORT bool SixDegreesSDK_IsDeviceSupported(void);

/*!
 @brief Initializes ARKit and the SDK's internal states using the Metal pipeline. Call this first!
 @discussion Most other API calls in this SDK will not work until you call this. Call only once!
 
 This API call returns early, use @link SixDegreesSDK_IsInitialized() @/link to monitor initialization progress.
 
 Initialization will fail if the current device is not supported.
 Use @link SixDegreesSDK_IsDeviceSupported() @/link to check that the device is supported.
 */
FOUNDATION_EXPORT void SixDegreesSDK_Initialize(void);

/*!
 @brief Initializes ARKit and the SDK's internal states using the OpenGL pipeline. Call this first!
 @discussion Most other API calls in this SDK will not work until you call this. Call only once!
 
 This API call returns early, use @link SixDegreesSDK_IsInitialized() @/link to monitor initialization progress.
 
 Initialization will fail if the current device is not supported.
 Use @link SixDegreesSDK_IsDeviceSupported() @/link to check that the device is supported.
 
 This API call is deprecated, we recommend you use Metal instead with @link SixDegreesSDK_Initialize() @/link.
 @param eaglContext optional pointer to the desired EAGLContext handle
 (6D will use the current one or create one if it is null).
 */
FOUNDATION_EXPORT __attribute__((deprecated)) void SixDegreesSDK_InitializeWithEAGL(void* eaglContext);

/*!
 @brief Returns true once the SDK is initialized.
 @return true when the SDK is initialized and the other methods below are ready to be called.
 */
FOUNDATION_EXPORT bool SixDegreesSDK_IsInitialized(void);

/*!
 @brief Returns the Metal texture pointer of the camera background.
 @return the Metal texture pointer of the camera background, null if none available.
 */
FOUNDATION_EXPORT void* SixDegreesSDK_GetBackgroundTexture(void);

/*!
 @brief Returns the OpenGL texture handle of the camera background.
 @return the OpenGL texture handle of the camera background, 0 if none available.
 */
FOUNDATION_EXPORT __attribute__((deprecated)) int SixDegreesSDK_GetEAGLBackgroundTexture(void);

/*!
 @brief Populates widthOut and heightOut with the pixel size of the camera background texture, if available.
 @param widthOut the pointer to the width value.
 @param heightOut the pointer to the height value.
 */
FOUNDATION_EXPORT void SixDegreesSDK_GetBackgroundTextureSize(int* widthOut, int* heightOut);

//! @brief Quality of the AR pose tracking system.
enum SixDegreesTrackingQuality {
    //! No pose available
    SixDegreesTrackingQualityNone = 0,
    //! Pose available, tracking is limited
    SixDegreesTrackingQualityLimited = 1,
    //! Pose available, tracking is good
    SixDegreesTrackingQualityGood = 2,
};

/*!
 @brief Populates poseDataOut with a column-major 4x4 matrix describing the position and orientation of the camera
 in Unity world coordinates.
 @discussion Unity world coordinates assume Y is up (aligned with gravity), X is right and Z forward, relatively
 to the orientation of the device the first time the map was created.
 @param poseDataOut pointer to the first element a float array of size 16.
 @param bufferSize must be 16.
 @return tracking quality (@link SixDegreesTrackingQuality@/link).
 */
FOUNDATION_EXPORT int SixDegreesSDK_GetPose(float* poseDataOut, int bufferSize);

/*!
 @brief Populates projectionDataOut with a column-major 4x4 matrix describing the projection of the camera.
 @discussion Takes into account the pixel size of the background texture. Clipping planes are at 0.001 and 1000.0.
 @param projectionDataOut pointer to the first element float array of size 16.
 @param bufferSize must be 16.
 */
FOUNDATION_EXPORT void SixDegreesSDK_GetProjection(float* projectionDataOut, int bufferSize);

/*!
 @brief Populates locationIdOut with the unique identifier of the location for which the current pose is applicable.
 @discussion locationIdOut will be populated with an empty string until:
 
 - location map data was loaded from the AR Cloud AND the device successfully relocalized;
 
 - or the location map data was successfully saved to the AR Cloud.
 
 It will then be populated with a unique location identifier, common to all users and sessions at this location.
 
 It guarantees the consistency of the coordinate system across time and devices, enabling persistence and multiplayer.
 @param locationIdOut pointer to the first element of a char array of size 16.
 @param bufferSize must be 16.
 */
FOUNDATION_EXPORT void SixDegreesSDK_GetLocationId(char* locationIdOut, int bufferSize);

/*!
 @brief Persists the location map data to the 6D AR Cloud.
 @discussion This API call returns early, use @link SixDegreesSDK_GetSaveStatus() @/link to monitor saving progress.
 */
FOUNDATION_EXPORT void SixDegreesSDK_SaveToARCloud(void);

/*!
 @brief Interrupts and cancels the current SaveToARCloud process.
 */
FOUNDATION_EXPORT void SixDegreesSDK_CancelSave(void);

//! @brief State of the SaveToARCloud process.
enum SixDegreesSaveState {
    //! SaveToARCloud() was never called.
    SixDegreesSaveStateNone = 0,
    //! Approximate GPS Position is being requested.
    SixDegreesSaveStatePositioning = 1,
    //! Location map data is being packaged and transmitted to the AR Cloud.
    SixDegreesSaveStateUploading = 2,
    //! Location map data was saved successfully.
    SixDegreesSaveStateDoneSuccess = 3,
    //! Location map data could not be saved, check the error enum for details.
    SixDegreesSaveStateDoneFailed = 4,
    //! SaveARCloud() was interrupted by a call to CancelSave().
    SixDegreesSaveStateDoneCancelled = 5,
};

//! @brief Error in the SaveToARCloud process.
enum SixDegreesSaveError {
    //! SaveToARCloud() did not produce any errors.
    SixDegreesSaveErrorNone = 0,
    //! Placeholder for unknown and future errors.
    SixDegreesSaveErrorUnknown = 1,
    //! The device file system is too full to package the location map data.
    SixDegreesSaveErrorNotEnoughSpace = 2,
    //! The device is not connected to the Internet.
    SixDegreesSaveErrorOffline = 3,
    //! The AR Cloud cannot be reached.
    SixDegreesSaveErrorCloudNotAvailable = 4,
    //! The AR Cloud rejected the save request.
    SixDegreesSaveErrorNotAuthorized = 5,
    //! The device cannot approximate its location by GPS or Wi-Fi.
    SixDegreesSaveErrorLocationNotAvailable = 6,
};

/*!
 @brief Provides information about the current or last SaveToARCloud process.
 @discussion Populates stateOut with the current state code of the SaveToARCloud process,
 
 errorOut with an error code if applicable,
 
 timestampOut with the POSIX timestamp of the last time @link SixDegreesSDK_SaveToARCloud() @/link was called.
 @param stateOut optional pointer to the state value (@link SixDegreesSaveState@/link).
 @param errorOut optional pointer to the error value (@link SixDegreesSaveError@/link).
 @param timestampOut optional pointer to the timestamp value.
 */
FOUNDATION_EXPORT void SixDegreesSDK_GetSaveStatus(int* stateOut, int* errorOut, long int* timestampOut);

/*!
 @brief Loads the location map data from the 6D AR Cloud.
 @discussion This API call returns early, use @link SixDegreesSDK_GetLoadStatus() @/link to monitor saving progress.
 */
FOUNDATION_EXPORT void SixDegreesSDK_LoadFromARCloud(void);

/*!
 @brief Interrupts and cancels the current LoadFromARCloud process.
 */
FOUNDATION_EXPORT void SixDegreesSDK_CancelLoad(void);

//! @brief State of the LoadFromARCloud process.
enum SixDegreesLoadState {
    //! LoadFromARCloud() was never called.
    SixDegreesLoadStateNone = 0,
    //! Approximate GPS Position is being requested.
    SixDegreesLoadStatePositioning = 1,
    //! Location map data is being received and unpackaged from the AR Cloud.
    SixDegreesLoadStateDownloading = 2,
    //! Location map data is being used to relocalize the device at this location.
    SixDegreesLoadStateRelocalizing = 3,
    //! Location map data was successfully loaded and the device relocalized.
    SixDegreesLoadStateDoneSuccess = 4,
    //! Location map data could not be loaded, or the device could not relocalize, check the error enum for details.
    SixDegreesLoadStateDoneFailed = 5,
    //! LoadFromARCloud() was interrupted by a call to CancelLoad().
    SixDegreesLoadStateDoneCancelled = 6,
};

//! @brief Error in the LoadFromARCloud process.
enum SixDegreesLoadError {
    //! LoadFromARCloud() did not produce any errors.
    SixDegreesLoadErrorNone = 0,
    //! Placeholder for unknown and future errors.
    SixDegreesLoadErrorUnknown = 1,
    //! The device file system is too full to download and unpackage the location map data.
    SixDegreesLoadErrorNotEnoughSpace = 2,
    //! The device is not connected to the Internet.
    SixDegreesLoadErrorOffline = 3,
    //! The AR Cloud cannot be reached.
    SixDegreesLoadErrorCloudNotAvailable = 4,
    //! The AR Cloud rejected the load request.
    SixDegreesLoadErrorNotAuthorized = 5,
    //! The device cannot approximate its location by GPS or Wi-Fi.
    SixDegreesLoadErrorLocationNotAvailable = 6,
    //! The AR Cloud doesn't have map data for this location.
    SixDegreesLoadErrorDataNotAvailable = 7,
    //! The device could not relocalize in less than 30 seconds at this location.
    SixDegreesLoadErrorFailedToRelocalize = 8,
};

/*!
 @brief Provides information about the current or last LoadFromARCloud process.
 @discussion Populates stateOut with the current state code of the LoadFromARCloud process,
 
 errorOut with an error code if applicable,
 
 timestampOut with the POSIX timestamp of the last time @link SixDegreesSDK_LoadFromARCloud() @/link was called.
 @param stateOut optional pointer to the state value (@link SixDegreesLoadState@/link).
 @param errorOut optional pointer to the error value (@link SixDegreesLoadError@/link).
 @param timestampOut optional pointer to the timestamp value.
 */
FOUNDATION_EXPORT void SixDegreesSDK_GetLoadStatus(int* stateOut, int* errorOut, long int* timestampOut);

/*!
 @brief Returns real world mesh information needed for @link SixDegreesSDK_GetMeshBlocks()@/link calls.
 @param blockBufferSizeOut optional pointer to the minimum block buffer size in ints.
 @param vertexBufferSizeOut optional pointer to the minimum vertex buffer size in floats.
 @param faceBufferSizeOut optional pointer to the minimum face index buffer size in ints.
 @discussion The mesh is cut into blocks, which are are cubes which size is obtained with
 @link SixDegreesSDK_GetMeshBlockSize()@/link.
 Blocks provide a reliable way to break down the mesh and track changes between updates.
 
 Block data can be obtained with the @link SixDegreesSDK_GetMeshBlocks()@/link method, which
 required passing pointers to buffers. This function gives you the minimum size for those buffers.
 
 The returned version number increments every time the mesh changes. You can keep track of the
 version number to avoid unnecessary calls to GetMeshBlocks().
 @return the version number of the mesh, -1 if no mesh is available.
 */
FOUNDATION_EXPORT int SixDegreesSDK_GetMeshBlockInfo(int* blockBufferSizeOut, int* vertexBufferSizeOut, int* faceBufferSizeOut);

/*!
 @brief Returns the size of a mesh block.
 @return the size of the side of a block in meters.
 */
FOUNDATION_EXPORT float SixDegreesSDK_GetMeshBlockSize(void);

/*!
 @brief Populates the provided buffers with real world mesh information.
 @discussion All three buffers come with their respective size to prevent the implementation from overflowing,
 and the function returns the number of blocks fully populated.
 @param blockBuffer contains a contiguous array of signed integers { x, y, z, vertexCount, faceCount, version }
 where x, y, z is the location of the block in space.
 @param vertexBuffer contains a contiguous array of floats { x, y, z, nx, ny, nz }, each 6 representing a point
 in 3D space where x, y, z is the location of the vertex, and nx, ny, nz is the normal vector of the vertex.
 @param faceBuffer contains a contiguous array of vertex indices { a, b, c }, each 3 representing a triangle.
 @param blockBufferSize the size of the block buffer, in ints
 @param vertexBufferSize the size of the vertex buffer, in floats
 @param faceBufferSize the size of the index buffer, in ints
 @return the number of blocks fully populated, or -1 if the mesh is not ready or if at least one of the buffer
 was too small to accommodate all the mesh data. Use @link SixDegreesSDK_GetMeshBlockInfo()@/link to get the
 recommended buffer sizes.
 */
FOUNDATION_EXPORT int SixDegreesSDK_GetMeshBlocks(int* blockBuffer, float* vertexBuffer, int* faceBuffer, int blockBufferSize, int vertexBufferSize, int faceBufferSize);
