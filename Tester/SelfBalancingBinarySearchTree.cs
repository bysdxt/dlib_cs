using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tester {
    internal static class SelfBalancingBinarySearchTree {
        public static class Counter {
            public static ulong nTest = 0;

            public static ulong nTotalDeepth_dlib = 0;
            public static ulong nTotalDeepth_RedBlack = 0;
            public static (uint dlib, uint RedBlack) dTotalDeepth_dlib_RedBlack = (0, 0);
            public static (uint RedBlack, uint dlib) dTotalDeepth_RedBlack_dlib = (0, 0);

            public static ulong nRotation_dlib = 0;
            public static ulong nRotation_RedBlack = 0;
            public static (uint dlib, uint RedBlack) dRotation_dlib_RedBlack = (0, 0);
            public static (uint RedBlack, uint dlib) dRotation_RedBlack_dlib = (0, 0);

            public static ulong nCompare_dlib = 0;
            public static ulong nCompare_RedBlack = 0;
            public static (uint dlib, uint RedBlack) dCompare_dlib_RedBlack = (0, 0);
            public static (uint RedBlack, uint dlib) dCompare_RedBlack_dlib = (0, 0);

            public static uint kRotation = 0;
            public static uint kCompare = 0;
        }
        private class TesterComparer<T> : IComparer<T> {
            private static readonly IComparer<T> Default = Comparer<T>.Default;
            public int Compare(T x, T y) {
                ++Counter.kCompare;
                return Default.Compare(x, y);
            }
        }
        public static readonly IComparer<int> Comparer = new TesterComparer<int>();
        public static readonly Action OnRotation = () => ++Counter.kRotation;
        private static uint TotalDeepth(this dlib.SelfBalancingBinarySearchTree.ObjectReference<int>.Node root) {
            if (root.count is 0) return 0;
            return root.count + TotalDeepth(root.left) + TotalDeepth(root.right);
        }
        private static uint TotalDeepth(this dlib.SelfBalancingBinarySearchTree.ObjectReference<int> target) {
            if (target is null) return 0u;
            return target.root.TotalDeepth();
        }
        private static (uint count, uint deepth) TotalDeepth(this dotNetSystem.Collections.Generic.SortedSet<int>.Node root) {
            if (root is null) return (0, 0);
            var left = TotalDeepth(root.Left);
            var right = TotalDeepth(root.Right);
            var c = 1u + left.count + right.count;
            return (c, c + left.deepth + right.deepth);
        }
        private static uint TotalDeepth(this dotNetSystem.Collections.Generic.SortedSet<int> target) {
            if (target is null) return 0;
            return target.root.TotalDeepth().deepth;
        }
        public static (Thread, EventWaitHandle) Main() {
            dlib.SelfBalancingBinarySearchTree.ObjectReference<int>.AfterRotation = OnRotation;
            dotNetSystem.Collections.Generic.SortedSet<int>.AfterRotation = OnRotation;
            var hstop = new EventWaitHandle(false, EventResetMode.ManualReset);
            var th = new Thread(() => {

            });
            return (null, hstop);
        }
        public static void EchoTree1(dlib.SelfBalancingBinarySearchTree.ObjectReference<int> target) {
            if (target is null)
                Console.WriteLine("<null tree>");
            else
                EchoTree2(target.root);
        }
        public static void EchoTree2(dlib.SelfBalancingBinarySearchTree.ObjectReference<int>.Node root) {
            if (root is null) {
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
