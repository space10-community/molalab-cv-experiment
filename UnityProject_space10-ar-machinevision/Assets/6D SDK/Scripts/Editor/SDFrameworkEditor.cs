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

using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

namespace SixDegrees 
{
    public class SDFrameworkEditor
    {
        [PostProcessBuildAttribute(100)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
            if (target != BuildTarget.iOS) {
                UnityEngine.Debug.LogWarning ("Target is not iPhone. XCodePostProcess will not run");
                return;
            }
            // Open Project
            string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            PBXProject proj = new PBXProject();
            proj.ReadFromFile(projPath);
            string targetGuid = proj.TargetGuidByName("Unity-iPhone");
            // Embed SixDegreesSDK.framework
            const string defaultLocationInProj = "Plugins/iOS";
            const string coreFrameworkName = "SixDegreesSDK.framework";
            string framework = Path.Combine(defaultLocationInProj, coreFrameworkName);
            string fileGuid = proj.AddFile(framework, "Frameworks/" + framework, PBXSourceTree.Sdk);
            PBXProjectExtensions.AddFileToEmbedFrameworks(proj, targetGuid, fileGuid);
            proj.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
            // Disable Bitcode
            proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
            // Prevent Encryption Compliance Info in AppStoreConnect
            string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            PlistElementDict rootDict = plist.root;
            rootDict.SetString("ITSAppUsesNonExemptEncryption", "false");
            File.WriteAllText(plistPath, plist.WriteToString());
            // Include SixDegreesSDK.plist
            FileUtil.ReplaceFile ("Assets/Plugins/iOS/SixDegreesSDK.plist", pathToBuiltProject + "/SixDegreesSDK.plist");
            proj.AddFileToBuild (targetGuid, proj.AddFile("SixDegreesSDK.plist", "SixDegreesSDK.plist"));
            proj.WriteToFile (projPath);
        }
    }
}


