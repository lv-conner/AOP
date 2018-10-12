using System;
using System.Threading.Tasks;
using AspectCore;
using AspectCore.DynamicProxy;
using AspectCore.Injector;

namespace AopConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxyGenerator = new ProxyGeneratorBuilder()
                .ConfigureService(services =>
                {
                    services.AddInstance<IName>(new ProgramName());
                })
                .Configure(p => { }).Build();
            var proxyHello = proxyGenerator.CreateInterfaceProxy<IHello, Hello>();
            proxyHello.Say("tim lv");
            Console.WriteLine("Hello World!");
        }
    }
    public interface IName
    {
        string Name { get; }
    }
    public class ProgramName : IName
    {
        public ProgramName()
        {
            _name = "test program";
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
        }
    }


    public interface IHello
    {
        [LogInterceptor]
        [CapInterceptor]
        void Say(string name);
    }
    public class Hello : IHello
    {
        public void Say(string name)
        {
            Console.WriteLine("Hello" + name);
        }
    }
    class CapInterceptorAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            for (int i = 0; i < context.Parameters.Length; i++)
            {
                if(context.Parameters[i] is string)
                {
                    context.Parameters[i] = context.Parameters[i].ToString().ToUpper();
                }
            }
            await next(context);

        }
    }

    class LogInterceptorAttribute : AspectCore.DynamicProxy.AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            Console.WriteLine("before call");
            await next(context);
            Console.WriteLine("after call");
        }
    }
}
