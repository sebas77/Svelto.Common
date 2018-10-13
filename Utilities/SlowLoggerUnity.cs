#if UNITY_5_3_OR_NEWER || UNITY_5
using System.Collections.Generic;

namespace Svelto.Utilities
{
    public class SlowUnityLogger : ILogger
    {
        public void Log(string txt, string stack = null, LogType type = LogType.Log, Dictionary<string, string> data = null)
        {
            switch (type)
            {
                case LogType.Log:
                    UnityEngine.Debug.Log(stack != null ? txt.FastConcat(stack) : txt);
                    break;
                case LogType.Warning:
                    UnityEngine.Debug.LogWarning(stack != null ? txt.FastConcat(stack) : txt);
                    break;
                case LogType.Exception:
                case LogType.Error:
                    UnityEngine.Debug.LogError(stack != null ? txt.FastConcat(stack) : txt);
                    break;
            }
        }
    }
}
#endif