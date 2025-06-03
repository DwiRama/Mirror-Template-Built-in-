using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildVersionProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    private const string initialVersion = "0.0.0";

    public void OnPreprocessBuild(BuildReport report)
    {
        string currentVersion = FindCurrentVersion();
        UpdateVersion(currentVersion);
    }

    private string FindCurrentVersion()
    {
        // Split to find string of current version
        string[] currentVersion = PlayerSettings.bundleVersion.Split('[', ']');

        // If not the proper format, start with the initial version
        return currentVersion.Length == 1? initialVersion : currentVersion[1];
    }

    private void UpdateVersion(string version)
    {
        //Version build is separte between 3 parts: major.minor.patch
        //We store each part into the currentVersion array
        string[] parts = version.Split('.');

        if (parts.Length == 3)
        {
            // Parse the patch value from string to float
            if (float.TryParse(parts[2], out float patch))
            {
                //Increase patch value
                float newPatchVersion = patch + 1;

                // Create new build version string and set it in Player Settings
                PlayerSettings.bundleVersion = $"[{parts[0]}.{parts[1]}.{newPatchVersion}]";
                Debug.Log($"Build Version: {PlayerSettings.bundleVersion}");
            }
        }
    }
}
