using System;

namespace Svelto.Common
{
    public interface IPlatformProfiler: IDisposable
    {
        DisposableSampler Sample(string samplerName);
        DisposableSampler Sample<W>(W sampled);
    }

#if !ENABLE_PLATFORM_PROFILER
    public struct DisposableSampler: IDisposable
    {
        public void Dispose() { }

        public PauseProfiler Yield() { return default; }
    }

    public struct PlatformProfilerMT: IPlatformProfiler
    {
        public PlatformProfilerMT(string info) { }

        public DisposableSampler Sample(string samplerName)
        {
            return default;
        }

        public DisposableSampler Sample<T>(T sampled)
        {
            return default;
        }

        public void Dispose() { }
    }

    public struct PlatformProfiler: IPlatformProfiler
    {
        public PlatformProfiler(string info) { }
        
        public DisposableSampler Sample()
        {
            return default;
        }

        public DisposableSampler Sample(string samplerName)
        {
            return default;
        }

        public DisposableSampler Sample<T>(T samplerName)
        {
            return default;
        }

        public PauseProfiler Yield() { return default; }

        public void Dispose() { }

        public static PlatformProfiler PreCreate(string p0)
        {
            return default;
        }
    }

    public readonly struct PauseProfiler: IDisposable
    {
        public void Dispose() { }
    }
#endif
}