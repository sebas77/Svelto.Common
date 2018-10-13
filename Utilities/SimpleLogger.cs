using System.Collections.Generic;

namespace Svelto.Utilities
{
    public class SimpleLogger : ILogger
    {
        public void Log(string txt, string stack = null, LogType type = LogType.Log, Dictionary<string, string> data = null)
        {
            switch (type)
            {
                case LogType.Log:
                    Console.SystemLog(stack != null ? txt.FastConcat(stack) : txt);
                    break;
                case LogType.Warning:
                    Console.SystemLog(stack != null ? txt.FastConcat(stack) : txt);
                    break;
                case LogType.Exception:
                case LogType.Error:
                    Console.SystemLog(stack != null ? txt.FastConcat(stack) : txt);
                    break;
            }
        }
    }
}