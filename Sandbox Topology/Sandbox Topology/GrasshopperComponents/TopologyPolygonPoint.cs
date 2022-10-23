using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Sandbox.Topology.Core;
using Sandbox.Topology.Properties;
using System;
using System.Collections.Generic;

namespace Sandbox.Topology.GrasshopperComponents
{
    public class TopologyPolygonPoint : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public TopologyPolygonPoint()
            : base("Polygon Topology Point", "Poly Topo Point",
                  "Analyses the point topology of a network consisting of closed polylines",
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
            pManager.AddPointParameter("List of points", "P", "Ordered list of unique points", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Loop-Point structure", "LP", "For each polyline lists all point indices", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Point-Loop structure", "PL", "For each point lists all adjacent polyline indices", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // 1.Declare placeholder variables and assign initial invalid data.
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
            List<PointTopological> _ptList = TopologyShared.getPointTopo(_polyList, _T);
            List<PLineTopological> _fList = TopologyShared.getPLineTopo(_polyList, _ptList, _T);
            TopologyShared.setPointPLineTopo(_fList, _ptList);
            // Dim _ptFaceDict As Dictionary(Of String, List(Of String)) = getPointFaceDict(_fList, _ptList)

            // 4.3: return results
            List<Point3d> _PValues = new List<Point3d>();
            foreach (PointTopological _ptTopo in _ptList)
                _PValues.Add(_ptTopo.Point);

            DataTree<Int32> _FPValues = new DataTree<Int32>();
            foreach (PLineTopological _lineTopo in _fList)
            {
                GH_Path _path = new GH_Path(_FPValues.BranchCount);
                foreach (Int32 _index in _lineTopo.PointIndices)
                    _FPValues.Add(_index, _path);
            }

            DataTree<Int32> _PFValues = new DataTree<Int32>();
            foreach (PointTopological _ptTopo in _ptList)
            {
                GH_Path _path = new GH_Path(_PFValues.BranchCount);
                foreach (PLineTopological _lineTopo in _ptTopo.PLines)
                    _PFValues.Add(_lineTopo.Index, _path);
            }

            DA.SetDataList(0, _PValues);
            DA.SetDataTree(1, _FPValues);
            DA.SetDataTree(2, _PFValues);
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
                return Resources.TopologyPolyPoint;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("C96AE70F-46D8-4E54-AAD4-DA9907F5521C"); }
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