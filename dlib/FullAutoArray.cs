using System;
using System.Collections;
using System.Collections.Generic;

namespace dlib {
    /// <summary>
    /// support negative index
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class FullAutoArray<T> : IList<T>, IReadOnlyList<T> {
        internal readonly AutoArray<T> positive;
        internal readonly AutoArray<T> negative;
        public FullAutoArray(int bound = AutoArray<T>.MinSize) {
            this.positive = new AutoArray<T>(bound);
            this.negative = new AutoArray<T>(bound);
        }
        public bool IsReadOnly => false;
        public T this[int index] {
            get => index < 0 ? this.negative[~index] : this.positive[index];
            set {
                if (index < 0)
                    this.negative[~index] = value;
                else
                    this.positive[index] = value;
            }
        }
        public int LBound => ~this.negative.Count;
        public int UBound => this.positive.Count;
        public int Count => this.negative.Count + this.positive.Count;
        public bool Contains(T item) => this.positive.Contains(item) || this.negative.Contains(item);
        public void Clear() {
            this.positive.Clear();
            this.negative.Clear();
        }
        public void Add(T item) => this.positive.Add(item);
        #region IList
        void IList<T>.Insert(int index, T item) => throw new NotImplementedException();
        void IList<T>.RemoveAt(int index) => throw new NotImplementedException();
        bool ICollection<T>.Remove(T item) => throw new NotImplementedException();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
        int IList<T>.IndexOf(T item) => throw new NotImplementedException();
        void ICollection<T>.Add(T item) => throw new NotImplementedException();
        void ICollection<T>.Clear() => throw new NotImplementedException();
        bool ICollection<T>.Contains(T item) => throw new NotImplementedException();
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
        #endregion
    }
}
