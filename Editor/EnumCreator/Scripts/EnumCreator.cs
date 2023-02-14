using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.Compilation;

namespace JoyKirito.ObjectPool.Editor
{
    public class EnumCreator
    {
        private const string OpenCodeBlock = "{";
        private const string CloseCodeBlock = "\n}";
        
        public static void Create(EnumCreatorConfig creatorConfig, EnumMembersConfig membersConfig)
        {
            var filePath = GetReplacedFilePath(creatorConfig);
            Create(creatorConfig, membersConfig, filePath);
        }

        public static async void Create(EnumCreatorConfig creatorConfig, EnumMembersConfig membersConfig, string customFilePath)
        {
            if (File.Exists(customFilePath))
            {
                File.Delete(customFilePath);
            }

            var currentEnumText = "";
            StartDefine(creatorConfig, ref currentEnumText);
            StartNamespace(creatorConfig, ref currentEnumText);
            StartEnum(creatorConfig, ref currentEnumText);
            WriteMembers(membersConfig, ref currentEnumText);
            EndEnum(ref currentEnumText);
            EndNamespace(creatorConfig, ref currentEnumText);
            EndDefine(creatorConfig, ref currentEnumText);
            WriteToFile(customFilePath, currentEnumText);
        }

        private static string GetReplacedFilePath(EnumCreatorConfig creatorConfig)
        {
            var assetsGuidList = AssetDatabase.FindAssets(creatorConfig.csFileName, 
                searchInFolders: new []{ PoolerEditorInitializer.ResourcesPath, PoolerEditorInitializer.ObjectPoolerResourcePath });
            string assetPath;
            if (assetsGuidList.IsNullOrEmpty())
            {
                Debug.LogError($"{creatorConfig.csFileName} file was deleted. Will created is Assets folder.");
                assetPath = Application.dataPath;
            }
            else
            {
                assetPath = AssetDatabase.GUIDToAssetPath(assetsGuidList[0]);
            }
            
            var directory = Directory.GetParent(Application.dataPath);
            var fullFilePath = directory.ToString().Replace('\\', '/') + "/" + assetPath;

            return fullFilePath;
        }

        private static void StartDefine(EnumCreatorConfig config, ref string currentEnumText)
        {
            if (config.useDefines)
            {
                currentEnumText += $"#if {config.targetDefine}\n";
            }
        }

        private static void EndDefine(EnumCreatorConfig config, ref string currentEnumText)
        {
            if (config.useDefines)
            {
                currentEnumText += "\n#endif";
            }
        }

        private static void StartNamespace(EnumCreatorConfig config, ref string currentEnumText)
        {
            if (config.useNamespace)
            {
                currentEnumText += $"namespace {config.targetNamespace}\n{OpenCodeBlock}";
            }
        }

        private static void EndNamespace(EnumCreatorConfig config, ref string currentEnumText)
        {
            if (config.useNamespace)
            {
                currentEnumText += CloseCodeBlock;
            }
        }

        private static void StartEnum(EnumCreatorConfig config, ref string currentEnumText)
        {
            currentEnumText += $"\npublic enum {config.csFileName}\n" + OpenCodeBlock;
        }

        private static void EndEnum(ref string currentEnumText)
        {
            currentEnumText += CloseCodeBlock;
        }

        private static void WriteMembers(EnumMembersConfig enumMembersConfig, ref string currentEnumText)
        {
            for (int i = 0; i < enumMembersConfig.members.Length; i++)
            {
                int associatedInt = enumMembersConfig.associatedInts.ContainsIndex(i) ? 
                    enumMembersConfig.associatedInts[i] : 
                    int.MaxValue - i;
                string member = enumMembersConfig.members[i];
                currentEnumText += $"\n{Format(member)} = {associatedInt},";
            }
        }

        private static string Format(string target)
        {
            return target.Replace(" ", "");
        }

        private static void WriteToFile(string path, string currentEnumText)
        {
            File.WriteAllText(path, currentEnumText);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            CompilationPipeline.RequestScriptCompilation();
        }
    }
}