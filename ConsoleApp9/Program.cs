using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading;

namespace ConsoleApp9
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = ProxyGenerator.Generate<ICallService, CallService>();
            service.Call();
            //service.CallName("tim lv");
            Console.WriteLine(service.GetName());

            Console.ReadKey();
            //Builder();
            //try
            //{
            //    Console.WriteLine("hello");
            //    var a = 1;
            //    var b = 2;
            //    var c = 3;
            //    Console.WriteLine(a + b + c);
            //    throw new NullReferenceException();
            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine("error");
            //}

            //Console.ReadLine();

        }

        static void Builder()
        {
            AssemblyName assemblyName = new AssemblyName("App1");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("AppModule1","AppModule1.exe");
            var typeBuilder = moduleBuilder.DefineType("person");
            var methodBuilder = typeBuilder.DefineMethod("test", MethodAttributes.Static | MethodAttributes.Public);
            var il = methodBuilder.GetILGenerator();
            il.DeclareLocal(typeof(string));
            il.DeclareLocal(typeof(int));
            il.Emit(OpCodes.Ldstr, "hello");
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) }));
            il.Emit(OpCodes.Ret);
            var type = typeBuilder.CreateType();
            assemblyBuilder.SetEntryPoint(type.GetMethod("test"));
            assemblyBuilder.Save("AppModule1.exe");
        }

        private static void case1()
        {
            AssemblyName assemblyName = new AssemblyName("ILEmit");
            AppDomain appDomain = Thread.GetDomain();
            var asmBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = asmBuilder.DefineDynamicModule("ILModule", "ILModule.dll");
            var typeBuilder = moduleBuilder.DefineType("Class1", TypeAttributes.Public | TypeAttributes.Serializable);
            //定义构造函数
            var constructorMethodBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(int), typeof(string) });
            //函数必须包含OpCodes.Ret；
            constructorMethodBuilder.GetILGenerator().Emit(OpCodes.Ret);
            var helloMethodBuilder = typeBuilder.DefineMethod("Hello", MethodAttributes.Public | MethodAttributes.Virtual);
            var helloILGenerator = helloMethodBuilder.GetILGenerator();
            var localBuilder = helloILGenerator.DeclareLocal(typeof(string));
            helloILGenerator.Emit(OpCodes.Ldstr, "Hello, World");
            helloILGenerator.EmitCall(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }), null);
            helloILGenerator.Emit(OpCodes.Ldstr, "Nihao");
            helloILGenerator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
            helloILGenerator.Emit(OpCodes.Ret);
            typeBuilder.CreateType();
            asmBuilder.Save("ILModule.dll");
        }

        public virtual void Say()
        {

        }

        public static void CreateHelloClass()
        {
            AssemblyName assemblyName = new AssemblyName("Hello");
            AppDomain appDomain = Thread.GetDomain();
            var asmBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuidler = asmBuilder.DefineDynamicModule("HelloModule", "Hello.dll");
            var typeBuilder = moduleBuidler.DefineType("HelloChinese", TypeAttributes.Public, typeof(object), new Type[] { typeof(IHello) });
            var methodBuilder = typeBuilder.DefineMethod("Say", MethodAttributes.Public | MethodAttributes.Virtual);
            var il = methodBuilder.GetILGenerator();
            il.Emit(OpCodes.Ldstr, "Nihao");
            il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
            il.Emit(OpCodes.Ret);

            var helloMethodBuilder = typeBuilder.DefineMethod("Hello", MethodAttributes.Public | MethodAttributes.Virtual, null, new Type[] { typeof(string), typeof(int) });
            var heIl = helloMethodBuilder.GetILGenerator();
            heIl.Emit(OpCodes.Ldarg_0);
            heIl.Emit(OpCodes.Ret);
            typeBuilder.CreateType();
            asmBuilder.Save("Hello.dll");
            var obj = asmBuilder.CreateInstance("HelloChinese") as IHello;
            if (obj == null)
            {
                throw new NullReferenceException();
            }
            obj.Say();
        }


        public static void Test()
        {
            AssemblyName assemblyName = new AssemblyName("Study");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("StudyModule", "StudyOpCodes.dll");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("StudyOpCodes", TypeAttributes.Public);
            MethodBuilder showMethod = typeBuilder.DefineMethod("Show", MethodAttributes.Public | MethodAttributes.Static, null, new Type[] { typeof(int), typeof(string) });

            ILGenerator ilOfShow = showMethod.GetILGenerator();
            ilOfShow.Emit(OpCodes.Ldstr, "姓名：{1} 年龄：{0}");
            //静态方法参数索引从0开始
            ilOfShow.Emit(OpCodes.Ldarg_0);
            ilOfShow.Emit(OpCodes.Ldarg_1);
            ilOfShow.Emit(OpCodes.Call, typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(int), typeof(string) }));
            ilOfShow.Emit(OpCodes.Pop);
            ilOfShow.Emit(OpCodes.Ret);
            Type t = typeBuilder.CreateType();
            assemblyBuilder.Save("StudyOpCodes.dll");
        }

        public static async Task GetTaskAsync()
        {
            await Task.Delay(10000);
            Console.WriteLine("after 10 second write");
        }

        public static Task GetTask()
        {
            return Task.Delay(10000).ContinueWith(t =>
            {
                Console.WriteLine("after 10 second write");
            });
        }

        public static async Task<string> GetStringAsync()
        {
            await Task.Delay(10000);
            return "content";
        }

        public static Task<string> GetString()
        {
            return Task.Run(() =>
            {
                return "content";
            });
        }



    }



    public interface IHello
    {
        void Say();
        void Hello(string name, int Id);
    }


    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public event EventHandler<string> CustomerEvent;
        public Person(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", Id, Name);
        }

    }

}


