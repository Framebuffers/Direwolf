using System;

namespace Forest.MockAttributesTest
{
    public class Program
    {

        //private const string db = @"postgresql://wolf:awoo@127.0.0.1:5432/direwolf?schema=public";
        public static void Main(string[] args)
        {
            PostgresConnection p = new();
            p.Connect();
        }


        //// Mocks
        //[Transaction(TransactionMode.Manual)]
        //[RevitCommand("Test", "11037623-EDDC-42FC-AD0E-ACBE3FE52B96", typeof(TransactionAttribute), "this", "path")]
        ////[Construction(TransactionMode.Manual)]
        //public class TestClass
        //{

        //    public void HelloWorld() => System.Console.WriteLine("Hello, from within the test class!");

        //    // code from within the attributes is ran first.
        //    public TestClass()
        //    {

        //        Console.WriteLine("Listing all custom attributes:");
        //        foreach (var attr in typeof(TestClass).GetCustomAttributes())
        //        {

        //        }


        //    }
        //}
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
