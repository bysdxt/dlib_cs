using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tester {
    internal class Program {
        private static void Main(string[] args) {
            Console.InputEncoding = Console.OutputEncoding = Encoding.Unicode;
            var ConsoleSyncObject = new object();
            var test1 = SelfBalancingBinarySearchTree.Main(ConsoleSyncObject);
            while (test1.th.IsAlive) {
                Thread.Sleep(333);
                bool b;
                lock (ConsoleSyncObject)
                    b = Console.KeyAvailable && Console.ReadKey(false).Key == ConsoleKey.Spacebar;
                if (b) {
                    test1.stop.Set();
                    break;
                }
            }
            test1.th.Join();
            Console.WriteLine("\ndone");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;
        }
    }
}
