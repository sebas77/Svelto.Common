using System;

namespace Svelto.Common
{
#if UNITY_5_3_OR_NEWER && PROFILER    
    public struct PlatformProfilerMT : IDisposable
    {
        readonly UnityEngine.Profiling.CustomSampler sampler;

        public PlatformProfilerMT(string name):this()
        {
            UnityEngine.Profiling.Profiler.BeginThreadProfiling("Svelto.Tasks", name);
        }

        public void Dispose()
        {
            UnityEngine.Profiling.Profiler.EndThreadProfiling();
        }

        public DisposableStruct Sample(string samplerName)
        {
            return new DisposableStruct(UnityEngine.Profiling.CustomSampler.Create(samplerName));
        }

        public struct DisposableStruct : IDisposable
        {
            readonly UnityEngine.Profiling.CustomSampler _sampler;

            public DisposableStruct(UnityEngine.Profiling.CustomSampler customSampler)
            {
                _sampler = customSampler;
                _sampler.Begin();
            }

            public void Dispose()
            {
                _sampler.End();
            }
        }
    }
    
    public struct PlatformProfiler : IDisposable
    {
        public PlatformProfiler(string name):this()
        {}

        public void Dispose()
        {}

        public DisposableStruct Sample(string samplerName)
        {
            return new DisposableStruct(samplerName);
        }

        public struct DisposableStruct : IDisposable
        {
            public DisposableStruct(string samplerName)
            {
                UnityEngine.Profiling.Profiler.BeginSample(samplerName);
            }

            public void Dispose()
            {
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }
    }
#else
    public struct PlatformProfiler : IDisposable
    {
        public PlatformProfiler(string name)
        {}

        public void Dispose()
        {}

        public DisposableStruct Sample(string samplerName)
        {
            return new DisposableStruct();
        }

        public struct DisposableStruct : IDisposable
        {
            public void Dispose()
            {}
        }
    }
    
    public struct PlatformProfilerMT : IDisposable
    {
        public PlatformProfilerMT(string name)
        {}

        public void Dispose()
        {}

        public DisposableStruct Sample(string samplerName)
        {
            return new DisposableStruct();
        }

        public struct DisposableStruct : IDisposable
        {
            public void Dispose()
            {}
        }
    }
#endif    
}