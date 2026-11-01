using System.IO;
using UnityEditor;
using UnityEngine;

public static class DocShotBuilder
{
    [MenuItem("Tools/Render Concept Shots")]
    public static void RenderShots()
    {
        RenderOverview();
        Debug.Log("Concept shots rendered.");
    }

    static void RenderOverview()
    {
        const int width = 1600;
        const int height = 900;

        GameObject camGo = new GameObject("DocCam_Temp");
        Camera cam = camGo.AddComponent<Camera>();
        cam.transform.position = new Vector3(0f, 4.2f, -7.5f);
        cam.transform.LookAt(new Vector3(0f, 0.8f, 0f));
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.03f, 0.16f, 0.26f, 1f);
        cam.fieldOfView = 55f;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 200f;

        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 4;
        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        string dir = "Assets/Screenshots";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        File.WriteAllBytes(Path.Combine(dir, "doc_overview.png"), tex.EncodeToPNG());

        cam.targetTexture = null;
        Object.DestroyImmediate(tex);
        Object.DestroyImmediate(camGo);
        AssetDatabase.Refresh();
    }
}
