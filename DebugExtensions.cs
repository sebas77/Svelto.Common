namespace Svelto.Common.Internal
{
    public static class DebugExtensions
    {
        public static string TypeName<T>(this T any)
        {
#if DEBUG && !PROFILE_SVELTO          
            var type = any.GetType();
            if (_names.TryGetValue(type, out var name) == false)
            {
                name = type.ToString();
                _names[type] = name;
            }

            return name;
#else
            return "";
#endif
        }
#if DEBUG && !PROFILE_SVELTO          
<<<<<<< HEAD
        static readonly System.Collections.Generic.Dictionary<System.Type, string> _names = new System.Collections.Generic.Dictionary<System.Type, string>();
=======
        static readonly Dictionary<Type, string> _names = new Dictionary<Type, string>();
>>>>>>> 800c1a9abe35986fabb6562178e27d3b17c34b5c
#endif
    }
}