using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;

namespace Forest.MockAttributesTest
{
    public class Program
    {

        public static void Main(string[] args)
        {
            RecursiveLoad.Execute();
            //var a = new(typeof(Program));
            //Console.WriteLine();
            //TestClass t = new();
            //Console.WriteLine(nameof(TestClass));
            //TestClass t = new();
            //t.HelloWorld();
        }

        // Mocks
        [Transaction(TransactionMode.Manual)]
        [RevitCommand("Test", "11037623-EDDC-42FC-AD0E-ACBE3FE52B96", typeof(TransactionAttribute), "this", "path")]
        //[Construction(TransactionMode.Manual)]
        public class TestClass
        {
     
            public void HelloWorld() => System.Console.WriteLine("Hello, from within the test class!");
            
            // code from within the attributes is ran first.
            public TestClass()
            {
                
                Console.WriteLine("Listing all custom attributes:");
                foreach (var attr in typeof(TestClass).GetCustomAttributes())
                {
                    
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
