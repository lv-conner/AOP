using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp9
{
    public class ConstructProxy
    {
        public static void GenerateProxy()
        {
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new System.Reflection.AssemblyName("CProxy"), AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("CProxy", "CProxy.dll");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Person");
            var fieldABuilder = typeBuilder.DefineField("numA", typeof(Int32), FieldAttributes.Private);
            var fieldBBuilder = typeBuilder.DefineField("numA", typeof(Int32), FieldAttributes.Private);

            
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.HasThis, new Type[] { typeof(Int32), typeof(Int32) });
            var ctorIL = constructorBuilder.GetILGenerator();
            // numA = a;
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_1);
            ctorIL.Emit(OpCodes.Stfld, fieldABuilder);
            //NumB = b;
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_2);
            ctorIL.Emit(OpCodes.Stfld, fieldBBuilder);
            ctorIL.Emit(OpCodes.Ret);

            var type = typeBuilder.CreateType();
            var obj =  Activator.CreateInstance(type, 4, 6);


            assemblyBuilder.Save("CProxy.dll");

        }

        public static void man(string[] args)
        {
            //1.构建程序集
            var asmName = new AssemblyName("Elvinle");
            var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);

            //2.创建模块 
            var mdlBldr = asmBuilder.DefineDynamicModule("Elvin", "Elvin.dll");

            //3.定义类, public class Add
            var typeBldr = mdlBldr.DefineType("Add", TypeAttributes.Public | TypeAttributes.BeforeFieldInit);

            //4. 定义属性和字段
            //4.1字段 FieldBuilder
            var fieldABuilder = typeBldr.DefineField("numA", typeof(Int32), FieldAttributes.Private);
            //fieldABuilder.SetConstant(0); 此处为副初始值, 这里可省略

            var fieldBBuilder = typeBldr.DefineField("numB", typeof(Int32), FieldAttributes.Private);

            //4.2属性 PropertyBuilder
            var propertyABuilder = typeBldr.DefineProperty("NumA", PropertyAttributes.None, CallingConventions.HasThis, typeof(Int32), null);

            var propertyBBuilder = typeBldr.DefineProperty("NumB", PropertyAttributes.None, CallingConventions.HasThis, typeof(Int32), null);

            //5.定义属性numA的get;set;方法 MethodBuilder
            //5.1 get方法
            var getPropertyABuilder = typeBldr.DefineMethod("get", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(Int32), Type.EmptyTypes);
            //ILGenerator
            GetPropertyIL(getPropertyABuilder, fieldABuilder);
            //var getAIL = getPropertyABuilder.GetILGenerator();
            //getAIL.Emit(OpCodes.Ldarg_0);   //this
            //getAIL.Emit(OpCodes.Ldfld, fieldABuilder); //numA
            //getAIL.Emit(OpCodes.Ret); //return numA

            //5.2 set方法
            var setPropertyABuilder = typeBldr.DefineMethod("set", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(void), new Type[] { typeof(Int32) });
            //ILGenerator
            SetPropertyIL(setPropertyABuilder, fieldABuilder);
            //var setAIL = setPropertyABuilder.GetILGenerator();
            ////setAIL.Emit(OpCodes.Nop);   //这句可省略
            //setAIL.Emit(OpCodes.Ldarg_0);  //this
            //setAIL.Emit(OpCodes.Ldarg_1);  //value
            //setAIL.Emit(OpCodes.Stfld, fieldABuilder); //numA = value;
            //setAIL.Emit(OpCodes.Ret);   //return;

            //5.3 绑定
            propertyABuilder.SetGetMethod(getPropertyABuilder);
            propertyABuilder.SetSetMethod(setPropertyABuilder);

            //6.定义属性numA的get;set;方法 MethodBuilder
            var getPropertyBBuilder = typeBldr.DefineMethod("get", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(Int32), Type.EmptyTypes);
            GetPropertyIL(getPropertyBBuilder, fieldBBuilder);

            var setPropertyBBuilder = typeBldr.DefineMethod("set", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(void), new Type[] { typeof(Int32) });
            SetPropertyIL(setPropertyBBuilder, fieldBBuilder);

            propertyBBuilder.SetGetMethod(getPropertyBBuilder);
            propertyBBuilder.SetSetMethod(setPropertyBBuilder);

            //7.定义构造函数 ConstructorBuilder
            var constructorBuilder = typeBldr.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.HasThis, new Type[] { typeof(Int32), typeof(Int32) });
            var ctorIL = constructorBuilder.GetILGenerator();
            // numA = a;
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_1);
            ctorIL.Emit(OpCodes.Stfld, fieldABuilder);
            //NumB = b;
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_2);
            ctorIL.Emit(OpCodes.Stfld, fieldBBuilder);
            ctorIL.Emit(OpCodes.Ret);

            //8.定义方法 MethodBuilder
            var calcMethodBuilder = typeBldr.DefineMethod("Calc", MethodAttributes.Public | MethodAttributes.HideBySig, typeof(Int32), Type.EmptyTypes);
            var calcIL = calcMethodBuilder.GetILGenerator();
            //加载私有字段numA
            calcIL.Emit(OpCodes.Ldarg_0);
            calcIL.Emit(OpCodes.Ldfld, fieldABuilder);
            //加载属性NumB
            calcIL.Emit(OpCodes.Ldarg_0);
            calcIL.Emit(OpCodes.Ldfld, fieldBBuilder);
            //相加并返回栈顶的值
            calcIL.Emit(OpCodes.Add);
            calcIL.Emit(OpCodes.Ret);

            //9.结果
            Type type = typeBldr.CreateType();
            int a = 2;
            int b = 3;
            Object ob = Activator.CreateInstance(type, new object[] { a, b });
            Console.WriteLine("The Result of {0} + {1} is {2}", type.GetProperty("NumA").GetValue(ob), type.GetProperty("NumB").GetValue(ob), ob.GetType().GetMethod("Calc").Invoke(ob, null));

            asmBuilder.Save("Elvin.dll");
            Console.ReadKey();
        }

        private static void GetPropertyIL(MethodBuilder getPropertyBuilder, FieldBuilder fieldBuilder)
        {
            //ILGenerator
            var getAIL = getPropertyBuilder.GetILGenerator();
            getAIL.Emit(OpCodes.Ldarg_0);   //this
            getAIL.Emit(OpCodes.Ldfld, fieldBuilder); //numA
            getAIL.Emit(OpCodes.Ret); //return numA
        }

        private static void SetPropertyIL(MethodBuilder setPropertyBuilder, FieldBuilder fieldBuilder)
        {
            //ILGenerator
            var setAIL = setPropertyBuilder.GetILGenerator();
            //setAIL.Emit(OpCodes.Nop);   //这句可省略
            setAIL.Emit(OpCodes.Ldarg_0);  //this
            setAIL.Emit(OpCodes.Ldarg_1);  //value
            setAIL.Emit(OpCodes.Stfld, fieldBuilder); //numA = value;
            setAIL.Emit(OpCodes.Ret);   //return;
        }
    }
}
