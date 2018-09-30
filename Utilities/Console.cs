using System;
using System.Collections.Generic;
#if NETFX_CORE
using Windows.System.Diagnostics;
#else
using System.Diagnostics;
#endif
using System.Text;
using Svelto.DataStructures;

namespace Utility
{
    public static class Console
    {
        static readonly StringBuilder _stringBuilder = new StringBuilder(256);
        static readonly FasterList<Svelto.DataStructures.WeakReference<ILogger>> _loggers;

        static readonly ILogger _standardLogger;
        
        static Console()
        {
            _loggers = new FasterList<Svelto.DataStructures.WeakReference<ILogger>>();
#if UNITY_5_3_OR_NEWER || UNITY_5
            _standardLogger = new SlowUnityLogger();
#else
            _standardLogger = new SimpleLogger();
#endif
            _loggers.Add(new Svelto.DataStructures.WeakReference<ILogger>(_standardLogger));
        }

        public static void SetLogger(ILogger log)
        {
            _loggers[0] = new Svelto.DataStructures.WeakReference<ILogger>(log);
        }
        
        public static void AddLogger(ILogger log)
        {
            _loggers.Add(new Svelto.DataStructures.WeakReference<ILogger>(log));
        }

        static void Log(string                                                txt,
                        string                                                stack,
                        LogType                                               type,
                        System.Collections.Generic.Dictionary<string, string> extraData = null)
        {
            for (int i = 0; i < _loggers.Count; i++)
            {
                if (_loggers[i].IsValid == true)
                    _loggers[i].Target.Log(txt, stack, type, extraData);
                else
                {
                    _loggers.UnorderedRemoveAt(i);
                    i--;
                }
            }
        }
        
        public static void Log(string txt)
        {
            Log(txt, null, LogType.Log);
        }

        public static void LogError(string txt, string stack = null,
                                    System.Collections.Generic.Dictionary<string, string> extraData = null)
        {
            lock (_stringBuilder)
            {
                _stringBuilder.Length = 0;
                _stringBuilder.Append("-!!!!!!-> ");
                _stringBuilder.Append(txt);

                var toPrint = _stringBuilder.ToString();
                
                Log(toPrint, stack, LogType.Error);
            }
        }

        public static void LogException(Exception e)
        {
            string toPrint;
            string stackTrace;

            lock (_stringBuilder)
            {
                _stringBuilder.Length = 0;
                _stringBuilder.Append("-!!!!!!-> ").Append(e.Message);

                stackTrace = e.StackTrace;

                if (e.InnerException != null)
                {
                    e = e.InnerException;

                    _stringBuilder.Append(" Inner Message: ").Append(e.Message).Append(" Inner Stacktrace:")
                        .Append(e.StackTrace);

                    stackTrace = e.StackTrace;
                }

                toPrint = _stringBuilder.ToString();
                
                Log(toPrint, stackTrace, LogType.Exception);
            }
        }

        public static void LogWarning(string txt)
        {
            string toPrint;

            lock (_stringBuilder)
            {
                _stringBuilder.Length = 0;
                _stringBuilder.Append("------> ");
                _stringBuilder.Append(txt);

                toPrint = _stringBuilder.ToString();
            }

            Log(toPrint, null, LogType.Warning);
        }

        /// <summary>
        /// Use this function if you don't want the message to be batched
        /// </summary>
        /// <param name="txt"></param>
        public static void SystemLog(string txt)
        {
            string toPrint;

            lock (_stringBuilder)
            {
#if NETFX_CORE
                string currentTimeString = DateTime.UtcNow.ToString("dd/mm/yy hh:ii:ss");
                string processTimeString = (DateTime.UtcNow - ProcessDiagnosticInfo.
                                                GetForCurrentProcess().ProcessStartTime.DateTime.ToUniversalTime()).ToString();
#else
                string currentTimeString = DateTime.UtcNow.ToLongTimeString(); //ensure includes seconds
                string processTimeString = (DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime()).ToString();
#endif

                _stringBuilder.Length = 0;
                _stringBuilder.Append("[").Append(currentTimeString);
                _stringBuilder.Append("][").Append(processTimeString);
                _stringBuilder.Length = _stringBuilder.Length - 3; //remove some precision that we don't need
                _stringBuilder.Append("] ").AppendLine(txt);

                toPrint = _stringBuilder.ToString();
            }

#if !UNITY_EDITOR
#if !NETFX_CORE
            System.Console.WriteLine(toPrint);
#else
            //find a way to adopt a logger externally, if this is still needed
#endif
#else
            UnityEngine.Debug.Log(toPrint);
#endif
        }
    }
}