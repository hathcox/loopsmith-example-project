#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSetup : IGameSetup
{
    // Execution order - runs early in the setup sequence
    private const int SetupOrder = 100;

    // Ground plane constants
    private const float GroundSizeX = 20f;
    private const float GroundSizeZ = 20f;
    private const float GroundThickness = 0.1f;
    private const float GroundPositionY = 0f;

    // Camera constants
    private const float CameraHeight = 20f;
    private static readonly Quaternion CameraRotation = Quaternion.Euler(90f, 0f, 0f);

    // Material constants
    private static readonly Color GroundColor = new Color(0.35f, 0.55f, 0.35f, 1f);
    private const string GroundMaterialPath = "Assets/Generated/Materials/GroundMaterial.mat";

    // Scene constants
    private const string ScenePath = "Assets/Scenes/GameScene.unity";

    public int ExecutionOrder => SetupOrder;

    public void Execute()
    {
        var scene = CreateScene();

        var groundMaterial = CreateGroundMaterial();
        if (groundMaterial == null)
        {
            Debug.LogError("[SceneSetup] Aborting scene setup — material creation failed.");
            return;
        }

        CreateGroundPlane(groundMaterial);
        ConfigureCamera();

        SaveScene(scene);
        Debug.Log("[SceneSetup] Scene setup complete.");
    }

    private Scene CreateScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        return scene;
    }

    private Material CreateGroundMaterial()
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            Debug.LogWarning("[SceneSetup] URP Lit shader not found, falling back to Standard.");
            shader = Shader.Find("Standard");
        }

        if (shader == null)
        {
            Debug.LogError("[SceneSetup] No shader found (tried URP Lit and Standard). Cannot create GroundMaterial.");
            return null;
        }

        var material = new Material(shader);
        material.SetColor("_BaseColor", GroundColor);

        AssetDatabase.CreateAsset(material, GroundMaterialPath);
        Debug.Log("[SceneSetup] Created GroundMaterial at " + GroundMaterialPath);

        return material;
    }

    private void CreateGroundPlane(Material material)
    {
        var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "GroundPlane";
        ground.transform.position = new Vector3(0f, GroundPositionY, 0f);
        ground.transform.localScale = new Vector3(GroundSizeX, GroundThickness, GroundSizeZ);
        ground.isStatic = true;

        // BoxCollider is automatically added by CreatePrimitive(Cube)
        // Ensure it's not a trigger
        var collider = ground.GetComponent<BoxCollider>();
        collider.isTrigger = false;

        // Apply material
        var renderer = ground.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;

        Debug.Log("[SceneSetup] Created ground plane.");
    }

    private void ConfigureCamera()
    {
        var cameraGO = new GameObject("Main Camera");
        cameraGO.tag = "MainCamera";

        var camera = cameraGO.AddComponent<Camera>();
        camera.orthographic = false;
        camera.fieldOfView = 60f;
        cameraGO.AddComponent<AudioListener>();

        cameraGO.transform.position = new Vector3(0f, CameraHeight, 0f);
        cameraGO.transform.rotation = CameraRotation;

        Debug.Log("[SceneSetup] Configured main camera (top-down, fixed).");
    }

    private void SaveScene(Scene scene)
    {
        // Ensure Scenes directory exists
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }

        EditorSceneManager.SaveScene(scene, ScenePath);
        Debug.Log("[SceneSetup] Saved scene to " + ScenePath);
    }
}
#endif
