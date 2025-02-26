using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Forest.MockAttributesTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TestClass t = new();
            t.HelloWorld();
        }

        // Mocks
        [Transaction(TransactionMode.Manual)]
        [Construction(TransactionMode.Manual)]
        internal class TestClass
        {
            public void HelloWorld() => System.Console.WriteLine("Hello, from within the test class!");
            
            // code from within the attributes is ran first.
            public TestClass()
            {
                Console.WriteLine("Listing all custom attributes:");
                foreach (var attr in typeof(TestClass).GetCustomAttributes())
                {
                    Console.WriteLine(attr.ToString());
                }


            }
        }
    }

    // Mocks
    internal enum TransactionMode
    {
        Manual = 1,
        ReadOnly
    }
    internal class TransactionAttribute : Attribute
    {
        private TransactionMode Mode { get; set; }
        public TransactionAttribute(TransactionMode mode)
        {
            Mode = mode;
            Console.WriteLine("Running inside the attribute: Transaction Attached!");
        }
    }

    internal class ConstructionAttribute : Attribute
    {
        public ConstructionAttribute(TransactionMode t)
        {
            //var check = this.GetType().GetCustomAttributes().OfType<TransactionAttribute>().Any();
            //if (check)
            //{
            //    Console.WriteLine("Found");
            //}
            //else
            //{
            //    Console.WriteLine("Not found");
            //}

            Console.WriteLine($"Running inside the attribute: Construction Attached! Mode is {t}");
        }
    }
}
