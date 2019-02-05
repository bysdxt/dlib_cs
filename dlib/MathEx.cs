using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dlib {
    public static class MathEx {
        public static int CeilLog2(this int value) {
            if (value <= 1) return 1;
            var w = 1;
            for (--value; (value >>= 1) > 0; ++w) ;
            return w;
        }
        public static int CeilLog2(this uint value) {
            if (value <= 1) return 1;
            var w = 1;
            for (--value; (value >>= 1) > 0; ++w) ;
            return w;
        }
        public static int CeilLog2(this long value) {
            if (value <= 1) return 1;
            var w = 1;
            for (--value; (value >>= 1) > 0; ++w) ;
            return w;
        }
        public static int CeilLog2(this ulong value) {
            if (value <= 1) return 1;
            var w = 1;
            for (--value; (value >>= 1) > 0; ++w) ;
            return w;
        }
    }
}
