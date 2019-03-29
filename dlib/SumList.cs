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

    public class SumList : IList<ulong>, IReadOnlyList<ulong> {
        public const byte MaxSizeBitCount = 30 - 3;//30 - Log2(sizeof(ulong))
        public const int MaxSize = 1 << MaxSizeBitCount;
        public const int MinSize = 4;
        public const int HalfCutSize = 65536;
        private ulong[] sumdata, valdata;
        private int size;
        public int RealCount => this.size;
        public SumList(int initsize = MinSize) {
            this.size = initsize = 1 << Math.Min(MaxSizeBitCount, Math.Max(initsize, MinSize).CeilLog2());
            this.sumdata = new ulong[initsize];
            this.valdata = new ulong[initsize];
        }
        private static ulong[] MakeBigSumData(ulong[] old_data, int old_size, int new_size) {
            //var new_data = old_data.MakeBig(old_size, new_size);
            var new_data = old_data;
            Array.Resize(ref new_data, new_size);
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
namespace dlib {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    public class ShiftableSumList : IList<ulong>, IReadOnlyList<ulong> {
        public const byte MaxSizeBitCount = SumList.MaxSizeBitCount;
        public const int MaxSize = 1 << MaxSizeBitCount;
        public const int MinSize = 8;
        public const int HalfCutSize = 65536;
        private const string overflow = "Result is too large to return with ulong";
        private ulong[] sumdata, valdata;
        private byte[] shiftdata;
        private int size;
        private static readonly ulong[] MaxValAfterShift;

        public int Count => this.size;

        public bool IsReadOnly => false;

        public ulong this[int index] { get => this.GetValue(index); set => this.SetValue(index, value); }

        static ShiftableSumList() {
            var MaxValAfterShift = ShiftableSumList.MaxValAfterShift = new ulong[256];
            for (var i = 0; i <= 63; ++i)
                MaxValAfterShift[i] = ulong.MaxValue >> i;
        }
        public ShiftableSumList(int initsize = MinSize) {
            byte nbit;
            this.size = initsize = 1 << (nbit = Math.Min(MaxSizeBitCount, (Math.Max(initsize, MinSize) + 1).CeilLog2()));
            this.sumdata = new ulong[initsize];
            this.valdata = new ulong[initsize];
            (this.shiftdata = new byte[initsize])[0] = nbit;
        }
        private static void ApplyAllShift(int size, ulong[] sumdata, byte[] shiftdata, ulong[] valdata) {
            if (sumdata[0] is 0) return;
            var halfsize = size >> 1;
            int p, q, q1; byte n;
            for (p = 0; ++p < halfsize;) {
                if ((n = shiftdata[p]) is 0) continue;
                shiftdata[p] = 0;
                q1 = (q = p << 1) | 1;
                shiftdata[q] += n;
                shiftdata[q1] += n;
                sumdata[q] <<= n;
                sumdata[q1] <<= n;
            }
            for (var i = 0; i < halfsize; ++i) {
                if ((n = shiftdata[p = halfsize + i]) is 0) continue;
                shiftdata[p] = 0;
                valdata[q = i << 1] <<= n;
                valdata[q | 1] <<= n;
            }
            sumdata[0] = 0;
        }
        private static void ApplyShift(int index, int size, ulong[] sumdata, byte[] shiftdata, ulong[] valdata) {
            ulong sshift;
            if ((sshift = sumdata[0]) is 0) return;
            var nbit = shiftdata[0];
            var p = 1;
            byte shift;
            for (int halfsize = size >> 1, c, c1; ;) {
                shift = shiftdata[p];
                if (shift > 0) {
                    c1 = (c = p << 1) | 1;
                    shiftdata[p] = 0;
                    shiftdata[c] += shift;
                    shiftdata[c1] += shift;
                    sshift += shift;
                    sumdata[c] <<= shift;
                    sumdata[c1] <<= shift;
                    if ((p = (index & (1 << --nbit)) is 0 ? c : c1) >= halfsize) break;
                } else if ((index & (1 << --nbit)) is 0)
                    p <<= 1;
                else
                    p = (p << 1) | 1;
            }
            shift = shiftdata[p];
            if (shift > 0) {
                shiftdata[p] = 0;
                sshift -= shift;
                valdata[index] <<= shift;
                valdata[index ^ 1] <<= shift;
            }
            sumdata[0] = sshift;
        }
        // shiftdata should already be 0
        private static void Maintain(int size, ulong[] sumdata, ulong[] valdata) {
            var halfsize = size >> 1;
            for (var q = 0; q < size; q += 2)
                sumdata[halfsize + (q >> 1)] = valdata[q] + valdata[q | 1];
            for (var q = size; (q -= 2) > 1;)
                sumdata[q >> 1] = sumdata[q] + sumdata[q | 1];
        }
        private int Bigger(int newsize) {
            if (newsize > MaxSize) throw new OutOfMemoryException();
            var oldsize = this.size;
            if (newsize <= oldsize) return oldsize;
            var shiftdata = this.shiftdata;
            var valdata = this.valdata;
            var sumdata = this.sumdata;
            ApplyAllShift(oldsize, sumdata, shiftdata, valdata);
            byte nbit;
            newsize = 1 << (nbit = newsize.CeilLog2());
            Array.Resize(ref valdata, newsize); this.valdata = valdata;
            Array.Resize(ref sumdata, newsize); this.sumdata = sumdata;
            Array.Resize(ref shiftdata, newsize); (this.shiftdata = shiftdata)[0] = nbit;
            Maintain(this.size = newsize, sumdata, valdata);
            return newsize;
        }
        public ulong Sum() => this.sumdata[1];
        public ulong Sum(int count) {
            if (count <= 0) return 0;
            var size = this.size;
            if (count >= size) return this.Sum();
            var sumdata = this.sumdata;
            ulong sum = 0;
            if (sumdata[0] is 0) {
                for (var p = 1; count > 0;) {
                    size >>= 1;
                    p <<= 1;
                    if (count >= size) {
                        count -= size;
                        sum += sumdata[p++];
                    }
                }
            } else {
                var shiftdata = this.shiftdata;
                byte shift = 0;
                for (var p = 1; count > 0;) {
                    shift += shiftdata[p];
                    size >>= 1;
                    p <<= 1;
                    if (count >= size) {
                        count -= size;
                        sum += sumdata[p++] << shift;
                    }
                }
            }
            return sum;
        }
        public ulong Sum(int begin, int end) {
            if (begin > end) throw new ArgumentOutOfRangeException();
            if (end <= 0 || begin == end) return 0;
            return this.Sum(end) - this.Sum(begin);
        }
        public ulong GetValue(int index) {
            if (index < 0) return 0;
            var size = this.size;
            if (size <= index) return 0;
            var val = this.valdata[index];
            if (val is 0) return 0;
            var shiftdata = this.shiftdata;
            val <<= shiftdata[index = (index + size) >> 1];
            while ((index >>= 1) > 0)
                val <<= shiftdata[index];
            return val;
        }
        public ulong AddValue(int index, ulong dVal) {
            if (dVal is 0) return this.GetValue(index);
            if (index < 0) throw new IndexOutOfRangeException();
            var size = this.size;
            if (index >= size) size = this.Bigger(index + 1);
            var sumdata = this.sumdata;
            checked { sumdata[1] += dVal; }
            var valdata = this.valdata;
            ApplyShift(index, size, sumdata, this.shiftdata, valdata);
            unchecked {
                var ret = valdata[index] += dVal;
                sumdata[index = (index + size) >> 1] += dVal;
                while ((index >>= 1) > 1)
                    sumdata[index] += dVal;
                return ret;
            }
        }
        public ulong SubValue(int index, ulong dVal) {
            if (dVal is 0) return this.GetValue(index);
            if (index < 0) throw new IndexOutOfRangeException();
            var size = this.size;
            if (index >= size) throw new OverflowException();
            var sumdata = this.sumdata;
            var valdata = this.valdata;
            ApplyShift(index, size, sumdata, this.shiftdata, valdata);
            var ret = checked(valdata[index] -= dVal);
            unchecked {
                sumdata[index = (index + size) >> 1] -= dVal;
                while ((index >>= 1) > 0)
                    sumdata[index] -= dVal;
            }
            return ret;
        }
        public void SetValue(int index, ulong newValue) {
            var oldValue = this.GetValue(index);
            if (newValue == oldValue) return;
            if (newValue < oldValue)
                this.SubValue(index, unchecked(oldValue - newValue));
            else
                this.AddValue(index, unchecked(newValue - oldValue));
        }
        public ulong LeftShift(byte nbit) {
            var sumdata = this.sumdata;
            ulong sum;
            if ((sum = sumdata[1]) is 0) return 0;
            if (nbit is 0) return sumdata[1];
            if (sum > MaxValAfterShift[nbit]) throw new OverflowException();
            sumdata[0] += nbit;
            this.shiftdata[1] += nbit;
            return sumdata[1] = sum << nbit;
        }
        public void ForEach(Action<ulong> f) {
            ulong[] valdata;
            ApplyAllShift(this.size, this.sumdata, this.shiftdata, valdata = this.valdata);
            Array.ForEach(valdata, f);
        }
        public void ForEach(Func<ulong, ulong> f) {
            int size;
            ulong[] valdata, sumdata;
            ApplyAllShift(size = this.size, sumdata = this.sumdata, this.shiftdata, valdata = this.valdata);
            for (var i = 0; i < size; ++i)
                valdata[i] = f(valdata[i]);
            Maintain(size, sumdata, valdata);
        }

        public int IndexOf(ulong item) {
            ApplyAllShift(this.size, this.sumdata, this.shiftdata, this.valdata);
            return Array.IndexOf(this.valdata, item);
        }
        public void Insert(int index, ulong item) => throw new NotImplementedException();
        public void RemoveAt(int index) => throw new NotImplementedException();
        public void Add(ulong item) => throw new NotImplementedException();
        public void Clear() {
            var size = this.size;
            Array.Clear(this.sumdata, 0, size);
            Array.Clear(this.valdata, 0, size);
            Array.Clear(this.shiftdata, 1, size - 1);
        }
        public bool Contains(ulong item) {
            ApplyAllShift(this.size, this.sumdata, this.shiftdata, this.valdata);
            return Array.IndexOf(this.valdata, item) >= 0;
        }
        public void CopyTo(ulong[] array, int arrayIndex) {
            ApplyAllShift(this.size, this.sumdata, this.shiftdata, this.valdata);
            this.valdata.CopyTo(array, arrayIndex);
        }
        public bool Remove(ulong item) => throw new NotImplementedException();
        IEnumerator<ulong> IEnumerable<ulong>.GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }
}
