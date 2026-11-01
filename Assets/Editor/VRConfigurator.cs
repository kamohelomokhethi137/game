using UnityEditor;
using UnityEditor.Build;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;

public static class VRConfigurator
{
    [MenuItem("Tools/Configure Android VR")]
    public static void ConfigureAndroidVR()
    {
        XRGeneralSettingsPerBuildTarget perBuildTarget;
        EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.settingsKey, out perBuildTarget);

        if (perBuildTarget == null)
        {
            perBuildTarget = ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
            if (!AssetDatabase.IsValidFolder("Assets/XR"))
            {
                AssetDatabase.CreateFolder("Assets", "XR");
            }
            AssetDatabase.CreateAsset(perBuildTarget, "Assets/XR/XRGeneralSettingsPerBuildTarget.asset");
            EditorBuildSettings.AddConfigObject(XRGeneralSettings.settingsKey, perBuildTarget, true);
            Debug.Log("Created XR general settings asset.");
        }

        XRGeneralSettings generalSettings = perBuildTarget.SettingsForBuildTarget(BuildTargetGroup.Android);
        if (generalSettings == null)
        {
            perBuildTarget.CreateDefaultSettingsForBuildTarget(BuildTargetGroup.Android);
            generalSettings = perBuildTarget.SettingsForBuildTarget(BuildTargetGroup.Android);
        }

        if (generalSettings == null)
        {
            Debug.LogError("Could not create XR settings for Android.");
            return;
        }

        if (generalSettings.Manager == null)
        {
            generalSettings.Manager = ScriptableObject.CreateInstance<XRManagerSettings>();
        }

        bool assigned = XRPackageMetadataStore.AssignLoader(
            generalSettings.Manager, "UnityEngine.XR.OpenXR.OpenXRLoader", BuildTargetGroup.Android);
        Debug.Log("OpenXR loader assigned for Android: " + assigned);

        OpenXRSettings openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
        if (openXRSettings != null)
        {
            MetaQuestFeature questFeature = openXRSettings.GetFeature<MetaQuestFeature>();
            if (questFeature != null)
            {
                questFeature.enabled = true;
                Debug.Log("Meta Quest Support feature enabled.");
            }
            else
            {
                Debug.LogWarning("Meta Quest Support feature not found in OpenXR settings.");
            }
        }
        else
        {
            Debug.LogWarning("OpenXR settings for Android not found.");
        }

        PlayerSettings.companyName = "VR Student";
        PlayerSettings.productName = "Deep Sea Treasure Hunt";
        PlayerSettings.colorSpace = ColorSpace.Linear;

        NamedBuildTarget android = NamedBuildTarget.Android;
        PlayerSettings.SetApplicationIdentifier(android, "com.vrstudent.deepseatreasure");
        PlayerSettings.SetScriptingBackend(android, ScriptingImplementation.IL2CPP);

        try
        {
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            Debug.Log("Android player settings configured (IL2CPP, ARM64, min SDK 29).");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Could not set all Android player settings: " + e.Message);
        }

        EditorUtility.SetDirty(perBuildTarget);
        AssetDatabase.SaveAssets();
    }
}
