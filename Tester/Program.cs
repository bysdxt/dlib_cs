using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester {
    internal class Program {
        private static void Main(string[] args) {
            SelfBalancingBinarySearchTree.Main();
            Console.WriteLine("\ndone");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;
        }
    }
}
