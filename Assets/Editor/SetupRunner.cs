using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class SetupRunner
{
    [MenuItem("Tools/Rebuild _F5")]
    public static void Rebuild()
    {
        Debug.Log("[SetupRunner] Starting full rebuild...");

        ClearGenerated();
        RunAllSetups();

        Debug.Log("[SetupRunner] Rebuild complete.");
    }

    private static void ClearGenerated()
    {
        string generatedPath = "Assets/Generated";

        if (!AssetDatabase.IsValidFolder(generatedPath))
        {
            Debug.Log("[SetupRunner] Assets/Generated/ not found, skipping clear.");
            return;
        }

        // Find ALL assets under Assets/Generated (including root-level and subfolder contents)
        string[] allAssetGuids = AssetDatabase.FindAssets("", new[] { generatedPath });
        foreach (string guid in allAssetGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            // Skip folders themselves and .gitkeep files
            if (!AssetDatabase.IsValidFolder(path) && !path.EndsWith(".gitkeep"))
            {
                AssetDatabase.DeleteAsset(path);
            }
        }

        // Recreate subfolder structure in case folders were removed
        if (!AssetDatabase.IsValidFolder("Assets/Generated/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets/Generated", "Prefabs");
        }
        if (!AssetDatabase.IsValidFolder("Assets/Generated/Materials"))
        {
            AssetDatabase.CreateFolder("Assets/Generated", "Materials");
        }

        AssetDatabase.Refresh();
        Debug.Log("[SetupRunner] Cleared all content under Assets/Generated/.");
    }

    private static void RunAllSetups()
    {
        List<IGameSetup> setups = new List<IGameSetup>();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            // Skip assemblies that cannot contain IGameSetup implementations
            string assemblyName = assembly.GetName().Name;
            if (assemblyName.StartsWith("Unity") ||
                assemblyName.StartsWith("System") ||
                assemblyName.StartsWith("mscorlib") ||
                assemblyName.StartsWith("Mono") ||
                assemblyName.StartsWith("nunit") ||
                assemblyName.StartsWith("Microsoft") ||
                assemblyName.StartsWith("Newtonsoft") ||
                assemblyName.StartsWith("netstandard"))
            {
                continue;
            }

            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null).ToArray();
            }

            foreach (var type in types)
            {
                if (typeof(IGameSetup).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    try
                    {
                        var instance = (IGameSetup)Activator.CreateInstance(type);
                        setups.Add(instance);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[SetupRunner] Failed to instantiate {type.Name}: {e.Message}");
                    }
                }
            }
        }

        setups = setups.OrderBy(s => s.ExecutionOrder).ToList();

        foreach (var setup in setups)
        {
            Debug.Log($"[SetupRunner] Executing {setup.GetType().Name} (Order: {setup.ExecutionOrder})");
            try
            {
                setup.Execute();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SetupRunner] Error in {setup.GetType().Name}: {e.Message}\n{e.StackTrace}");
            }
        }

        if (setups.Count == 0)
        {
            Debug.Log("[SetupRunner] No setup classes found.");
        }
        else
        {
            Debug.Log($"[SetupRunner] Executed {setups.Count} setup class(es).");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
