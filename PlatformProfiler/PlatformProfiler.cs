using System;
using Svelto.Common.Internal;

namespace Svelto.Common
{
    public interface IPlatformProfiler: IDisposable
    {
        DisposableStruct Sample(string samplerName, string samplerInfo = null);
        DisposableStruct Sample<T>(T sampled, string samplerInfo = null);
    }
    
    public struct DisposableStruct : IDisposable
    {
        readonly Action _endAction;
        readonly Action<object> _beginAction;
        readonly object         _beginInfo;

        public DisposableStruct(Action<object> beginAction, object beginInfo, Action endEndAction)
        {
            _endAction = endEndAction;
            _beginAction = beginAction;
            _beginInfo = beginInfo;

            _beginAction?.Invoke(_beginInfo);
        }

        public void Dispose()
        {
            _endAction?.Invoke();
        }

        public InverseDisposableStruct Yield()
        {
            _endAction?.Invoke();

            return new InverseDisposableStruct(_beginAction, _beginInfo);
        }
    }
    
    public struct InverseDisposableStruct : IDisposable
    {
        readonly object _beginInfo;
        readonly Action<object> _beginAction;

        public InverseDisposableStruct(Action<object> beginAction, object beginInfo)
        {
            _beginInfo = beginInfo;
            _beginAction = beginAction;
        }

        public void Dispose()
        {
            _beginAction?.Invoke(_beginInfo);
        }
    }
#if UNITY_2017_3_OR_NEWER && ENABLE_PLATFORM_PROFILER    
    public struct PlatformProfilerMT : IPlatformProfiler
    {
        static readonly Action END_SAMPLE_ACTION =() => UnityEngine.Profiling.Profiler.EndSample(); 
        static readonly Action<object> BEGIN_SAMPLE_ACTION =
            info => UnityEngine.Profiling.Profiler.BeginSample(info as string); 

        public PlatformProfilerMT(string info)
        {
            UnityEngine.Profiling.Profiler.BeginThreadProfiling("Svelto.Tasks", info);

            BEGIN_SAMPLE_ACTION(info);
        }
        
        public DisposableStruct Sample(string samplerName, string samplerInfo = null)
        {
#if !PROFILER        
            var name = samplerName.FastConcat("-", samplerInfo);
#else
            var name = samplerName;
#endif            
            return new DisposableStruct(BEGIN_SAMPLE_ACTION, name, END_SAMPLE_ACTION);
        }

        public DisposableStruct Sample<T>(T samplerName, string samplerInfo = null)
        {
            return Sample(samplerName.TypeName(), samplerInfo);
        }

        public void Dispose()
        {
            END_SAMPLE_ACTION();
            
            UnityEngine.Profiling.Profiler.EndThreadProfiling();
        }
    }

    public struct PlatformProfiler: IPlatformProfiler
    {
        static readonly Action END_SAMPLE_ACTION  = () => UnityEngine.Profiling.Profiler.EndSample(); 
        static readonly Action<object> BEGIN_SAMPLE_ACTION =
            (info) => UnityEngine.Profiling.Profiler.BeginSample(info as string);

        public PlatformProfiler(string info)
        {
            BEGIN_SAMPLE_ACTION(info);
        }
        
        public DisposableStruct Sample(string samplerName, string samplerInfo = null)
        {
#if !PROFILER                    
            var name = samplerName.FastConcat("-", samplerInfo);
#else
            var name = samplerName;
#endif            
            
            return new DisposableStruct(BEGIN_SAMPLE_ACTION, name, END_SAMPLE_ACTION);
        }

        public DisposableStruct Sample<T>(T sampled, string samplerInfo = null)
        {
            return Sample(sampled.TypeName(), samplerInfo);
        }

        public void Dispose()
        {
            END_SAMPLE_ACTION();
        }
    }
#else    
    public struct PlatformProfilerMT : IPlatformProfiler
    {
        public PlatformProfilerMT(string info)
        {}
        
        public DisposableStruct Sample(string samplerName, string samplerInfo = null)
        {
            return new DisposableStruct();
        }

        public DisposableStruct Sample<T>(T sampled, string samplerInfo = null)
        {
            return new DisposableStruct();
        }

        public void Dispose()
        {}
    }

    public struct PlatformProfiler: IPlatformProfiler
    {
        public PlatformProfiler(string info)
        {}

        public DisposableStruct Sample(string samplerName, string samplerInfo = null)
        {
            return new DisposableStruct();
        }
        
        public DisposableStruct Sample<T>(T samplerName, string samplerInfo = null)
        {
            return new DisposableStruct();
        }

        public void Dispose()
        {}
    }
#endif
}