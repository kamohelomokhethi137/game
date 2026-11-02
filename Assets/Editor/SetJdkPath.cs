using UnityEditor;
using UnityEditor.Android;
using UnityEngine;

public class SetJdkPath
{
    [MenuItem("Tools/Set JDK Path")]
    public static void SetJdk()
    {
        AndroidExternalToolsSettings.jdkRootPath = "C:/Program Files/Java/jdk-17";
        Debug.Log("JDK root path set to: C:/Program Files/Java/jdk-17");
    }
}
