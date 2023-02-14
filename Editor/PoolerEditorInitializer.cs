#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace JoyKirito.ObjectPool.Editor
{
    public class PoolerEditorInitializer
    {
        #region Fields
        
        private const string ObjectPoolerDefine = "OBJECT_POOLER";
        
        // Paths:
        internal static readonly string ResourcesPath = "Assets/Resources";
        internal static readonly string ObjectPoolerResourcePath = "Assets/Resources/ObjectPooler";
        
        private const string ToGeneralAssemblyReferenceText = "{\"reference\": \"ObjectPooler.Runtime\" }";

        #endregion
        
        #region Methods
        
        public static void Init(string enumMembersAssetName, string enumCreatorConfigAssetName, string pooledObjectsInfoAssetName)
        {
            EnumMembersConfig enumMembersConfig;
            EnumCreatorConfig enumCreatorConfig;
            if (!TryCreateScriptable(enumMembersAssetName, enumCreatorConfigAssetName, pooledObjectsInfoAssetName,
                out enumCreatorConfig, out enumMembersConfig))
            {
                Debug.LogWarning("Creating of Scriptable Objects for ObjectPooler fail.");
                return;
            }

            EnumCreator.Create(enumCreatorConfig, enumMembersConfig, ObjectPoolerResourcePath + $"/{enumCreatorConfig.csFileName}.cs");
            CreateAssemblyReference();
            CompilationPipeline.RequestScriptCompilation();
            AddDefine(ObjectPoolerDefine);
        }

        private static bool TryCreateScriptable(string enumMembersAssetName, string enumCreatorConfigAssetName, string pooledObjectsInfoAssetName, 
            out EnumCreatorConfig enumCreatorConfig, out EnumMembersConfig enumMembersConfig)
        {
            enumMembersConfig = ScriptableObject.CreateInstance<EnumMembersConfig>();
            enumCreatorConfig = ScriptableObject.CreateInstance<EnumCreatorConfig>();
            var pooledObjectsInfo = ScriptableObject.CreateInstance<PooledObjectsInfo>();

            enumCreatorConfig.targetDefine = ObjectPoolerDefine;
            enumCreatorConfig.targetNamespace = "JoyKirito.ObjectPool";
            enumCreatorConfig.csFileName = "PooledObjectType";
            enumCreatorConfig.useDefines = true;
            enumCreatorConfig.useNamespace = true;

            if (!AssetDatabase.IsValidFolder(ResourcesPath))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder(ObjectPoolerResourcePath))
                AssetDatabase.CreateFolder(ResourcesPath, "ObjectPooler");
            
            AssetDatabase.CreateAsset(enumMembersConfig, $"{ObjectPoolerResourcePath}/{enumMembersAssetName}.asset");
            AssetDatabase.CreateAsset(enumCreatorConfig, $"{ObjectPoolerResourcePath}/{enumCreatorConfigAssetName}.asset");
            AssetDatabase.CreateAsset(pooledObjectsInfo, $"{ObjectPoolerResourcePath}/{pooledObjectsInfoAssetName}.asset");
            AssetDatabase.SaveAssets();

            return true;
        }

        private static void CreateAssemblyReference()
        {
            var path = $"{ObjectPoolerResourcePath}\\ToGeneralAssemblyReference.asmref";
            File.WriteAllText(path, ToGeneralAssemblyReferenceText);
        }

        private static void AddDefine(string defineName)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defineName);
        }
        
        #endregion
    }
}
#endif