using System.Collections.Generic;
using UnityEngine;

namespace JoyKirito.ObjectPool
{
    public class Pool
    {
        #region Properties

        public ObjectInfo Info { get; }
        public Transform Container { get; }
        public Queue<GameObject> Objects { get; }

        #endregion

        #region Methods
        
        public Pool(Transform container, ObjectInfo info)
        {
            this.Info = info;
            this.Container = container;
            this.Objects = new Queue<GameObject>(info.startNumber);
        }

        #endregion
    }
}