/*
 * Based on http://docs.unity3d.com/ScriptReference/MenuItem.html, http://answers.unity3d.com/questions/829349/command-line-flag-to-build-webgl.html
 * and http://jonathanpeppers.com/Blog/automating-unity3d-builds-with-fake
 * Command line invocation should be similar to:
 * 	/Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -quit -projectPath="path/to/project" -executeMethod BuildScript.All -logFile ./last_batch_build.log
 */

using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

public class BuildScript
{
	[MenuItem ("Build/All")]
	static void All()
	{
		WebGL ();
		OSX ();
		LinuxHeadless ();
	}
	[MenuItem ("Build/WebGL")]
	static void WebGL()
    {
		string targetDir = "Build/" + PlayerSettings.productName + "-game-webgl-dist";
		if(Directory.Exists (targetDir))
        {
            Directory.Delete (targetDir, true);
        }
		BuildPipeline.BuildPlayer(GetScenes(), targetDir, BuildTarget.WebGL, BuildOptions.None);
    }

	[MenuItem ("Build/OSX Universal")]
	static void OSX()
	{
		BuildPipeline.BuildPlayer(GetScenes(), "Build/" + PlayerSettings.productName + "-game-osx", BuildTarget.StandaloneOSX, BuildOptions.None);
	}

    [MenuItem ("Build/Linux Universal Headless")]
	static void LinuxHeadless()
	{
		BuildPipeline.BuildPlayer(GetScenes(), "Build/" + PlayerSettings.productName + "-game-linux-headless", BuildTarget.StandaloneLinuxUniversal, BuildOptions.EnableHeadlessMode);
	}

    static string[] GetScenes()
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        List<string> enabledScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in scenes)
        {
             if (scene.enabled)
             {
                 enabledScenes.Add(scene.path);
             }
         }
         return enabledScenes.ToArray();
    }
}
