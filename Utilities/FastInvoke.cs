using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;

namespace Svelto.Utilities
{
    //https://stackoverflow.com/questions/321650/how-do-i-set-a-field-value-in-an-c-sharp-expression-tree/321686#321686

    public static class FastInvoke<T> 
    {
#if ENABLE_IL2CPP
        public static CastedAction<T> MakeSetter(FieldInfo field)
        {
            if (field.FieldType.IsInterfaceEx() == true && field.FieldType.IsValueTypeEx() == false)
            {
                return new CastedAction<T>(field.SetValue);
            }

            throw new ArgumentException("<color=orange>Svelto.ECS</color> unsupported field (must be an interface and a class)");
        }
#elif !NETFX_CORE && !NET_STANDARD_2_0 && !UNITY_WSA_10_0 && !NETSTANDARD2_0
        public static CastedAction<T> MakeSetter(FieldInfo field)
        {
            if (field.FieldType.IsInterfaceEx() == true && field.FieldType.IsValueTypeEx() == false)
            {
                DynamicMethod m = new DynamicMethod("setter", typeof(void),  typeof(T), typeof(object) });
                ILGenerator cg = m.GetILGenerator();

                // arg0.<field> = arg1
                cg.Emit(OpCodes.Ldarg_0);
                cg.Emit(OpCodes.Ldarg_1);
                cg.Emit(OpCodes.Stfld, field);
                cg.Emit(OpCodes.Ret);

                var del = m.CreateDelegate(typeof(Action<T, object>));

                return new CastedAction<T>(del);
            }

            throw new ArgumentException("<color=orange>Svelto.ECS</color> unsupported field (must be an interface and a class)");
        }
#else
        public static CastedAction<T> MakeSetter(FieldInfo field)
        {
            if (field.FieldType.IsInterfaceEx() == true && field.FieldType.IsValueTypeEx() == false)
            {
                ParameterExpression targetExp = Expression.Parameter(typeof(T).MakeByRefType(), "target");
                ParameterExpression valueExp = Expression.Parameter(typeof(object), "value");

                MemberExpression fieldExp = Expression.Field(targetExp, field);
                UnaryExpression convertedExp = Expression.TypeAs(valueExp, field.FieldType);
                BinaryExpression assignExp = Expression.Assign(fieldExp, convertedExp);

                var setter = Expression.Lambda<ActionRef<T, object>>(assignExp, targetExp, valueExp).Compile();

                return new CastedAction<T>(setter); 
            }

            throw new ArgumentException("<color=orange>Svelto.ECS</color> unsupported field (must be an interface and a class)");
        }
#endif
    }

    public delegate void ActionRef<T, O>(ref T target, O value);
    
    public class CastedAction<T>  
    {
        readonly ActionRef<T, object> setter;

        public CastedAction(Delegate setter)
        {
            this.setter = (ActionRef<T, object>)setter;
        }

        public CastedAction(ActionRef<T, object> setter)
        {
            this.setter = setter;
        }

        public void Call(ref T target, object value)
        {
            setter(ref target, value);
        }
    }
}