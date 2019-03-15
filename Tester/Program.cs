﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester {
    internal class Program {
        private static void Main(string[] args) {
            var s = new dlib.SelfBalancingBinarySearchTree.ObjectReference<int>();
            int i ;
            for (i = 1; i <= 10; ++i)
                s.Add(i);
            foreach (var x in s) {
                Console.WriteLine(x);
                if (i < 20)
                    s.Add(i++);
            }
            Console.WriteLine("\ndone");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;
        }
    }
    internal static class SelfBalancingBinarySearchTree {
        public static void EchoTree1(dlib.SelfBalancingBinarySearchTree.ObjectReference<int> target) {
            if (target is null)
                Console.WriteLine("<null tree>");
            else
                EchoTree2(target.root);
        }
        public static void EchoTree2(dlib.SelfBalancingBinarySearchTree.ObjectReference<int>.Node root) {
            if(root is null) {
                Console.WriteLine("<null node tree>");
                return;
            }
            var (ca, w, h) = dlib.dlibExtends.EchoTree(root, x => x == dlib.SelfBalancingBinarySearchTree.ObjectReference<int>.Node.Nil, x => x.left, x => x.right, x => x.value.ToString());
            Console.WriteLine();
            for (var y = 0; y < h; ++y) {
                for (var x = 0; x < w; ++x)
                    Console.Write(ca[x, y]);
                Console.WriteLine();
            }
        }
    }
}
