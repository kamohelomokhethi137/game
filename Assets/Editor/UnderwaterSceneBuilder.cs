using System.Linq;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public static class UnderwaterSceneBuilder
{
    const string ArtFolder = "Assets/Art";

    [MenuItem("Tools/Build Underwater Scene")]
    public static void BuildScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        EnsureFolder(ArtFolder);

        // ---------- Lighting / atmosphere ----------
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.05f, 0.20f, 0.29f);
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.048f;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.26f, 0.40f, 0.48f);
        RenderSettings.ambientEquatorColor = new Color(0.16f, 0.28f, 0.34f);
        RenderSettings.ambientGroundColor = new Color(0.08f, 0.13f, 0.15f);

        GameObject sun = new GameObject("Directional Light");
        Light sunLight = sun.AddComponent<Light>();
        sunLight.type = LightType.Directional;
        sunLight.color = new Color(0.78f, 0.92f, 0.96f);
        sunLight.intensity = 1.15f;
        sunLight.shadows = LightShadows.Soft;
        sun.transform.rotation = Quaternion.Euler(55f, -35f, 0f);

        // ---------- Materials ----------
        Texture2D sandTexture = MakeSandTexture();
        Material sand = MakeMaterial("Sand", new Color(0.30f, 0.30f, 0.25f), sandTexture, 0.05f);
        Material dune = MakeMaterial("SandDune", new Color(0.27f, 0.27f, 0.22f), sandTexture, 0.05f);
        Material rock = MakeMaterial("Rock", new Color(0.26f, 0.31f, 0.36f), null, 0.18f);
        Material rockDark = MakeMaterial("RockDark", new Color(0.16f, 0.21f, 0.25f), null, 0.12f);
        Material kelp = MakeMaterial("Kelp", new Color(0.20f, 0.44f, 0.24f), null, 0.30f);
        Material kelpDark = MakeMaterial("KelpDark", new Color(0.12f, 0.32f, 0.19f), null, 0.30f);
        Material wood = MakeMaterial("WreckWood", new Color(0.22f, 0.15f, 0.10f), null, 0.08f);
        Material gold = MakeGlowingMaterial("Gold", new Color(1f, 0.74f, 0.18f));

        // ---------- Environment ----------
        GameObject env = new GameObject("Environment");

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "SeaFloor";
        floor.transform.SetParent(env.transform);
        floor.transform.position = Vector3.zero;
        floor.transform.localScale = new Vector3(14f, 1f, 14f);
        floor.GetComponent<Renderer>().sharedMaterial = sand;

        GameObject dunes = new GameObject("Dunes");
        dunes.transform.SetParent(env.transform);
        Random.InitState(13);
        for (int i = 0; i < 9; i++)
        {
            GameObject d = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            d.name = "Dune" + i;
            d.transform.SetParent(dunes.transform);
            float angle = i * 0.7f + 0.2f;
            float radius = Random.Range(6f, 16f);
            d.transform.position = new Vector3(Mathf.Cos(angle) * radius, -Random.Range(0.5f, 1.1f), Mathf.Sin(angle) * radius);
            float s = Random.Range(3.5f, 7f);
            d.transform.localScale = new Vector3(s, Random.Range(0.8f, 1.6f), s * Random.Range(0.7f, 1.2f));
            d.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            DestroyCollider(d);
            d.GetComponent<Renderer>().sharedMaterial = dune;
        }

        GameObject rocks = new GameObject("Rocks");
        rocks.transform.SetParent(env.transform);
        Random.InitState(7);
        for (int i = 0; i < 20; i++)
        {
            GameObject r = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            r.name = "Rock" + i;
            r.transform.SetParent(rocks.transform);
            float angle = i * 0.55f;
            float radius = 4f + (i % 4) * 1.8f;
            r.transform.position = new Vector3(Mathf.Cos(angle) * radius, Random.Range(-0.35f, 0.15f), Mathf.Sin(angle) * radius);
            float s = Random.Range(0.6f, 1.8f);
            r.transform.localScale = new Vector3(s, s * Random.Range(0.45f, 0.8f), s * Random.Range(0.8f, 1.2f));
            r.transform.rotation = UnityEngine.Random.rotation;
            r.GetComponent<Renderer>().sharedMaterial = i % 3 == 0 ? rockDark : rock;
        }

        GameObject kelps = new GameObject("Kelp");
        kelps.transform.SetParent(env.transform);
        for (int i = 0; i < 14; i++)
        {
            GameObject stalk = new GameObject("Kelp" + i);
            stalk.transform.SetParent(kelps.transform);
            float angle = i * 1.02f + 0.4f;
            float radius = 3.6f + (i % 3) * 1.1f;
            stalk.transform.position = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            stalk.transform.rotation = Quaternion.Euler(Random.Range(-6f, 6f), Random.Range(0f, 360f), Random.Range(-6f, 6f));

            int segments = 4;
            float height = Random.Range(1.8f, 3.2f) / segments;
            Material stalkMaterial = i % 2 == 0 ? kelp : kelpDark;
            for (int s = 0; s < segments; s++)
            {
                GameObject seg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                seg.name = "Segment" + s;
                seg.transform.SetParent(stalk.transform);
                seg.transform.localPosition = new Vector3(0f, height * (s + 0.5f), 0f);
                seg.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-7f, 7f) + s * 2f);
                float taper = 1f - s * 0.18f;
                seg.transform.localScale = new Vector3(0.09f * taper, height * 0.55f, 0.09f * taper);
                DestroyCollider(seg);
                seg.GetComponent<Renderer>().sharedMaterial = stalkMaterial;
            }

            for (int f = 0; f < 5; f++)
            {
                GameObject frond = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                frond.name = "Frond" + f;
                frond.transform.SetParent(stalk.transform);
                float h = Random.Range(0.5f, height * segments * 0.85f);
                frond.transform.localPosition = new Vector3(0f, h, 0f);
                frond.transform.localRotation = Quaternion.Euler(Random.Range(35f, 65f), Random.Range(0f, 360f), 0f);
                frond.transform.localScale = new Vector3(0.055f, Random.Range(0.28f, 0.45f), 0.02f);
                DestroyCollider(frond);
                frond.GetComponent<Renderer>().sharedMaterial = stalkMaterial;
            }
        }

        // Broken hull ribs suggest the wreck the treasures came from.
        GameObject wreck = new GameObject("Wreck");
        wreck.transform.SetParent(env.transform);
        Random.InitState(31);
        for (int i = 0; i < 7; i++)
        {
            GameObject rib = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rib.name = "Rib" + i;
            rib.transform.SetParent(wreck.transform);
            float angle = 180f + i * 11f;
            float rad = Mathf.Deg2Rad * angle;
            rib.transform.position = new Vector3(Mathf.Cos(rad) * 8.5f, 0.55f, Mathf.Sin(rad) * 8.5f);
            rib.transform.rotation = Quaternion.Euler(Random.Range(-14f, 14f), angle + 90f, Random.Range(-18f, 18f));
            rib.transform.localScale = new Vector3(0.35f, 3.0f, 0.55f);
            rib.GetComponent<Renderer>().sharedMaterial = wood;
        }

        // ---------- Player rig ----------
        GameObject xrOriginGo = new GameObject("XR Origin");
        XROrigin origin = xrOriginGo.AddComponent<XROrigin>();
        origin.Origin = xrOriginGo;
        origin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Floor;

        GameObject camOffset = new GameObject("Camera Offset");
        camOffset.transform.SetParent(xrOriginGo.transform, false);
        origin.CameraFloorOffsetObject = camOffset;

        GameObject camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        camGo.transform.SetParent(camOffset.transform, false);
        Camera cam = camGo.AddComponent<Camera>();
        cam.nearClipPlane = 0.05f;
        cam.farClipPlane = 120f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.03f, 0.16f, 0.26f);
        camGo.AddComponent<AudioListener>();
        camGo.AddComponent<TrackedPoseDriver>();
        origin.Camera = cam;

        GameObject hitbox = new GameObject("Player Body");
        hitbox.tag = "Player";
        hitbox.transform.SetParent(xrOriginGo.transform, false);
        hitbox.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        SphereCollider hitboxCollider = hitbox.AddComponent<SphereCollider>();
        hitboxCollider.isTrigger = true;
        hitboxCollider.radius = 0.6f;
        Rigidbody hitboxBody = hitbox.AddComponent<Rigidbody>();
        hitboxBody.isKinematic = true;
        hitboxBody.useGravity = false;

        PlayerMovement movement = xrOriginGo.AddComponent<PlayerMovement>();
        movement.head = camGo.transform;

        // ---------- Game manager ----------
        GameObject managerGo = new GameObject("GameManager");
        GameManager manager = managerGo.AddComponent<GameManager>();

        // ---------- Treasures ----------
        GameObject treasureRoot = new GameObject("Treasures");
        int treasureCount = 5;
        for (int i = 0; i < treasureCount; i++)
        {
            GameObject t = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            t.name = "Treasure" + (i + 1);
            t.transform.SetParent(treasureRoot.transform);
            float angle = i * (Mathf.PI * 2f / treasureCount);
            t.transform.position = new Vector3(Mathf.Cos(angle) * 3f, 1.25f, Mathf.Sin(angle) * 3f);
            t.transform.localScale = Vector3.one * 0.42f;
            SphereCollider treasureCollider = t.GetComponent<SphereCollider>();
            treasureCollider.isTrigger = true;
            treasureCollider.radius = 0.8f;
            t.GetComponent<Renderer>().sharedMaterial = gold;
            t.AddComponent<Treasure>();
            t.AddComponent<TreasureFloat>();

            GameObject glow = new GameObject("Glow");
            glow.transform.SetParent(t.transform, false);
            Light point = glow.AddComponent<Light>();
            point.type = LightType.Point;
            point.color = new Color(1f, 0.72f, 0.25f);
            point.range = 2.2f;
            point.intensity = 1.5f;
        }

        // ---------- Extra underwater decor ----------
        Random.InitState(99);
        GameObject decor = new GameObject("Decor");
        decor.transform.SetParent(env.transform);

        // Coral clusters.
        for (int i = 0; i < 18; i++)
        {
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            c.name = "Coral" + i;
            c.transform.SetParent(decor.transform);
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float radius = Random.Range(2.5f, 15f);
            c.transform.position = new Vector3(Mathf.Cos(angle) * radius, Random.Range(-0.6f, 0.4f), Mathf.Sin(angle) * radius);
            c.transform.rotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            float h = Random.Range(0.4f, 1.2f);
            c.transform.localScale = new Vector3(0.12f, h * 0.5f, 0.12f);
            DestroyCollider(c);
            c.GetComponent<Renderer>().sharedMaterial = i % 3 == 0 ? kelpDark : kelp;
        }

        // Small fish (spheres that bob).
        for (int i = 0; i < 12; i++)
        {
            GameObject f = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            f.name = "Fish" + i;
            f.transform.SetParent(decor.transform);
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float radius = Random.Range(2f, 10f);
            f.transform.position = new Vector3(Mathf.Cos(angle) * radius, Random.Range(0.6f, 2.5f), Mathf.Sin(angle) * radius);
            f.transform.localScale = Vector3.one * Random.Range(0.12f, 0.22f);
            SphereCollider fc = f.GetComponent<SphereCollider>();
            fc.isTrigger = true;
            f.GetComponent<Renderer>().sharedMaterial = gold;
        }

        // The player is a simple capsule — easy to see, easy to control.
        GameObject playerSelf = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        playerSelf.name = "PlayerSelf";
        playerSelf.transform.SetParent(camOffset.transform, false);
        playerSelf.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        playerSelf.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        playerSelf.GetComponent<Renderer>().sharedMaterial = gold;
        movement.visibleBody = playerSelf.transform;

        // Camera pivot at character head height (where the TP camera looks at).
        GameObject camPivot = new GameObject("CamPivot");
        camPivot.transform.SetParent(hitbox.transform, false);
        camPivot.transform.localPosition = new Vector3(0f, 1.0f, 0f);

        // Third-person over-the-shoulder follow camera (like PUBG/Fortnite).
        GameObject tpCamGo = new GameObject("PlayerCam_TP");
        Camera tpCam = tpCamGo.AddComponent<Camera>();
        tpCam.nearClipPlane = 0.1f;
        tpCam.farClipPlane = 200f;
        tpCam.clearFlags = CameraClearFlags.SolidColor;
        tpCam.backgroundColor = new Color(0.03f, 0.16f, 0.26f, 1f);
        tpCam.enabled = false; // off until C is pressed
        tpCamGo.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
        ThirdPersonCamera tpCamScript = tpCamGo.AddComponent<ThirdPersonCamera>();
        tpCamScript.target = hitbox.transform;
        tpCamScript.pivot = camPivot.transform;

        // Wire movement to the TP camera script so keyboard/mouse can rotate yaw.
        movement.thirdPersonCameraScript = tpCamScript;

        manager.thirdPersonCamera = tpCam;

        // ---------- Save & build settings ----------
        const string scenePath = "Assets/Scenes/Underwater.unity";
        EnsureFolder("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, scenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath, true) };

        AssetDatabase.SaveAssets();
        Debug.Log("Underwater scene built and saved to " + scenePath);
    }

    static GameObject AddDiver(Transform parent, string modelPath, string name, float radius, float startAngle,
        float height, float yawOffset, float degreesPerSecond, float bobAmplitude)
    {
        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        if (model == null)
        {
            Debug.LogWarning("Diver model missing: " + modelPath);
            return null;
        }

        GameObject diver = new GameObject(name);
        diver.transform.SetParent(parent);

        GameObject body = (GameObject)PrefabUtility.InstantiatePrefab(model, diver.transform);
        body.name = "Body";
        body.transform.localPosition = Vector3.zero;
        body.transform.localRotation = Quaternion.identity;

        // The source models are rigged in centimetres; normalise to a human-sized diver.
        float extent = MaxExtent(body);
        if (extent > 0.001f)
        {
            body.transform.localScale = Vector3.one * (1.9f / extent);
        }

        foreach (Collider c in body.GetComponentsInChildren<Collider>())
        {
            Object.DestroyImmediate(c);
        }

        Material diverMaterial = MakeDiverMaterial(modelPath);
        if (diverMaterial != null)
        {
            foreach (Renderer r in body.GetComponentsInChildren<Renderer>())
            {
                Material[] materials = new Material[r.sharedMaterials.Length == 0 ? 1 : r.sharedMaterials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = diverMaterial;
                }
                r.sharedMaterials = materials;
            }
        }

        Animator animator = body.GetComponent<Animator>();
        if (animator == null)
        {
            animator = body.AddComponent<Animator>();
        }
        animator.runtimeAnimatorController = MakeDiverController(name, modelPath);

        SwimPatrol patrol = diver.AddComponent<SwimPatrol>();
        patrol.radius = radius;
        patrol.startAngle = startAngle;
        patrol.height = height;
        patrol.yawOffset = yawOffset;
        patrol.degreesPerSecond = degreesPerSecond;
        patrol.bobAmplitude = bobAmplitude;
        return body;
    }

    static Material MakeDiverMaterial(string modelPath)
    {
        const string path = ArtFolder + "/Diver.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(mat, path);
        }

        string folder = System.IO.Path.GetDirectoryName(modelPath).Replace('\\', '/');
        Texture2D diffuse = AssetDatabase.LoadAssetAtPath<Texture2D>(folder + "/textures/Ch04_1001_Diffuse.png");
        Texture2D normal = AssetDatabase.LoadAssetAtPath<Texture2D>(folder + "/textures/Ch04_1001_Normal.png");

        mat.SetColor("_BaseColor", Color.white);
        mat.SetFloat("_Smoothness", 0.45f);
        if (diffuse != null)
        {
            mat.SetTexture("_BaseMap", diffuse);
            mat.SetTextureScale("_BaseMap", Vector2.one);
        }
        if (normal != null)
        {
            mat.SetTexture("_BumpMap", normal);
            mat.EnableKeyword("_NORMALMAP");
        }
        EditorUtility.SetDirty(mat);
        return mat;
    }

    static float MaxExtent(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return 0f;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        return Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
    }

    static RuntimeAnimatorController MakeDiverController(string name, string modelPath)
    {
        string assetName = name.Replace(" ", string.Empty) + ".controller";
        string path = ArtFolder + "/" + assetName;
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
        if (controller != null)
        {
            return controller;
        }

        controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        AnimationClip clip = AssetDatabase.LoadAllAssetsAtPath(modelPath)
            .OfType<AnimationClip>()
            .FirstOrDefault(c => !c.name.StartsWith("__preview__"));

        if (clip == null)
        {
            Debug.LogWarning("No animation clip found in " + modelPath);
            return controller;
        }

        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        controller.AddMotion(clip);
        return controller;
    }

    static void EnsureFolder(string path)    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
            string leaf = System.IO.Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }
            AssetDatabase.CreateFolder(parent, leaf);
        }
    }

    static Texture2D MakeSandTexture()
    {
        const string path = ArtFolder + "/SandTexture.png";
        Texture2D existing = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (existing != null)
        {
            return existing;
        }

        const int size = 256;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGB24, true);
        Color tint = new Color(0.72f, 0.68f, 0.55f);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float broad = Mathf.PerlinNoise(x * 0.035f, y * 0.035f);
                float mid = Mathf.PerlinNoise(x * 0.16f, y * 0.16f);
                float fine = Mathf.PerlinNoise(x * 0.55f, y * 0.55f);
                float v = 0.72f + broad * 0.22f + (mid - 0.5f) * 0.18f + (fine - 0.5f) * 0.10f;
                tex.SetPixel(x, y, new Color(tint.r * v, tint.g * v, tint.b * v));
            }
        }
        tex.Apply();
        System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path);

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.wrapMode = TextureWrapMode.Repeat;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = true;
        importer.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    static Material MakeMaterial(string name, Color color, Texture2D map, float smoothness)
    {
        string path = ArtFolder + "/" + name + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }

        mat.SetColor("_BaseColor", color);
        mat.SetFloat("_Smoothness", smoothness);
        if (map != null)
        {
            mat.SetTexture("_BaseMap", map);
            mat.SetTextureScale("_BaseMap", new Vector2(18f, 18f));
        }
        EditorUtility.SetDirty(mat);
        return mat;
    }

    static Material MakeGlowingMaterial(string name, Color color)
    {
        Material mat = MakeMaterial(name, color, null, 0.55f);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", color * 2.2f);
        mat.SetFloat("_Metallic", 0.85f);
        EditorUtility.SetDirty(mat);
        return mat;
    }

    static void DestroyCollider(GameObject go)
    {
        Collider c = go.GetComponent<Collider>();
        if (c != null)
        {
            Object.DestroyImmediate(c);
        }
    }
}
