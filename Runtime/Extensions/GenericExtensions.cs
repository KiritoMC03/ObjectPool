namespace JoyKirito.ObjectPool
{
    public static class GenericExtensions
    {
        public static bool IsNull<T>(this T obj)
        {
            if (obj is UnityEngine.Object uniObject) 
                return uniObject == null || (object) uniObject == null;
            return obj == null;
        }

        public static bool NotNull<T>(this T obj)
        {
            if (obj is UnityEngine.Object uniObject) 
                return uniObject != null && (object) uniObject != null;
            return obj != null;
        }
    }
}