﻿#if NEW_C_SHARP || !UNITY_5_3_OR_NEWER
using System.Runtime.CompilerServices;

//todo needs to be unit tested
public struct FixedTypedArray4<T> where T : unmanaged
{
    static readonly int Length = 4;

#pragma warning disable CS0169
    T field0;
    T field1;
    T field2;
    T field3;
#pragma warning restore CS0169    

    public int length => Length;

    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            DBC.Common.Check.Require(index < Length, "out of bound index");
                
            return Unsafe.Add(ref Unsafe.As<FixedTypedArray4<T>, T>(ref this), index);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            DBC.Common.Check.Require(index < Length, "out of bound index");

            Unsafe.Add(ref Unsafe.As<FixedTypedArray4<T>, T>(ref this), index) = value;
        }
    }
}
#endif