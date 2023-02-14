using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace JoyKirito.ObjectPool
{
    internal class PoolsConstructor
    {
        #region Fields

        private GameObject tempStartGameObject;
        private GameObject tempInstantiateGameObject;
        
        private readonly Transform containersParent;
        private readonly Dictionary<PooledObjectType, Pool> pools;
        private readonly PooledObjectsInfo pooledObjectsInfo;
        
        private static readonly string PoolSuffix = "Pool";

        #endregion

        #region Constructors

        public PoolsConstructor(Transform containersParent, 
            Dictionary<PooledObjectType, Pool> pools, 
            PooledObjectsInfo pooledObjectsInfo)
        {
            this.containersParent = containersParent;
            this.pools = pools;
            this.pooledObjectsInfo = pooledObjectsInfo;
        }

        #endregion
        
        #region Methods

        internal async void CreatePool(ObjectInfo poolInfo, bool useAsync = false)
        { 
            GameObject poolContainer = CreateContainer(poolInfo.type);
            pools[poolInfo.type] = new Pool(poolContainer.transform, poolInfo);
            for (int i = 0; i < poolInfo.startNumber; i++)
            {
                tempStartGameObject = InstantiateObject(poolInfo.type, poolContainer.transform);
                pools[poolInfo.type].Objects.Enqueue(tempStartGameObject);
                if (useAsync) await Task.Yield();
            }
        }

        internal void CreatePool(PooledObjectType type)
        {
            ObjectInfo current;
            for (int i = 0; i < pooledObjectsInfo.list.Count; i++)
            {
                current = pooledObjectsInfo.list[i];
                if (current.type != type) continue;
                CreatePool(current);
                return;
            }

            Debug.LogWarning($"Can not create pool of type {type}. Pooled Objects Info not found.");
        }

        internal GameObject CreateContainer(PooledObjectType poolType)
        {
            GameObject result = new GameObject(poolType + PoolSuffix);
            result.transform.parent = containersParent;
            return result;
        }

        internal GameObject InstantiateObject(PooledObjectType type, Transform parent)
        {
            tempInstantiateGameObject = UnityEngine.Object.Instantiate(pooledObjectsInfo.list.Find(elem => elem.type == type).prefab, parent);
            SetType(tempInstantiateGameObject, type);
            tempInstantiateGameObject.SetActive(false);
            return tempInstantiateGameObject;
        }

        internal static void SetType(GameObject obj, PooledObjectType type)
        {
            if (!obj.TryGetComponent(out IPooledObject polledObj)) 
                polledObj = obj.AddComponent<IPooledObject>();

            polledObj.Type = type;
        }

        #endregion
    }
}