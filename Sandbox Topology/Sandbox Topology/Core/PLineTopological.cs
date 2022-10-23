using System;
using System.Collections.Generic;

namespace Sandbox.Topology.Core
{
    public class PLineTopological
    {
        private List<Int32> _v;
        private Int32 _i;     // internal indexing of the lines

        public PLineTopological(List<Int32> V, Int32 I)
        {
            _v = V;
            _i = I;
        }

        // ##### PROPERTIES #####

        public Int32 Index
        {
            get
            {
                return _i;
            }
        }

        public List<Int32> PointIndices
        {
            get
            {
                return _v;
            }
        }
    }

}
