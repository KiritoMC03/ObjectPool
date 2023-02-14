#if UNITY_EDITOR
using UnityEngine;

namespace JoyKirito.ObjectPool.Editor
{
    [CreateAssetMenu(fileName = "EnumMembersAsset", menuName = "EnumCreator/New Enum Members Config", order = 0)]
    public class EnumMembersConfig : ScriptableObject
    {
        #region Fields

        public string[] members = new string[0];
        public int[] associatedInts = new int[0];

        #endregion
    }
}
#endif