This is an open repository for the small experiment made in collaboration with Molamil and Space10

Build in Unity 2018.3.0f2.
The project includes 6D.ai and PolyToolkit SDK's. Feel free to update them
To run the project, open the scene: `Assets/_project/main`

## Setup

You need to create and insert 3 keys:
- 6D.ai
- Google Vision
- GooglePoly

### 6D.ai

Create and download your 6D key and place it in `Assets/Plugins/iOS/SixDegreesSDK.plist` To get a key you must create a 6D developer profile. You can read more here: [dashboard.6d.ai](https://dashboard.6d.ai/user/dashboard/?view=home)

### Google Vision

Generate a key for [Google Vision](https://cloud.google.com/vision/) and paste it into the AR Camera object in the  at the script called "Texture To Cloud Vision".

### GooglePoly

Inside `Assets/PolyToolkit/Resources/PtSettings` you need to select "Runtime" and paste in your own API key for Google Poly. Read more here: [developers.google.com/poly/develop/api](https://developers.google.com/poly/develop/api)

## Shaders

For the sake of our prototype, we bought the Hololens Shader pack off the Unity asset store, which let us easily create the dotted surfaces. It is obviously not included in this package. Feel free to [buy it](https://assetstore.unity.com/packages/vfx/shaders/hololens-shader-pack-89989) and only import their shaders. They are nice.

Now you should be able to build the project to an iPhone. We currently have only tested on iPhoneX

## Troubleshooting

- In XCode, if you get "*unexpected duplicate task: CodeSign*", then go to Build Phases and fold out **Embed Frameworks** and remove the extra SixDegreesSDK.framework if there are more than 1
- In XCode, if you get "library not found for -lGTMSessionFetcher", then it's because you are using Google Cardboard for VR, so it has nothing to do with 6D.ai. To fix it, you simply open the **.xcworkspace** file in stead of **.xcodeproj**
