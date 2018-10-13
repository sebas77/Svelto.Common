﻿namespace Svelto.Utilities
{
        public delegate void ActionRef<T, W>(ref T target, ref W value);
        public delegate void ActionRef<T>(ref    T target);
}