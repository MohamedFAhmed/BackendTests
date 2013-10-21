using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiTrendzBETest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting BE tests .......");
            Console.WriteLine("Starting public APIs testing ..........");
            StartTesting();
            Console.WriteLine("Done public APIs testing!");
            Console.WriteLine("PRESS ANY KEY TO EXIT");
            Console.ReadKey();
        }
        static async void StartTesting()
        {
            PublicAPIsTester apisTester = new PublicAPIsTester();
            await Task.Run(() => apisTester.StartTesting());
        }
    }
}
