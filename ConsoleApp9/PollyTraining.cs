using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polly;
using Polly.Bulkhead;

namespace ConsoleApp9
{
   public class PollyTraining
    {
        private BulkheadPolicy _proxy;
        public PollyTraining()
        {
            _proxy = Policy.BulkheadAsync(2, 1, c =>
            {
                Console.WriteLine("reject request");
                return Task.CompletedTask;
            });
        }
        public void case1(int number)
        {
            //并发控制。

            _proxy.ExecuteAsync(async () =>
            {
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(number);
                await Task.Delay(1000);
            });
            
        }

        public void TestCase1()
        {
            Parallel.For(0, 10, p =>
            {
                case1(p);
            });
            Console.ReadKey();
        }
    }
}
