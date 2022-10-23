using Rhino.Geometry;
using System;
using System.Collections.Generic;


namespace Sandbox.Topology.Core
{
    public static class TopologyShared
    {
        private static bool containsPoint(List<PointTopological> _points, Point3d _check, double _T)
        {
            foreach (PointTopological _item in _points)
            {
                if (_item.Point.DistanceTo(_check) < _T)
                    // consider it the same point
                    return true;
            }

            return false;
        }

        public static List<PointTopological> getPointTopo(List<Polyline> P, double _T)
        {
            List<PointTopological> _ptList = new List<PointTopological>();

            Int32 _count = 0;
            foreach (Polyline _poly in P)
            {
                Point3d[] _points = _poly.ToArray();

                for (Int32 i = 0; i <= _points.Length; i++)
                {

                    // check if point exists in _ptList already
                    if (!containsPoint(_ptList, _points[i], _T))
                    {
                        _ptList.Add(new PointTopological(_points[i], _count));
                        _count = _count + 1;
                    }
                }
            }

            return _ptList;
        }

        public static List<PLineTopological> getPLineTopo(List<Polyline> P, List<PointTopological> _ptDict, double _T)
        {
            List<PLineTopological> _lDict = new List<PLineTopological>();

            Int32 _count = 0;
            foreach (Polyline _poly in P)
            {
                Point3d[] _points = _poly.ToArray();

                List<Int32> _indices = new List<Int32>();

                for (Int32 i = 0; i <= _points.Length; i++)
                {
                    foreach (PointTopological _item in _ptDict)
                    {
                        if (_item.Point.DistanceTo(_points[i]) < _T)
                        {
                            _indices.Add(_item.Index);
                            break;
                        }
                    }
                }

                _lDict.Add(new PLineTopological(_indices, _count));

                _count = _count + 1;
            }

            return _lDict;
        }

        public static void setPointPLineTopo(List<PLineTopological> _lineList, List<PointTopological> _pointList)
        {
            foreach (PointTopological _pt in _pointList)
            {
                List<PLineTopological> _lList = new List<PLineTopological>();

                foreach (PLineTopological _l in _lineList)
                {
                    foreach (Int32 _index in _l.PointIndices)
                    {
                        if (_index == _pt.Index)
                        {
                            _lList.Add(_l);
                            break;
                        }
                    }
                }

                _pt.PLines = _lList;
            }
        }
    }

}

