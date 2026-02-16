#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CubeCollector;

public class PickupSetup : IGameSetup
{
    private const int SetupOrder = 300;

    private const string PickupMaterialPath = "Assets/Generated/Materials/PickupMaterial.mat";
    private const string PickupPrefabPath = "Assets/Generated/Prefabs/Pickup.prefab";

    private static readonly Color PickupBaseColor = new Color(1f, 0.85f, 0.2f, 1f);
    private static readonly Color PickupEmissionColor = new Color(1f, 0.85f, 0.2f, 1f) * 2f;

    private static readonly Vector3[] PickupPositions = new Vector3[]
    {
        new Vector3(5f, 0.5f, 5f),
        new Vector3(-5f, 0.5f, 5f),
        new Vector3(5f, 0.5f, -5f),
        new Vector3(-5f, 0.5f, -5f),
        new Vector3(0f, 0.5f, 5f)
    };

    public int ExecutionOrder => SetupOrder;

    public void Execute()
    {
        EnsureDirectories();
        CleanupExistingAssets();

        var material = CreatePickupMaterial();
        if (material == null)
        {
            Debug.LogError("[PickupSetup] Aborting — material creation failed.");
            return;
        }

        var prefab = CreatePickupPrefab(material);
        if (prefab == null)
        {
            Debug.LogError("[PickupSetup] Aborting — prefab creation failed.");
            return;
        }

        CreateGameManager();
        PlacePickups(prefab);

        EditorSceneManager.SaveOpenScenes();
        Debug.Log("[PickupSetup] Pickup setup complete.");
    }

    private void EnsureDirectories()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Generated"))
            AssetDatabase.CreateFolder("Assets", "Generated");
        if (!AssetDatabase.IsValidFolder("Assets/Generated/Materials"))
            AssetDatabase.CreateFolder("Assets/Generated", "Materials");
        if (!AssetDatabase.IsValidFolder("Assets/Generated/Prefabs"))
            AssetDatabase.CreateFolder("Assets/Generated", "Prefabs");
    }

    private void CleanupExistingAssets()
    {
        if (AssetDatabase.LoadAssetAtPath<Object>(PickupMaterialPath) != null)
            AssetDatabase.DeleteAsset(PickupMaterialPath);
        if (AssetDatabase.LoadAssetAtPath<Object>(PickupPrefabPath) != null)
            AssetDatabase.DeleteAsset(PickupPrefabPath);
    }

    private Material CreatePickupMaterial()
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        bool isUrp = shader != null;
        if (shader == null)
        {
            Debug.LogWarning("[PickupSetup] URP Lit shader not found, falling back to Standard.");
            shader = Shader.Find("Standard");
        }

        if (shader == null)
        {
            Debug.LogError("[PickupSetup] No shader found. Cannot create PickupMaterial.");
            return null;
        }

        var material = new Material(shader);

        if (isUrp)
        {
            material.SetColor("_BaseColor", PickupBaseColor);
            material.SetColor("_EmissionColor", PickupEmissionColor);
            material.EnableKeyword("_EMISSION");
        }
        else
        {
            material.SetColor("_Color", PickupBaseColor);
            material.SetColor("_EmissionColor", PickupEmissionColor);
            material.EnableKeyword("_EMISSION");
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }

        AssetDatabase.CreateAsset(material, PickupMaterialPath);
        Debug.Log("[PickupSetup] Created PickupMaterial at " + PickupMaterialPath);
        return material;
    }

    private GameObject CreatePickupPrefab(Material material)
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "Pickup";
        sphere.tag = "Pickup";

        // Configure SphereCollider as trigger
        var collider = sphere.GetComponent<SphereCollider>();
        collider.isTrigger = true;

        // Attach Pickup MonoBehaviour
        sphere.AddComponent<Pickup>();

        // Apply material
        var renderer = sphere.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;

        // Save as prefab
        var prefab = PrefabUtility.SaveAsPrefabAsset(sphere, PickupPrefabPath);
        Object.DestroyImmediate(sphere);

        Debug.Log("[PickupSetup] Created Pickup.prefab at " + PickupPrefabPath);
        return prefab;
    }

    private void CreateGameManager()
    {
        // Idempotency: skip if GameManager already exists in scene
        if (Object.FindAnyObjectByType<GameManager>() != null)
        {
            Debug.Log("[PickupSetup] GameManager already exists in scene — skipping creation.");
            return;
        }

        var gmObj = new GameObject("GameManager");
        gmObj.AddComponent<GameManager>();
        Debug.Log("[PickupSetup] Created GameManager in scene.");
    }

    private void PlacePickups(GameObject prefab)
    {
        for (int i = 0; i < PickupPositions.Length; i++)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = $"Pickup_{i + 1}";
            instance.transform.position = PickupPositions[i];
        }

        Debug.Log($"[PickupSetup] Placed {PickupPositions.Length} pickups in scene.");
    }
}
#endif
