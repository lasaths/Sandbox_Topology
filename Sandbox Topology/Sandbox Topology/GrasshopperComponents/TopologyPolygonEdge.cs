using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Sandbox.Topology.Properties;
using System;
using System.Collections.Generic;


namespace Sandbox.Topology.GrasshopperComponents
{
    public class TopologyPolygonEdge : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public TopologyPolygonEdge()
          : base("Polygon Topology Edge",
                "Poly Topo Edge",
                "Analyses the edge topology of a curve network consisting of closed polylines",
                "Sandbox", "Topology")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("List of polylines", "C", "Network of closed polylines", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance", "T", "Tolerance value", GH_ParamAccess.item, 0.001);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("List of edges", "E", "Ordered list of unique polyline edges", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Loop-Edge structure", "LE", "For each polyline lists edge indices belonging to polyline", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Edge-Loop structure", "EL", "For each edge lists adjacent polyline indices", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // 1. Declare placeholder variables and assign initial invalid data.
            // This way, if the input parameters fail to supply valid data, we know when to abort.
            List<GH_Curve> _C = new List<GH_Curve>();
            double _T = 0;

            // 2. Retrieve input data.
            if ((!DA.GetDataList(0, _C)))
                return;
            if ((!DA.GetData(1, ref _T)))
                return;

            // 3. Abort on invalid inputs.
            if (!(_C.Count > 0))
                return;
            if (!(_T > 0))
                return;

            // 4. Do something useful.
            List<Polyline> _polyList = new List<Polyline>();

            // 4.1. check inputs
            foreach (GH_Curve _crv in _C)
            {
                Polyline _poly = null/* TODO Change to default(_) if this is not a reference type */;
                if (!_crv.Value.TryGetPolyline(out _poly))
                    return;
                _polyList.Add(_poly);
            }

            // 4.2. get topology
            Dictionary<string, Line> _edgeDict = getEdgeDict(_polyList, _T);
            Dictionary<string, List<string>> _fDict = getFaceDict(_polyList, _edgeDict, _T);
            Dictionary<string, List<string>> _edgeFaceDict = getEdgeFaceDict(_fDict, _edgeDict);

            // 4.3: return results
            List<Line> _EValues = new List<Line>();
            foreach (KeyValuePair<string, Line> _pair in _edgeDict)
                _EValues.Add(_pair.Value);

            DataTree<Int32> _FEValues = new DataTree<Int32>();
            foreach (List<string> _edgeIndexList in _fDict.Values)
            {
                GH_Path _path = new GH_Path(_FEValues.BranchCount);
                foreach (string _item in _edgeIndexList)
                {
                    _FEValues.Add(int.Parse(_item.Substring(1)), _path);

                }
            }

            DataTree<Int32> _EFValues = new DataTree<Int32>();
            foreach (List<string> _fList in _edgeFaceDict.Values)
            {
                GH_Path _path = new GH_Path(_EFValues.BranchCount);
                foreach (string _item in _fList)
                    _EFValues.Add(int.Parse(_item.Substring(1)), _path);
            }

            DA.SetDataList(0, _EValues);
            DA.SetDataTree(1, _FEValues);
            DA.SetDataTree(2, _EFValues);
        }

        private Dictionary<string, List<string>> getEdgeFaceDict(Dictionary<string, List<string>> _fDict, Dictionary<string, Line> _edgeDict)
        {
            Dictionary<string, List<string>> _edgeFaceDict = new Dictionary<string, List<string>>();

            foreach (string _edgeID in _edgeDict.Keys)
            {
                List<string> _fList = new List<string>();

                foreach (string _key in _fDict.Keys)
                {
                    List<string> _values;
                    _fDict.TryGetValue(_key, out _values);

                    foreach (string _value in _values)
                    {
                        if (_edgeID == _value)
                            _fList.Add(_key);
                    }
                }

                _edgeFaceDict.Add(_edgeID, _fList);
            }

            return _edgeFaceDict;
        }

        private Dictionary<string, List<string>> getFaceDict(List<Polyline> _polyList, Dictionary<string, Line> _edgeDict, double _T)
        {
            Dictionary<string, List<string>> _fDict = new Dictionary<string, List<string>>();

            Int32 _count = 0;
            foreach (Polyline _poly in _polyList)
            {
                string _Fkey = "F" + _count;

                Line[] _edges;
                _edges = _poly.GetSegments();

                List<string> _value = new List<string>();

                for (Int32 i = 0; i <= _edges.Length; i++)
                {
                    foreach (string _key in _edgeDict.Keys)
                    {
                        Line edgeLine;
                        _edgeDict.TryGetValue(_key, out edgeLine);
                        if (compareEdges(edgeLine, _edges[i], _T))
                        {
                            _value.Add(_key);
                            break;
                        }
                    }
                }

                _fDict.Add(_Fkey, _value);

                _count = _count + 1;
            }

            return _fDict;
        }

        private Dictionary<string, Line> getEdgeDict(List<Polyline> _polyList, double _T)
        {
            Dictionary<string, Line> _edgeDict = new Dictionary<string, Line>();

            Int32 _count = 0;
            foreach (Polyline _poly in _polyList)
            {
                Line[] _edges;
                _edges = _poly.GetSegments();

                for (Int32 i = 0; i <= _edges.Length; i++)
                {

                    // check if edge exists in _edgeDict already
                    if (!containsEdge(_edgeDict, _edges[i], _T))
                    {
                        string _key = "E" + _count;
                        Line _value = _edges[i];
                        _edgeDict.Add(_key, _value);
                        _count = _count + 1;
                    }
                }
            }

            return _edgeDict;
        }

        private bool compareEdges(Line _line1, Line _line2, double _T)
        {
            Point3d _startPt = _line1.PointAt(0);
            Point3d _endPt = _line1.PointAt(1);
            if ((_startPt.DistanceTo(_line2.PointAt(0)) < _T) & (_endPt.DistanceTo(_line2.PointAt(1)) < _T))
                // consider it the same edge
                return true;
            else if ((_startPt.DistanceTo(_line2.PointAt(1)) < _T) & (_endPt.DistanceTo(_line2.PointAt(0)) < _T))
                // consider it the same edge
                return true;

            return false;
        }

        private bool containsEdge(Dictionary<string, Line> _edgeDict, Line _check, double _T)
        {
            foreach (Line _l in _edgeDict.Values)
            {
                Point3d _startPt = _l.PointAt(0);
                Point3d _endPt = _l.PointAt(1);
                if ((_startPt.DistanceTo(_check.PointAt(0)) < _T) & (_endPt.DistanceTo(_check.PointAt(1)) < _T))
                    // consider it the same edge
                    return true;
                else if ((_startPt.DistanceTo(_check.PointAt(1)) < _T) & (_endPt.DistanceTo(_check.PointAt(0)) < _T))
                    // consider it the same edge
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Resources.TopologyPolyEdge;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("C1A5A391-2550-4612-919D-44B0B451521B"); }
        }


        public override GH_Exposure Exposure
        {
            get
            {
                return GH_Exposure.secondary;
            }
        }
    }
}