# Experiment Title

![experiment-hero](https://space10.io/content/uploads/2019/04/molamil-16-9-no-ui.png)

This is an open repository for the small experiment made in collaboration between Molamil and **SPACE10**.

## Setup

Build in Unity 2018.3.0f2.
The project includes 6D.ai and PolyToolkit SDK's. Feel free to update them.
To run the project, open the scene: `Assets/_project/main`

You need to create and insert 3 keys:
- 6D.ai
- Google Vision
- Google Poly

### 6D.ai

Create and download your 6D key and place it in `Assets/Plugins/iOS/SixDegreesSDK.plist` To get a key you must create a 6D developer profile. You can read more here: [dashboard.6d.ai](https://dashboard.6d.ai/user/dashboard/?view=home)

### Google Vision

Generate a key for [Google Vision](https://cloud.google.com/vision/) and paste it into the AR Camera object in the  at the script called "Texture To Cloud Vision".

### Google Poly

Inside `Assets/PolyToolkit/Resources/PtSettings` you need to select "Runtime" and paste in your own API key for Google Poly. Read more here: [developers.google.com/poly/develop/api](https://developers.google.com/poly/develop/api)

## Shaders

For the sake of our prototype, we bought the Hololens Shader pack off the Unity asset store, which let us easily create the dotted surfaces. It is obviously not included in this package. Feel free to [buy it](https://assetstore.unity.com/packages/vfx/shaders/hololens-shader-pack-89989) and only import their shaders. They are nice.

Now you should be able to build the project to an iPhone. We recommend running the project on an iPhone 8 or newer.

## Google Cardboard

While the experiment is fully capable of running without it, we encourage you to purchase a Google Cardboard and fit it to your own phone, cutting a whole for the front-facing camera. That way the experience becomes truly embodied and more immersive.

## Troubleshooting

- In XCode, if you get "*unexpected duplicate task: CodeSign*", then go to Build Phases and fold out **Embed Frameworks** and remove the extra SixDegreesSDK.framework if there are more than 1
- In XCode, if you get "library not found for -lGTMSessionFetcher", then it's because you are using Google Cardboard for VR, so it has nothing to do with 6D.ai. To fix it, you simply open the **.xcworkspace** file in stead of **.xcodeproj**
