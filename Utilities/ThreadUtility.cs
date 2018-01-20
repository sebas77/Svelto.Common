using UnityEngine;
#if NETFX_CORE
using System.Threading.Tasks;
#endif
using System.Threading;

namespace Svelto.Utilities
{
    public static class ThreadUtility
    {
        public static void MemoryBarrier()
        {
#if NETFX_CORE || NET_4_6
            Interlocked.MemoryBarrier();
#else
            Thread.MemoryBarrier();
#endif
        }

        public static void Yield()
        {
#if NETFX_CORE            
            Task.Yield();
#elif NET_4_6
            Thread.Yield(); 
#else
            Thread.Sleep(0);
#endif    
        }
        
        public static void SleepZero()
        {
#if NETFX_CORE            
            Task.Yield();
#elif NET_4_6
            Thread.Sleep(0); 
#else
            Thread.Sleep(0);
            #endif    
        }
    }
#if NETFX_CORE || NET_4_6
    public sealed class ManualResetEventEx : ManualResetEventSlim
    {
        public new void Wait()
        {
            base.Wait();
        }

        public new void Reset()
        {
            base.Reset();
        }

        public new void Set()
        {
            base.Set();
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
#else
    public class ManualResetEventEx
    {
        ManualResetEvent _manualReset = new ManualResetEvent(false);
        
        public void Wait()
        {
            _manualReset.WaitOne();
        }

        public void Reset()
        {
            _manualReset.Reset();
        }

        public void Set()
        {
            _manualReset.Set();
        }

        public void Dispose()
        {
            _manualReset.Close();
        }
    }
#endif
}