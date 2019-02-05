using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dlib {
    public static class dlib {
        public static T[] MakeBig<T>(this T[] old_data, int old_size, int new_size) {
            T[] new_data;
            Array.Copy(old_data, 0, new_data = new T[new_size], 0, old_size);
            return new_data;
        }
    }
}
