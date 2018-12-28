using System;
using Svelto.DataStructures;
using UnityEngine.Profiling;

namespace Svelto.Common
{
#if UNITY_5_3_OR_NEWER && ENABLE_PLATFORM_PROFILER    
    public struct PlatformProfilerMT : IDisposable
    {
        readonly CustomSampler sampler;

        public PlatformProfilerMT(string name):this()
        {
            Profiler.BeginThreadProfiling("Svelto.Tasks", name);
        }

        public void Dispose()
        {
            Profiler.EndThreadProfiling();
        }

        public DisposableStruct Sample(string samplerName, string samplerInfo = null)
        {
            return new DisposableStruct(samplePool.FetchSampler(samplerInfo != null ? samplerName.FastConcat(" ", samplerInfo) : samplerName));
        }

        public struct DisposableStruct : IDisposable
        {
            readonly CustomSampler _sampler;

            public DisposableStruct(CustomSampler customSampler)
            {
                _sampler = customSampler;
                _sampler.Begin();
            }

            public void Dispose()
            {
                _sampler.End();
                samplePool.pool.Enqueue(_sampler);
            }
        }

        static class samplePool
        {
            public static LockFreeQueue<CustomSampler> pool = new LockFreeQueue<CustomSampler>();

            public static CustomSampler FetchSampler(string name)
            {
                CustomSampler sampler;
                if (pool.Dequeue(out sampler) == false)
                    return CustomSampler.Create(name);

                return sampler;
            }
        }
    }
    
    public struct PlatformProfiler : IDisposable
    {
        public PlatformProfiler(string name) : this()
        {
            Profiler.BeginSample(name);
        }

        public void Dispose()
        {
            Profiler.EndSample();
        }

        public DisposableStruct Sample(string samplerName, string samplerInfo = null)
        {
            return new DisposableStruct(samplerInfo != null ? samplerName.FastConcat(" ", samplerInfo) : samplerName);
        }

        public struct DisposableStruct : IDisposable
        {
            public DisposableStruct(string samplerName)
            {
                Profiler.BeginSample(samplerName);
            }

            public void Dispose()
            {
                Profiler.EndSample();
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

        public DisposableStruct Sample(string samplerName, string sampleInfo = null)
        {
            return new DisposableStruct();
        }
        
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