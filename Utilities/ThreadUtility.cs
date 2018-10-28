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
#if NETFX_CORE && !NET_STANDARD_2_0 && !NETSTANDARD2_0
            throw new Exception("Svelto doesn't support UWP without NET_STANDARD_2_0 support");
#elif NET_4_6 || NET_STANDARD_2_0 || NETSTANDARD2_0
            Thread.Yield(); 
#else
            Thread.Sleep(0); 
#endif
        }

        public static void TakeItEasy()
        {
#if NETFX_CORE && !NET_STANDARD_2_0 && !NETSTANDARD2_0
            throw new Exception("Svelto doesn't support UWP without NET_STANDARD_2_0 support");
#else
            Thread.Sleep(1); 
#endif
        }

        /// <summary>
        /// Yield the thread every so often
        /// </summary>
        /// <param name="quickIterations">will be increment by 1</param>
        /// <param name="frequency">must be multipel of 2</param>
        public static bool Wait(ref int quickIterations, int frequency = 256)
        {
            if ((quickIterations++ & (frequency - 1)) == 0)
            {
                Yield();

                return true;
            }

            return false;
        }
    }

#if NETFX_CORE || NET_4_6
    public sealed class ManualResetEventEx
    {
        readonly ManualResetEventSlim _manualReset = new ManualResetEventSlim(false);
        
        public void Wait()
        {
            _manualReset.Wait();
        }

        public void Wait(int ms)
        {
            _manualReset.Wait(ms);
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
            _manualReset.Dispose();
        }
    }
#else
    public class ManualResetEventEx
    {
        readonly ManualResetEvent _manualReset = new ManualResetEvent(false);
        
        public void Wait()
        {
            _manualReset.WaitOne();
        }

        public void Wait(int ms)
        {
            _manualReset.WaitOne(ms);
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