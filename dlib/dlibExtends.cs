using System;

namespace dlib {
    public static class dlibExtends {
        /// <summary>
        /// 我写这玩意的时候并不知道 Array.Resize 囧
        /// </summary>
        public static T[] MakeBig<T>(this T[] old_data, int old_size, int new_size) {
            T[] new_data;
            Array.Copy(old_data, 0, new_data = new T[new_size], 0, old_size);
            return new_data;
        }
        public static T[] MakeBig<T>(this T[] old_data, int new_size) {
            if (old_data is null) return null;
            var old_size = old_data.Length;
            if (new_size < old_size) throw new ArgumentException();
            if (new_size == old_size) return old_data;
            return MakeBig(old_data, old_size, new_size);
        }
        #region echo tree to console screen
        public static (char[,] canvas, int width, int height)
            EchoTree<Node>(Node root, Func<Node, bool> isNull, Func<Node, Node> getLeft, Func<Node, Node> getRight, Func<Node, string> getValString) {
            if (isNull(root)) return (new char[1, 1], 1, 1);
            var chars = new AutoArray<char>();
            var vals_offset = new AutoArray<int>();
            var vals_length = new AutoArray<int>();
            var lefts = new AutoArray<int>();
            var rights = new AutoArray<int>();
            int Flat(Node node, int index) {
                var thisi = index++;
                var val = getValString(node) ?? " ";
                var val_length = val.Length;
                if (val_length < 1)
                    val_length = (val = " ").Length;
                var chari = chars.Count;
                vals_offset[thisi] = chari;
                vals_length[thisi] = val_length;
                for (var i = 0; i < val_length; ++i)
                    chars[chari + i] = val[i];
                var left = getLeft(node);
                if (isNull(left))
                    lefts[thisi] = 0;
                else
                    index = Flat(left, lefts[thisi] = index);
                var right = getRight(node);
                if (isNull(right))
                    rights[thisi] = 0;
                else
                    index = Flat(right, rights[thisi] = index);
                return index;
            }
            Flat(root, 1);
            var widths = new AutoArray<int>();
            // return height
            int PreCalcTreeCanvasSize(int index) {
                //if (isNull(root)) return (0, 0);
                var self_width = vals_length[index];
                var left = lefts[index];
                var right = rights[index];
                var noleft = left == 0;
                var noright = right == 0;
                if (noleft) {
                    if (noright) {
                        widths[index] = self_width;
                        return 1;
                    } else {
                        // no left but right
                        var h = PreCalcTreeCanvasSize(right);
                        widths[index] = Math.Max(self_width, 1 + widths[right]);
                        return 2 + h;
                    }
                } else if (noright) {
                    // no right but left
                    var h = PreCalcTreeCanvasSize(left);
                    widths[index] = Math.Max(self_width, 1 + widths[left]);
                    return 2 + h;
                } else {
                    // left/right both
                    var lefth = PreCalcTreeCanvasSize(left);
                    var righth = PreCalcTreeCanvasSize(right);
                    widths[index] = Math.Max(self_width, widths[left] + 1 + widths[right]);
                    return 2 + Math.Max(lefth, righth);
                }
            }
            var height = PreCalcTreeCanvasSize(1);
            var width = widths[1];
            var canvas = new char[width, height];
            // \0 is seen ok
            //for (var j = 0; j < height; ++j)
            //    for (var i = 0; i < width; ++i)
            //        canvas[i, j] = ' ';
            // return x
            int DrawTreeOnCanvas(int x0, int y0, int index) {
                //if (isNull(root)) return x0;
                var self = vals_offset[index];
                var self_width = vals_length[index];
                var left = lefts[index];
                var right = rights[index];
                var noleft = left == 0;
                var noright = right == 0;
                if (noleft) {
                    if (noright) {
                        for (var p = 0; p < self_width; ++p)
                            canvas[x0 + p, y0] = chars[self + p];
                        return x0 + self_width / 2;
                    }
                    // no left but right
                    var x = DrawTreeOnCanvas(x0 + Math.Max(1, self_width - widths[right]), y0 + 2, right);
                    canvas[x, y0 + 1] = '\\';
                    for (var p = 0; p < self_width; ++p)
                        canvas[x0 + p, y0] = chars[self + p];
                    for (var p = x0 + self_width; p < x; ++p)
                        canvas[p, y0] = '_';
                    return x0 + self_width / 2;
                } else if (noright) {
                    // no right but left
                    var x = DrawTreeOnCanvas(x0, y0 + 2, left);
                    canvas[x, y0 + 1] = '/';
                    var x2 = x0 + widths[index] - self_width;
                    for (var p = 0; p < self_width; ++p)
                        canvas[x2 + p, y0] = chars[self + p];
                    for (var p = x + 1; p < x2; ++p)
                        canvas[p, y0] = '_';
                    return x2 + self_width / 2;
                } else {
                    var xleft = DrawTreeOnCanvas(x0, y0 + 2, left);
                    canvas[xleft, y0 + 1] = '/';
                    var xright = DrawTreeOnCanvas(x0 + widths[left] + 1 + Math.Max(0, self_width - widths[left] - widths[right] - 1), y0 + 2, right);
                    canvas[xright, y0 + 1] = '\\';
                    //var x2 = Math.Min(x0 + widths[index] - self_width, Math.Max(x0, (xleft + xright) / 2 - self_width / 2));
                    var x2 = x0 + widths[left] - self_width / 2;
                    for (var p = 0; p < self_width; ++p)
                        canvas[x2 + p, y0] = chars[self + p];
                    for (var p = xleft + 1; p < x2; ++p)
                        canvas[p, y0] = '_';
                    for (var p = x2 + self_width; p < xright; ++p)
                        canvas[p, y0] = '_';
                    return x2 + self_width / 2;
                }
            }
            DrawTreeOnCanvas(0, 0, 1);
            return (canvas, width, height);
        }
        #endregion
    }
}
