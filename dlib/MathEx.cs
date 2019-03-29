using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dlib {
    public static class MathEx {
        public static byte CeilLog2(this int value) {
            if (value <= 2) return 1;
            byte w = 1;
            for (--value; (value >>= 1) > 0; ++w) ;
            return w;
        }
        public static byte CeilLog2(this uint value) {
            if (value <= 2) return 1;
            byte w = 1;
            for (--value; (value >>= 1) > 0; ++w) ;
            return w;
        }
        public static byte CeilLog2(this long value) {
            if (value <= 2) return 1;
            byte w = 1;
            for (--value; (value >>= 1) > 0; ++w) ;
            return w;
        }
        public static byte CeilLog2(this ulong value) {
            if (value <= 2) return 1;
            byte w = 1;
            for (--value; (value >>= 1) > 0; ++w) ;
            return w;
        }
    }
}
