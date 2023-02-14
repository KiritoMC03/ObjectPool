using System;
using System.Collections.Generic;

namespace JoyKirito.ObjectPool
{
    public static class IDictionaryExtension
    {
        public static void RemoveWithSuchValues<T, U>(this IDictionary<T, U> dictionary, Predicate<U> predicate)
        {
            T[] keysArray = dictionary.GetKeysArray();
            for (int i = 0; i < keysArray.Length; i++)
            {
                T currentKey = keysArray[i];
                U currentValue = dictionary[keysArray[i]];
                if (predicate.Invoke(currentValue)) 
                    dictionary.Remove(currentKey);
            }
        }
        
        public static void RemoveWithSuchKeys<T, U>(this IDictionary<T, U> dictionary, Predicate<T> predicate)
        {
            T[] keysArray = dictionary.GetKeysArray();
            for (int i = 0; i < keysArray.Length; i++)
            {
                T currentKey = keysArray[i];
                if (predicate.Invoke(currentKey)) 
                    dictionary.Remove(currentKey);
            }
        }

        public static T[] GetKeysArray<T, U>(this IDictionary<T, U> dictionary)
        {
            ICollection<T> keys = dictionary.Keys;
            T[] keysArray = new T[keys.Count];
            keys.CopyTo(keysArray, 0);
            return keysArray;
        }
    }
}