using UnityEngine;

namespace JoyKirito.ObjectPool
{
    public class IPooledObject : MonoBehaviour
    {
        public PooledObjectType Type { get; set; }
    }
}