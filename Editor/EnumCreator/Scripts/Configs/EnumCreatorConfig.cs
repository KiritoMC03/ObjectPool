#if UNITY_EDITOR
using UnityEngine;

namespace JoyKirito.ObjectPool.Editor
{
    [CreateAssetMenu(fileName = "EnumCreatorConfigAsset", menuName = "EnumCreator/New Config", order = 0)]
    public class EnumCreatorConfig : ScriptableObject
    {
        #region Fields

        public string csFileName;
        public bool useNamespace;
        public bool useDefines;
        public string targetDefine;
        public string targetNamespace;
        
        #endregion
    }
}
#endif