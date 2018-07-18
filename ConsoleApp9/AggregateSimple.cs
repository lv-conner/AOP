using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace ConsoleApp9
{
    public class AggregateSimple
    {
        public static void Case()
        {
            var list = GetList(100).ToList();
            var result = list.Aggregate((a, b) => a + b);
            Console.WriteLine(result);
        }


        static IEnumerable<int> GetList(int count)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < count; i++)
            {
                yield return random.Next(100);
            }
        }
    }
}
