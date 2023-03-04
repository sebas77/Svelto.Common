using System;
using Svelto.Common.Internal;

namespace Svelto.Common
{
    public static class TypeCache<T>
    {
        public static readonly Type   type = typeof(T);
        public static readonly string name = type.Name;
#if !UNITY_BURST
        public static readonly bool isUnmanaged = type.IsUnmanagedEx();
#else
        public static readonly bool isUnmanaged = Unity.Collections.LowLevel.Unsafe.UnsafeUtility.IsUnmanaged<T>();
#endif
    }

    public static class TypeType
    {
        public static bool isUnmanaged<T>(this T obj)
        {
            return TypeCache<T>.isUnmanaged;
        }
    }

    public static class TypeHash<T>
    {
#if !UNITY_BURST
        public static readonly int hash = TypeCache<T>.type.GetHashCode();
#else
        public static readonly int hash = Unity.Burst.BurstRuntime.GetHashCode32<T>();
#endif
    }
}