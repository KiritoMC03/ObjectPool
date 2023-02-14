using System.Collections.Generic;

namespace JoyKirito.ObjectPool
{
    /// <summary>
    /// Not recommended for use with UnityEngine.Object inheritors.
    /// </summary>
    public class Pool<T> 
        where T : IPoolable, new()
    {
        #region Fields

        private List<T> items;

        #endregion

        #region Constructors

        public Pool(int startedCapacity = 4) => items = new List<T>(startedCapacity);

        #endregion

        #region Methods

        public T Get()
        {
            T result;
            if (items.Count > 0)
            {
                result = items[items.LastIndex()];
                items.RemoveAt(items.LastIndex());
                return result;
            }

            result = new T();
            result.Reset();
            return result;
        }

        public void Send(T item)
        {
            item.Reset();
            items.Add(item);
        }

        #endregion
    }
}