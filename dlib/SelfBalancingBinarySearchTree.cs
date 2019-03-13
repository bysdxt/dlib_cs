/*
 * 本文件实现的平衡二叉树基于同一个贪心算法：
 * 通过 左旋、右旋、左右旋、右左旋 来降低总节点深度；
 * 当一个节点不能通过上述四种旋转操作来降低总深度时，有 左树节点数/总节点数<2/3 且 右树节点数/总节点数<2/3
 * 在访问节点前尝试旋转操作，可以把访问深度降至对数级 log1.5(n)
 */
using System;
using System.Collections.Generic;

namespace dlib.SelfBalancingBinarySearchTree {
    public class ObjectReference<T> {
        // 仅 增删 操作 进行维护，查找就不维护了，毕竟 增删 时的维护已经足够好了
        private class Node {
            public T value;
            public Node left, right;
            public uint count;
            private Node(object _) => this.left = this.right = this;
            public static readonly Node Nil = new Node(null) { count = 0 };
            protected Node() { this.left = this.right = Nil; this.count = 1; }
            public void Count() => this.count = 1 + this.left.count + this.right.count;
            public bool LeftBetter() => this.right.right.count > this.left.count;
            public bool RightBetter() => this.left.left.count > this.right.count;
            public bool LeftRightBetter() => this.left.right.count > this.right.count;
            public bool RightLeftBetter() => this.right.left.count > this.left.count;
            #region Rotate
            public static Node LeftRotate(Node root) {
                var right = root.right;
                root.right = right.left;
                right.left = root;
                root.Count();
                right.Count();
                return right;
            }
            public static Node LeftRotate(ref Node root) => root = LeftRotate(root);
            public static Node RightRotate(Node root) {
                var left = root.left;
                root.left = left.right;
                left.right = root;
                root.Count();
                left.Count();
                return left;
            }
            public static Node RightRotate(ref Node root) => root = RightRotate(root);
            public static Node LeftRightRotate(Node root) {
                var left = root.left;
                var leftright = left.right;
                root.left = leftright.right;
                left.right = leftright.left;
                leftright.left = left;
                leftright.right = root;
                root.Count();
                left.Count();
                leftright.Count();
                return leftright;
            }
            public static Node LeftRightRotate(ref Node root) => root = LeftRightRotate(root);
            public static Node RightLeftRotate(Node root) {
                var right = root.right;
                var rightleft = right.left;
                root.right = rightleft.left;
                right.left = rightleft.right;
                rightleft.left = root;
                rightleft.right = right;
                root.Count();
                right.Count();
                rightleft.Count();
                return rightleft;
            }
            #endregion
            public static Node Find(Node root, T value, IComparer<T> cmp) {
                if (Nil == root) return Nil;
                for (int dvalue; ;) {
                    if ((dvalue = cmp.Compare(value, root.value)) is 0)
                        return root;
                    if (Nil == (root = dvalue < 0 ? root.left : root.right))
                        return Nil;
                }
            }
            public static Node Find(Node root, uint index) {
                if (index >= root.count) return Nil;
                Node left;
                for (uint nleft; ;) {
                    if (index == (nleft = (left = root.left).count))
                        return root;
                    if (index > nleft) {
                        root = root.right;
                        index -= 1 + nleft;
                    } else
                        root = left;
                }
            }
            public static bool FindIndex(Node root, T value, IComparer<T> cmp, out uint index) {
                uint iresult = 0;
                for (int dvalue; Nil != root;) {
                    if ((dvalue = cmp.Compare(value, root.value)) >= 0) {
                        iresult += root.left.count;
                        if (dvalue is 0)
                            break;
                        ++iresult;
                        root = root.right;
                    } else
                        root = root.left;
                }
                index = iresult;
                return Nil != root;
            }
            public static Node TryRotate(Node root) {
                //for (; ; ) {
                //    if (root.LeftBetter()) {
                //        root = LeftRotate(root);
                //        continue;
                //    }
                //    if (root.RightBetter()) {
                //        root = RightRotate(root);
                //        continue;
                //    }
                //    if (root.LeftRightBetter()) {
                //        root = LeftRightRotate(root);
                //        continue;
                //    }
                //    if (root.RightLeftBetter()) {
                //        root = RightLeftRotate(root);
                //        continue;
                //    }
                //    break;
                //}
                for (Node left, right; ;) {
                    var nleft = (left = root.left).count;
                    var nright = (right = root.right).count;
                    if (nright > nleft) {
                        if (right.right.count > nleft) {
                            root = LeftRotate(root);
                            continue;
                        }
                        if (right.left.count > nleft) {
                            root = RightLeftRotate(root);
                            continue;
                        }
                    } else {
                        if (left.left.count > nright) {
                            root = RightRotate(root);
                            continue;
                        }
                        if (left.right.count > nright) {
                            root = LeftRightRotate(root);
                            continue;
                        }
                    }
                    return root;
                }
            }
            public static Node TryRotate(ref Node root) => root = TryRotate(root);
            public static bool Add(ref Node root, T value, IComparer<T> cmp, Node[] parents) {
                if (Nil == root)
                    root = new Node() { value = value };
                else {
                    var nparent = 0;
                    Node child;
                    var parent = parents[nparent++] = root;
                    for (; ; ) {
                        var dvalue = cmp.Compare(value, parent.value);
                        if (dvalue is 0) {
                            Array.Clear(parents, 0, nparent);
                            return false;
                        }
                        if (dvalue < 0) {
                            if (Nil == (child = parent.left)) {
                                parent.left = new Node() { value = value };
                                break;
                            }
                            parent = parents[nparent++] = child;
                        } else {
                            if (Nil == (child = parent.right)) {
                                parent.right = new Node() { value = value };
                                break;
                            }
                            parent = parents[nparent++] = child;
                        }
                    }
                    for (var i = nparent; --i >= 0;) {
                        (parent = parents[i]).Count();
                        parent.left = TryRotate(parent.left);
                        parent.right = TryRotate(parent.right);
                    }
                    root = TryRotate(parent);
                    Array.Clear(parents, 0, nparent);
                }
                return true;
            }
            public static Node RemoveMinimum(ref Node root, Node[] parents, int offset) {
                if (Nil == root) return Nil;
                Node child, result;
                var nparent = 0;
                var parent = parents[offset + nparent++] = root;
                while (Nil != (child = parent.left))
                    parent = parents[offset + nparent++] = child;
                // parent is the minimum now
                var rest = TryRotate((result = parent).right);
                for (var i = nparent - 1; --i >= 0;) {
                    (parent = parents[offset + i]).left = rest;
                    parent.Count();
                    rest = TryRotate(parent);
                }
                root = rest;
                Array.Clear(parents, offset, nparent);
                return result;
            }
            public static Node RemoveMaximum(ref Node root, Node[] parents, int offset) {
                if (Nil == root) return Nil;
                Node child, result;
                var nparent = 0;
                var parent = parents[offset + nparent++] = root;
                while (Nil != (child = parent.right))
                    parent = parents[offset + nparent++] = child;
                // parent is the maximum now
                var rest = TryRotate((result = parent).left);
                for (var i = nparent - 1; --i >= 0;) {
                    (parent = parents[offset + i]).right = rest;
                    parent.Count();
                    rest = TryRotate(parent);
                }
                root = rest;
                Array.Clear(parents, offset, nparent);
                return result;
            }
            public static Node Remove(ref Node root, T value, IComparer<T> cmp, Node[] parents) {
                if (Nil == root) return Nil;
                var nparent = 0;
                Node child, rest, result;
                var parent = parents[nparent++] = root;
                var dvalue = cmp.Compare(value, parent.value);
                if (dvalue is 0) {
                    result = parent;
                    var left = result.left;
                    var right = result.right;
                    if (Nil != (rest = left.count > right.count ?
                        RemoveMaximum(ref left, parents, nparent) :
                        RemoveMinimum(ref right, parents, nparent))
                        ) {
                        rest.left = left;
                        rest.right = right;
                        rest.Count();
                        rest = TryRotate(rest);
                    }
                    root = rest;
                    Array.Clear(parents, 0, nparent);
                    return result;
                }
                for (; ; ) {
                    if (dvalue < 0) {
                        if (Nil == (child = parent.left)) return Nil;
                        parent = parents[nparent++] = parent.left = TryRotate(child);
                        dvalue = cmp.Compare(value, parent.value);
                        if (dvalue is 0) {
                            result = parent;
                            var left = result.left;
                            var right = result.right;
                            if (Nil != (rest = left.count > right.count ?
                                RemoveMaximum(ref left, parents, nparent) :
                                RemoveMinimum(ref right, parents, nparent))
                                ) {
                                rest.left = left;
                                rest.right = right;
                                rest.Count();
                                rest = TryRotate(rest);
                            }
                            parents[nparent - 2].left = rest;
                            break;
                        }
                    } else {
                        if (Nil == (child = parent.right)) return Nil;
                        parent = parents[nparent++] = parent.right = TryRotate(child);
                        if (dvalue is 0) {
                            result = parent;
                            var left = result.left;
                            var right = result.right;
                            if (Nil != (rest = left.count > right.count ?
                                RemoveMaximum(ref left, parents, nparent) :
                                RemoveMinimum(ref right, parents, nparent))
                                ) {
                                rest.left = left;
                                rest.right = right;
                                rest.Count();
                                rest = TryRotate(rest);
                            }
                            parents[nparent - 2].right = rest;
                            break;
                        }
                    }
                }
                for (var i = nparent - 1; --i >= 0;) {
                    parent = parents[i];
                    parent.left = TryRotate(parent.left);
                    parent.right = TryRotate(parent.right);
                }
                root = TryRotate(root);
                Array.Clear(parents, 0, nparent);
                return result;
            }
        }
        public ObjectReference() {
        }
    }
}
