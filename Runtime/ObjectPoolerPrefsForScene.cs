using UnityEngine;

namespace JoyKirito.ObjectPool
{
    public class ObjectPoolerPrefsForScene : MonoBehaviour
    {
        #region Properties

        [field: SerializeField]
        internal PooledObjectType[] RequiredForSceneTypes { get; set; }

        #endregion
    }
}