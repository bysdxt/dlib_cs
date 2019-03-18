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
            public static ulong nData = 0;

            public static ulong nTotalDeepth_dlib = 0;
            public static ulong nTotalDeepth_RedBlack = 0;
            public static (uint dlib, uint RedBlack) mTotalDeepth_dlib_RedBlack = (0, 0);
            public static (uint RedBlack, uint dlib) mTotalDeepth_RedBlack_dlib = (0, 0);

            public static ulong nRotation_dlib = 0;
            public static ulong nRotation_RedBlack = 0;
            public static (uint dlib, uint RedBlack) mRotation_dlib_RedBlack = (0, 0);
            public static (uint RedBlack, uint dlib) mRotation_RedBlack_dlib = (0, 0);

            public static ulong nCompare_dlib = 0;
            public static ulong nCompare_RedBlack = 0;
            public static (uint dlib, uint RedBlack) mCompare_dlib_RedBlack = (0, 0);
            public static (uint RedBlack, uint dlib) mCompare_RedBlack_dlib = (0, 0);

            public static uint kRotation = 0;
            public static uint kCompare = 0;

            public static uint kRotation_dlib = 0;
            public static uint kRotation_RedBlack = 0;
            public static uint kCompare_dlib = 0;
            public static uint kCompare_RedBlack = 0;
            public static void Calc(uint kTotalDeepth_dlib, uint kTotalDeepth_RedBlack) {
                nTotalDeepth_dlib += kTotalDeepth_dlib;
                nTotalDeepth_RedBlack += kTotalDeepth_RedBlack;
                nRotation_dlib += kRotation_dlib;
                nRotation_RedBlack += kRotation_RedBlack;
                nCompare_dlib = kCompare_dlib;
                nCompare_RedBlack = kCompare_RedBlack;
                if (kTotalDeepth_dlib + kTotalDeepth_RedBlack > 0) {
                    // kTotalDeepth_dlib/kTotalDeepth_RedBlack >= mTotalDeepth_dlib_RedBlack.dlib/mTotalDeepth_dlib_RedBlack.RedBlack
                    if ((ulong)kTotalDeepth_dlib * mTotalDeepth_dlib_RedBlack.RedBlack >= mTotalDeepth_dlib_RedBlack.dlib * (ulong)kTotalDeepth_RedBlack)
                        mTotalDeepth_dlib_RedBlack = (kTotalDeepth_dlib, kTotalDeepth_RedBlack);
                    // kTotalDeepth_RedBlack/kTotalDeepth_dlib >= mTotalDeepth_RedBlack_dlib.RedBlack/mTotalDeepth_RedBlack_dlib.dlib
                    if ((ulong)kTotalDeepth_RedBlack * mTotalDeepth_RedBlack_dlib.dlib >= mTotalDeepth_RedBlack_dlib.RedBlack * (ulong)kTotalDeepth_dlib)
                        mTotalDeepth_RedBlack_dlib = (kTotalDeepth_RedBlack, kTotalDeepth_dlib);
                }
                if (kRotation_dlib + kRotation_RedBlack > 0) {
                    // kRotation_dlib/kRotation_RedBlack >= mRotation_dlib_RedBlack.dlib/mRotation_dlib_RedBlack.RedBlack
                    if ((ulong)kRotation_dlib * mRotation_dlib_RedBlack.RedBlack >= mRotation_dlib_RedBlack.dlib * (ulong)kRotation_RedBlack)
                        mRotation_dlib_RedBlack = (kRotation_dlib, kRotation_RedBlack);
                    // kRotation_RedBlack/kRotation_dlib >= mRotation_RedBlack_dlib.RedBlack/mRotation_RedBlack_dlib.dlib
                    if ((ulong)kRotation_RedBlack * mRotation_RedBlack_dlib.dlib >= mRotation_RedBlack_dlib.RedBlack * (ulong)kRotation_dlib)
                        mRotation_RedBlack_dlib = (kRotation_RedBlack, kRotation_dlib);
                }
                if (kCompare_dlib + kCompare_RedBlack > 0) {
                    // kCompare_dlib/kCompare_RedBlack >= mCompare_dlib_RedBlack.dlib/mCompare_dlib_RedBlack.RedBlack
                    if ((ulong)kCompare_dlib * mCompare_dlib_RedBlack.RedBlack >= mCompare_dlib_RedBlack.dlib * (ulong)kCompare_RedBlack)
                        mCompare_dlib_RedBlack = (kCompare_dlib, kCompare_RedBlack);
                    // kCompare_RedBlack/kCompare_dlib >= mCompare_RedBlack_dlib.RedBlack/mCompare_RedBlack_dlib.dlib
                    if ((ulong)kCompare_RedBlack * mCompare_RedBlack_dlib.dlib >= mCompare_RedBlack_dlib.RedBlack * (ulong)kCompare_dlib)
                        mCompare_RedBlack_dlib = (kCompare_RedBlack, kCompare_dlib);
                }
            }
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
        public static (Thread, EventWaitHandle) Main(object ConsoleSyncObject) {
            if (ConsoleSyncObject is null) ConsoleSyncObject = Comparer;
            dlib.SelfBalancingBinarySearchTree.ObjectReference<int>.AfterRotation = OnRotation;
            dotNetSystem.Collections.Generic.SortedSet<int>.AfterRotation = OnRotation;
            var now = DateTime.Now;
            var rand = new Random(unchecked((int)((now.Ticks + Environment.TickCount) % int.MaxValue)));
            var next = now.AddSeconds(8191);
            byte inext = 0;
            var hstop = new EventWaitHandle(false, EventResetMode.ManualReset);
            var th = new Thread(() => {
                try {
                    string line;
                    int mline = 0, nline, dline;
                    var spaces = new string[64];
                    int[] buffer = null;
                    var nbuffer = 0;
                    while (!hstop.WaitOne(0)) {
                        if (unchecked(++inext > 127)) {
                            inext = 0;
                            lock (ConsoleSyncObject) {
                                if ((nline = (line = $"\rnData/nTest={Counter.nData}/{Counter.nTest}").Length) > mline)
                                    mline = nline;
                                else if ((dline = mline - nline) > 0) {
                                    if (dline >= spaces.Length) Array.Resize(ref spaces, dline + 1);
                                    line += spaces[dline] ?? (spaces[dline] = new string(' ', dline));
                                }
                                Console.Write(line);
                            }
                            if ((now = DateTime.Now) > next && (now.Ticks & 65536) is 0) {
                                var k = Environment.TickCount;
                                GC.Collect();
                                Thread.Sleep(7);
                                rand = new Random(unchecked((int)(((now = DateTime.Now).Ticks + k) % int.MaxValue)));
                                next = now.AddSeconds(8191);
                            }
                        }
                        var pview = 1.0 / rand.Next(2, 11);
                        var padd = (1.0 - pview) / 2 + pview;
                        var n = rand.Next(1024, short.MaxValue);
                        if (n > nbuffer) Array.Resize(ref buffer, nbuffer = n);
                        for (var i = 0; i < n; ++i) {
                            var p = rand.NextDouble();
                            if (p < pview)
                                buffer[i] = 0;
                            else {
                                var v = 1 + rand.Next(9999);
                                if (p <= padd)
                                    buffer[i] = v;
                                else
                                    buffer[i] = -v;
                            }
                        }
                        try {
                            var target_dlib = new dlib.SelfBalancingBinarySearchTree.ObjectReference<int>(Comparer);
                            var target_RedBlack = new dotNetSystem.Collections.Generic.SortedSet<int>(Comparer);
                            bool c_dlib, c_RedBlack;
                            for (var i = 0; i < n; ++i) {
                                var v = buffer[i];
                                if (v is 0) {
                                    if (target_dlib.Count != target_RedBlack.Count) throw new Exception("target_dlib.Count != target_RedBlack.Count");
                                    var e_dlib = target_dlib.GetEnumerator();
                                    var e_RedBlack = target_RedBlack.GetEnumerator();
                                    for (; ; ) {
                                        if (e_dlib.MoveNext() != (c_RedBlack = e_RedBlack.MoveNext())) throw new Exception("e_dlib.MoveNext() != (c = e_RedBlack.MoveNext())");
                                        if (!c_RedBlack) break;
                                        if (e_dlib.Current != e_RedBlack.Current) throw new Exception("e_dlib.Current != e_RedBlack.Current");
                                    }
                                } else {
                                    if (v > 0) {
                                        Counter.kCompare = Counter.kRotation;
                                        c_dlib = target_dlib.Add(v);

                                        c_RedBlack = target_RedBlack.Add(v);
                                        if (c_dlib != c_RedBlack) throw new Exception("c_dlib != c_RedBlack");
                                    } else {
                                        if (target_dlib.Remove(v) != target_RedBlack.Remove(v)) throw new Exception("target_dlib.Add(v) != target_RedBlack.Add(v)");
                                    }
                                }
                                ++Counter.nData;
                            }
                        } catch {
                            try {
                                lock (ConsoleSyncObject) {
                                    Console.WriteLine();
                                    Console.WriteLine("测试数据：");
                                    for (var i = 0; i < n; ++i) {
                                        Console.Write(buffer[i]);
                                        Console.Write(',');
                                    }
                                    Console.WriteLine();
                                    Console.WriteLine("<end>");
                                    Console.WriteLine();
                                }
                            } catch { }
                            throw;
                        }
                    }
                } catch (Exception e) {
                    try { lock (ConsoleSyncObject) Console.WriteLine(e); } catch { }
                    throw;
                }
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
