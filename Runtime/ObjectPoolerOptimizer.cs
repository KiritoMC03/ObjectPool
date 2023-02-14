using System.Collections.Generic;
using UnityEngine;

namespace JoyKirito.ObjectPool
{
    internal class ObjectPoolerOptimizer
    {
        #region Fields

        private readonly PooledObjectsInfo pooledObjectsInfo;
        private readonly Dictionary<PooledObjectType, Pool> pools;
        
        private const int MaxCachePoolSize = 4096;

        #endregion
        
        #region Constructors

        public ObjectPoolerOptimizer(PooledObjectsInfo pooledObjectsInfo, Dictionary<PooledObjectType, Pool> pools)
        {
            this.pooledObjectsInfo = pooledObjectsInfo;
            this.pools = pools;
        }

        #endregion

        #region Methods

        internal void CheckPoolsSizeNumber()
        {
#if UNITY_EDITOR
            foreach (KeyValuePair<PooledObjectType, Pool> current in pools)
            {
                int objectNumber = current.Value.Container.childCount;
                ObjectInfo info = pooledObjectsInfo.GetInfo(current.Key);
                if (objectNumber > info.startNumber) 
                    info.startNumber = Mathf.Clamp(objectNumber, 0, MaxCachePoolSize);
            }
#endif
        }

        #endregion
    }
}