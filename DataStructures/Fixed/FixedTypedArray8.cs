﻿#if NEW_C_SHARP || !UNITY_5_3_OR_NEWER
using System.Runtime.CompilerServices;

//todo needs to be unit tested
public struct FixedTypedArray8<T> where T : unmanaged
{
    static readonly int Length = 8;

#pragma warning disable CS0169
    FixedTypedArray4<T> foursA;
    FixedTypedArray4<T> foursB;
#pragma warning restore CS0169    

    public int length => Length;

    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            DBC.Common.Check.Require(index < Length, "out of bound index");
                
            return Unsafe.Add(ref Unsafe.As<FixedTypedArray8<T>, T>(ref this), index);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            DBC.Common.Check.Require(index < Length, "out of bound index");

            Unsafe.Add(ref Unsafe.As<FixedTypedArray8<T>, T>(ref this), index) = value;
        }
    }

}
#endif