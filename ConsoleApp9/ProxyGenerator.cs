using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp9
{
    public class ProxyGenerator
    {
        public static IService Generate<IService, Implement>() where Implement : IService where IService : class
        {
            var serviceType = typeof(IService);
            var implementType = typeof(Implement);

            AssemblyName assemblyName = new AssemblyName("Proxy");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("ProxyModule", "Proxy.dll");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("ProxyType" + implementType.Name, TypeAttributes.Public, null, new Type[] { serviceType });

            var fieldBuilder = typeBuilder.DefineField("_innerService", serviceType, FieldAttributes.Private);

            var intercepMethods = serviceType.GetMethods().Where(p => p.GetCustomAttribute<InterceptorAttribute>() != null);
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, new Type[0]);

            var cIlGenerator = constructorBuilder.GetILGenerator();
            cIlGenerator.Emit(OpCodes.Ldarg_0);
            cIlGenerator.Emit(OpCodes.Newobj, implementType.GetConstructor(new Type[0]));
            cIlGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            //cIlGenerator.Emit(OpCodes.Ldarg_0); //this
            //cIlGenerator.Emit(OpCodes.Newobj, implementType.GetConstructor(new Type[0]));
            //cIlGenerator.Emit(OpCodes.Stfld);
            cIlGenerator.Emit(OpCodes.Ret);


            foreach (var item in intercepMethods)
            {
                var paraArr = item.GetParameters().Select(p => p.ParameterType).ToArray();
                MethodBuilder methodBuilder = typeBuilder.DefineMethod(item.Name, MethodAttributes.Public | MethodAttributes.Virtual, item.ReturnType, paraArr);
                ILGenerator iLGenerator = methodBuilder.GetILGenerator();
                iLGenerator.Emit(OpCodes.Newobj, implementType.GetConstructor(new Type[0]));
                iLGenerator.Emit(OpCodes.Ldstr, "Before Call");
                iLGenerator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
                for (int i = 0; i < paraArr.Length; i++)
                {
                    iLGenerator.Emit(OpCodes.Ldarg, i+1);
                }
                //iLGenerator.Emit(OpCodes.Ldarg_0);
                //iLGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                //iLGenerator.Emit(OpCodes.Callvirt, serviceType.GetMethod(item.Name, paraArr));

                iLGenerator.Emit(OpCodes.Call, implementType.GetMethod(item.Name, paraArr));
                if(item.ReturnType != typeof(void)){
                    LocalBuilder localBuilder = iLGenerator.DeclareLocal(item.ReturnType);
                    iLGenerator.Emit(OpCodes.Stloc_0);
                    iLGenerator.Emit(OpCodes.Ldstr, "after Call");
                    iLGenerator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
                    iLGenerator.Emit(OpCodes.Ldloc_0);
                    iLGenerator.Emit(OpCodes.Ret);
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Ldstr, "after Call");
                    iLGenerator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
                    iLGenerator.Emit(OpCodes.Ret);
                }
            }
            var type = typeBuilder.CreateType();
            assemblyBuilder.Save("Proxy.dll");
            return Activator.CreateInstance(type) as IService;
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class InterceptorAttribute:Attribute
    {
    }

    public interface ICallService
    {
        [Interceptor]
        void Call();

        [Interceptor]
        void CallName(string name);

        [Interceptor]
        string GetName();
    }

    public class CallService : ICallService
    {
        public void Call()
        {
            Console.WriteLine("call");
        }

        public void CallName(string name)
        {
            Console.WriteLine(name);
        }

        public string GetName()
        {
            return "Tim lv Get Name";
        }
    }
}
