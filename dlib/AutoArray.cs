namespace dlib {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    public sealed class AutoArray<T> : IList<T>, IReadOnlyList<T> {
        public const int MaxSizeBitCount = 30;
        public const int MaxSize = 1 << MaxSizeBitCount;
        public const int MinSize = 4;
        public const int HalfCutSize = 65536;
        internal T[] saved;

        public AutoArray(int capacity = MinSize) => this.saved = new T[1 << (Math.Max(capacity, MinSize).CeilLog2())];

        public T this[int index] {
            get => index < 0 ? throw new ArgumentOutOfRangeException(nameof(index)) : this.saved is var saved && index < saved.Length ? saved[index] : index < MaxSize ? default(T) : throw new ArgumentOutOfRangeException(nameof(index));
            set {
                if (index < 0 || MaxSize <= index) throw new ArgumentOutOfRangeException(nameof(index));
                T[] saved;
                var maxsize = (saved = this.saved).Length;
                //(index < maxsize ? saved : (this.saved = saved.MakeBig(maxsize, 1 << (index + 1).CeilLog2())))[index] = value;
                if (index >= maxsize) {
                    Array.Resize(ref saved, 1 << (index + 1).CeilLog2());
                    this.saved = saved;
                }
                saved[index] = value;
                if (index >= this.Count) this.Count = index + 1;
            }
        }

        public int Count { get; private set; } = 0;

        public bool IsReadOnly => false;

        public void Clear() {
            int maxsize;
            if ((maxsize = this.saved.Length) >= HalfCutSize)
                Array.Resize(ref this.saved, maxsize >>= 1);
            Array.Clear(this.saved, 0, maxsize);
            this.Count = 0;
        }
        public void Add(T item) => this[this.Count] = item;

        public bool Contains(T item) => Array.IndexOf(this.saved, item) >= 0;
        public int IndexOf(T item) => Array.IndexOf(this.saved, item);

        public void CopyTo(T[] array, int arrayIndex) {
            if (array is null) return;
            var arr_size = array.Length - arrayIndex;
            if (arr_size <= 0) return;
            Array.Copy(this.saved, 0, array, arrayIndex, Math.Min(this.Count, arr_size));
        }
        #region IList
        void IList<T>.Insert(int index, T item) => (this.saved as IList<T>).Insert(index, item);
        void IList<T>.RemoveAt(int index) => (this.saved as IList<T>).RemoveAt(index);
        bool ICollection<T>.Remove(T item) => (this.saved as ICollection<T>).Remove(item);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        #endregion
    }
}
