#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PlayerSetup : IGameSetup
{
    private const int SetupOrder = 200;

    private static readonly Color PlayerColor = new Color(0.2f, 0.4f, 0.8f, 1f);
    private const string PlayerMaterialPath = "Assets/Generated/Materials/PlayerMaterial.mat";

    public int ExecutionOrder => SetupOrder;

    public void Execute()
    {
        EnsureMaterialDirectory();

        var material = CreatePlayerMaterial();
        if (material == null)
        {
            Debug.LogError("[PlayerSetup] Aborting player setup — material creation failed.");
            return;
        }

        CreatePlayerCube(material);
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("[PlayerSetup] Player setup complete.");
    }

    private void EnsureMaterialDirectory()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Generated"))
        {
            AssetDatabase.CreateFolder("Assets", "Generated");
        }
        if (!AssetDatabase.IsValidFolder("Assets/Generated/Materials"))
        {
            AssetDatabase.CreateFolder("Assets/Generated", "Materials");
        }
    }

    private Material CreatePlayerMaterial()
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        bool isUrp = shader != null;
        if (shader == null)
        {
            Debug.LogWarning("[PlayerSetup] URP Lit shader not found, falling back to Standard.");
            shader = Shader.Find("Standard");
        }

        if (shader == null)
        {
            Debug.LogError("[PlayerSetup] No shader found (tried URP Lit and Standard). Cannot create PlayerMaterial.");
            return null;
        }

        var material = new Material(shader);
        string colorProperty = isUrp ? "_BaseColor" : "_Color";
        material.SetColor(colorProperty, PlayerColor);

        AssetDatabase.CreateAsset(material, PlayerMaterialPath);
        Debug.Log("[PlayerSetup] Created PlayerMaterial at " + PlayerMaterialPath);

        return material;
    }

    private void CreatePlayerCube(Material material)
    {
        var player = GameObject.CreatePrimitive(PrimitiveType.Cube);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 0.5f, 0f);

        // Configure Rigidbody: non-kinematic, gravity enabled, freeze X/Z rotation
        var rb = player.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // BoxCollider is automatically added by CreatePrimitive(Cube) — ensure non-trigger
        var collider = player.GetComponent<BoxCollider>();
        collider.isTrigger = false;

        // Attach PlayerController
        player.AddComponent<PlayerController>();

        // Apply material
        var renderer = player.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = material;

        Debug.Log("[PlayerSetup] Created player cube at (0, 0.5, 0).");
    }
}
#endif
