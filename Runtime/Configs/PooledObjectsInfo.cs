using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JoyKirito.ObjectPool
{
    [CreateAssetMenu(fileName = "PooledObjectsInfoAsset", menuName = "ObjectPooler/New Pooled Objects Info", order = 0)]
    public class PooledObjectsInfo : ScriptableObject
    {
        #region Fields

        public StartedPoolsCreationMode startedPoolsCreationMode = StartedPoolsCreationMode.AllWithInit;
        
        [Space]
        public List<ObjectInfo> list = new List<ObjectInfo>(0);

        #endregion

        #region Methods

        public ObjectInfo GetInfo(PooledObjectType type)
        {
            ObjectInfo current = default;
            for (int i = 0; i < list.Count; i++)
            {
                current = list[i];
                if (current.type == type) return current;
            }
            return default;
        }

        #endregion
    }
}