using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace JoyKirito.ObjectPool
{
    internal class ObjectPoolerWorkWithScenes
    {
        #region Fields

        private readonly ObjectPoolerOptimizer optimizer;
        private readonly Dictionary<PooledObjectType, Pool> pools;
        private readonly ObjectPoolerPrefsForScene prefsForScene;

        #endregion

        #region Constructors

        public ObjectPoolerWorkWithScenes(ObjectPoolerOptimizer optimizer, 
            Dictionary<PooledObjectType, Pool> pools,
            ObjectPoolerPrefsForScene prefsForScene)
        {
            this.optimizer = optimizer;
            this.pools = pools;
            this.prefsForScene = prefsForScene;
            SceneManager.activeSceneChanged += HandleSceneChanged;
        }

        #endregion

        #region Methods

        internal void ClearSubscribes() => SceneManager.activeSceneChanged -= HandleSceneChanged;

        private void HandleSceneChanged(Scene oldScene, Scene newScene)
        {
            RemoveSceneSpecificPools(prefsForScene);
        }

        private void RemoveSceneSpecificPools(ObjectPoolerPrefsForScene prefs = default)
        {
            optimizer.CheckPoolsSizeNumber();
            pools.RemoveWithSuchValues(pool =>
            {
                bool needDestroy = !pool.Info.isDontDestroyOnload && NotRequireForNewScene(pool.Info.type, prefs);
                if (needDestroy) UnityEngine.Object.Destroy(pool.Container.gameObject);
                return needDestroy;
            });
        }

        private static bool NotRequireForNewScene(PooledObjectType type, ObjectPoolerPrefsForScene prefs = default)
        {
            if (prefs == null) return false;
            PooledObjectType[] requiredTypes = prefs.RequiredForSceneTypes;
            int requiredTypesLength = requiredTypes.Length;
            for (int i = 0; i < requiredTypesLength; i++)
                if (requiredTypes[i] == type)
                    return false;
            return true;
        }

        #endregion
    }
}