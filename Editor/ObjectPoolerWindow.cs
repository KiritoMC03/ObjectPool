using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JoyKirito.ObjectPool.Editor
{
    public class ObjectPoolerWindow : EditorWindow
    {
        #region Fields

        // Assets names
        private static readonly string EnumMembersAssetName = "PoolerEnumMembersAsset";
        private static readonly string EnumCreatorConfigAssetName = "PoolerEnumCreatorConfigAsset";
        private static readonly string PooledObjectsInfoAssetName = "PooledObjectsInfoAsset";
        
        // Assets properties names
        private static readonly string EnumMemberPropertyName = "members";
        private static readonly string AssociatedIntsPropertyName = "associatedInts";
        private static readonly string PooledObjectsInfoListPropertyName = "list";
        private static readonly string PooledObjectStartNumberPropertyName = "startNumber";
        private static readonly string PooledObjectPrefabPropertyName = "prefab";
        private static readonly string PooledObjectIsDontDestroyOnLoadPropertyName = "isDontDestroyOnload";
        private static readonly string PooledObjectIsDontDestroyOnLoadPropertyDescription = "Make DontDestroyOnLoad  ";
        private static readonly string DefaultNamePrefixForNewMember = "NewMember_";

        // Window preferences
        private static readonly string TitleText = "Object Pooler";
        private static readonly Vector2 MinWindowSize = new Vector2(450f, 200f);
        
        // Table layout:
        private static readonly float HeaderHeight = 18f;
        private static readonly float Column0Width = 58f;
        private static readonly float Column1Width = 120f;
        private static readonly float Column2Width = 48f;
        private static readonly float Column3Width = 160f;
        private static readonly float Column4Width = 60f;
        private static readonly float Column5Width = 160f;
        private static readonly float ToggleWidth = 12f;
        private static readonly float DefaultButtonWidth = 100f;
        private static readonly float DefaultButtonHeight = 25f;

        private static readonly string Column0Label = "Enum int";
        private static readonly string Column1Label = "Member Name";
        private static readonly string Column2Label = "Number";
        private static readonly string Column3Label = "Prefab";
        
        private static readonly string AddEnumMemberButtonText = "New";

        private EnumMembersConfig _enumMembersConfig;
        private EnumCreatorConfig _enumCreatorConfig;
        private PooledObjectsInfo _pooledObjectsInfo;
        private Vector2 scrollPos;
        private bool initSuccess;

        #endregion

        #region Unity lifecycle

        private void OnEnable()
        {
            if (FindUniqueAsset(EnumMembersAssetName, ref _enumMembersConfig) &&
                FindUniqueAsset(EnumCreatorConfigAssetName, ref _enumCreatorConfig) &&
                FindUniqueAsset(PooledObjectsInfoAssetName, ref _pooledObjectsInfo))
            {
                initSuccess = true;
            }
        }

        private void OnGUI()
        {
            if (initSuccess)
            {
                DrawHeader();
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height - HeaderHeight));
                DrawPoolConfigurator(ref _enumMembersConfig, ref _enumCreatorConfig, ref _pooledObjectsInfo);
                DrawAddMemberButton(ref _enumCreatorConfig, ref _enumMembersConfig);
                DrawBakePoolMembers(_enumCreatorConfig, _enumMembersConfig);
                DrawInitPoolButton();
                EditorGUILayout.EndScrollView();
            }
        }

        #endregion

        #region MenuItem Methods

        [MenuItem("Tools/JoyKirito/Object Pooler/Init")]
        private static void InitPoolerFiles()
        {
            PoolerEditorInitializer.Init(EnumMembersAssetName, EnumCreatorConfigAssetName, PooledObjectsInfoAssetName);
        }
        
        [MenuItem("Tools/JoyKirito/Object Pooler/Menu")]
        private static void ShowWindow()
        {
            var window = GetWindow<ObjectPoolerWindow>();
            window.titleContent = new GUIContent(TitleText);
            window.minSize = MinWindowSize;
            window.Show();
        }

        #endregion

        #region Methods

        private void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Column0Label, GUILayout.Width(Column0Width), GUILayout.Height(HeaderHeight));
            GUILayout.Label(Column1Label, GUILayout.Width(Column1Width), GUILayout.Height(HeaderHeight));
            GUILayout.Label(Column2Label, GUILayout.Width(Column2Width), GUILayout.Height(HeaderHeight));
            GUILayout.Label(Column3Label, GUILayout.Width(Column3Width), GUILayout.Height(HeaderHeight));
            GUILayout.EndHorizontal();
        }

        private void DrawPoolConfigurator(ref EnumMembersConfig poolerEnumMembersConfig, ref EnumCreatorConfig enumCreatorConfig, ref PooledObjectsInfo pooledObjectsInfo)
        {
            SerializedObject enumMembersConfigAsset = new SerializedObject(poolerEnumMembersConfig);
            SerializedProperty membersListProperty = enumMembersConfigAsset.FindProperty(EnumMemberPropertyName);
            SerializedProperty associatedIntsProperty = enumMembersConfigAsset.FindProperty(AssociatedIntsPropertyName);
            
            SerializedObject pooledObjectsInfoAsset = new SerializedObject(pooledObjectsInfo);
            SerializedProperty pooledObjectsInfoListProperty = pooledObjectsInfoAsset.FindProperty(PooledObjectsInfoListPropertyName);

            for (int i = 0; i < membersListProperty.arraySize; i++)
            {
                SerializedProperty enumMemberProperty = membersListProperty.GetArrayElementAtIndex(i);
                if (pooledObjectsInfo.list.Count <= i) pooledObjectsInfo.list.Add(new ObjectInfo());
                if (associatedIntsProperty.arraySize <= i) associatedIntsProperty.InsertArrayElementAtIndex(i);
                SerializedProperty associatedIntProperty = associatedIntsProperty.GetArrayElementAtIndex(i);
                ObjectInfo currentObjectInfo = pooledObjectsInfo.list[i];
                currentObjectInfo.type = (PooledObjectType) i;

                GUILayout.BeginHorizontal();

                try
                {
                    DrawCurrentEnumMemberParameters(i, enumMemberProperty, associatedIntProperty, associatedIntsProperty, ref currentObjectInfo, pooledObjectsInfoListProperty);
                    DrawRemoveCurrentMemberButton(i, ref enumCreatorConfig, ref poolerEnumMembersConfig, 
                        pooledObjectsInfoAsset, 
                        pooledObjectsInfoListProperty, 
                        enumMembersConfigAsset, 
                        membersListProperty,
                        associatedIntsProperty);
                    DrawAfterCurrentMemberButton(i, enumMemberProperty, ref currentObjectInfo, pooledObjectsInfoListProperty);
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }

                GUILayout.EndHorizontal();

                if (pooledObjectsInfo.NotNull() && 
                    pooledObjectsInfo.list.NotNull() &&
                    pooledObjectsInfo.list.ContainsIndex(i) &&
                    pooledObjectsInfo.list[i].NotNull())
                {
                    pooledObjectsInfo.list[i] = currentObjectInfo;
                }
            }

            enumMembersConfigAsset.ApplyModifiedProperties();
            pooledObjectsInfoAsset.ApplyModifiedProperties();
        }

        private void DrawCurrentEnumMemberParameters(int currentMemberIndex, 
            SerializedProperty enumMemberProperty, 
            SerializedProperty associatedIntProperty, 
            SerializedProperty associatedIntsProperty,
            ref ObjectInfo currentObjectInfo, 
            SerializedProperty pooledObjectsInfoListProperty)
        {
            if (currentMemberIndex > pooledObjectsInfoListProperty.arraySize - 1) return;
            
            int currentStartNumber = default;
            int currentAssociatedInt = default;
            GameObject currentPrefab = default;
            try
            {
                currentAssociatedInt = EditorGUILayout.IntField(associatedIntProperty.intValue, GUILayout.Width(Column0Width));
                EditorGUILayout.PropertyField(enumMemberProperty, GUIContent.none, GUILayout.Width(Column1Width));
                currentStartNumber = EditorGUILayout.IntField(currentObjectInfo.startNumber, GUILayout.Width(Column2Width));
                currentPrefab = EditorGUILayout.ObjectField(currentObjectInfo.prefab, typeof(GameObject), false, GUILayout.Width(Column3Width)) as GameObject;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            SetAssociatedInt(associatedIntsProperty, associatedIntProperty, currentAssociatedInt);
            pooledObjectsInfoListProperty.GetArrayElementAtIndex(currentMemberIndex).FindPropertyRelative(PooledObjectStartNumberPropertyName).intValue = currentStartNumber;
            pooledObjectsInfoListProperty.GetArrayElementAtIndex(currentMemberIndex).FindPropertyRelative(PooledObjectPrefabPropertyName).objectReferenceValue = currentPrefab;
        }

        private void DrawRemoveCurrentMemberButton(int currentMemberIndex,
            ref EnumCreatorConfig enumCreatorConfig,
            ref EnumMembersConfig enumMembersConfig,
            SerializedObject pooledObjectsInfoAsset,
            SerializedProperty pooledObjectsInfoListProperty,
            SerializedObject enumMembersConfigAsset, 
            SerializedProperty membersListProperty,
            SerializedProperty associatedIntsProperty)
        {
            if (GUILayout.Button("Remove", GUILayout.Width(Column4Width)))
            {
                membersListProperty.DeleteArrayElementAtIndex(currentMemberIndex);
                associatedIntsProperty.DeleteArrayElementAtIndex(currentMemberIndex);
                enumMembersConfigAsset.ApplyModifiedProperties();
                    
                pooledObjectsInfoListProperty.DeleteArrayElementAtIndex(currentMemberIndex);
                pooledObjectsInfoAsset.ApplyModifiedProperties();
                    
                EnumCreator.Create(enumCreatorConfig, enumMembersConfig);
            }
        }

        private void DrawAfterCurrentMemberButton(int currentMemberIndex, 
            SerializedProperty memberNameProperty, 
            ref ObjectInfo currentObjectInfo, 
            SerializedProperty pooledObjectsInfoListProperty)
        {
            if (currentMemberIndex > pooledObjectsInfoListProperty.arraySize - 1) return;
            
            bool isDontDestroyOnLoad = default;
            isDontDestroyOnLoad = EditorGUILayout.Toggle(currentObjectInfo.isDontDestroyOnload, GUILayout.Width(ToggleWidth));
            GUILayout.Label(PooledObjectIsDontDestroyOnLoadPropertyDescription, GUILayout.Width(Column5Width));
            pooledObjectsInfoListProperty.GetArrayElementAtIndex(currentMemberIndex).FindPropertyRelative(PooledObjectIsDontDestroyOnLoadPropertyName).boolValue = isDontDestroyOnLoad;
        }
        
        private void DrawAddMemberButton(ref EnumCreatorConfig enumCreatorConfig, ref EnumMembersConfig poolerEnumMembersConfig)
        {
            var tempColor = GUI.backgroundColor; 
            var tempContentColor = GUI.contentColor; 
            GUI.backgroundColor = new Color(0.77f, 0.77f, 0.77f);
            GUI.contentColor = Color.white;

            if (GUILayout.Button(AddEnumMemberButtonText, GUILayout.Width(DefaultButtonWidth), GUILayout.Height(DefaultButtonHeight)))
            {
                TryAddEnumMember(ref enumCreatorConfig, ref poolerEnumMembersConfig);
            }
            GUI.backgroundColor = tempColor;
            GUI.contentColor = tempContentColor;
        }

        private bool TryAddEnumMember(ref EnumCreatorConfig enumCreatorConfig, ref EnumMembersConfig enumMembersConfig)
        {
            CheckRepeatingPoolElements(ref enumMembersConfig, out bool elementsIsRepeats);
            if (elementsIsRepeats) return false;
            SerializedObject serializedObject = new SerializedObject(enumMembersConfig);
            SerializedProperty membersProperty = serializedObject.FindProperty(EnumMemberPropertyName);
            SerializedProperty associatedIntsProperty = serializedObject.FindProperty(AssociatedIntsPropertyName);
                    
            membersProperty.InsertArrayElementAtIndex(membersProperty.arraySize);
            associatedIntsProperty.InsertArrayElementAtIndex(associatedIntsProperty.arraySize);
            serializedObject.ApplyModifiedProperties();
            enumMembersConfig.members[membersProperty.arraySize-1] = $"{DefaultNamePrefixForNewMember}{(int)EditorApplication.timeSinceStartup}";
            enumMembersConfig.associatedInts[membersProperty.arraySize - 1] = GetFreeAssociatedInt(enumMembersConfig.associatedInts);
            EnumCreator.Create(enumCreatorConfig, enumMembersConfig);

            serializedObject.ApplyModifiedProperties();
            return true;

        }
        
        private void DrawBakePoolMembers(EnumCreatorConfig enumCreatorConfig, EnumMembersConfig poolerEnumMembersConfig)
        {
            Color tempColor = GUI.backgroundColor; 
            GUI.backgroundColor = new Color(0.08f, 0.5f, 1f);
            if (GUILayout.Button("Apply Rename", GUILayout.Width(DefaultButtonWidth), GUILayout.Height(DefaultButtonHeight)))
            {
                CheckRepeatingPoolElements(ref poolerEnumMembersConfig, out bool elementsIsRepeats);
                if (!elementsIsRepeats) EnumCreator.Create(enumCreatorConfig, poolerEnumMembersConfig);
            }
            GUI.backgroundColor = tempColor;
        }
        
        private void CheckRepeatingPoolElements(ref EnumMembersConfig poolerEnumMembersConfig, out bool elementsIsRepeats)
        {
            elementsIsRepeats = false;
            var members = poolerEnumMembersConfig.members;
            for (var i = 0; i < members.Length; i++)
            {
                for (var j = 0; j < members.Length; j++)
                {
                    if (i == j) continue;
                    if (members[i] == members[j])
                    {
                        Debug.LogWarning($"Pool contains repeating elements names! Please, fix it and Re Bake pool members.\nElement at {i} and Element at {i} indexes.");
                        elementsIsRepeats = true;
                    }
                }
            }
        }

        private void DrawInitPoolButton()
        {
            var tempColor = GUI.backgroundColor; 
            GUI.backgroundColor = new Color(0.1f, 0.5f, 0.3f);
            if (GUILayout.Button("Init plugin", GUILayout.Width(DefaultButtonWidth), GUILayout.Height(DefaultButtonHeight)))
            {
                InitPoolerFiles();
            }
            GUI.backgroundColor = tempColor;
        }

        private bool FindUniqueAsset<T>(string name, ref T result)
            where T : UnityEngine.Object
        {
            var assetsList = AssetDatabase.FindAssets(name);
            if (assetsList.Length > 1)
            {
                Debug.LogWarning($"Asset of type {typeof(T)} and name \"{name}\" number > 1. Was selected first.");
            }
            else if (assetsList.Length == 0)
            {
                Debug.LogError($"Asset of type {typeof(T)} and name \"{name}\" not found.");
                return false;
            }

            result = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(assetsList[0]), typeof(T)) as T;
            return true;
        }

        private static void SetAssociatedInt(SerializedProperty associatedIntsProperty, SerializedProperty targetAssociatedIntProperty, int associatedInt)
        {
            while (IsAssociatedIntUsed(associatedIntsProperty, associatedInt, targetAssociatedIntProperty)) 
                associatedInt++;

            targetAssociatedIntProperty.intValue = associatedInt;
        }

        private static bool IsAssociatedIntUsed(SerializedProperty associatedIntsProperty, int associatedInt, SerializedProperty ignoredAssociatedIntProperty = default)
        {
            for (int i = 0; i < associatedIntsProperty.arraySize; i++)
            {
                SerializedProperty current = associatedIntsProperty.GetArrayElementAtIndex(i);
                if (ignoredAssociatedIntProperty != null && current.propertyPath == ignoredAssociatedIntProperty.propertyPath) continue;
                if (current.intValue == associatedInt)
                    return true;
            }

            return false;
        }

        private static int GetFreeAssociatedInt(IReadOnlyList<int> associatedInts)
        {
            if (associatedInts.Count == 1) return associatedInts[0];
            int result = 0;
            while (associatedInts.ExistItem(item => item == result)) 
                result++;

            return result;
        }

        #endregion
    }
}