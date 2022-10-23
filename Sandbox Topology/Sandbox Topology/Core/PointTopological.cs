using Rhino.Geometry;
using System;
using System.Collections.Generic;


namespace Sandbox.Topology.Core
{
    public class PointTopological
    {
        private Point3d _p;
        private Int32 _i;     // internal indexing of the points
        private List<PLineTopological> _l = null;

        public PointTopological(Point3d P, Int32 I)
        {
            _p = P;
            _i = I;
        }

        // ##### PROPERTIES #####

        public Point3d Point
        {
            get
            {
                return _p;
            }
        }

        public Int32 Index
        {
            get
            {
                return _i;
            }
        }

        public List<PLineTopological> PLines
        {
            set
            {
                _l = value;
            }
            get
            {
                return _l;
            }
        }
    }

}
