namespace dlib.BitOp.IndexTree.Base0 {
    public static class IndexTree {
        public static int Parent(this int index) => index | (index + 1);
        public static int ChildCount(this int index) => index - ((index + 1) & index);
        public static int Count(this int index) => index.ChildCount() + 1;
    }
}
namespace dlib {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using global::dlib.BitOp.IndexTree.Base0;

    internal class SumSet : IList<ulong>, IReadOnlyList<ulong> {
        public const int MaxSizeBitCount = 30 - 3;//30 - Log2(sizeof(ulong))
        public const int MaxSize = 1 << MaxSizeBitCount;
        public const int MinSize = 4;
        public const int HalfCutSize = 65536;
        private ulong[] sumdata, valdata;
        private int size;
        public int RealCount => this.sumdata.Length;
        public SumSet(int initsize = MinSize) {
            this.size = initsize = 1 << Math.Min(MaxSizeBitCount, Math.Max(initsize, MinSize).CeilLog2());
            this.sumdata = new ulong[initsize];
            this.valdata = new ulong[initsize];
        }
        private static ulong[] MakeBigSumData(ulong[] old_data, int old_size, int new_size) {
            var new_data = old_data.MakeBig(old_size, new_size);
            int p = old_size, q;
            for (var last_val = new_data[p - 1]; ;) {
                new_data[(q = p << 1) - 1] = last_val;
                if (q >= new_size) return new_data;
                new_data[(p = q << 1) - 1] = last_val;
                if (p >= new_size) return new_data;
            }
        }
        public ulong GetValue(int index) {
            if (index < 0) return 0;
            var size = this.size;
            if (size <= index) return 0;
            return this.valdata[index];
        }
        public void AddValue(int index, ulong dValue) {
            if (dValue == 0) return;
            if (index < 0 || MaxSize <= index) throw new IndexOutOfRangeException();
            var sumdata = this.sumdata;
            var valdata = this.valdata;
            var size = this.size;
            if (index >= size) {
                var newsize = this.size = 1 << (index + 1).CeilLog2();
                this.sumdata = sumdata = MakeBigSumData(sumdata, size, newsize);
                this.valdata = valdata = valdata.MakeBig(size, newsize);
                size = newsize;
            }
            checked { sumdata[--size] += dValue; }
            valdata[index] += dValue;
            if (index < size)
                for (sumdata[index] += dValue; (index = index.Parent()) < size;)
                    sumdata[index] += dValue;
        }
        public void SubValue(int index, ulong dValue) {
            if (dValue == 0) return;
            if (index < 0 || MaxSize <= index) throw new IndexOutOfRangeException();
            var sumdata = this.sumdata;
            var valdata = this.valdata;
            var size = this.size;
            if (index >= size) {
                var newsize = this.size = 1 << (index + 1).CeilLog2();
                this.sumdata = sumdata = MakeBigSumData(sumdata, size, newsize);
                this.valdata = valdata = valdata.MakeBig(size, newsize);
                size = newsize;
            }
            checked { valdata[index] -= dValue; }
            sumdata[--size] -= dValue;
            if (index < size)
                for (sumdata[index] -= dValue; (index = index.Parent()) < size;)
                    sumdata[index] -= dValue;
        }
        public void SetValue(int index, ulong newValue) {
            var curValue = this.GetValue(index);
            if (newValue == curValue) return;
            if (newValue > curValue)
                this.AddValue(index, newValue - curValue);
            else
                this.SubValue(index, curValue - newValue);
        }
        public ulong Sum(int count) {
            if (count <= 0) return 0;
            var sumdata = this.sumdata;
            var size = sumdata.Length;
            if (count > size) count = size;
            ulong sum = 0;
            while (--count >= 0) {
                sum += sumdata[count];
                count -= count.ChildCount();
            }
            return sum;
        }
        public ulong Sum(int begin, int end) {
            if (begin > end) throw new ArgumentOutOfRangeException();
            if (end <= 0) return 0;
            if (begin == end) return 0;
            return this.Sum(end) - this.Sum(begin);
        }
        public ulong Sum() => this.sumdata[this.size - 1];
        public void Multiply(ulong multiplier) {
            if (1 == multiplier) return;
            if (0 == multiplier) {
                var size0 = this.size;
                Array.Clear(this.sumdata, 0, size0);
                Array.Clear(this.valdata, 0, size0);
                return;
            }
            var sumdata = this.sumdata;
            var valdata = this.valdata;
            var size = this.size - 1;
            checked { sumdata[size] *= multiplier; }
            valdata[size] *= multiplier;
            for (var i = 0; i < size; ++i) {
                sumdata[i] *= multiplier;
                valdata[i] *= multiplier;
            }
        }
        public void LeftShift(int n) {
            if (n < 0) throw new ArgumentOutOfRangeException();
            if (n == 0) return;
            var sumdata = this.sumdata;
            var valdata = this.valdata;
            var lastIndex = this.size - 1;
            var old_sum = sumdata[lastIndex];
            var new_sum = old_sum << n;
            if (new_sum < old_sum) throw new OverflowException();
            sumdata[lastIndex] = new_sum;
            valdata[lastIndex] <<= n;
            for (var i = 0; i < lastIndex; ++i) {
                sumdata[i] <<= n;
                valdata[i] <<= n;
            }
        }
        public int AtLeastIndex(ulong sum) {
            var sumdata = this.sumdata;
            if (sum <= sumdata[0]) return 0;
            var size = sumdata.Length;
            if (sum > sumdata[size - 1]) return int.MaxValue;
            var half = size >> 1;
            var index = half - 1;
            while ((half >>= 1) > 0)
                if (sumdata[index] >= sum)
                    index -= half;
                else
                    break;
            if (half <= 0) return 1;    //  sumdata[0] < sum <= sumdata[1]
            sum -= sumdata[index];
            for (int index2; ;) {
                if (sum > sumdata[index2 = index + half])
                    sum -= sumdata[index = index2];
                if ((half >>= 1) <= 0) break;
            }
            return index + 1;
        }
        public bool IsReadOnly => false;
        public ulong this[int index] { get => this.GetValue(index); set => this.SetValue(index, value); }
        public int Count => MaxSize;

        public void Clear() {
            var size = this.size;
            if (size >= HalfCutSize) this.size = size = size <<= 1;
            this.sumdata = new ulong[size];
            this.valdata = new ulong[size];
        }
        public void CopyTo(ulong[] array, int arrayIndex) {
            if (array is null) return;
            var arr_size = array.Length - arrayIndex;
            if (arr_size <= 0) return;
            var size = this.size;
            if (arr_size > size) {
                Array.Clear(array, size + arrayIndex, Math.Min(MaxSize, arr_size) - size);
                arr_size = size;
            }
            Array.Copy(this.valdata, 0, array, arrayIndex, arr_size);
        }
        public int IndexOf(ulong item) => Array.IndexOf(this.valdata, item);
        public bool Contains(ulong item) => item > 0 ? Array.IndexOf(this.valdata, item) >= 0 : true;

        #region IList
        void IList<ulong>.Insert(int index, ulong item) => throw new NotImplementedException();
        void IList<ulong>.RemoveAt(int index) => throw new NotImplementedException();
        void ICollection<ulong>.Add(ulong item) => throw new NotImplementedException();
        bool ICollection<ulong>.Remove(ulong item) => throw new NotImplementedException();
        IEnumerator<ulong> IEnumerable<ulong>.GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        #endregion
    }
}
