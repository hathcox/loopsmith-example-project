using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using CubeCollector.Setup;

namespace CubeCollector.Editor
{
    /// <summary>
    /// Editor script that binds to F5 to execute the full rebuild sequence:
    /// 1. Clear all content under Assets/Generated/ (preserving folder structure)
    /// 2. Discover and run all IGameSetup implementations in ExecutionOrder
    /// </summary>
    public static class SetupRunner
    {
        private const string GeneratedPath = "Assets/Generated";
        private const string GameAssemblyPrefix = "GameScripts";

        [MenuItem("Tools/Rebuild _F5")]
        public static void Rebuild()
        {
            Debug.Log("[SetupRunner] Starting F5 rebuild...");

            ClearGenerated();
            RunAllSetups();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[SetupRunner] Rebuild complete.");
        }

        private static void ClearGenerated()
        {
            if (!AssetDatabase.IsValidFolder(GeneratedPath))
            {
                Debug.Log("[SetupRunner] Generated folder does not exist, skipping clear.");
                return;
            }

            // Single pass: FindAssets with GeneratedPath searches recursively
            string[] guids = AssetDatabase.FindAssets("", new[] { GeneratedPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!AssetDatabase.IsValidFolder(path))
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("[SetupRunner] Cleared all content under Assets/Generated/");
        }

        private static void RunAllSetups()
        {
            List<IGameSetup> setups = DiscoverSetups();

            if (setups.Count == 0)
            {
                Debug.Log("[SetupRunner] No IGameSetup implementations found.");
                return;
            }

            setups.Sort((a, b) => a.ExecutionOrder.CompareTo(b.ExecutionOrder));

            foreach (IGameSetup setup in setups)
            {
                string typeName = setup.GetType().Name;
                Debug.Log($"[SetupRunner] Executing {typeName} (order: {setup.ExecutionOrder})...");

                try
                {
                    setup.Execute();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SetupRunner] Error executing {typeName}: {e.Message}\n{e.StackTrace}");
                }
            }

            Debug.Log($"[SetupRunner] Executed {setups.Count} setup(s).");
        }

        private static List<IGameSetup> DiscoverSetups()
        {
            var setups = new List<IGameSetup>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Only scan game assemblies, not Unity/System internals
                string assemblyName = assembly.GetName().Name;
                if (!assemblyName.StartsWith(GameAssemblyPrefix) && assemblyName != "Assembly-CSharp")
                    continue;

                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types.Where(t => t != null).ToArray();
                }

                foreach (Type type in types)
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

            return setups;
        }
    }
}
